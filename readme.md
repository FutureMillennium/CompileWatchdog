### Watches files in a folder and runs a compile command when any are changed
A portable Windows utility mainly intended for the [Haxe](https://haxe.org/) programming language, but can run any commands, including your favourite compilers.

[![A screenshot of Compile Watchdog by Zdeněk Gromnica](docs/images/compile-watchdog-screenshot.png 'Download Compile Watchdog v1.1 by Zdeněk Gromnica')](https://github.com/FutureMillennium/CompileWatchdog/releases/download/v1.1.0/CompileWatchdog.exe)

### [Download *Compile Watchdog* v1.1](https://github.com/FutureMillennium/CompileWatchdog/releases/download/v1.1.0/CompileWatchdog.exe)

Requires **[.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)**, which should already be included with Windows.

**Watched directories** – a list of directories that are being monitored for file changes. Drag and drop a directory into the window to add it to the list. When checked, watches for all file changes including subdirectories.

**Compile command** – the command that is run via `cmd.exe /c` when any files are changed. The working directory is always the watched directory, regardless of which file was changed. Chain multiple commands with `&` (always) or `&&` (only on success of the previous command).

**Ignore** – a list of paths that are ignored, separated by semicolons (;), relative to the watched directory. For example, if “Ignore” is `bin;.git` and the watched directory is `C:\foo`, then these paths will be ignored: `C:\foo\bin`, `C:\foo\.git`

**Last [standard output](https://en.wikipedia.org/wiki/Standard_streams#Standard_output_(stdout))** – the output of the compile command when it was last run.

**Last [standard error](https://en.wikipedia.org/wiki/Standard_streams#Standard_error_(stderr))** – typically, if the compile command fails, the error message will be here.

If both outputs are empty, "(The output was empty.)" is shown.

**Compile all now** – runs the compile command immediately for all watched directories with a checkmark.

**Pop up on compilation error** – if checked, the window will pop up when the compile command fails, so you can see the error message.

**Minimise to tray** – hide the window into the system tray aka “notification area”. This also happens when you close the window. You can also hide the window by pressing [Escape].

**Quit** – stop watching files and quit the application.

### New in v1.1
+ Live console output while compiling.
+ No longer blocks while compiling.
+ Compilation can be cancelled.
+ Handles settings.xml errors.
+ Does not save settings.xml if watched dirs are empty (like on a settings.xml read error).

I made this because existing solutions I found required several (!) package managers and had dozens of dependencies, and that's just no good.

It should be possible to compile this with much older versions of .NET Framework, as it doesn't do anything unusual. I have no idea which versions of .NET Windows ships with these days. I would hope it would “just work”, but you never know with Microsoft.
