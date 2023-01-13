using System.Management.Automation;

namespace DirectoryPredictor;

public struct DirectoryPredictorOptions
{
    public bool OptionsUsed;
    public FileExtensions FileExtensions;
    public int ResultsLimit;
}
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
        internal FileExtensions? _fileExtensions;

        [Parameter]
        [ValidateRange(1, 500)]
        public int ResultsLimit
        {
            get => _resultsLimit.GetValueOrDefault();
            set => _resultsLimit = value;
        }
        internal int? _resultsLimit;

        public static DirectoryPredictorOptions _options = new DirectoryPredictorOptions
        {
            OptionsUsed = false,
            FileExtensions = FileExtensions.None,
            ResultsLimit = 10,
        };

        protected override void ProcessRecord()
        {
            _options.OptionsUsed = true;

            if (FileExtensions != FileExtensions.None)
                _options.FileExtensions = FileExtensions;

            if (ResultsLimit > 0)
                _options.ResultsLimit = ResultsLimit;
        }

        protected override void EndProcessing()
        {
            SessionState.PSVariable.Set("FileExtensions", _options.FileExtensions);
            SessionState.PSVariable.Set("ResultsLimit", _options.ResultsLimit);
        }
    }
}
