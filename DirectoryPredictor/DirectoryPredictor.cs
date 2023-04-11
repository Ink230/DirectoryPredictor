using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Subsystem.Prediction;

namespace DirectoryPredictor;

public partial class DirectoryPredictor : ICommandPredictor, IDisposable
{
    private readonly Guid _guid;
    private Runspace _runspace { get; }

    private DirectoryPredictorOptions Options { get => DirectoryPredictorOptions.Options; }
    private DirectoryMode DirectoryMode { get => Options.DirectoryMode; }
    private SortMixedResults SortMixedResults { get => Options.SortMixedResults; }
    private bool IncludeFileExtensions { get => Options.IsEnabledFileExtensions(); }
    private bool ExtensionMode { get => Options.IsEnabledExtensionMode(); }
    private int ResultsLimit { get => Options.ResultsLimit.GetValueOrDefault(); }
    private string[] IgnoreCommands { get => Options.GetIgnoreCommands(); }
    private Func<string, string> FileNameFormat
    {
        get => IncludeFileExtensions ?
            (file => Path.GetFileName(file).ToLower()) :
            (file => Path.GetFileNameWithoutExtension(file).ToLower());
    }

    public Guid Id => _guid;
    public string Name => "Directory";
    public string Description => "Directory predictor";

    internal DirectoryPredictor(string guid)
    {
        _guid = new Guid(guid);
        _runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
        _runspace.Name = this.GetType().Name;
        _runspace.Open();

        RegisterEvents();
    }

    public SuggestionPackage GetSuggestion(PredictionClient client, PredictionContext context, CancellationToken cancellationToken)
    {
        var token = context.TokenAtCursor;
        var input = context.InputAst.Extent.Text.Replace("\"", "").Replace("\'", "");

        if (!ValidInput(token, input)) return default;

        if (IsIgnoredCommand(input)) return default;

        var pattern = GetSearchPattern(input);
        var matches = GetDirectorySearchResults(pattern);
        var suggestions = BuildSuggestionPackage(input, matches);

        return suggestions;
    }

    private bool ValidInput(Token token, string input)
    {
        if (token is null) return false;

        if (token.TokenFlags.HasFlag(TokenFlags.CommandName)) return false;

        if (string.IsNullOrWhiteSpace(input)) return false;

        return true;
    }

    private bool IsIgnoredCommand(string input)
    {
        var firstWordIndex = input.IndexOf(' ');
        var command = input.Substring(0, firstWordIndex);

        if (IgnoreCommands.Any(c => c == command)) return true;

        return false;
    }

    private string GetSearchPattern(string input)
    {
        var lastWordIndex = input.LastIndexOf(' ');
        var inputSearchText = input.Substring(lastWordIndex + 1);
        string pattern = "";

        var searchTexts = inputSearchText.Split("|", StringSplitOptions.RemoveEmptyEntries);

        foreach (var searchText in searchTexts)
        {

            if (ExtensionMode)
            {
                pattern += $"*.{searchText}*|";
            }
            else
            {
                pattern += $"{searchText}*.*|";
            }
        }

        return pattern;
    }

    private string[] GetDirectorySearchResults(string pattern)
    {
        var dir = _runspace.SessionStateProxy.Path.CurrentLocation.ToString();
        var searchOptions = SearchOption.TopDirectoryOnly;

        var resultList = new List<string>();
        var collectedFiles = new HashSet<string>();

        var subPatterns = pattern.Split('|', StringSplitOptions.RemoveEmptyEntries);

        List<string> DoSearch(string sub)
        {
            var subResultList = new List<string>();

            if (DirectoryMode == DirectoryMode.None || DirectoryMode == DirectoryMode.Files || DirectoryMode == DirectoryMode.Mixed)
            {
                subResultList.AddRange(Directory.GetFiles(dir, sub, searchOptions)
                    .Catch(typeof(UnauthorizedAccessException))
                    .Where(file =>
                    {
                        if (collectedFiles.Contains(file)) return false;

                        collectedFiles.Add(file);
                        return true;
                    })
                    .Select(FileNameFormat)
                    .Take(ResultsLimit));

            }

            if (DirectoryMode == DirectoryMode.Folders || DirectoryMode == DirectoryMode.Mixed)
            {
                subResultList.AddRange(Directory.GetDirectories(dir, sub, searchOptions)
                    .Catch(typeof(UnauthorizedAccessException))
                    .Where(folder =>
                    {
                        if (collectedFiles.Contains(folder)) return false;

                        collectedFiles.Add(folder);
                        return true;
                    })
                    .Select(FileNameFormat)
                    .Select(x => DirectoryMode == DirectoryMode.Mixed ? x + " #folder" : x)
                    .Take(ResultsLimit));
            }

            return subResultList;
        }

        foreach (var subPattern in subPatterns)
        {
            resultList.AddRange(DoSearch(subPattern));
        }

        if (DirectoryMode == DirectoryMode.Mixed && SortMixedResults == SortMixedResults.Folders)
        {
            resultList.Sort((a, b) =>
            {
                if (a.EndsWith("#folder") && !b.EndsWith("#folder"))
                {
                    return -1;
                }
                else if (!a.EndsWith("#folder") && b.EndsWith("#folder"))
                {
                    return 1;
                }
                else
                {
                    return a.CompareTo(b);
                }
            });
        }

        return resultList.ToArray();
    }

