### <p align="center">⚠️ EXPERIMENTAL IMPLEMENTATIONS ⚠️</p>

# Directory Predictor

The Directory Predictor permits live directory lookups for PSReadLine's auto-complete functionality. 

https://user-images.githubusercontent.com/29797557/211944927-5a849c78-f700-4ce8-b3e2-819a8bf4a75a.mp4

You will currently need to enable ExperimentalFeatures in PowerShell 7.3.0+.

# Installation

### PowerShell via [PSGallery](https://www.powershellgallery.com/packages/DirectoryPredictor/)

```Install-Module -Name DirectoryPredictor```

### Filepath .dll

Download the relevant .dll release and place where desired. In your $profile

```Import-Module 'path\to\folder\DirectoryPredictor.dll'```

### PowerShell Configurations

1. Add the following to your $profile and restart the shell once or twice

   ```Enable-ExperimentalFeature PSSubsystemPluginModel```

2. Check experimental features are enabled
   
   ```Get-PSSubsystem -Kind CommandPredictor```

3. In $profile again
   
   ```Set-PSReadLineOption -PredictionSource HistoryAndPlugin```


4. Restart or reload

5. Check the module is installed
   
   ```Get-PSSubsystem -Kind CommandPredictor```

6. Test by typing in a directory
   
   ```<somecmd> <any letter or word>```

# Configuration

There is an options cmdlet with a single options parameter that takes Include or Exclude
  
```Set-DirectoryPredictorOption -FileExtensions <Include | Exclude>```

You can disable the regular history results and only use the plugin with

```Set-PSReadLineOption -PredictionSource Plugin```

# Behaviour

- Only files are searched and only one directory deep
- All symbols but spaces are respected
- Only the last word is used to search (no multiword expressions)
- Commands are ignored and no searching will display with just a command
- File extensions can be toggled on or off via $profile
- Accepted completions are saved to the regular history at this time
- Check the intro gif above for more insight

# Roadmap

### More Cmdlet Option Parameters
- -ResultsLimit: Limit results from 1 to 10 (current max in PSReadLine)
- -Folders: <Include | Exclude> | Discussion on the behaviour is needed

The main roadblock is that an attempted -ResultsLimit was implemented. With a second options parameter in $profile, the original FileExtensions flag ceased to work. The session state is desynced, thread unsafe with >1 parameter or is being overwritten.

A better means of sharing state from the Cmdlet parameters is needed in the GetSuggestion method, either using the proper documented means or using a lock. The plan is to create a separate branch and work on it as time permits.

### Further Issues

Once the main issue of multiple options is solved and the implementation of the Cmdlet's is verified, further issues can be addressed in the Issue Tracker formally.

# Disclaimers

1. The projects is currently a learning experience for all involved.

2. This is dabbling in ExperimentalFeatures that come with a warning of *breaking changes* or *unexpected behaviour*.

3. The intended purpose of PSReadLine predictive plugins are to be predictive. This predictor does not predict anything.

# Credit / Resources
### [PSReadLine Github](https://github.com/PowerShell/PSReadLine)

### [CompletionPredictor Github](https://github.com/PowerShell/CompletionPredictor)

### [Microsoft Docs - Creating a Cmdlet](https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/creating-a-cmdlet-to-access-a-data-store?view=powershell-7.3)

### [Basic PSReadLine Configurations](https://jdhitsolutions.com/blog/powershell/8969/powershell-predicting-with-style/)

### [PowerShell - Experimental Features](https://learn.microsoft.com/en-us/powershell/scripting/learn/experimental-features?view=powershell-7.3)