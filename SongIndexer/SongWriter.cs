using System;
using System.Collections.Generic;
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

        public SongWriter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool connect()
        {
            try {
                conn = new OdbcConnection(connectionString);
                return true;
            } catch (Exception e) {
                Console.Error.WriteLine("Error connecting to database: " + this.connectionString);
                Console.Error.WriteLine("  Exception: " + e);
                return false;
            }
        }

        public bool writeSong(SongFile song)
        {
            string query = "INSERT INTO songs ( title, artist, album, year, track ) VALUES ( ?, ?, ?, ?, ? )";

            OdbcCommand cmd = new OdbcCommand(query, conn);
            cmd.Parameters[0].Value = song.Title;
            cmd.Parameters[1].Value = song.Artist;
            cmd.Parameters[2].Value = song.Album;
            cmd.Parameters[3].Value = song.Year;
            cmd.Parameters[4].Value = song.Track;
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
