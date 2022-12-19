using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

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
                    string messageBoxText = "File '" + file + "' does not exist.";
                    MessageBoxDisplayer.DisplayErrorMessageBox(messageBoxText);
                    continue;
                }

                if (ImportedFiles.Contains(file))
                {
                    continue;
                }

                ImportedFiles.Add(file);
            }
        }

        public void ImportFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
            {
                string messageBoxText = "Directory '" + path + "' does not exist."; 
                MessageBoxDisplayer.DisplayErrorMessageBox(messageBoxText);
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