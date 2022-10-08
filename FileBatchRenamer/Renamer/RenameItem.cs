using System;
using System.Collections.Generic;
using System.Linq;

namespace FileBatchRenamer
{
    public class RenameItem
    {
        public string FilePath { get; private set; }
        public string SearchName { get; private set; }
        public string NewName { get; private set; }

        public RenameItem()
        {
            FilePath = string.Empty;
            SearchName = string.Empty;
            NewName = string.Empty;
        }

        public RenameItem(string filePath, string searchName, string newName)
        {
            FilePath = filePath;
            SearchName = searchName;
            NewName = newName;
        }
    }
}
