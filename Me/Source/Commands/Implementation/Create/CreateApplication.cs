using System.Diagnostics;

namespace Me;

/* [TODO] 
 * Create class that will describe application structure
 * need it for comunication between defferent commands
 */
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

        var checkIfAppAlreadyExists = new Step($"Register {appName}", () =>
        {
            var pathToAppData = fileManager.GetAppDataPath();

            var appRegistry = AppRegistry.Load();

            var hasInRegistry = appRegistry.Contains(appName);
            var folderExists = fileManager.IsDirectiryExists(appHomeFolder);

            if (hasInRegistry && folderExists)
            {
                Print.Error($"Application with name: {appHomeFolder} already exists!");
                return false;
            }
            if (folderExists && !hasInRegistry)
            {
                Print.Error($"Couldn't create application! Folder: {appPath} already exists! Choose another path" +
                    $" or cleanup the existing one");
                return false;
            }

            if (!folderExists && hasInRegistry)
            {
                Print.Error($"Applicatoin with name: {appName} already defined, but folder is empty. It seems like " +
                    $"it has been deleted or moved");
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
            var homeFolderCreationResult = fileManager.CreateDirectory(appHomeFolder);
            if (homeFolderCreationResult != IOResultEnum.Success)
            {
                Print.Error($"Failed to create home folder for application: {appName}");
                return false;
            }
            var subDirectories = new string[]
            {
                "tests",
                "automation",
                "services"
            };
            foreach (var subdir in subDirectories)
            {
                var subDirPath = Path.Combine(appHomeFolder, subdir);
                var opResult = fileManager.CreateDirectory(subDirPath);
                if (opResult != IOResultEnum.Success)
                {
                    Print.Error($"Failed to create subdirectory {subdir} for app {appName}");
                    return false;
                }
            }

            return true;
        });

        var initGitRepository = new Step("Creating local git repository", () => 
        {
            Git.Init(appHomeFolder);
            return true;
        });

        var allOperations = new MultiStepOperation("Create microservice application"
            , ExecutionModeEnum.Immediate
            , checkIfAppAlreadyExists
            , createDirectiryStructure
            , initGitRepository
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
