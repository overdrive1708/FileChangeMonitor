# FileChangeMonitor

Monitor file existence and timestamp.  
Example:  

- Checking change released files
- Checking change Reviewed File

## Features

- Monitor file existence
- Monitor file timestamp
- Able make notes

## Requirement

- .NET Framework 4.7.2
- System.Data.SQLite.dll
- SQLite.Interop.dll

## Usage

### Add monitoring files

1. Run "FileChangeMonitor.exe".
2. Click "Add" button.
3. Drag & Drop monitoring files.
4. Click "Add monitor files" button.

### Del monitoring files

1. Run "FileChangeMonitor.exe".
2. Click "Del all item" button or select item and "Del selected item" button.
3. Click "Yes" button.

### Checking monitoring files

1. Run "FileChangeMonitor.exe".
2. If you confirm timestamp diff, select and click "TimeStampUpdate" button.
3. Checking is automaticly.If you want refresh selfy, click "ReCheck" button.

## Build

Clone the Git repo, open "FileChangeMonitor.sln" in Visual Studio 2022, and build.  
Make sure you have the System.Data.SQLite.Core installed using NuGet.  
Make sure you have the System.Data.Linq Referenced.

## Licence

This project is licensed under the MIT License.  
See the [LICENSE](LICENSE) file for details.

## Author

[overdrive1708](https://github.com/overdrive1708)

## Changelog

See the [CHANGELOG](CHANGELOG.md) file for details.
