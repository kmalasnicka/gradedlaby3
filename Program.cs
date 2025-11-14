using System;

class Program{
    static void Main(string[] args){
        string home = Enviroment.GetFolderPath(Enviroment.SpecialFolder.UserProfile);
        IEnumerable<string> markdowns = Directory.EnumerateFiles(home, "*.md", SearchOption.AllDirectories)
        .Select(f => Path.Combine(Path.GetDirectoryName(f), Path.GetFileNameWithoutExtension(f)));
    }
    foreach(var md in markdowns){
        Console.WriteLine(md);
    }
    IEnumerable<string> pdfs = Directory.EnumerateFiles(home, "*.pdf", SearchOption.AllDirectories)
        .Select(f => Path.Combine(Path.GetDirectoryName(f), Path.GetFileNameWithoutExtension(f)));
    foreach(var pdf in pdfs){
        Console.WriteLine(pdf);
    }
    IEnumerable<string> toRemove = markdowns.Join(pdfs, str => str, str => str, (md, pdf) => md);//?
    foreach(var file in toRemove){
        Console.WriteLine(file);
    }
}