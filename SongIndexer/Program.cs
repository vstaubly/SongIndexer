using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Hello " + args[0]);
            string startingDirectory = ".";
            if (args.Length > 1)
                startingDirectory = args[1];
            string[] files = Directory.GetFiles(startingDirectory, "*.mp3", SearchOption.AllDirectories);
            for (string file in files)
                Console.Out.WriteLine("File=" + file);
        }
    }
}
