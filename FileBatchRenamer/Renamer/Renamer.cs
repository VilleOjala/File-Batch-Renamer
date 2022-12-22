/*
File Batch Renamer by Ville Ojala
Apache License, Version 2.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System;
using System.Collections.Generic;
using System.IO;
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
            try
            {
                if (!Directory.Exists(AppDataPath))
                {
                    Directory.CreateDirectory(AppDataPath);
                }
            }
            catch (Exception e)
            {
                string errorMessage = "Renaming failed: An exception (" + e.GetType().ToString() + ") " +
                                      "occured when trying to create directory '" + AppDataPath + "'. " + e.Message;

                MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
                return;
            }

            string logFilePath = AppDataPath + @"\" + "RenamingLog_" + string.Format("{0:yyyy-MM-dd_HH-mm--ss-fff}", DateTime.Now) + ".txt";

            try
            {
                using var streamWriter = new StreamWriter(logFilePath);
                var stopWatch = new Stopwatch();
                string startLog = "Starting to rename " + RenameItems.Count + " files.";
                streamWriter.WriteLine(startLog);

                int successCount = 0;
                int failCount = 0;
                stopWatch.Start();

                foreach (var renameItem in RenameItems)
                {
                    string filePath = renameItem.FilePath;

                    if (!File.Exists(filePath))
                    {
                        string errorLog = "ERROR: File '" + filePath + "' does not exist.";
                        streamWriter.WriteLine(errorLog);
                        failCount++;
                        continue;
                    }

                    string newName = renameItem.NewName;

                    if (string.IsNullOrEmpty(newName))
                    {
                        string errorLog = "ERROR: The new name for file '" + filePath + "' is null or empty.";
                        streamWriter.WriteLine(errorLog);
                        failCount++;
                        continue;
                    }

                    string fileType = Path.GetExtension(filePath);
                    string newNameWithExtension = newName + fileType;
                    string newPath = Path.GetDirectoryName(filePath) + @"\" + newNameWithExtension;

                    try
                    {
                        File.Move(filePath, newPath, overwrite: false);
                        string log = "Renamed file '" + filePath + "' as '" + newPath + "'.";
                        streamWriter.WriteLine(log);
                        successCount++;
                    }
                    catch (Exception e)
                    {
                        string errorLog = "ERROR: An exception (" + e.GetType().ToString() + ") occured when trying to rename file '"
                                                                + filePath + "' as ' " + newPath + "'. " + e.Message;

                        streamWriter.WriteLine(errorLog);
                        failCount++;
                    }
                }

                stopWatch.Stop();
                streamWriter.WriteLine("Renaming finished: " + successCount + " succeeded / " + failCount + " failed / " + "elapsed time "
                                       + stopWatch.ElapsedMilliseconds / 1000f + " seconds.");

                try
                {
                    string args = string.Format("/e, /select, \"{0}\"", logFilePath);
                    var processStartInfo = new ProcessStartInfo("explorer", args);
                    Process.Start(processStartInfo);
                }
                catch (Exception e)
                {
                    string errorMessage = "An exception (" + e.GetType().ToString() + ") occured when trying to open the logging folder. " + e.Message;
                    MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
                }
            }
            catch (Exception e)
            {
                string errorMessage = "An exception (" + e.GetType().ToString() + ") occured when trying to initialize a log file. " + e.Message;
                MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
            }        
        }
    }
}