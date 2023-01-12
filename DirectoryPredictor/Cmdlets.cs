using System.Management.Automation;

namespace DirectoryPredictor;

public class Cmdlets
{
    [Cmdlet("Set", "DirectoryPredictorOption")]
    public class SetDirectoryPredictorOption : PSCmdlet
    {
        [Parameter]
        public FileExtensions FileExtensions
        {
            get => _fileExtensions.GetValueOrDefault();
            set => _fileExtensions = value;
        }
        internal FileExtensions? _fileExtensions = FileExtensions.Include;

        //Bug 1 - Part 2
        [Parameter]
        [ValidateRange(1, 500)]
        public int ResultsLimit
        {
            get => _resultsLimit.GetValueOrDefault();
            set => _resultsLimit = value;
        }
        internal int? _resultsLimit = 10;

        protected override void BeginProcessing()
        {
            SessionState.PSVariable.Set("FileExtensions", FileExtensions);
            SessionState.PSVariable.Set("ResultsLimit", ResultsLimit); //Bug 1 - Part 3 - Last
        }
    }
}

public enum FileExtensions
{
    Include,
    Exclude
}
