using System;
using System.IO;
using Newtonsoft.Json;

namespace Corruptor
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      File.WriteAllText("exampleconfig.conf", JsonConvert.SerializeObject(new Config()));
      if (args.Length != 3)
      {
        Console.WriteLine("Usage: Corruptor.exe <config> <file> <output> or Corruptor.exe <config> <directory> <output>");
        return;
      }
      var config = args[0];
      var file = args[1];
      var output = args[2];
      int fileOrDirectory = File.Exists(file) ? 1 : Directory.Exists(file) ? 2 : 0;
      if (fileOrDirectory == 0)
      {
        Console.WriteLine("File or directory not found");
        return;
      }
      if (fileOrDirectory == 1)
      {
        var corruptor = new Corruptor(config);
        corruptor.Corrupt(file, output);
      }
      else
      {
        var corruptor = new Corruptor(config);
        corruptor.CorruptDirectory(file, output);
      }
    }
  }
}