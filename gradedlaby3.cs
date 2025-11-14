using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

class Program
{//when sth happens to a file in the folder FileSystemWatcher reacts 
    private static FileSystemWatcher watcher; // object watching a folder
    private static Dictionary<string, DateTime> _lastEvents = new(); // remembers timestamps of recent events

    static void Main(string[] args){ // @ ignores backslashes
        string path = "desc"; // folder to watch

        watcher = new FileSystemWatcher(path) // we start watching the folder path
        {
            IncludeSubdirectories = false, // do not watch subfolders
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite // react only when the file name changes or the file is modified
        };
//FileSystemWatcher is a publisher that raises the events, under the hood: public event FileSystemEventHandler Created;
//Every time something happens in the folder (desc), FileSystemWatcher invokes the appropriate event  
        watcher.Created += OnCreated; //file appears
        watcher.Changed += OnChanged; //file modified
        watcher.Deleted += OnDeleted; //file removed
        watcher.Renamed += OnRenamed; // file renamed
        watcher.Error += OnError; // watcher crashed

        watcher.EnableRaisingEvents = true; // this starts watching if we want to stop watcher.EnableRaisingEvents = false;

        Console.WriteLine("Watching directory...");
        Console.ReadLine(); 
    }

//event handelers:
//sender -> the watcher object, e -> data about the file (path, change type)
//FileSystemEventArgs → for Created, Changed, Deleted contains FullPath, Name, etc.
// RenamedEventArgs → old + new name
// ErrorEventArgs → exception
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (IsDuplicateEvent(e.FullPath)) return; // we check if the event is a duplicate

        Console.WriteLine($"Created: {e.FullPath}"); // we print file path
        HandleNewFile(e.FullPath); //extracts year, month created folder images/yyyy/mm moves the file there
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;
// if the event is changed we print:
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e)
    { // we just print deleted path
        Console.WriteLine($"Deleted: {e.FullPath}");
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine("Renamed:");
        Console.WriteLine($"    Old: {e.OldFullPath}");
        Console.WriteLine($"    New: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine("Error:");
        Console.WriteLine(e.GetException());
    }

//event aggregation, filesystemwatcher often fires multiple events for the same file
    private static bool IsDuplicateEvent(string path)
    { // this filters out duplicate events that happen too fast
        DateTime now = DateTime.Now;

        if (_lastEvents.TryGetValue(path, out DateTime last))
        {
            if ((now - last).TotalMilliseconds < 200)
                return true; // ignore duplicate
        }

        _lastEvents[path] = now;
        return false;
    }


    private static void HandleNewFile(string fullPath)
    {
        string fileName = Path.GetFileName(fullPath); // we extract filenam from the path: fullPath = "desc/20240210211522.png" fileName = "20240210211522.png"

        if (fileName.Length < 6) // it cannot be too short
        {
            Console.WriteLine("Filename too short to extract date.");
            return;
        }
//we extract year and month
        string year = fileName.Substring(0, 4);  // year - YYYY
        string month = fileName.Substring(4, 2); // month - MM

        string destDir = Path.Combine("Images", year, month); // we build destination folder path-> Images/YYYY/MM
        Directory.CreateDirectory(destDir); // works even if folder already exists

//Images/2024/02/<fileName>
        string destPath = Path.Combine(destDir, fileName); // we build destination full path Images/2024/02/20240210211522.png

        try
        {
            File.Move(fullPath, destPath); //removes file from desc/ places it into Images/YYYY/MM/
            Console.WriteLine($"Moved to: {destPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Move failed: {ex.Message}");
        }
    }
}
