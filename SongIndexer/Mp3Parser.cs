using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongIndexer
{
    public class Mp3Parser
    {
        private FileStream stream;
        private byte[] buffer = new byte[1024];
        private int bytesRead = 0;
        private SongFile song;

        public Mp3Parser(string filename)
        {
            song = new SongFile();
            song.Filename = filename;
        }

        public Mp3Parser(string filename, SongFile song)
        {
            this.song = song;
            song.Filename = filename;
        }

        public SongFile parse()
        {
            try
            {
                stream = new FileStream(song.Filename, FileMode.Open);
                bytesRead = stream.Read(buffer, 0, 1024); // should be enough to include ID3 headers & tags
                int offset = readHeader(0);
                while (offset > 0) {
                    offset = readNextTag(offset);
                }
            } catch {
                // should log error here
                return null;
            } finally {
                if (stream != null)
                    stream.Close();
                stream = null;
            }
            return song;
        }

        // read header starting at offset, return offset of next tag, -1 if not an ID3 header
        private int readHeader(int offset)
        {
            byte[] idHeader = { (byte)'I', (byte)'D', (byte)'3' };

            for (int i = offset; i < idHeader.Length; i++) {
                if (i >= bytesRead) {
                    // not enough data to match ID3 header
                    return -1;
                }
                if (buffer[i] != idHeader[i - offset]) {
                    return -1;
                }
            }
            offset += idHeader.Length;
            // should check version here... not going to for now
            offset += 2; // skip version
            Int32 length = 0;
            for (int i = 0; i < 4; i++) {
                length |= (buffer[offset + i] & 0x7F) << (7 * i);
            }
            if (length > bytesRead) {
                Console.Error.WriteLine("Warning: did not read enough data to parse all ID3 tags, length=" + length);
            }
            offset += 4;
            return offset;
        }

        int readNextTag(int offset)
        {
            if (offset + 10 > bytesRead) {
                return -1; // not enough data for fixed part of tag header
            }
            
            byte[] typeTag = new byte[4];
            for (int i = 0; i < typeTag.Length; i++) {
                typeTag[i] = buffer[i + offset];
            }
            offset += 4; // advance past tag ID bytes
            Int32 length = 0;
            for (int i = 0; i < 4; i++)
            {
                length |= (buffer[offset + i] & 0x7F) << (7 * i);
            }
            offset += 6; // advance past tag length field, and flags
            byte[] tagData = null;
            if (length > 0) // length field does not include tag header, so we check for any data at all
            {
                tagData = new byte[length];
                for (int i = 0; i < length; i++) {
                    tagData[i] = buffer[i + offset];
                }
            }
            handleTagTypes(typeTag, tagData);
            return offset + length;
        }

        void handleTagTypes(byte[] type, byte[] data)
        {
            string typeStr = Encoding.UTF8.GetString(type, 0, type.Length);
            string dataStr = Encoding.UTF8.GetString(data, 0, data.Length);

            if ("TIT2".Equals(typeStr))
            {
                song.Title = dataStr;
            }
            else if ("TALB".Equals(typeStr))
            {
                song.Album = dataStr;
            }
            else if ("TPE1".Equals(typeStr))
            {
                song.Artist = dataStr;
            }
            else if ("TYER".Equals(typeStr))
            {
                song.Year = dataStr;
            }
            else if ("TRCK".Equals(typeStr))
            {
                song.Track = dataStr;
            }
        }
    }
}
