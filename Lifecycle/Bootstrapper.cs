using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArisenEditorFramework.Lifecycle;

public class Bootstrapper
{
    private readonly List<IBootStep> _steps = new();
    public event Action<string, string, double> ProgressChanged;

    public void AddStep(IBootStep step) => _steps.Add(step);

    public async Task<BootContext> RunAsync(string projectPath, Action<IBootStep> callback = null)
    {
        var context = new BootContext { ProjectPath = projectPath };
        double totalSteps = _steps.Count;
        
        for (int i = 0; i < _steps.Count; i++)
        {
            var step = _steps[i];
            double progress = (i / totalSteps) * 100.0;
            ProgressChanged?.Invoke(step.Name, step.Description, progress);

            try
            {
                callback?.Invoke(step);
                await step.ExecuteAsync(context);
                if (!context.Success) break;
            }
            catch (Exception ex)
            {
                context.Success = false;
                context.ErrorMessage = ex.Message;
                break;
            }
        }

        if (context.Success)
        {
            ProgressChanged?.Invoke("Completed", "Engine is ready.", 100.0);
        }

        return context;
    }
}
