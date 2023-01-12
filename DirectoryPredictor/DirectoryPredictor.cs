using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Subsystem.Prediction;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Management.Automation;

namespace DirectoryPredictor;

public partial class DirectoryPredictor : PSCmdlet, ICommandPredictor, IDisposable
{
    private readonly Guid _guid;
    private Runspace _runspace { get; }
    public bool _includeFileExtensions = true;
    public int _resultsLimit = 10;
    
    internal DirectoryPredictor(string guid)
    {
        _guid = new Guid(guid);
        _runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
        _runspace.Open();

        RegisterEvents();
    }

    public Guid Id => _guid;

    public string Name => "Directory";

    public string Description => "Directory predictor";

    public SuggestionPackage GetSuggestion(PredictionClient client, PredictionContext context, CancellationToken cancellationToken)
    {
        Token tokenAtCursor = context.TokenAtCursor;

        //White space is intensive, skip it in all cases
        if (tokenAtCursor is null)
        {
            return default;
        }

        //Skip parsing commands as they are not needed in directory searches
        if (tokenAtCursor is not null && tokenAtCursor.TokenFlags.HasFlag(TokenFlags.CommandName))
        {
            return default;
        }
        
        //Skip if the input is blank
        string input = context.InputAst.Extent.Text;
        if (string.IsNullOrWhiteSpace(input))
        {
            return default;
        }

        string searchText = (input?.Split(' ')?.LastOrDefault()?.ToLower()) ?? "";
        string returnInput = (input?.Split(' ')?.FirstOrDefault()) ?? "";
        var pattern = searchText + "*.*";
        var dir = _runspace.SessionStateProxy.Path.CurrentLocation.ToString();

        string[] files = Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly)
            .Catch(typeof(UnauthorizedAccessException))
            .Select(file => _includeFileExtensions ? Path.GetFileName(file).ToLower() : Path.GetFileNameWithoutExtension(file).ToLower())
            .Take(_resultsLimit)
            .ToArray();

        List<PredictiveSuggestion> listOfMatches = files.Select(file => new PredictiveSuggestion($"{returnInput} {file}")).ToList();

        return new SuggestionPackage(listOfMatches);
    }

    public void Dispose()
    {
        UnregisterEvents();
        _runspace.Dispose();
    }

    #region "interface methods for processing feedback"

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