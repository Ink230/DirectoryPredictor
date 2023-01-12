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

        protected override void BeginProcessing()
        {
            SessionState.PSVariable.Set("FileExtensions", FileExtensions);
        }
    }
}

public enum FileExtensions
{
    Include,
    Exclude
}
