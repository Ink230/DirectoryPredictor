using System.Management.Automation;

namespace DirectoryPredictor;

public sealed class DirectoryPredictorOptions
{
    private static readonly DirectoryPredictorOptions _options = new DirectoryPredictorOptions();
    public static DirectoryPredictorOptions Options => _options;
    static DirectoryPredictorOptions() { }
    private DirectoryPredictorOptions() { }

    public FileExtensions FileExtensions { get; set; } = FileExtensions.None;

    public bool IsEnabledFileExtensions()
    {
        return (FileExtensions == FileExtensions.None || FileExtensions == FileExtensions.Include);
    }

    public DirectoryMode DirectoryMode { get; set; } = DirectoryMode.None;

    public SortMixedResults SortMixedResults { get; set; } = SortMixedResults.None;

    public ExtensionMode ExtensionMode { get; set; } = ExtensionMode.None;

    public bool IsEnabledExtensionMode()
    {
        if (ExtensionMode == ExtensionMode.Enabled) return true;

        return false;
    }

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
        public DirectoryMode DirectoryMode
        {
            get => _directoryMode.GetValueOrDefault();
            set => _directoryMode = value;
        }
        internal DirectoryMode? _directoryMode = DirectoryMode.None;

        [Parameter]
        public SortMixedResults SortMixedResults
        {
            get => _sortMixedResults.GetValueOrDefault();
            set => _sortMixedResults = value;
        }
        internal SortMixedResults? _sortMixedResults = SortMixedResults.None;

        [Parameter]
        public ExtensionMode ExtensionMode
        {
            get => _extensionMode.GetValueOrDefault();
            set => _extensionMode = value;
        }
        internal ExtensionMode? _extensionMode = ExtensionMode.None;

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

            if (DirectoryMode != DirectoryMode.None)
            {
                Options.DirectoryMode = DirectoryMode;
            }

            if (SortMixedResults != SortMixedResults.None)
            {
                Options.SortMixedResults = SortMixedResults;
            }

            if (ExtensionMode != ExtensionMode.None)
            {
                Options.ExtensionMode = ExtensionMode;
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
