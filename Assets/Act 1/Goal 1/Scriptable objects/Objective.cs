using System.Linq;

public class Objective
{
    public ObjectiveData data;
    public TaskStep[] steps;

    public bool IsComplete => steps.All(s => s.IsComplete);

    public Objective(ObjectiveData data)
    {
        this.data = data;
        steps = data.steps.Select(s => new TaskStep(s)).ToArray();
    }
}