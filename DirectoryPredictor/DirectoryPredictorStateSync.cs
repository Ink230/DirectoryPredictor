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

        var tempFileExtensions = source.SessionStateProxy.GetVariable("FileExtensions");

        if (tempFileExtensions != null)
        {
            _includeFileExtensions = (FileExtensions)tempFileExtensions == FileExtensions.Include;
            _runspace.SessionStateProxy.PSVariable.Set("FileExtensions", (FileExtensions)tempFileExtensions);
        }

        //Bug 1 - Part 1 - Explanation
        /*
         * A second Set-DirectoryPredictorOption flag is causing the first flag (-FileExtensions) to be ignored.
         * With Set-DirectoryPredictorOption -ResultsLimit 4 set, alongside -FileExtensions Exclude
         * the file extensions still show up (they shouldn't) but the results are limited to 4.
         * 
         * With the -ResultsLimit commented out, -FileExtensions behaviour works as intended.
         * 
         * Unclear where the issue arrives from.
         * 
         * • Race condition / thread unsafe conditions / need a lock?
         * • Improper state sharing of Cmdlet option parameters to the DirectoryPredictor _runspace?
         */ 
        var tempResultsLimit = source.SessionStateProxy.GetVariable("ResultsLimit");

        if (tempResultsLimit != null)
        {
            _resultsLimit = (int)tempResultsLimit;
            _runspace.SessionStateProxy.PSVariable.Set("ResultsLimit", (int)tempResultsLimit);
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