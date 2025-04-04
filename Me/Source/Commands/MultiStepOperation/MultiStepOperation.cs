using System.Collections;
using System.Diagnostics;

namespace Me;

public enum ExecutionModeEnum 
{
    Immediate = 0,
    StepByStep = 1
}

public class MultiStepOperation : IEnumerable<IStep>, IDisposable
{
    public string Title => _title;

    public event Action<int, string> OnStepStarting;
    public event Action<int, string> OnStepCompleted;
    public event Action<int, string> OnStepFailed;
    public event Action OnStepByStepWaited;

    public int StepsCount => _steps.Length;
    public IStep this[int index] => _steps[index];
    public IEnumerator<IStep> GetEnumerator() 
    {
        foreach (var step in _steps) 
        {
            yield return step;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => _steps.GetEnumerator();


    private readonly IStep[] _steps;
    private readonly ExecutionModeEnum _executionMode;
    private readonly string _title;

    public MultiStepOperation(string title, ExecutionModeEnum mode, params IStep[] steps)
    {
        Debug.Assert(steps is not null);
        Debug.Assert(steps.Length > 0);
        Debug.Assert(!String.IsNullOrEmpty(title));

        _steps = steps;
        _executionMode = mode;
        _title = title;
    }

    public void Proceed() 
    {
        for(int i =0; i < _steps.Length; ++i)
        {
            if (_executionMode == ExecutionModeEnum.StepByStep)
            {
                OnStepByStepWaited?.Invoke();
            }
            var currentStepName = _steps[i].GetName();
            OnStepStarting?.Invoke(i, currentStepName);

            if (!_steps[i].Exec()) 
            {
                OnStepFailed?.Invoke(i, currentStepName);
                Dispose();
                return;
            }

            OnStepCompleted?.Invoke(i, currentStepName);
        }

        Dispose();
    }

    public void Dispose()
    {
        if(OnStepStarting is not null)
            OnStepStarting -= OnStepStarting;

        if(OnStepCompleted is not null)
            OnStepCompleted -= OnStepCompleted;

        if(OnStepFailed is not null)
            OnStepFailed -= OnStepFailed;

        if(OnStepByStepWaited is not null)
            OnStepByStepWaited -= OnStepByStepWaited;
    }
}