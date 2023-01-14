using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace DirectoryPredictor;

public partial class DirectoryPredictor
{
    private void SyncRunspaceState(object? sender, RunspaceAvailabilityEventArgs e)
    {
        if (sender is null || e.RunspaceAvailability != RunspaceAvailability.Available)
        {
            return;
        }

        var pwshRunspace = (Runspace)sender;
        pwshRunspace.AvailabilityChanged -= SyncRunspaceState;

        try
        {
            using var ps = PowerShell.Create();
            ps.Runspace = pwshRunspace;
            
            SyncCurrentValues(pwshRunspace);
        }
        finally
        {
            pwshRunspace.AvailabilityChanged += SyncRunspaceState;
        }
    }

    private void SyncCurrentValues(Runspace source)
    {
        PathInfo currentPath = source.SessionStateProxy.Path.CurrentLocation;
        _runspace.SessionStateProxy.Path.SetLocation(currentPath.Path);
    }

    private void RegisterEvents()
    {
        Runspace.DefaultRunspace.AvailabilityChanged += SyncRunspaceState;
    }

    private void UnregisterEvents()
    {
        Runspace.DefaultRunspace.AvailabilityChanged -= SyncRunspaceState;
    }
}