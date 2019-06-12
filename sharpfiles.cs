using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class RecursiveFileProcessor 
{	
    public static void Main(string[] args) 
    {
	string line;
	List<string> shares = new List<string>();

	string sharesFile = args[0];
	int maxThreads = Int32.Parse(args[1]);
	string outFile = args[2];

	System.IO.StreamReader file =   
		new System.IO.StreamReader(args[0]);  
	while((line = file.ReadLine()) != null)  
	{  
		line = line.Split('\t')[0].Trim();
		if(!line.Contains("$") && !line.Contains("NETLOGON") && ! line.Contains("SYSVOL"))
		{
			shares.Add(line);
		}
	}

	file.Close();

	FileSearcher fSearcher = new FileSearcher(maxThreads, outFile);
	fSearcher.ProcessShares(shares);   
    }
}

public class FileSearcher
{
	private readonly Object _CountLock = new Object();
	private readonly Object _ListLock = new Object();
	
	private int _ThreadCount;
	private int _MaxThreads;
	private string _OutputFile;

	private string[] terms = new string[] {".iso", ".wim", ".vmdk", "pass", "cred", "admin", "login", "logon", ".sql", "secret", "unattend", "sensitive", "root"};
	
	private List<string> _FilesFound = new List<string>();
	
	public FileSearcher(int maxThreads, string outputFile)
	{
		_ThreadCount = 0;
		_MaxThreads = maxThreads;
		_OutputFile = outputFile;
	}
	
	private Thread StartThread(string path) 
	{
		Thread t = new Thread(() => ProcessShare( path ));
		t.Start();
		return t;
	}
	
	public void ProcessShares(List<string> shares)
	{
		int counter = 0;
		foreach(string path in shares) 
		{
			try
			{	
				while(true)
				{
					lock(_CountLock)
					{
						if(_ThreadCount < _MaxThreads)
						{
							_ThreadCount++;
							break;
						}
					}

					Thread.Sleep(1000);
				}

				StartThread(path);
			}
			catch
			{
				continue;
			}

			counter++;
			Console.WriteLine(String.Format("Processed : {0}", path));
			Console.WriteLine(String.Format("{0} of {1} Directories", counter, shares.Count));
		}     
	}
	
	private void ProcessShare(string path)
	{
		Queue<String> _Dirs = new Queue<String>();
		
		_Dirs.Enqueue( path );
		
		while( 0 < _Dirs.Count )
		{
			string currDir = _Dirs.Dequeue();
			ProcessDirectory(currDir, _Dirs);
		}
		lock(_CountLock)
		{
			_ThreadCount--;
		}
	}
	
	public void ProcessDirectory(string targetDirectory, Queue<String> _Dirs) 
	{
	// Process the list of files found in the directory.
		try
		{
			string [] fileEntries = Directory.GetFiles(targetDirectory);
			foreach(string fileName in fileEntries)
				ProcessFile(fileName);
		}
		catch{}

	// Recurse into subdirectories of this directory.
		try
		{
			string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
			foreach(string subdirectory in subdirectoryEntries)
				_Dirs.Enqueue(subdirectory);
		}
		catch{}
	}
        
	public void ProcessFile(string path) 
	{
		string[] filesplit = path.Split('\\');
		string file = filesplit[filesplit.Length - 1];

		foreach( string term in terms)
		{
			if( file.ToLower().Contains(term.ToLower()) )
			{
				lock(_ListLock)
				{
					_FilesFound.Add(path);

					using (System.IO.StreamWriter outFile =
							new System.IO.StreamWriter(_OutputFile, true))
					{
						outFile.WriteLine(path);
					}
				}

				break;
			}
		}			
	}
}
