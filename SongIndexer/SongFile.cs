using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongIndexer
{
    public class SongFile
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Track { get; set; }

        public override string ToString()
        {
            string descr = "filename=" + Filename;
            if (Title != null)
                descr += " title=" + Title;
            if (Artist != null)
                descr += " artist=" + Artist;
            if (Album != null)
                descr += " album=" + Album;
            if (Year != null)
                descr += " year=" + Year;
            if (Track != null)
                descr += " track#=" + Track;
            return descr;
        }
    }
}
