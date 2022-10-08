using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FileBatchRenamer
{
    public static class FileHandler
    {
        public static HashSet<string> ImportedFiles { get; private set; } = new HashSet<string>();

        public static void ImportFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || ImportedFiles.Contains(filePath))
                return;

            ImportedFiles.Add(filePath);
        }
        
        public static void RemoveFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && ImportedFiles.Contains(filePath))
            {
                ImportedFiles.Remove(filePath);
            }
        }

        public static void ClearFiles()
        {
            ImportedFiles.Clear();
        }
    }
}