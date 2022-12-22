/*
File Batch Renamer by Ville Ojala
GNU General Public License, version 3.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System.Collections.Generic;
using System.IO;

namespace FileBatchRenamer
{
    public class FileHandler : IFileHandler
    {
        public HashSet<string> ImportedFiles { get; private set; } = new HashSet<string>();

        public void ImportFiles(string[] files)
        {
            if (files == null)
                return;

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    string errorMessage = "File '" + file + "' does not exist.";
                    MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
                    continue;
                }

                if (ImportedFiles.Contains(file))
                    continue;

                ImportedFiles.Add(file);
            }
        }

        public void ImportFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
            {
                string errorMessage = "Directory '" + path + "' does not exist."; 
                MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
                return;
            }

            var files = Directory.GetFiles(path);
            ImportFiles(files);
        }

        public void RemoveFiles(string[] files)
        {
            if (files == null)
                return;

            foreach (var file in files)
            {
                if (!string.IsNullOrEmpty(file) && ImportedFiles.Contains(file))
                {
                    ImportedFiles.Remove(file);
                }
            }
        }

        public void ClearFiles(string[] files)
        {
            if (files == null)
                return;

            foreach (var file in files)
            {
                if (ImportedFiles.Contains(file))
                {
                    ImportedFiles.Remove(file);
                }
            }
        }

        public void ClearAllFiles()
        {
            ImportedFiles.Clear();
        }
    }
}