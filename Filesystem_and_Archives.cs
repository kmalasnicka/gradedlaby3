using System.IO; //provides a rich set of classes for interacting with the filesystem. 
//includes classes File, Directory, Path, their instance-based counterparts FileInfo and DirectoryInfo, and the DriveInfo class for retrieving information about mounted drives.

//Path class(static) -> paths are stored in string

// Path Manipulation: Combine, IsPathRooted, GetPathRoot, GetDirectoryName, GetFileName, GetFullPath
// Working with Extensions: HasExtension, GetExtension, GetFileNameWithoutExtension, ChangeExtension
// Cross-platform Paths: DirectorySeparatorChar, AltDirectorySeparatorChar, PathSeparator, VolumeSeparatorChar, GetInvalidPathChars, GetInvalidFileNameChars
// Working with Temporary Files: GetTempPath, GetRandomFileName, GetTempFileName

string path = Path.Combine("Workspace", "csharp-site", "hugo.yaml");
Console.WriteLine($"Path combined: {path}"); // Workspace/csharp-site/hugo.yaml
Console.WriteLine($"File Name: {Path.GetFileName(path)}"); // hugo.yaml
Console.WriteLine($"Name without extension: {Path.GetFileNameWithoutExtension(path)}"); // hugo
Console.WriteLine($"Extension: {Path.GetExtension(path)}");  // .yaml
Console.WriteLine($"Parent Directory: {Path.GetDirectoryName(path)}"); // Workspace/csharp-site
Console.WriteLine($"Full Path: {Path.GetFullPath(path)}"); // /home/tomasz/Workspace/csharp-site/hugo.yaml (resolves relative to the current working directory)
Console.WriteLine($"Directory Separator: {Path.DirectorySeparatorChar}"); // \ (on Wi

//File class(static)

// File Manipulation: Exists, Delete, Copy, Move, Replace, CreateSymbolicLink
// Attribute Operations: GetAttributes, SetAttributes
// Timestamp Operations: GetCreationTime, GetLastAccessTime, GetLastWriteTime, SetCreationTime, SetLastAccessTime, SetLastWriteTime
// Permission Operations: GetUnixFileMode, SetUnixFileMode
// It provides simple, high-level methods for file operations like copying, moving, or deleting.

string path = "lorem.txt";
File.WriteAllText(path, "Lorem ipsum");
File.Copy(path, "copy.txt", overwrite: true);
File.Move("copy.txt", "moved.txt", overwrite: true);
if (File.Exists("moved.txt"))
{
    File.Delete("moved.txt");
}

//FileInfo class -> Represents a specific file as an object, providing properties with information about it, properties like Name, Length, Extension

string path = "lorem.txt";
File.WriteAllText(path, "Lorem ipsum");
var fileInfo = new FileInfo(path);
if (fileInfo.Exists)
{
    Console.WriteLine($"Name: {fileInfo.Name}");
    Console.WriteLine($"Size: {fileInfo.Length} bytes");
    Console.WriteLine($"Extension: {fileInfo.Extension}");
    Console.WriteLine($"Directory: {fileInfo.DirectoryName}");
    Console.WriteLine($"Read-only: {fileInfo.IsReadOnly}");
    fileInfo.CopyTo("ipsum.txt", overwrite: true);
}

//Directory class(static)

// Directory Manipulation: CreateDirectory, Exists, Delete, Move, CreateSymbolicLink
// Getting Contents: GetFiles, GetDirectories, GetFileSystemEntries,
// Getting Contents (Lazily): EnumerateFiles, EnumerateDirectories, EnumerateFileSystemEntries
// Working with the Current Directory: GetCurrentDirectory, SetCurrentDirectory
// Timestamp Operations: GetCreationTime, GetLastAccessTime, GetLastWriteTime, SetCreationTime, SetLastAccessTime, SetLastWriteTime
// Other: GetParent, GetDirectoryRoot
// Used for performing one-off operations on directories.

string newDir = "NewDirectory";

// Create a directory (and all parent directories if they don't exist)
Directory.CreateDirectory(newDir);

if (Directory.Exists(newDir))
{
    Console.WriteLine("Directory exists.");
}

// Listing contents
Console.WriteLine("\nFiles in the current directory:");
string[] files = Directory.GetFiles(".");
foreach (var file in files) Console.WriteLine(file);

Console.WriteLine("\nDirectories in the current directory:");
string[] dirs = Directory.GetDirectories(".");
foreach (var d in dirs) Console.WriteLine(d);

