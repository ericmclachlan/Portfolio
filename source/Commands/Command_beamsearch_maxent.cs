using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
{
    internal class Command_beamsearch_maxent : ICommand
    {
        // Members

        public string CommandName { get { return "beamsearch_maxent"; } }

        public CommandParameterType[] CommandParamaters { get { return new CommandParameterType[] { }; } }

        #region Parameters

        [CommandParameter(Index = 0, Description = "Vector file in text format.")]
        public string test_data { get; set; }

        [CommandParameter(Index = 1, Description = "This file contains one number per line, which is the length of a sentence.")]
        /// <summary>This file contains one number per line, which is the length of a sentence.</summary>
        public string boundary_file { get; set; }

        [CommandParameter(Index = 2, Description = "A MaxEnt model in text format.")]
        /// <summary>A MaxEnt model in text format.</summary>
        public string model_file { get; set; }

        [CommandParameter(Index = 3, Description = "The classification result on the training and test data.")]
        public string sys_output { get; set; }

        [CommandParameter(Index = 4, Description = "The max gap between the lg-prob of the best path and the lg-prob of kept path.")]
        /// <summary>
        /// <para>The max gap between the lg-prob of the best path and the lg-prob of kept path.</para>
        /// <para>i.e. A kept path should satisfy lg(prob) + beam_size ≥ lg(max_prob), where max_prob is the probability of the best path for the current position.</para>
        /// <para>lg is base-10 log.</para>
        /// </summary>
        public int beam_size { get; set; }

        [CommandParameter(Index = 5, Description = "When expanding a node in the beam search tree, choose only the topN POS tags for the given word based on P(y|x).")]
        /// <summary>When expanding a node in the beam search tree, choose only the topN POS tags for the given word based on P(y|x).</summary>
        public int topN { get; set; }

        [CommandParameter(Index = 6, Description = "The max number of paths kept alive at each position after pruning.")]
        /// <summary>The max number of paths kept alive at each position after pruning.</summary>
        public int topK { get; set; }

        #endregion

        private ValueIdMapper<string> classToClassId;
        private ValueIdMapper<string> featureToFeatureId;
        private MaxEntPOSClassifier classifier;


        // Methods

        public void ExecuteCommand()
        {
            int noOfHeadersColumns = 2;
            int instanceName_i = 0;
            int gold_i = 1;
            ValueIdMapper<string>[] headerToHeaderIds;
            Program.CreateValueIdMappers(noOfHeadersColumns, gold_i, out featureToFeatureId, out headerToHeaderIds, out classToClassId);

            // Read the boundaries:
            int[] sentenceLengths = ReadBoundaryFile(boundary_file);

            // Read the classifier model:
            classifier = MaxEntPOSClassifier.LoadModel(File.ReadAllText(model_file), out classToClassId, out featureToFeatureId);
            int trainingFeatureCount = featureToFeatureId.Count;

            // Read the vectors:
            int[][] headers;
            var testVectors = FeatureVector.LoadFromSVMLight(test_data, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Continuous, featureDelimiter: ':', isSortRequiredForFeatures: false);
            int[] instanceNameIds = headers[instanceName_i];
            int[] goldClasses = headers[gold_i];

            // TODO: Neaten this up a little.
            string[] instanceNames = new string[instanceNameIds.Length];
            for (int i = 0; i < instanceNameIds.Length; i++)
            {
                int instanceNameId = instanceNameIds[i];
                instanceNames[i] = headerToHeaderIds[instanceName_i][i];
            }

            // Generate sys_output:
            ConfusionMatrix confusionMatrix;
            File.WriteAllText(sys_output, GenerateSysOutput(instanceNames, testVectors, sentenceLengths, out confusionMatrix, gold_i));

            // Generate acc:
            Console.WriteLine($"class_num={classToClassId.Count} feat_num={trainingFeatureCount}");
            Console.WriteLine();
            Console.WriteLine();
            ProgramOutput.ReportAccuracy(confusionMatrix, classToClassId, "Test");
        }


        // Private Methods

        private static int[] ReadBoundaryFile(string boundary_file)
        {
            string text = File.ReadAllText(boundary_file);
            string[] lines = TextHelper.SplitOnNewline(text);
            int[] sentenceLengths = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                sentenceLengths[i] = int.Parse(lines[i]);
            }

            return sentenceLengths;
        }

        private string GenerateSysOutput(
            IList<string> instanceNames
            , IList<FeatureVector> testVectors
            , int[] sentenceLengths
            , out ConfusionMatrix confusionMatrix
            , int gold_i
            )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("%%%%% test data:");

            FeatureVector[][] sentences = new FeatureVector[sentenceLengths.Length][];
            int v_i = 0;
            for (int s_i = 0; s_i < sentenceLengths.Length; s_i++)
            {
                sentences[s_i] = new FeatureVector[sentenceLengths[s_i]];
                for (int j = 0; j < sentenceLengths[s_i]; j++)
                {
                    sentences[s_i][j] = testVectors[v_i];
                    v_i++;
                }
            }
            Debug.Assert(v_i == testVectors.Count);

            // Iterate over each of the sentences.
            v_i = 0;
            confusionMatrix = new ConfusionMatrix(classToClassId.Count);
            for (int s_i = 0; s_i < sentenceLengths.Length; s_i++)
            {
                int[] sysClasses;
                double[] probabilities;
                GetBestTagSequence(sentences[s_i], out sysClasses, out probabilities);

                // Write the sys_output for each word in the sentence:
                for (int w_i = 0; w_i < sentenceLengths[s_i]; w_i++)
                {
                    string instanceName = instanceNames[w_i];
                    int goldClassId = sentences[s_i][w_i].Headers[gold_i];
                    string goldClass = classToClassId[goldClassId];

                    // sysClass is the tag c for the word w according to the best tag sequence found above.
                    int sysClassId = sysClasses[w_i];
                    string sysClass = classToClassId[sysClassId];
                    // prob is the probability of the tag c given the word w FOR the given best tag sequence.
                    double prob = probabilities[w_i];

                    confusionMatrix[goldClassId, sysClassId]++;

                    sb.AppendFormat($"{instanceName} {goldClass} {sysClass} {prob:0.00000}{Environment.NewLine}");

                    v_i++;
                }
            }
            return sb.ToString();
        }

        internal void GetBestTagSequence(IList<FeatureVector> vectors, out int[] sysClasses, out double[] distribution)
        {
            // Since we are working with logarithmic numbers, we want the weight of the root node to be zero.
            var beam = new BeamSearch<IdValuePair<double>>(0D);
            for (int beamDepth = 0; beamDepth < vectors.Count; beamDepth++)
            {
                Debug.Assert(beam.Level[beamDepth].Count > 0);
                foreach (BeamNode<IdValuePair<double>> node in beam.Level[beamDepth])
                {
                    double[] probs_v_c = new double[classToClassId.Count];
                    for (int c_i = 0; c_i < classToClassId.Count; c_i++)
                    {
                        probs_v_c[c_i] = CalculateProbability_v_c(vectors[beamDepth], c_i, node, beamDepth);
                    }
                    StatisticsHelper.NormalizeLogs(probs_v_c, Math.E);

                    // Prune: Idenitify N classes with highest probability:
                    IList<int> topNClasses = SearchHelper.GetMaxNItems(topN, probs_v_c);
                    for (int topN_i = 0; topN_i < topNClasses.Count; topN_i++)
                    {
                        int c_i = topNClasses[topN_i];
                        double prob = probs_v_c[c_i];
                        node.AddNextNode(new IdValuePair<double>(c_i, prob), Math.Log(prob, Math.E) + node.Weight);
                    }
                }
                beam.Prune(topK, beam_size);
            }
            // Repackage the sequence we just received in such a way that the consuming code will find it most digestable.
            var results = beam.GetBestSequence();
            sysClasses = new int[results.Length];
            distribution = new double[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                sysClasses[i] = results[i].Id;
                distribution[i] = results[i].Value;
            }
        }

        /// <summary><para>
        /// Calculates the probability of a <c>vector</c>, given a particular class <c>c_i</c>.
        /// </para><para>
        /// The features for prevT and prevTwoTags are added to the vector's features, dynamically, 
        /// based on values in the <c>BeamSearch</c> structure.
        /// </para></summary>
        private double CalculateProbability_v_c(FeatureVector vector, int c_i, BeamNode<IdValuePair<double>> beamNode, int beamDepth)
        {
            int prevT_f, prevTwoTags_f;
            GetPrevTAndPrevTwoTags(beamNode, beamDepth, out prevT_f, out prevTwoTags_f);
            // Sum the feature values.
            double logProb = classifier._lambda_c[c_i];
            logProb += classifier.CalculateLogProb_c_f(c_i, prevT_f);
            logProb += classifier.CalculateLogProb_c_f(c_i, prevTwoTags_f);
            for (int u_i = 0; u_i < vector.UsedFeatures.Length; u_i++)
            {
                int f_i = vector.UsedFeatures[u_i];
                logProb += classifier.CalculateLogProb_c_f(c_i, f_i);
            }
            return logProb;
        }

        private void GetPrevTAndPrevTwoTags(BeamNode<IdValuePair<double>> beamNode, int beamDepth, out int prevT_f, out int prevTwoTags_f)
        {
            // Find the feature: prevT={t-1}
            string prevT_name = beamDepth - 1 < 0 ? "BOS" : classToClassId[beamNode.Item.Id];
            string prevT_featureName = string.Format("prevT={0}", prevT_name);
            prevT_f = featureToFeatureId[prevT_featureName];

            // Find the feature: prevTwoTags={t-2}+{t-1}:
            string prevTwoTags_name = beamDepth - 2 < 0 ? "BOS" : classToClassId[beamNode.PreviousNode.Item.Id];
            string prevTwoTags_featureName = string.Format("prevTwoTags={0}+{1}", prevTwoTags_name, prevT_name);
            prevTwoTags_f = featureToFeatureId[prevTwoTags_featureName];
        }
    }
}
