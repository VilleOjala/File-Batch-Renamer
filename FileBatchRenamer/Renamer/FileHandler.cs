using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace FileBatchRenamer
{
    public static class FileHandler
    {
        public static HashSet<string> ImportedFiles { get; private set; } = new HashSet<string>();

        public static void ImportFiles(string[] files)
        {
            if (files == null)
                return;

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    //TODO: Error message
                    continue;
                } 

                if (ImportedFiles.Contains(file))
                {
                    //TODO: Notify about trying to import the same file twice
                    continue;
                }

                ImportedFiles.Add(file);
            }
        }

        public static void ImportFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
            {
                //TODO: Error message
                return;
            }

            var files = Directory.GetFiles(path);
            ImportFiles(files);
        }
        
        public static void RemoveFiles(string[] files)
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

        public static void ClearFiles()
        {
            ImportedFiles.Clear();
        }
    }
}