### <p align="center">⚠️ EXPERIMENTAL IMPLEMENTATIONS ⚠️</p>

# Directory Predictor

The Directory Predictor permits live directory file lookups for PSReadLine's auto-complete functionality. 

https://user-images.githubusercontent.com/29797557/212500726-26f98466-dd21-46e1-b793-a08c803e2c23.mp4

# Installation

### PowerShell via [PSGallery](https://www.powershellgallery.com/packages/DirectoryPredictor/)

```Install-Module -Name DirectoryPredictor```

### Filepath .dll

Download the relevant .dll release and place where desired. In your $profile

```Import-Module 'path\to\folder\DirectoryPredictor.dll'```

### PowerShell Configurations

1. Add the following to your $profile

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

### FileExtensions
Determine if file extensions should show or not in the prediction and in the autocomplete result.
  
```Set-DirectoryPredictorOption -FileExtensions <string> [Include | Exclude]```

### ResultsLimit
Specify how many results, from 1 to 10, to display. 10 is a current limit by PSReadLine but the module will accept up to 500 currently.

```Set-DirectoryPredictorOption -ResultsLimit <int> [1-10]```

### IgnoreCommands
Ignore specific commands in a comma separated string. This will cause those commands to not display [Directory] suggestions.

```Set-DirectoryPredictorOption -IgnoreCommands <string> [comma separated list]```

### Tips
* Each of these flags work great with aliases for fast on the fly adjustments
* Example
  ```
   Function ShowExtensions5 {
      Set-DirectoryPredictorOption -FileExtensions Include
   }
   Set-Alias -Name se -Value ShowExtensions

   Function HideExtensions {
   Set-DirectoryPredictorOption -FileExtensions Exclude
   }
   Set-Alias -Name he -Value HideExtensions
   ```

* You can disable the regular history results and only use the plugin with
  
  ```Set-PSReadLineOption -PredictionSource Plugin```

# Behaviour

- Only files are searched and only one directory deep (folder mode in the works)
- All symbols but spaces are respected
- Only the last word is used to search but previous input is respected
  - ```code -n hel``` will work and will match "hel" to filenames
- Commands are ignored and no searching will display with just a command
- Accepted completions are saved to the regular history at this time
- Check the intro gif above for more insight

# Roadmap

As of v0.0.4, the plan is to gather user suggestions!

If you have an idea, suggestion or want to contribute, please open an Issue!

- Folder mode currently works from source and has an option cmdlet to sort by folders or files first ✔
- Pattern matching (* wildcard for files, ? for folder mode listing all files...etc) is in development
- Search by filename extension is next
- These three features when completed will launch v0.0.5 || Until the Subsystem is taken out of experimental we will use this versioning scheme

# Disclaimers

1. This is dabbling in ExperimentalFeatures that come with a warning of *breaking changes* or *unexpected behaviour*.

2. The intended purpose of PSReadLine predictive plugins are to be predictive. This predictor does not predict anything.

# Credit / Resources
### [PSReadLine Github](https://github.com/PowerShell/PSReadLine)

### [CompletionPredictor Github](https://github.com/PowerShell/CompletionPredictor)

### [Microsoft Docs - Creating a Cmdlet](https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/creating-a-cmdlet-to-access-a-data-store?view=powershell-7.3)

### [Basic PSReadLine Configurations](https://jdhitsolutions.com/blog/powershell/8969/powershell-predicting-with-style/)

### [PowerShell - Experimental Features](https://learn.microsoft.com/en-us/powershell/scripting/learn/experimental-features?view=powershell-7.3)