    private SuggestionPackage BuildSuggestionPackage(string input, string[] matches)
    {
        if (DirectoryMode == DirectoryMode.Folders && ExtensionMode)
        {
            List<PredictiveSuggestion> shortCircuitMessage = new List<PredictiveSuggestion>();
            shortCircuitMessage.Add(new PredictiveSuggestion($"You have both Folder mode and Extension mode enabled. Disable one to see results."));
            return new SuggestionPackage(shortCircuitMessage);
        }
        var lastWordIndex = input.LastIndexOf(' ');
        var returnInput = input.Substring(0, lastWordIndex);

        List<PredictiveSuggestion> suggestions = matches.Select(file => new PredictiveSuggestion($"{returnInput} {file}")).ToList();

        return new SuggestionPackage(suggestions);
    }

    public void Dispose()
    {
        UnregisterEvents();
        _runspace.Dispose();
    }

    #region CommandPredictor
    /// <summary>
    /// Gets a value indicating whether the predictor accepts a specific kind of feedback.
    /// </summary>
    /// <param name="client">Represents the client that initiates the call.</param>
    /// <param name="feedback">A specific type of feedback.</param>
    /// <returns>True or false, to indicate whether the specific feedback is accepted.</returns>
    public bool CanAcceptFeedback(PredictionClient client, PredictorFeedbackKind feedback) => false;

    /// <summary>
    /// One or more suggestions provided by the predictor were displayed to the user.
    /// </summary>
    /// <param name="client">Represents the client that initiates the call.</param>
    /// <param name="session">The mini-session where the displayed suggestions came from.</param>
    /// <param name="countOrIndex">
    /// When the value is greater than 0, it's the number of displayed suggestions from the list
    /// returned in <paramref name="session"/>, starting from the index 0. When the value is
    /// less than or equal to 0, it means a single suggestion from the list got displayed, and
    /// the index is the absolute value.
    /// </param>
    public void OnSuggestionDisplayed(PredictionClient client, uint session, int countOrIndex) { }

    /// <summary>
    /// The suggestion provided by the predictor was accepted.
    /// </summary>
    /// <param name="client">Represents the client that initiates the call.</param>
    /// <param name="session">Represents the mini-session where the accepted suggestion came from.</param>
    /// <param name="acceptedSuggestion">The accepted suggestion text.</param>
    public void OnSuggestionAccepted(PredictionClient client, uint session, string acceptedSuggestion) { }

    /// <summary>
    /// A command line was accepted to execute.
    /// The predictor can start processing early as needed with the latest history.
    /// </summary>
    /// <param name="client">Represents the client that initiates the call.</param>
    /// <param name="history">History command lines provided as references for prediction.</param>
    public void OnCommandLineAccepted(PredictionClient client, IReadOnlyList<string> history) { }

    /// <summary>
    /// A command line was done execution.
    /// </summary>
    /// <param name="client">Represents the client that initiates the call.</param>
    /// <param name="commandLine">The last accepted command line.</param>
    /// <param name="success">Shows whether the execution was successful.</param>
    public void OnCommandLineExecuted(PredictionClient client, string commandLine, bool success) { }
    #endregion;
}