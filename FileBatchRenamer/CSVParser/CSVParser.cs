/*
File Batch Renamer by Ville Ojala
Apache License, Version 2.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace FileBatchRenamer
{
    public class CSVParser : ICSVParser
    {
        public Dictionary<string, string> ParseData { get; private set; }

        private bool isImported = false;
        public bool IsImported
        {
            get
            {
                return isImported;
            }
            private set
            {
                if (isImported == value)
                    return;

                isImported = value;
                ImportStatusChanged(isImported);
            }
        }

        public Action<bool> ImportStatusChanged { get; set; }

        public CSVParser()
        {
            ParseData = new Dictionary<string, string>();
        }

        public bool ParseCSV(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            var parseData = new Dictionary<string, string>();
            var duplicates = new List<string>();

            using (var parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.TrimWhiteSpace = true;

                // Allow the CSV-file to use both commas and semicolons as delimiters,
                // since Microsoft Excel CSV exports can on some machines use semicolons.
                var delimiters = new string[] { ",", ";" };
                parser.SetDelimiters(delimiters);

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (fields.Length >= 2)
                    {
                        string searchName = fields[0];
                        string newName = fields[1];

                        if (parseData.ContainsKey(searchName))
                        {
                            if (!duplicates.Contains(searchName))
                            {
                                duplicates.Add(searchName);
                            }
                        }
                        else
                        {
                            parseData.Add(searchName, newName);
                        }
                    }
                }
            }

            // Duplicate search names are not allowed, this will fail the import.
            if (duplicates.Count > 0)
            {
                errorMessage = "Importing CSV failed: duplicate search names are not allowed.\n\n" +
                               "Search names with duplicates: \n\n";

                foreach (var duplicate in duplicates)
                {
                    errorMessage += duplicate.ToString() + "\n";
                }

                return false;
            }

            ParseData = new Dictionary<string, string>(parseData);
            IsImported = true;
            return true;
        }

        public void ClearParseData()
        {
            ParseData.Clear();
            IsImported = false;
        }
    }
}