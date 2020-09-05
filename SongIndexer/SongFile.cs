using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongIndexer
{
    class SongFile
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }

        public void parseMP3(string filename)
        {
            Filename = filename;
            FileStream stream = new FileStream(filename, FileMode.Open);
            Byte[] buffer = new byte[500];
            int count = stream.Read(buffer, 0, 500);
            stream.Close();
        }

        public override string ToString()
        {
            return "filename=" + Filename;
        }
    }
}
