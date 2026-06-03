// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System.Collections.Generic;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface ITreeNode : IName
    {
        IEnumerable<ITreeNode> Children { get; }

        ITreeNode this[string name] { get; }

        ITreeNode Parent { get; }
    }
}