using System.Diagnostics;

namespace Me;

/* [ISSUE]
 * It seems like Appregistry and ApplicationRegistryMode should be same class...
 */
internal static class AppRegistry
{
    private const string REGISTRY_FILE_NAME = "application-registry.json";

    public static ApplicationRegistryModel Load() 
    {
        var fileManager = Application.FilesManager;
        var pathToHome = fileManager.GetAppDataPath();
        var pathToConfig = Path.Combine(pathToHome, REGISTRY_FILE_NAME);
        if (!fileManager.IsFileExists(pathToConfig)) 
        {
            var newRegistry = new ApplicationRegistryModel();
            var newRegistryJson = newRegistry.Serialize(ModelRepresentation.JSON);
            fileManager.WriteAllText(pathToConfig, newRegistryJson);
            return newRegistry;
        }
        var jsonText = fileManager.ReadAllText(pathToConfig);

        var data = new ApplicationRegistryModel();
        var result = data.Deserialze(ModelRepresentation.JSON, jsonText);

        return result;
    }

    public static void Add(this ApplicationRegistryModel registry, ApplicationInfoModel newApp) 
    {
        registry.ApplicationInfoModels.Add(newApp);
    }

    public static void Save(this ApplicationRegistryModel registry) 
    {
        var newData = registry.Serialize(ModelRepresentation.JSON);

        var fileManager = Application.FilesManager;
        var pathToHome = fileManager.GetAppDataPath();
        var pathToConfig = Path.Combine(pathToHome, REGISTRY_FILE_NAME);
        fileManager.WriteAllText(pathToConfig, newData);
    }

    public static bool Contains(this ApplicationRegistryModel registry, string appName) 
    {
        foreach (var item in registry.ApplicationInfoModels) 
        {
            if(item.Name == appName)
                return true;
        }
        return false;
    }
}
