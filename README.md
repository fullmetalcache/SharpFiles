# SharpFiles
SharpFiles now has two programs for your use:
SharpShares (taken and modified from: https://github.com/djhohnstein/SharpShares)
SharpFiles

Compile both using csc.exe, such as using the following on a Windows 10 system:

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /out:sharpshares.exe sharpshares.cs
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /out:sharpfiles.exe sharpfiles.cs

First, run sharpshares.exe

Usage: sharpshares.exe

SharpShares will output two files: systems.txt, shares.txt

The systems.txt contains a list of all systems in the environment. The shares.txt contains a list of all shares that are readable by the current user that ran sharpshares.exe.

Next, take the shares.txt file and feed it into sharpfiles.exe

Usage: sharpfiles.exe <path_to_shares_txt> <max_threads> <output_file>

Example: sharpfiles.exe shares.txt 50 filesfound.csv

Note that the search terms are currently hardcoded in the sharpfiles.cs code. Casing doesn't matter and you don't need wildcards. You will need to recompile the program after you change the search terms but it's pretty quick and easy.

Please note that, by default, it excludes shares NETLOGON, SYSVOL, and shares with $ in the name. You can change this behavior too by modifying the code. Will make all of these command-line options at some point.
