using Microsoft.CodeAnalysis;
using System.Management.Automation;

namespace DirectoryPredictor;

public sealed class DirectoryPredictorOptions
{
    private static readonly DirectoryPredictorOptions _instance = new DirectoryPredictorOptions();
    public static DirectoryPredictorOptions Instance => _instance;
    static DirectoryPredictorOptions() { }
    private DirectoryPredictorOptions() { }

    public FileExtensions FileExtensions = FileExtensions.None;

    public int? ResultsLimit = 10;
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
        internal FileExtensions? _fileExtensions = FileExtensions.None;

        [Parameter]
        [ValidateRange(1, 500)]
        public int ResultsLimit
        {
            get => _resultsLimit.GetValueOrDefault();
            set => _resultsLimit = value;
        }
        internal int? _resultsLimit;

        private static readonly DirectoryPredictorOptions _options = DirectoryPredictorOptions.Instance;
        public static DirectoryPredictorOptions Options => _options;

        protected override void EndProcessing()
        {
            if (FileExtensions != FileExtensions.None)
            {
                Options.FileExtensions = FileExtensions;
            }
            if (ResultsLimit > 0)
            {
                Options.ResultsLimit = ResultsLimit;
            }
        }
    }
}
