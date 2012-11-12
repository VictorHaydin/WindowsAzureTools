WindowsAzureTools
=================

Contains a set of command-line tools for automating common tasks related to running cloud applications on Windows Azure platform.

1. **AzureSqlDbBlobBackup**: back up Windows Azure SQL Database to Windows Azure Blob Storage.
2. **ZipToAzureBlob**: compress a file or folder and store the archive in Windows Azure Blob Storage.
3. **DownloadAzureBlob**: download a Windows Azure Blob to local filesystem.

System requirements
-------------------

* .NET Framework 4 (Full)

How to use
----------
Command-line syntax help is provided by each tool. Run the tool without any parameters or specify the `--help` parameter to see the list of available command-line switches.

Folders
-------

  `/build`   : contains the compiled binaries of each tool and all necessary libraries to run them.

  `/src`     : Visual Studio solutions for each tool.

Copyright
---------
Read `LICENSE.txt` in the root folder for licensing details.
