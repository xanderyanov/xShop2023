namespace MirchasovStore.Models
{

    public interface ITreeNode
    {
        string IdAsString { get; }
        string Name { get; }
        IEnumerable<ITreeNode> ChildNodes { get; }
        int ChildNodeCount { get; }
        ITreeNode ParentNode { get; }
    }
    public class TreeModel
    {
    }
}
