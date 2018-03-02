namespace ericmclachlan.Portfolio
{
    public interface ISaveModel
    {
        void SaveModel(string filename, TextIdMapper classToClassId, TextIdMapper featureToFeatureId);
    }
}
