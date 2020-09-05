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
            string startingDirectory = ".";
            if (args.Length > 0)
                startingDirectory = args[0];
            string[] files = Directory.GetFiles(startingDirectory, "*.mp3", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Mp3Parser parser = new Mp3Parser(file);
                SongFile song = parser.parse();
                if (song != null)
                    Console.Out.WriteLine("" + song);
                else
                    Console.Out.WriteLine("No data for " + file);
            }
        }
    }
}
