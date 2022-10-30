
using System.ComponentModel;

namespace FileBatchRenamer
{
    public enum MatchingMethod
    {
        [Description("File Name")]
        FileName,
        [Description("File Name Without Extension")]
        FileNameWithoutExtension,
        [Description("File Path")]
        FilePath
    }
}
