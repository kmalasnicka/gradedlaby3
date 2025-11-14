// streams - abstraction for i/o operations, three operations: Read, Write, Seak, you can see if a stream supports these operations using Can* 

//streams capabilities
using Stream stream = new FileStream("lorem.txt", FileMode.Open);
Console.WriteLine($"Can read: {stream.CanRead}");
Console.WriteLine($"Can write: {stream.CanWrite}");
Console.WriteLine($"Can Seek: {stream.CanSeek}");

//reading from stream
int Read(byte[] buffer, int offset, int count)//it returns the number of bytes read, which may differ from the number of bytes we would like to read., end of stream is signaled by returning value 0
//using statmrnt causes resources associated with it to be automatically released at the end of the block by calling the Dispose method
using Stream stream = new FileStream("lorem.txt", FileMode.Open);
byte[] buffer = new byte[256];
int read;
while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
{
    Console.WriteLine($"Read {read} bytes");
}

//writinf to a stream
void Write(byte[] buffer, int offset, int count)
using Stream stream = new FileStream("lorem.txt", FileMode.Create);
byte[] buffer = new byte[256];
for (int i = byte.MinValue; i <= byte.MaxValue; i++)
{
    buffer[i - byte.MinValue] = (byte)i;
}
stream.Write(buffer, 0, buffer.Length);

// positioning
using Stream stream = new FileStream("lorem.txt", FileMode.Open);
stream.Position = 0;
stream.Position = stream.Length;
stream.Seek(offset: 10, SeekOrigin.Begin);
stream.Seek(offset: -10, SeekOrigin.End);
stream.Seek(offset: -10, SeekOrigin.Current);

// Using constructor:
FileStream fs1 = new FileStream("file1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);

// File façade methods:
FileStream fs2 = File.Create("file2.txt");
FileStream fs3 = File.OpenRead("file3.txt");
FileStream fs4 = File.OpenWrite("file4.txt");
FileStream fs5 = File.Open("file5.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);

// Facade

// ReadAllText and WriteAllText:
string content = File.ReadAllText("lorem.txt");
File.WriteAllText("lorem.txt.bak", content);

// ReadAllLines and WriteAllLines:
string[] lines = File.ReadAllLines("lorem.txt");
File.WriteAllLines("lorem.txt.bak", lines);

// ReadAllBytes and WriteAllBytes:
byte[] bytes = File.ReadAllBytes("lorem.bin");
File.WriteAllBytes("lorem.bin.bak", bytes);

// AppendAllText:
const string logFile = "log.txt";
for (int i = 0; i < 100; i++)
{
    File.AppendAllText(logFile, $"[Info] This is a log entry no. {i}");
}
// ReadLines and AppendAllLines:
IEnumerable<string> lines = File.ReadLines("lorem.txt");
int errors = lines
        .Where(line => line.Contains("Error"))
        .Count();
IEnumerable<string> lines = File.ReadLines("lorem.txt");
File.AppendAllLines("lorem.txt.bak", lines);

//Stream decorators - decorator is a pattern which allows for dynamically adding new functionality to an object without changing its class
//BufferedStream and GZipStream both inherit from Stream
//BufferedStream adds buffering to a stream

// Write 100K bytes to a file:
File.WriteAllBytes("file.bin", new byte [100_000]);

using Stream fs = File.OpenRead("file.bin");
// Add 20k bytes buffering
using Stream bs = new BufferedStream(fs, 20_000);

bs.ReadByte();
Console.WriteLine(fs.Position); // 20000

//Compression - standard library provides three compression methods represented by the stream decorators GZipStream, DeflateStream, BrotliStream
//GZipStream when we write data to it, it first compresses this data before passing it on to the wrapped stream
//During reads, the process is reversed: GzipStream reads compressed data from the wrapped stream and decompresses it before returning it

// Compression

using Stream fsIn = File.OpenRead("file.txt");
using Stream fsOut = File.Create("file.txt.gz");
using Stream cs = new GZipStream(fsOut, CompressionMode.Compress);
byte[] buffer = new byte[4096];
int read;
while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
{
    cs.Write(buffer, 0, read);
}

// Decompression

using Stream fsIn = File.OpenRead("file.txt.gz");
using Stream ds = new GZipStream(fsIn, CompressionMode.Decompress);
using Stream fsOut = File.Create("file.txt");
byte[] buffer = new byte[4096];
int read;
while ((read = ds.Read(buffer, 0, buffer.Length)) > 0)
{
    fsOut.Write(buffer, 0, read);
}

//Combining Decorators 
using Stream fs = new FileStream("dane.bin.gz", FileMode.Create);
using Stream bs = new BufferedStream(fs);
using Stream cs = new GZipStream(bs, CompressionMode.Compress);
byte[] data = new byte[1000];
cs.Write(data, 0, data.Length);
// In the above example, the data we write goes through the following path:
// data -> GZipStream (compression) -> BufferedStream (buffering) -> FileStream (disk write).

//Stream Adapters

//Text Adapters - add the ability to read and write characters, words, and entire lines of text to a stream, They allow operations such as string? ReadLine(), string? ReadToEnd(), void WriteLine(string str), void Write(string str).

//StreamReader
using FileStream fs = File.OpenRead("lorem.txt");
using StreamReader sr = new StreamReader(fs);
while (sr.ReadLine() is { } line)
{
    Console.WriteLine(line);
}
// StreamWriter
using FileStream fs = File.OpenWrite("fizzbuzz.txt");
using StreamWriter sw = new StreamWriter(fs);
for (int i = 1; i <= 100; i++)
{
    sw.Write($"{i} : ");
    if (i % 3 == 0 && i % 5 == 0)
        sw.WriteLine("FizzBuzz");
    else if (i % 3 == 0)
        sw.WriteLine("Fizz");
    else if (i % 5 == 0)
        sw.WriteLine("Buzz");
    else
        sw.WriteLine(i);
}

//Binary Adapters - They allow operations such as int ReadInt32(), void Write(int value), double ReadDouble(), void Write(double value), string ReadString().

// BinaryWriter
using FileStream fs = File.OpenWrite("player.bin");
using BinaryWriter bw = new BinaryWriter(fs);
Player player = new Player("Bob", 100, 2500, 10, new Vector2(48.5f, 32.5f));
bw.Write(player.Name);
bw.Write(player.Health);
bw.Write(player.Experience);
bw.Write(player.Money);
bw.Write(player.Position.X);
bw.Write(player.Position.Y);
public record Player(string Name, int Health, long Experience, long Money, Vector2 Position);

// BinaryReader
using FileStream fs = File.OpenRead("player.bin");
using BinaryReader br = new BinaryReader(fs);
string name = br.ReadString();
int health = br.ReadInt32();
long experience = br.ReadInt64();
long money = br.ReadInt64();
float posX = br.ReadSingle();
float posY = br.ReadSingle();
Player player = new Player(name, health, experience, money, new Vector2(posX, posY));
Console.WriteLine(player);