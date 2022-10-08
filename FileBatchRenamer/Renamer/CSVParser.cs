using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FileBatchRenamer
{
    public static class CSVParser
    {
        public static Dictionary<string, string> ParseData { get; private set; } = new Dictionary<string, string>();

        //TODO: How to handle possible duplicates in the csv data? 
        public static void ParseCSV(string filePath)
        {

        }

        public static void ClearParseData()
        {
            ParseData.Clear();
        }    
    }
}