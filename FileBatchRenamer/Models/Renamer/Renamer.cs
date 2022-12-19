using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;

namespace FileBatchRenamer
{
    public class Renamer : IRenamer
    {
        public List<RenameItem> RenameItems { get; private set; }
        public string AppDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FileBatchRenamer";

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

        public void Rename()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                if (!Directory.Exists(AppDataPath))
                {
                    Directory.CreateDirectory(AppDataPath);
                }
            }
            catch (Exception e)
            {
                string errorMessage = "Renaming failed: An exception (" + e.GetType().ToString() + ") occured when trying to create directory '" + AppDataPath + "'. " + e.Message;
                MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
                return;
            }

            string logFilePath = AppDataPath + @"\" + "RenamingLog_" + string.Format("{0:yyyy-MM-dd_HH-mm--ss-fff}", DateTime.Now) + ".txt";
            
            try 
            {
                File.Create(logFilePath);
            }
            catch (Exception e)
            {
                //TODO:
                return;
            }

            using (var streamWriter = new StreamWriter(logFilePath))
            {
                string startLog = "Starting renaming...";
                streamWriter.WriteLine(startLog);

                // Check that a file still exists
                // Check that the new name (and the resulting new path) is valid
                // Handle the cases where duplicate new names cause clashes for files of same type in the same directory
                foreach (var renameItem in RenameItems)
                {
                    var filePath = renameItem.FilePath;

                    if (!File.Exists(filePath))
                    {
                        string errorLog = "ERROR: File '" + filePath + "' does not exist.";
                        streamWriter.WriteLine(errorLog);
                        continue;
                    }

                    var newName = renameItem.NewName;

                    if (string.IsNullOrEmpty(newName))
                    {
                        string errorLog = "ERROR: The new name for file '" + filePath + "' is null or empty.";
                        streamWriter.WriteLine(errorLog);
                        continue;
                    }

                    var fileType = Path.GetExtension(filePath);
                    var newNameWithExtension = newName + fileType;
                    var newPath = Path.GetDirectoryName(filePath) + @"\" + newNameWithExtension;

                    try
                    {
                        File.Move(filePath, newPath, overwrite: false);
                        string log = "Renamed file '" + filePath + "' as '" + newPath + "'.";
                        streamWriter.WriteLine(log);
                    }
                    catch (Exception e)
                    {
                        string errorLog = "ERROR: An exception (" + e.GetType().ToString() + ") occured when trying to rename file '" + filePath + "' as ' " + newPath + "'. " + e.Message;
                        streamWriter.WriteLine(errorLog);
                    }
                }

                stopWatch.Stop();
                streamWriter.WriteLine("Finished renaming, elapsed time " + stopWatch.ElapsedMilliseconds / 1000 + "seconds.");
            }
        }
    }
}