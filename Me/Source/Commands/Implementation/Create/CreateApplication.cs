using System.Collections.Specialized;
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

        var createDirectiryStructure = new Step("Create directory structure folder", () =>
        {
            var pathToAppRoot = Path.Combine(appPath, appName);
            var result = Application.FilesManager.CreateDirectory(pathToAppRoot);
            if (result == IOResultEnum.AlreadyExist)
            {
                Print.Error($"Failed to create root folder: folder with name: '{appName}' already exists" +
                    $" at {appPath}");
                return false;
            }
            return result == IOResultEnum.Success;
        });

        var empty = new Step("empty one", () => 
        {
            Thread.Sleep(1000);
            return true;
        });

        var allOperations = new MultiStepOperation("Create microservice application"
            , ExecutionModeEnum.StepByStep
            , createDirectiryStructure
            , empty
            , empty
            , empty
            , empty
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
