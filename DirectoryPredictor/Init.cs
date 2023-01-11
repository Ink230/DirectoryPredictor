using System.Management.Automation.Subsystem.Prediction;
using System.Management.Automation;
using System.Management.Automation.Subsystem;

namespace DirectoryPredictor;

public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
{
    private const string Identifier = "843b51d0-55c8-4c1a-8116-f0728d419316";

    public void OnImport()
    {
        var predictor = new DirectoryPredictor(Identifier);
        SubsystemManager.RegisterSubsystem<ICommandPredictor, DirectoryPredictor>(predictor);
    }

    public void OnRemove(PSModuleInfo psModuleInfo)
    {
        SubsystemManager.UnregisterSubsystem<ICommandPredictor>(new Guid(Identifier));
    }
}
