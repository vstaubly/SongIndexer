//
// Reads a SongFile object from an MP3 file... could serve as template for reading from other formats (ACC/AIFF/etc.)
//   (WAV and other files don't contain metadata about the song)
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongIndexer
{
    // based on MP3/ID3 format reference at https://id3.org/id3v2.4.0-structure
    public class Mp3Parser
    {
        private FileStream stream;
        private byte[] buffer = new byte[4096];
        private int bytesRead = 0;
        private SongFile song;
        private bool verbose = false;

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
                bytesRead = stream.Read(buffer, 0, buffer.Length); // should be enough to include ID3 headers & tags
                int offset = readHeader(0);
                while (offset > 0) {
                    offset = readNextTag(offset);
                }
            } catch (Exception e) {
                // should log error here
                Console.Error.WriteLine("Exception: " + e);
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

            for (int i = 0; i < idHeader.Length; i++) {
                if ((i + offset) >= bytesRead) {
                    // not enough data to match ID3 header
                    return -1;
                }
                if (buffer[i + offset] != idHeader[i]) {
                    return -1;
                }
            }
            offset += idHeader.Length;
            // should check version here... not going to for now
            offset += 3; // skip version and flags
            Int32 length = getID3LengthField(offset);
            if (verbose && (length > bytesRead)) {
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
            Int32 length = getID3LengthField(offset);
            offset += 6; // advance past tag length field, and flags
            byte[] tagData = null;
            if ((length > 0) && (length + offset < bytesRead)) {
                // length field does not include tag header, so we check for any data at all
                tagData = new byte[length];
                for (int i = 0; i < length; i++) {
                    tagData[i] = buffer[i + offset];
                }
                handleTagTypes(typeTag, tagData);
            }
            return offset + length;
        }

        // reads the weird ID3 length fields... big-endian, but only 7 bits about of every byte
        Int32 getID3LengthField(int offset)
        {
            Int32 length = 0;
            for (int i = 0, shift = 3; i < 4; i++, shift--) {
                length |= (buffer[offset + i] & 0x7F) << (7 * shift);
            }
            if (verbose)
                Console.Out.WriteLine("getID3LengthField: offset=" + offset + " length=" + length);
            return length;
        }

        string encodeTagData(byte[] data)
        {
            int zeroBytes = 0;
            for (int i = 0; i < data.Length; i++) {
                if (data[i] == 0)
                    zeroBytes++;
            }
            if (false) {
                return Encoding.UTF8.GetString(data, 0, data.Length);
            } else {
                string dataStr = "";
                for (int i = 0; i < data.Length; i++) {
                    if ((data[i] > 0x1F) && (data[i] < 0x7F)) // strict ASCII filter
                        dataStr += (char)data[i];
                }
                return dataStr;
            }
        }

        void handleTagTypes(byte[] type, byte[] data)
        {
            string typeStr = Encoding.UTF8.GetString(type, 0, type.Length);
            string dataStr = null;

            if (verbose)
                Console.Out.WriteLine("Tag=" + typeStr);
            if ("TIT2".Equals(typeStr))
            {
                dataStr = encodeTagData(data);
                song.Title = dataStr;
            }
            else if ("TALB".Equals(typeStr))
            {
                dataStr = encodeTagData(data);
                song.Album = dataStr;
            }
            else if ("TPE1".Equals(typeStr))
            {
                dataStr = encodeTagData(data);
                song.Artist = dataStr;
            }
            else if ("TYER".Equals(typeStr))
            {
                dataStr = encodeTagData(data);
                song.Year = dataStr;
            }
            else if ("TRCK".Equals(typeStr))
            {
                // TBD.... track is numeric field
                // song.Track = dataStr;
            }
        }
    }
}
