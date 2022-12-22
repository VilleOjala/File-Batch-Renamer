/*
File Batch Renamer by Ville Ojala
GNU General Public License, version 3.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System;
using System.Collections.Generic;

namespace FileBatchRenamer
{
    public interface ICSVParser
    {
        bool IsImported { get; }
        public Action<bool> ImportStatusChanged { get; set; }
        Dictionary<string, string> ParseData { get; }

        void ClearParseData();
        bool ParseCSV(string filePath, out string errorMessage);
    }
}