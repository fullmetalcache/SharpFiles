# SharpFiles

C# program that takes in the file output from PowerView's Invoke-ShareFinder and will search through the network shares for files containing terms that you specify.

Compile using csc.exe, such as using the following on a Windows 10 system:

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /out:sharpfiles.exe sharpfiles.cs

Usage: sharpfiles.exe <path_to_sharefinder_file> <max_threads> <output_file>

Example: sharpfiles.exe shares.txt 50 filesfound.csv

Note that the search terms are currently hardcoded in the sharpfiles.cs code. Casing doesn't matter and you don't need wildcards. 

Please note that, by default, it excludes shares NETLOGON, SYSVOL, and shares with $ in the name. You can change this behavior too by modifying the code. Will make all of these command-line options at some point.
