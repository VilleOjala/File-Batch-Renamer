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