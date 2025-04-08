using System.Text.Json;

namespace Me;

internal class ApplicationRegistryModel : ISerializable
                                        , IDeserializeable<ApplicationRegistryModel>
{
    public List<ApplicationInfoModel> ApplicationInfoModels;

    public ApplicationRegistryModel()
    {
        ApplicationInfoModels = new List<ApplicationInfoModel>();
    }

    public ApplicationRegistryModel(List<ApplicationInfoModel> appInfous)
    {
        ApplicationInfoModels = appInfous;
    }

    public string Serialize(ModelRepresentation format)
    {
        switch (format)
        {
            case ModelRepresentation.JSON:
                return ToJson();
        }

        return string.Empty;
    }

    public ApplicationRegistryModel Deserialze(ModelRepresentation format, string text)
    {
        switch (format)
        {
            case ModelRepresentation.JSON:
                return FromJson(text);
        }
        return null;
    }

    private string ToJson() 
    {
        var serialized = JsonSerializer.Serialize(ApplicationInfoModels);
        return serialized;
    }

    private ApplicationRegistryModel FromJson(string text) 
    {
        var listOfProjects = JsonSerializer.Deserialize<List<ApplicationInfoModel>>(text);
        var result = new ApplicationRegistryModel(listOfProjects);
        return result;
    }

}
