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