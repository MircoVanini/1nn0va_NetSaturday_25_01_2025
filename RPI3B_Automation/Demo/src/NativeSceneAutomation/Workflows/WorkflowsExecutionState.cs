namespace NativeSceneAutomation.Workflows;

public class WorkflowsExecutionState
{
    private Lock _lock = new Lock();

    public string? CurrentWorkflowId
    {
        get;
        set
        {
            lock(_lock)
            {
                field = value;
            }
        }
    }
}
