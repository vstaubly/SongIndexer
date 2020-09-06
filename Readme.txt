The purpose of this program is to search through a directory tree finding song files.
We currently only support MP3 files (via reading the ID3) headers).
For each song, basic info (song title, artist/band, album name, year recorded and track number)
are extracted and written to a DB. A companion program will be created to allow web access to this
database, with basic searching and sorting.

Should be able to use any DB, I'm currently testing with SQLite3.
Following command used to create table:

CREATE TABLE songs ( id INTEGER PRIMARY KEY, title TEXT, artist TEXT, album TEXT, year TEXT, track TEXT );

Usage:
SongIndexer.exe \path\to\MusicFiles ODBC-connect-string

For example:
SongIndexer.exe \Users\vicky\Music\blue_oyster_cult "DSN=SongsDB"
