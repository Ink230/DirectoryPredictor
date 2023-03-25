using System.Management.Automation;

namespace DirectoryPredictor;

public sealed class DirectoryPredictorOptions
{
    private static readonly DirectoryPredictorOptions _options = new DirectoryPredictorOptions();
    public static DirectoryPredictorOptions Options => _options;
    static DirectoryPredictorOptions() { }
    private DirectoryPredictorOptions() { }

    public FileExtensions FileExtensions { get; set; } = FileExtensions.None;

    public bool IncludeFileExtensions()
    {
        return (FileExtensions == FileExtensions.None || FileExtensions == FileExtensions.Include);
    }

    public bool ShowFolders { get; set; } = false;

    public int? ResultsLimit { get; set; } = 10;

    public string IgnoreCommands { get; set; } = string.Empty;

    public string[] GetIgnoreCommands()
    {
        return IgnoreCommands.Split(',');
    }
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
        public bool ShowFolders
        {
            get => _showFolders.GetValueOrDefault();
            set => _showFolders = value;
        }
        internal bool? _showFolders = false;

        [Parameter]
        [ValidateRange(1, 500)]
        public int ResultsLimit
        {
            get => _resultsLimit.GetValueOrDefault();
            set => _resultsLimit = value;
        }
        internal int? _resultsLimit;

        [Parameter]
        [ValidateLength(0, 1000)]
        public string IgnoreCommands
        {
            get => _ignoreCommands ?? string.Empty;
            set => _ignoreCommands = value;
        }
        internal string? _ignoreCommands;

        private static readonly DirectoryPredictorOptions _options = DirectoryPredictorOptions.Options;
        public static DirectoryPredictorOptions Options => _options;

        protected override void EndProcessing()
        {
            if (FileExtensions != FileExtensions.None)
            {
                Options.FileExtensions = FileExtensions;
            }

            if (ShowFolders)
            {
                Options.ShowFolders = ShowFolders;
            }

            if (ResultsLimit > 0)
            {
                Options.ResultsLimit = ResultsLimit;
            }

            if (!string.IsNullOrWhiteSpace(IgnoreCommands))
            {
                Options.IgnoreCommands = IgnoreCommands;
            }
        }
    }
}
