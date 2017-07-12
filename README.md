# AutoDependencyDetector
[![Build status](https://ci.appveyor.com/api/projects/status/jd0t5yk6cengkel6/branch/master?svg=true)](https://ci.appveyor.com/project/Seikilos/autodependencydetector/branch/master)
Static dependency detector and copier for (C++) dll dependencies (and additional files).

## Motivation
Every project that utilizes 3rd party libraries has (at least for C++) to provide include files and provide import libraries to be able to compile the code and after that the compiled binaries still probably won't run. After all you still need to provide the dependencies (dll files) to be found by the OS.

There are a couple of approaches to make the application run:
* Copy every dependency (and its dependencies and its satellites like pdb or xml or config files) entirely. While this might work for very small 3rd party, this is not feasible for larger ones like *OpenSceneGraph* or *Qt*. No one wants 2 GB of possibly useless dependencies just because the developer was not able to filter them.
* Write a fine-grained list of dependencies needed. This is very cumbersome for larger projects using multiple libraries. Spotting every dependency might be time consuming and complicated.
* Set environment search paths for dependencies. Very unportable and you never know whether your shipped product is actually complete. While there might be pre-requisites missing on a client machine, having parts of your own product missing is typically not desirable. Furthermore when the set of environment variables grows you actually might not even know which Qt or “Favourite XML” Tool is actually used during startup. You might compile against a version of a lib and the runtime finds another version of this library in a complete different 3rd party directory.
* Use CMake install target. CMake sacrifices usability for portability. If you won’t ship on multiple platforms, you might prefer to use other mean of managing your code. Install Target still requires you to specify your dependencies and it does only work for installed versions of the software, not your build. 

## What it does
`AutoDependencyDetector` searches a given input directory for binaries and analyzes each of the matched files for missing static dependencies. Static dependencies are dependencies which are directly or indirectly statically compiled into the binary file.
For each of those missing dependencies it performs a search for them. This is accomplished by providing a dependency directory and a set of optional rules. Every found dependency is then copied to the input directory.

### Features
* Iterate over input directory a configurable amount of times to find nested dependencies (dependencies of newly provided dependencies)
* Allows fine-grained filtering of dependency files
* Supports configuration sets (typically useful when the dependency root folder contains different build configurations with the same name (e.g. Debug and Release)
* Is able to also copy additional files for located dependencies like PDB or XML files
* Optionally writes all located and copied dependencies into a file with relative paths which can then be processed further.
* Handles 32 and 64-bit binaries.

## What it does not
This tool can only detect static dependencies. Other dependencies (dynamic) which are loaded during runtime (e.g. via `LoadLibrary`) or by any other plugin concept cannot be detected.

Detect dependencies fast. For large projects there might be multiple iterations of dependency lookup required. Finding missing dependencies is not very fast. For larger projects using the *file list* feature followed by a manual copy of each line might be a better approach.

# How to use
This software relies on [Dependency Walker]( http://www.dependencywalker.com/) a veteran for finding dependencies. Due to a limitation of its license it may not be bundled with other projects. The first thing `AutoDependencyDetector` therefore does is trying to obtain both 32 and 64-bit version of Dependency Walker. If you are behind a corporate proxy that uses credentials, start the application once with `-u ProxyUser -p ProxyPassword` to be able to get Dependency Walker. The proxy is taken automatically from the system settings.

Additionally on the first run it creates a default configuration called `config.json` with some default values. Start `AutoDependencyDetector` without any arguments to see all supported arguments.

## Usage

Get DependencyWalker through proxy with authentication

`AutoDependencyDetector -u ProxyUser -p ProxyPassword`

Get dependencies for all binaries in "C:\My_Application" directory using "C:\Depenencies"

`AutoDependencyDetector -i "C:\My_Application" -d "C:\Depenencies"`

Write file list of all found and copied dependencies

`AutoDependencyDetector -i "C:\My_Application" -d "C:\Depenencies" -f "C:\filelist.txt"`

Use a different configuration set in config.json (manually edited)

`AutoDependencyDetector -i "C:\My_Application" -d "C:\Depenencies" -s ReleaseWithDebug`



# Acknowledgements
`AutoDependencyDetector` incorporates a set of great technologies which are listed here:
* [Dependency Walker]( http://www.dependencywalker.com/) – of course (not bundled with this application).
* [Command Line Parser](https://github.com/gsscoder/commandline) – the best way of dealing with command line arguments.
* [Json.NET]( http://www.newtonsoft.com/json) from Newtonsoft. 

## During development additionally were used
* [NUnit](https://github.com/nunit/nunit) – A versatile unit test framework for .net
* [NSubstitute](http://nsubstitute.github.io) – A very well executed mocking framework.

