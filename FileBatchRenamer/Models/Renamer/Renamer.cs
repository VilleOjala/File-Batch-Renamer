using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.Specialized;

namespace FileBatchRenamer
{
    public class Renamer : IRenamer
    {
        public List<RenameItem> RenameItems { get; private set; }

        public Renamer()
        {
            RenameItems = new List<RenameItem>();
        }

        public void UpdateRenameItems(MatchingMethod matchingMethod, Dictionary<string, string> parseData, HashSet<string> files)
        {
            var renameItems = new List<RenameItem>();

            foreach (var file in files)
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

                if (parseData.ContainsKey(comparisonString))
                {
                    renameItem.MatchName = comparisonString;
                    renameItem.NewName = parseData[comparisonString];
                }

                renameItems.Add(renameItem);
            }

            RenameItems = new List<RenameItem>(renameItems);
        }

        //TODO: Also should produce a log of all the seteps
        public void Rename()
        {
            // Check that a file still exists
            // Check that the new name (and the resulting new path) is valid
            // Handle the cases where duplicate new names cause clashes for files of same type in the same directory

            foreach (var renameItem in RenameItems)
            {
                var filePath = renameItem.FilePath;

                if (!File.Exists(filePath))
                {
                    //TODO: Error handling
                    continue;
                }

                var newName = renameItem.NewName;

                if (string.IsNullOrEmpty(newName))
                {
                    //TODO: Error handling
                    continue;
                }
                var fileType = Path.GetExtension(filePath);
                var newNameWithExtension = newName + fileType;
                var newPath = Path.GetDirectoryName(filePath) + @"\" + newNameWithExtension;

                try
                {
                    File.Move(filePath, newPath, overwrite: false);
                }
                catch (FileNotFoundException e)
                {

                }
                catch (ArgumentNullException e)
                {

                }
                catch (UnauthorizedAccessException e)
                {

                }
                catch (PathTooLongException e)
                {

                }
                catch (DirectoryNotFoundException e)
                {

                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}