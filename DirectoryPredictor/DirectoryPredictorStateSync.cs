using System.Management.Automation.Runspaces;
using System.Management.Automation;
using static DirectoryPredictor.Cmdlets;

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
      
        if (SetDirectoryPredictorOption._options.OptionsUsed)
        {
            var tempFile = (FileExtensions)source.SessionStateProxy.GetVariable("FileExtensions");
            _includeFileExtensions = tempFile == FileExtensions.Include || tempFile == FileExtensions.None;
            _runspace.SessionStateProxy.PSVariable.Set("FileExtensions", tempFile);

            var tempResults = (int)source.SessionStateProxy.GetVariable("ResultsLimit");
            _resultsLimit = tempResults;
            _runspace.SessionStateProxy.PSVariable.Set("ResultsLimit", tempResults);
        }
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