/*
File Batch Renamer by Ville Ojala
GNU General Public License, version 3.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System.Collections.Generic;

namespace FileBatchRenamer
{
    public interface IFileHandler
    {
        HashSet<string> ImportedFiles { get; }

        void ClearAllFiles();
        void ClearFiles(string[] files);
        void ImportFiles(string[] files);
        void ImportFolder(string path);
        void RemoveFiles(string[] files);
    }
}