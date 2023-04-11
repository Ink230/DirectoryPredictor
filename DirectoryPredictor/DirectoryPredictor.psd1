@{
    RootModule = 'DirectoryPredictor.dll'
    ModuleVersion = '0.0.5'
    GUID = 'e242df59-e307-4476-9811-6a0c5cba8d63'

    Author = 'Justin Quinn'
    Copyright = '(c) Justin Quinn. All rights reserved.'

    Description = 'Used with PSReadLine as a predictor to enable active Directory lookups.'
    PowerShellVersion = '7.3.0'
    CmdletsToExport = 'Set-DirectoryPredictorOption'

    PrivateData = @{

        PSData = @{

            Tags = @('Directory','Searcher', 'PSReadLine', 'Predictor', 'Plugin', 'File', 'Files', 'Filenames', 'Path', 'Folders')

            # Full source code available
            ProjectUri = 'https://github.com/Ink230/DirectoryPredictor'

            SetDirectoryPredictorCmdletOptions = '
Set-DirectoryPredictorOption -FileExtensions <string> [Include] | [Exclude]
Set-DirectoryPredictorOption -ResultsLimit <int> [1-10]
Set-DirectoryPredictorOption -IgnoreCommands <string> [comma separated list]
Set-DirectoryPredictorOption -DirectoryMode <string> [Files | Folders | Mixed]
Set-DirectoryPredictorOption -SortMixedResults <string> [Files | Folders]
Set-DirectoryPredictorOption -ExtensionMode <string> [Enabled | Disabled]

*Tip: Most of these commands become very useful with alias setters/togglers for on the fly changes

*Have an idea for an option? Suggest it on GitHub!
'

            ReleaseNotes = '
v0.0.5
• New Pattern matching behaviour with ? (any), * (wildcard, before, after), and | (OR, match multiple search patterns, prepend with a quote)
• New -DirectoryMode [Files | Folders | Mixed] to search any combination of files and folders
• New -SortMixedResults [Files | Folders] to prioritize which suggestions are sorted at top
• New -ExtensionMode [Enable | Disable] to search just file extensions (does not work with -DirectoryMode Folders)
• Reworked code to be more extensible for future cmdlet options

v0.0.4
• New -IgnoreCommands [comma separated list] to ignore some cmds from using the predictor
• New -ResultsLimit [1-10] to limit the number of results (10 is PSReadLine limit for now)
• Multiword cmdline support; Your cmd flags are now respected and usable
• Refactored how state is shared to better represent PSReadLines best practices with DirectoryPredictorOption
• Refactored how cmdline string parsing is conducted for better performance
• General underlying code cleanup

v0.0.3
• Fixed case sensitivities by ignoring all case and conducting all strings in lowercase
• Future option possible to change this behaviour, future issue
• Minor variable initialization precautions
• Minor string handling cleanup

v0.0.2
• Initial public release with -FileExtension option only
• Bug with case sensitivities present in this version

v0.0.1
• Proof of concept private release'

        }

    }

}

