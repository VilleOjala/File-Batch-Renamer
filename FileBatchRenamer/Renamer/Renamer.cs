using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.Specialized;

namespace FileBatchRenamer
{
    public static class Renamer
    {
        public static List<RenameItem> RenameItems { get; private set; } = new List<RenameItem>();

        public static void UpdateRenameItems(MatchingMethod matchingMethod)
        {
            var renameItems = new List<RenameItem>();

            foreach (var file in FileHandler.ImportedFiles)
            {
                var renameItem = new RenameItem(file);
                string comparisonString;

                switch (matchingMethod)
                {
                    case MatchingMethod.FileName:
                        comparisonString = Path.GetFileName(file);
                        break;
                    case MatchingMethod.FileNameWithoutExtension:
                        comparisonString = Path.GetFileNameWithoutExtension(file);
                        break;
                    default:
                        comparisonString = file;
                        break;
                }

                if (CSVParser.ParseData.ContainsKey(comparisonString))
                {
                    renameItem.SearchName = comparisonString;
                    renameItem.NewName = CSVParser.ParseData[comparisonString];
                }

                renameItems.Add(renameItem);
            }

            RenameItems = new List<RenameItem>(renameItems);
        }

        //TODO: Also should produce a log of all the seteps
        public static void Rename()
        {
            // Check that a file still exists
            // Check that the new name (and the resulting new path) is valid
            // Handle the cases where duplicate new names cause clashes for files of same type in the same directory
        }
    }
}