Console.WriteLine("\nAll entries in the current directory:");
string[] entries = Directory.GetFileSystemEntries(".");
foreach (var e in entries) Console.WriteLine(e);

Directory.Delete(newDir, recursive: true);

// The Get* and Enumerate*(more eficient for large directoeis) methods are overloaded to accept a search pattern (string searchPattern) and additional options (SearchOption options). Passing SearchOption.AllDirectories will cause the search to be recursive.
//For example, to recursively search for all PDF files in the current directory:
IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(".", "*.pdf", SearchOption.AllDirectories);
foreach (var e in entries) Console.WriteLine(e);

//DirectoryInfo class -> Represents a specific directory. It’s convenient when you need to perform multiple operations on the same directory.
var dirInfo = new DirectoryInfo("."); // Current directory

Console.WriteLine($"Name: {dirInfo.Name}");
Console.WriteLine($"Full Path: {dirInfo.FullName}");
Console.WriteLine($"Parent Folder: {dirInfo.Parent?.FullName}");
Console.WriteLine($"Root: {dirInfo.Root}");

DirectoryInfo subDir = dirInfo.CreateSubdirectory("Subdir");

FileInfo[] filesInDir = dirInfo.GetFiles("*.dll");
Console.WriteLine("\nDLL files in the directory:");
foreach (var f in filesInDir)
    Console.WriteLine($"    {f.Name}");

Console.WriteLine("\nSubdirectories in the directory:");
DirectoryInfo[] dirsInDir = dirInfo.GetDirectories();
foreach (var d in dirsInDir)
    Console.WriteLine($"    {d.Name}");

//Environment.GetFolderPath - allows you to retrieve paths to special, predefined system folders, such as the user’s home directory or desktop, in a cross-platform way.
string userHomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

// DriveInfo - allows you to get information about the mounted drives in the system.
const long GB = 1024 * 1024 * 1024;
DriveInfo root = new DriveInfo("/");

Console.WriteLine($"Total size: {root.TotalSize / GB} GB");
Console.WriteLine($"Free size: {root.TotalFreeSpace / GB} GB"); // Ignoring quotas
Console.WriteLine($"Available size: {root.AvailableFreeSpace / GB} GB");

foreach (DriveInfo drive in DriveInfo.GetDrives())
{
    Console.WriteLine(drive.Name);
}

//Filesystem events
//FileSystemWatcher - for a given path, it can monitor for the creation, deletion, modification, or renaming of files and directories
namespace FileSystemEvents;

class Program
{
    static void Main(string[] args)
    {
        Watch(".", "*", false);
    }
    
    static void Watch(string path, string filter, bool includeSubDirs)
    {
        using var watcher = new FileSystemWatcher(path, filter);
        watcher.Created += OnCreated;
        watcher.Changed += OnChanged;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;
        watcher.IncludeSubdirectories = includeSubDirs;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Listening for events - press <enter> to finish");
        Console.ReadLine();
    }
    
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"Deleted: {e.FullPath}");
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"Renamed:");
        Console.WriteLine($"    Old: {e.OldFullPath}");
        Console.WriteLine($"    New: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine(e.GetException());
    }
}

//Archives - files that act as containers for other files and folders
//zip - groups files and also offers data compression by default
//ZipFile class - allows creating archive from a folder (CreateFromDirectory) or extract (ExtractToDirectory)

const string sourceDirectory = "my_files_to_zip";
const string zipPath = "archive.zip";
const string extractPath = "extracted_files";

Directory.CreateDirectory(sourceDirectory);
File.WriteAllText(Path.Combine(sourceDirectory, "file1.txt"), "Hello");
Directory.CreateDirectory(Path.Combine(sourceDirectory, "subfolder"));
File.WriteAllText(Path.Combine(sourceDirectory, "subfolder", "file2.txt"), "World");

// Creating .zip archive (override existing)
if (File.Exists(zipPath)) File.Delete(zipPath);
ZipFile.CreateFromDirectory(sourceDirectory, zipPath);

// Extracting archive (override if files exist)
if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
ZipFile.ExtractToDirectory(zipPath, extractPath);

//ZipFile.Open method -> returns a ZipArchive object, it allows for precise manipulation of archive contents entry by entry, Archive can be opemned in read, create or update mode
using ZipArchive zip = ZipFile.Open("archive.zip", ZipArchiveMode.Read);

Console.WriteLine("Archive content:");
foreach (ZipArchiveEntry entry in zip.Entries)
{
    Console.WriteLine($"{entry.FullName,-32} Size: {entry.Length} bytes");
}
