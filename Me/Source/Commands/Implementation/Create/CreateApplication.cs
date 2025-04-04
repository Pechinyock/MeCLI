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
        /* Firstly create every single directory */
        var createRoot = new Step("Create root folder", () =>
        {
            var result = Application.FilesManager.CreateDirectory(Path.Combine(appPath, appName));
            Thread.Sleep(3000);
            return result == IOResultEnum.Success;
        });

        var empty = new Step("Create test folder", () =>
        {
            Thread.Sleep(2000);
            return true;
        });

        var empty1 = new Step("Create automation folder", () =>
        {
            Thread.Sleep(2000);
            Print.Error("Failed to create automation folder: you don't have permissions");
            return false;
        });

        var allOperations = new MultiStepOperation("Create microservice application"
            , ExecutionModeEnum.Immediate
            , createRoot
            , empty
            , empty1
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
