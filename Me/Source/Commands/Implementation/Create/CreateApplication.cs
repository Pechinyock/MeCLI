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
            if (folderExists) 
            {
                Print.Error("App already exists!");
                return false;
            }
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
