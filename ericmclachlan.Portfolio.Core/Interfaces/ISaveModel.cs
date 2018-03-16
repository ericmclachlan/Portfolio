namespace ericmclachlan.Portfolio.Core
{
    public interface ISaveModel
    {
        void SaveModel(string filename, TextIdMapper classToClassId, TextIdMapper featureToFeatureId);
    }
}
