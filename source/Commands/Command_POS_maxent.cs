using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ericmclachlan.Portfolio
{
    internal class Command_POS_maxent : ICommand
    {
        // Members

        public string CommandName { get { return "beamsearch_pos_maxent"; } }

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
            // Read the boundaries:
            int[] sentenceLengths = ReadBoundaryFile(boundary_file);

            // Read the classifier model:
            classifier = MaxEntPOSClassifier.LoadModel(File.ReadAllText(model_file), out classToClassId, out featureToFeatureId);

            // Read the vectors:
            List<string> instanceNames;
            Func<int, int> transformationF = (i) => { return i; };
            var testVectors = FeatureVector.FromModifiedSVMLight(File.ReadAllText(test_data), featureToFeatureId, classToClassId, transformationF, out instanceNames);
            
            // Generate sys_output:
            var confusionMatrix = GenerateSysOutput(sys_output, instanceNames, testVectors, sentenceLengths);

            // Generate acc:
            Console.WriteLine($"class_num={classToClassId.Count} feat_num={featureToFeatureId.Count}");
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

        private ConfusionMatrix GenerateSysOutput(
            string sys_output
            , IList<string> instanceNames
            , IList<FeatureVector> testVectors
            , int[] sentenceLengths
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
            ConfusionMatrix confusionMatrix = new ConfusionMatrix(classToClassId.Count);
            for (int s_i = 0; s_i < sentenceLengths.Length; s_i++)
            {
                double[] prob_c_w;
                int[] c_w;
                GetBestTagSequence(sentences[s_i], out prob_c_w, out c_w);

                // Write the sys_output for each word in the sentence:
                for (int w_i = 0; w_i < sentenceLengths[s_i]; w_i++)
                {
                    string instanceName = instanceNames[w_i];
                    int goldClass = sentences[s_i][w_i].GoldClass;
                    string goldClassName = classToClassId[goldClass];

                    // sysClass is the tag c for the word w according to the best tag sequence found above.
                    int sysClass = c_w[w_i];
                    string sysClassName = classToClassId[sysClass];
                    confusionMatrix[goldClass, sysClass]++;
                    // prob is the probability of the tag c given the word w FOR the given best tag sequence.
                    double prob = prob_c_w[w_i];

                    sb.AppendFormat($"{instanceName} {goldClassName} {sysClassName} {prob:0.00000}{Environment.NewLine}");
                }
            }
            File.WriteAllText(sys_output, sb.ToString());
            return confusionMatrix;
        }

        internal void GetBestTagSequence(IList<FeatureVector> words, out double[] prob_c_w, out int[] c_w)
        {
            c_w = new int[words.Count];
            prob_c_w = new double[words.Count];

            double[][] probs_v_c = new double[words.Count][];
            for (int v_i = 0; v_i < words.Count; v_i++)
            {
                probs_v_c[v_i] = classifier.Classify(words[v_i]);

                string prevT_name = v_i - 1 < 0 ? "BOS" : classToClassId[c_w[v_i - 1]];
                string prevT_featureName = string.Format("prevT={0}", prevT_name);
                string prevTwoTags_name = v_i - 2 < 0 ? "BOS" : classToClassId[c_w[v_i - 2]];
                string prevTwoTags_featureName = string.Format("prevTwoTags={0}+{1}", prevTwoTags_name, prevT_name);
                var prevT_f = featureToFeatureId[prevT_featureName];
                var prevTwoTags_f = featureToFeatureId[prevTwoTags_featureName];

                for (int c_i = 0; c_i < classToClassId.Count; c_i++)
                {
                    double logProb = Math.Log(probs_v_c[v_i][c_i], Math.E);
                    logProb += classifier.CalculateLogProb_c_f(c_i, prevT_f);
                    logProb += classifier.CalculateLogProb_c_f(c_i, prevTwoTags_f);
                    probs_v_c[v_i][c_i] = Math.Pow(Math.E, logProb);
                }
                StatisticsHelper.Normalize(probs_v_c[v_i]);
                int bestClass = StatisticsHelper.ArgMax(probs_v_c[v_i]);
                c_w[v_i] = bestClass;
                prob_c_w[v_i] = probs_v_c[v_i][bestClass];
            }
        }
    }
}
