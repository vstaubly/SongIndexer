using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongIndexer
{
    public class SongWriter
    {
        private string connectionString = null;
        private OdbcConnection conn = null;
        public bool Verbose { get; set; }

        public SongWriter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool connect()
        {
            try {
                conn = new OdbcConnection(connectionString);
                conn.Open();
                return true;
            } catch (Exception e) {
                Console.Error.WriteLine("Error connecting to database: " + this.connectionString);
                Console.Error.WriteLine("  Exception: " + e);
                return false;
            }
        }

        public OdbcParameter makeStringParam(OdbcCommand cmd, string fieldname, string value)
        {
            OdbcParameter param = cmd.CreateParameter();
            param.ParameterName = fieldname;
            param.DbType = System.Data.DbType.String;
            param.Value = value;
            return param;
        }

        /*
        public bool writeSong(SongFile song)
        {
            string query = "INSERT INTO songs ( title, artist, album, year, track ) VALUES ( ?, ?, ?, ?, ? )";

            OdbcCommand cmd = new OdbcCommand(query, conn);
            cmd.Parameters.Add(makeStringParam(cmd, "title", song.Title));
            cmd.Parameters.Add(makeStringParam(cmd, "artist", song.Artist));
            cmd.Parameters.Add(makeStringParam(cmd, "album", song.Album));
            cmd.Parameters.Add(makeStringParam(cmd, "year", song.Year));
            cmd.Parameters.Add(makeStringParam(cmd, "track", song.Track));
            cmd.ExecuteNonQuery();
            return true;
        }
        */

        private string quoteString(string s)
        {
            if ((s == null) || (s.Length < 1)) {
                return "null";
            } else {
                return "'" + s.Replace("'", "''") + "'";
            }
        }

        public bool writeSong(SongFile song)
        {
            string query = "INSERT INTO songs ( title, artist, album, year, track ) VALUES ( " + quoteString(song.Title) +
                ", " + quoteString(song.Artist) + ", " + quoteString(song.Album) + ", " + quoteString(song.Year) + ", " +
                quoteString(song.Track) + " )";
            if (Verbose)
                Console.Out.WriteLine("Query=" + query);
            OdbcCommand cmd = new OdbcCommand(query, conn);
            cmd.ExecuteNonQuery();
            return true;
        }

        public void close()
        {
            if (conn != null)
                conn.Close();
        }
    }
}
