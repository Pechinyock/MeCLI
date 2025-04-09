using System.Diagnostics;

namespace Me;

internal static class CreateApplication
{
    private static readonly string[] _availableTypes = new string[]
    {
        "microservices"
    };

    private static void CreateMicroserviceApplication(Dictionary<string, string> parameters, string[] args)
    {
        var appName = parameters["name"];
        var appPath = parameters["path"];

        var fileManager = Application.FilesManager;
        var appHomeFolder = Path.Combine(appPath, appName);

        var checkIfAppAlreadyExists = new Step($"Check if app already exists. App home folder: {appPath}; app name: {appName}", () => 
        {
            var folderExists = fileManager.IsDirectiryExists(appHomeFolder);
            var pathToAppData = fileManager.GetAppDataPath();

            var appRegistry = AppRegistry.Load();
            if (appRegistry.Contains(appName))
            {
                /* get app from config get its path check if directory exists
                 * if exists: you can't create is is already exists
                 * if not: could be old project and source was deleted, use applicatio restore to pull source from git
                 *         OR delete app from registry...
                 */
                Print.Error("Config contains it already");
                return false;
            }
            if (folderExists)
            {
                Print.Error("App already exists!");
                return false;
            }

            var newApp = new ApplicationInfoModel() { Name = appName
                , HomePath = appPath
                , Type = _availableTypes[0]
                , RepositoryRemoteURL = "" 
            };

            appRegistry.Add(newApp);
            appRegistry.Save();
            return true;
        });

        var createDirectiryStructure = new Step("Create directory structure folder", () =>
        {
            var result = fileManager.CreateDirectory(appHomeFolder);
            return result == IOResultEnum.Success;
        });

        var allOperations = new MultiStepOperation("Create microservice application"
            , ExecutionModeEnum.Immediate
            , checkIfAppAlreadyExists
            , createDirectiryStructure
        );

        var interactivePannel = new InteractivePannel(allOperations);
        Print.InteractivePannel(interactivePannel);
        allOperations.Proceed();
    }

    private static readonly Dictionary<string, Action<Dictionary<string, string>, string[]>> _createTypeMap = new()
    {
        { _availableTypes[0], CreateMicroserviceApplication },
    };

    public static void Do(Dictionary<string, string> parameters, string[] args) 
    {
        Debug.Assert(parameters is not null);
        Debug.Assert(parameters.Count > 0);

        var type = parameters["type"];

        if (!_createTypeMap.ContainsKey(type))
        {
            Print.Error($"Unknown type: {type}");
            Print.Info("List of available application types: ");
            foreach (var availabelType in _availableTypes) 
            {
                Print.Info($"     - {availabelType}");
            }
            return;
        }

        _createTypeMap[type].Invoke(parameters, args);
    }
}
