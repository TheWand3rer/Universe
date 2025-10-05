// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface ITreeNode
    {
        IEnumerable<ITreeNode> Children { get; }

        ITreeNode this[string name] { get; }

        ITreeNode Parent { get; }
        string Name { get; }
    }

    public static class Tree
    {
        public static string RenderTree(ITreeNode root, int indentLength = 2)
        {
            string indent = string.Empty;
            for (int i = 0; i < indentLength; i++)
            {
                indent += " ";
            }

            StringBuilder sb = new();

            PrintNode(sb, root, string.Empty, true);

            return sb.ToString();

            void PrintNode(StringBuilder sb, ITreeNode node, string currentIndent, bool isLast)
            {
                sb.Append(currentIndent);
                if (isLast)
                {
                    sb.Append("└── ");
                }
                else
                {
                    sb.Append("├── ");
                }

                sb.AppendLine(node.Name);

                ITreeNode[] orbiters = node.Children.ToArray();
                for (int i = 0; i < orbiters.Length; i++)
                {
                    ITreeNode treeNode  = orbiters[i];
                    bool      lastChild = i == orbiters.Length - 1;
                    PrintNode(sb, treeNode, currentIndent + (isLast ? $"{indent}  " : $"│{indent} "), lastChild);
                }
            }
        }

        public static IEnumerable<ITreeNode> PreOrderVisit(ITreeNode root)
        {
            yield return root;

            foreach (ITreeNode orbiter in root.Children)
            {
                foreach (ITreeNode child in PreOrderVisit(orbiter))
                {
                    yield return child;
                }
            }
        }

        public static IEnumerable<ITreeNode> LevelOrderVisit(ITreeNode root)
        {
            Queue<ITreeNode> queue = new();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                ITreeNode body = queue.Dequeue();
                yield return body;

                foreach (ITreeNode orbiter in body.Children)
                {
                    queue.Enqueue(orbiter);
                }
            }
        }

        public static void VisitHierarchy<TOrbiter>(
            TOrbiter root, Action<TOrbiter> callback = null, Func<ITreeNode, IEnumerable<ITreeNode>> visitAlgorithm = null)
            where TOrbiter : ITreeNode
        {
            visitAlgorithm ??= PreOrderVisit;

            foreach (ITreeNode body in visitAlgorithm(root))
            {
                if (body is TOrbiter orbiter)
                {
                    callback?.Invoke(orbiter);
                }
            }
        }

        public static TOrbiter FindAncestor<TOrbiter>(ITreeNode treeNode) where TOrbiter : class, ITreeNode
        {
            foreach (ITreeNode ancestor in Ancestors<ITreeNode>(treeNode))
            {
                if (ancestor is TOrbiter ancestorOrbiter)
                {
                    return ancestorOrbiter;
                }
            }

            return null;
        }

        public static IEnumerable<TOrbiter> Ancestors<TOrbiter>(ITreeNode treeNode) where TOrbiter : ITreeNode
        {
            ITreeNode current = treeNode;

            while (current.Parent != null)
            {
                current = current.Parent;
                yield return (TOrbiter)current;
            }
        }
    }
}