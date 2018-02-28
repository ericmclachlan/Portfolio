using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ericmclachlan.Portfolio
{
    /// <summary>
    /// This is a classifier that uses a SVM (support vector machine) to determine an optimal hyperplane 
    /// for discriminating between two classes.
    /// </summary>
    public class SVMClassifier : Classifier
    {
        public enum SVMType
        {
            c_svc
        }
        public enum KernelType
        {
            linear,
            polynomial,
            RBF,
            sigmoid
        }

        public class SVMHeader
        {
            // Members

            public SVMType svm_type;
            public KernelType kernel_type;
            public int nr_class;
            public int total_sv;
            public double rho;
            public string[] labels;
            public int[] nr_sv;

            // Methods

            public static SVMHeader Load(IList<string> lines)
            {
                SVMHeader header = new SVMHeader();

                header.svm_type = (SVMType)Enum.Parse(typeof(SVMType), ReadExpectedParameter("svm_type", lines[0]));
                header.kernel_type = (KernelType)Enum.Parse(typeof(KernelType), ReadExpectedParameter("kernel_type", lines[1]));
                header.nr_class = int.Parse(ReadExpectedParameter("nr_class", lines[2]));
                header.total_sv = int.Parse(ReadExpectedParameter("total_sv", lines[3]));
                header.rho = double.Parse(ReadExpectedParameter("rho", lines[4]));
                header.labels = TextHelper.SplitOnWhitespace(ReadExpectedParameter("label", lines[5]));
                header.nr_sv = new int[header.labels.Length];
                string[] nr_sv_asText = TextHelper.SplitOnWhitespace(ReadExpectedParameter("nr_sv", lines[6]));
                Debug.Assert(nr_sv_asText.Length == header.labels.Length);
                for (int i = 0; i < nr_sv_asText.Length; i++)
                {
                    header.nr_sv[i] = int.Parse(nr_sv_asText[i]);
                }
                Debug.Assert(lines[7] == "SV");
                return header;
            }
        }

        private SVMHeader _header;
        private double[] _alphas;

        protected SVMClassifier(SVMHeader header, List<FeatureVector> trainingVectors, double[] alphas)
            : base(trainingVectors, noOfClasses:2)
        {
            _alphas = alphas;
            _header = header;
            // Nothing else needs to be done for now.
        }

        protected override double[] Test(FeatureVector vector)
        {
            return null;
            //throw new NotImplementedException();
        }

        protected override void Train()
        {
            //throw new NotImplementedException();
        }

        internal static Classifier LoadModel(string model_file, ValueIdMapper<string> classToclassId, ValueIdMapper<string> featureToFeatureId)
        {
            List<string> lines = new List<string>(File.ReadAllLines(model_file));

            // Read the model file header:
            SVMHeader svmHeader = SVMHeader.Load(lines);

            // Remove the header lines:
            lines.RemoveRange(0, 8);

            // Read each of the support vectors:
            int noOfHeadersColumns = 1;
            int alpha_i = 0;
            ValueIdMapper<string>[] headerToHeaderIds;
            featureToFeatureId = new ValueIdMapper<string>();
            headerToHeaderIds = new ValueIdMapper<string>[noOfHeadersColumns];
            for (int header_i = 0; header_i < noOfHeadersColumns; header_i++)
            {
                headerToHeaderIds[header_i] = new ValueIdMapper<string>();
            }

            ValueIdMapper<string> alphasToAlphaId = headerToHeaderIds[alpha_i];
            int[][] headers;
            var vectors = FeatureVector.LoadFromSVMLight(lines, featureToFeatureId, headerToHeaderIds, noOfHeadersColumns, out headers, FeatureType.Continuous, featureDelimiter: ':', isSortRequiredForFeatures: false);
            double[] alphas = new double[vectors.Count];
            for (int i = 0; i < alphas.Length; i++)
            {
                alphas[i] = Convert.ToDouble(alphasToAlphaId[headers[alpha_i][i]]);
            }

            return new SVMClassifier(svmHeader, vectors, alphas);
        }

        private static string ReadExpectedParameter(string name, string text)
        {
            int index = text.IndexOf(name);
            Debug.Assert(index >= 0);
            Debug.Assert(char.IsWhiteSpace(text[index + name.Length]));
            return text.Substring(index + name.Length + 1);
        }
    }
}
