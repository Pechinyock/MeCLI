using System.Diagnostics;

namespace Me;

internal class Step : IStep
{
    public string GetName() => _name;
    public bool Exec() => _operation.Invoke();

    private readonly string _name;
    private readonly Func<bool> _operation;

    public Step(string name, Func<bool> operation)
    {
        Debug.Assert(operation is not null);
        Debug.Assert(!String.IsNullOrEmpty(name));

        _name = name;
        _operation = operation;
    }

}

internal class StepOneParam<TParam> : IStep
{
    public string GetName() => _name;
    public bool Exec() => _operation.Invoke(_param);

    private readonly TParam _param;
    private readonly string _name;
    private readonly Func<TParam, bool> _operation;

    public StepOneParam(string name, Func<TParam, bool> operation, TParam paramValue)
    {
        Debug.Assert(_param is not null);
        Debug.Assert(!String.IsNullOrEmpty(name));

        _operation = operation;
        _name = name;
        _param = paramValue;
    }
}