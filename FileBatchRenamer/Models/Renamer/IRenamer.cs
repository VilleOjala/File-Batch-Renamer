using System.Collections.Generic;

namespace FileBatchRenamer
{
    public interface IRenamer
    {
        List<RenameItem> RenameItems { get; }

        void Rename();
        void UpdateRenameItems(MatchingMethod matchingMethod, Dictionary<string, string> parseData, HashSet<string> files);
    }
}