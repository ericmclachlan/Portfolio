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
    public abstract class SVMClassifier : Classifier
    {
        // TODO: Rename SVMClassifier to LibSVMClassifier.
        public enum LibSVM_KernelType
        {
            linear,
            polynomial,
            rbf,
            sigmoid
        }

        private readonly double Rho;
        protected readonly double[] Weights;


        protected SVMClassifier(List<FeatureVector> modelVectors, double[] weights, double rho)
            : base(modelVectors, noOfClasses:2)
        {
            this.Rho = rho;
            this.Weights = weights;
        }

        protected override int Test(FeatureVector vector, out double[] details)
        {
            double sum = 0;
            for (int i = 0; i < TrainingVectors.Count; i++)
            {
                // Consider each feature defined for either vector OR TrainingVectors[i]:
                sum += (Weights[i] * KernelFunc(vector, TrainingVectors[i]));
            }
            sum -= Rho;

            details = new double[] { sum };

            // Return the system classification as a distribution.
            if (sum > 0)
                return 0;
            else if (sum < 0)
                return 1;
            else
            {
                Console.Error.WriteLine("Warning: Ambiguous classification of vector. Reverted to first class bias.");
                return 0;
            }
        }

        protected override void Train()
        {
            // There is no training here as the model should be loaded from a file.
        }

        public static Classifier LoadModel(FeatureVectorFile vectorFile_model, TextIdMapper classToclassId, TextIdMapper featureToFeatureId, int alphaColumn_i, TextIdMapper[] headerToHeaderIds)
        {
            // Peek into the file to see what type of SVM model this is:
            int i = 0;
            LibSVM_KernelType kernel_type = LibSVM_KernelType.linear;
            foreach (var line in File.ReadLines(vectorFile_model.Path))
            {
                if (i == 0)
                    Debug.Assert(line.StartsWith("svm_type") && line.EndsWith("c_svc"));
                else if (i == 1)
                    kernel_type = (LibSVM_KernelType)Enum.Parse(typeof(LibSVM_KernelType), line.Substring(line.LastIndexOfAny(TextHelper.WhiteSpace)));
                else
                    break;
                i++;
            }

            // Override the number of header rows according to the model type.
            switch (kernel_type)
            {
                case LibSVM_KernelType.linear: vectorFile_model.NoOfHeaderRows = 8; break;
                case LibSVM_KernelType.polynomial: vectorFile_model.NoOfHeaderRows = 11; break;
                case LibSVM_KernelType.rbf: vectorFile_model.NoOfHeaderRows = 9; break;
                case LibSVM_KernelType.sigmoid: vectorFile_model.NoOfHeaderRows = 10; break;
                default: throw new NotImplementedException();
            }

            // Read each of the support vectors:
            var modelVectors = vectorFile_model.LoadFromSVMLight(featureToFeatureId, headerToHeaderIds, FeatureType.Continuous);

            // Read the model file header:
            double rho = 0;
            double gamma = 0;
            double coef = 0;
            double degree = 0;

            Debug.Assert(vectorFile_model.HeaderRows[vectorFile_model.NoOfHeaderRows - 1] == "SV");
            for (i = 2; i < vectorFile_model.NoOfHeaderRows - 1; i++)
            {
                string line = vectorFile_model.HeaderRows[i];

                // Ignore non-informative meta-data:
                if (line.StartsWith("nr_class") || line.StartsWith("total_sv") || line.StartsWith("label") || line.StartsWith("nr_sv"))
                    continue;

                string text = line.Substring(line.LastIndexOfAny(TextHelper.WhiteSpace));
                if (line.StartsWith("rho"))
                    rho = double.Parse(text);
                else if (line.StartsWith("gamma"))
                    gamma = double.Parse(text);
                else if (line.StartsWith("degree"))
                    degree = double.Parse(text);
                else if (line.StartsWith("coef"))
                    coef = double.Parse(text);
                else
                    throw new NotImplementedException();
            }

            double[] weights = new double[modelVectors.Count];
            for (i = 0; i < weights.Length; i++)
            {
                weights[i] = Convert.ToDouble(headerToHeaderIds[alphaColumn_i][vectorFile_model.Headers[alphaColumn_i][i]]);
            }

            switch (kernel_type)
            {
                case LibSVM_KernelType.linear:
                    return new LibSVMClassifier_Linear(modelVectors, weights, rho);
                case LibSVM_KernelType.polynomial:
                    return new LibSVMClassifier_Polynomial(modelVectors, weights, rho, degree, gamma, coef);
                case LibSVM_KernelType.rbf:
                    return new LibSVMClassifier_RBF(modelVectors, weights, rho, gamma);
                case LibSVM_KernelType.sigmoid:
                    return new LibSVMClassifier_Sigmoid(modelVectors, weights, rho, gamma, coef);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>Returns an indication of similarity between vector <c>v1</c> and <c>v2</c>.</summary>
        protected abstract double KernelFunc(FeatureVector v1, FeatureVector v2);

        private double RBFSimilarity(FeatureVector v1, FeatureVector v2, int f_i)
        {
            return 0D;
        }

        private double SigmoidSimilarity(FeatureVector v1, FeatureVector v2, int f_i)
        {
            return 0D;
        }
    }

    public class LibSVMClassifier_Linear : SVMClassifier
    {
        public LibSVMClassifier_Linear(List<FeatureVector> modelVectors, double[] weights, double rho)
            : base(modelVectors, weights, rho)
        {
            // Nothing else needs to be done.

        }

        /// <summary>
        /// <para>Returns an indication of similarity between vector <c>v1</c> and <c>v2</c>.</para>
        /// <para>In this case, we use a simple linear dot product.</para>
        /// </summary>
        protected override double KernelFunc(FeatureVector v1, FeatureVector v2)
        {
            double result = 0;
            foreach (int f_i in v2.FeatureUnionWith(v2))
                result += v1.Features[f_i] * v2.Features[f_i];
            return result;
        }
    }

    public class LibSVMClassifier_Polynomial : SVMClassifier
    {
        private readonly double Degree;
        private readonly double Gamma;
        private readonly double Coef;

        public LibSVMClassifier_Polynomial(List<FeatureVector> modelVectors, double[] weights, double rho, double degree, double gamma, double coef)
            : base(modelVectors, weights, rho)
        {
            Degree = degree;
            Gamma = gamma;
            Coef = coef;
        }

        /// <summary>
        /// <para>Returns an indication of similarity between vector <c>v1</c> and <c>v2</c>.</para>
        /// <para>In this case, we use a polynomial dot product.</para>
        /// </summary>
        protected override double KernelFunc(FeatureVector v1, FeatureVector v2)
        {
            double result = 0;
            foreach (int f_i in v2.FeatureUnionWith(v2))
                result += v1.Features[f_i] * v2.Features[f_i];
            return Math.Pow((Gamma * result) + Coef, Degree);
        }
    }

    public class LibSVMClassifier_RBF : SVMClassifier
    {
        private readonly double Gamma;

        public LibSVMClassifier_RBF(List<FeatureVector> modelVectors, double[] weights, double rho, double gamma)
            : base(modelVectors, weights, rho)
        {
            Gamma = gamma;
        }

        /// <summary>
        /// <para>Returns an indication of similarity between vector <c>v1</c> and <c>v2</c>.</para>
        /// <para>In this case, we use a radial basis function.</para>
        /// </summary>
        protected override double KernelFunc(FeatureVector v1, FeatureVector v2)
        {
            double result = 0;
            foreach (int f_i in v1.FeatureUnionWith(v2))
            {
                double diff = Math.Abs(v1.Features[f_i] - v2.Features[f_i]);
                result += (diff * diff);
            }
            // Optimization:
            // Here, you would usually square root the result to get the euclidean distance, 
            // but this kernel would ordinarily square the distance anyway.
            // So, here we have just skipped these steps.
            return Math.Pow(Math.E, -1 * Gamma * result);
        }
    }

    public class LibSVMClassifier_Sigmoid : SVMClassifier
    {
        private readonly double Gamma;
        private readonly double Coef;

        public LibSVMClassifier_Sigmoid(List<FeatureVector> modelVectors, double[] weights, double rho, double gamma, double coef)
            : base(modelVectors, weights, rho)
        {
            Gamma = gamma;
            Coef = coef;
        }

        /// <summary>
        /// <para>Returns an indication of similarity between vector <c>v1</c> and <c>v2</c>.</para>
        /// <para>In this case, we use a sigmoid function.</para>
        /// </summary>
        protected override double KernelFunc(FeatureVector v1, FeatureVector v2)
        {
            double result = 0;
            foreach (int f_i in v2.FeatureUnionWith(v2))
                result += v1.Features[f_i] * v2.Features[f_i];
            return Math.Tanh((Gamma * result) + Coef);
        }
    }
}
