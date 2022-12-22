/*
File Batch Renamer by Ville Ojala
Apache License, Version 2.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System.Collections.Generic;

namespace FileBatchRenamer
{
    public interface IRenamer
    {
        List<RenameItem> RenameItems { get; }
        string AppDataPath { get; }

        void Rename();
        void UpdateRenameItems(MatchingMethod matchingMethod, Dictionary<string, string> parseData, HashSet<string> files);
    }
}