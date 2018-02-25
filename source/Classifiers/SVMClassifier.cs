using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ericmclachlan.Portfolio
{
    public class SVMClassifier : Classifier
    {
        protected SVMClassifier(List<FeatureVector> trainingVectors, int noOfClasses) 
            : base(trainingVectors, noOfClasses)
        {
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

        internal static Classifier LoadModel(string model_file, out ValueIdMapper<string> classToclassId, out ValueIdMapper<string> featureToFeatureId)
        {
            classToclassId = new ValueIdMapper<string>();
            featureToFeatureId = new ValueIdMapper<string>();
            return null;// new SVMClassifier();
        }
    }
}
