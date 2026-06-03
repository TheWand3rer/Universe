// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace VindemiatrixCollective.Universe
{
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

        public static void VisitHierarchy<TNode>(
            TNode root, Action<TNode> callback = null, Func<ITreeNode, IEnumerable<ITreeNode>> visitAlgorithm = null)
            where TNode : ITreeNode
        {
            visitAlgorithm ??= PreOrderVisit;

            foreach (ITreeNode body in visitAlgorithm(root))
            {
                if (body is TNode orbiter)
                {
                    callback?.Invoke(orbiter);
                }
            }
        }

        public static TNode FindAncestor<TNode>(ITreeNode treeNode) where TNode : class, ITreeNode
        {
            foreach (ITreeNode ancestor in Ancestors<ITreeNode>(treeNode))
            {
                if (ancestor is TNode ancestorOrbiter)
                {
                    return ancestorOrbiter;
                }
            }

            return null;
        }

        public static IEnumerable<TNode> Ancestors<TNode>(ITreeNode treeNode, bool includeSelf = false) where TNode : ITreeNode
        {
            ITreeNode current = treeNode;

            if (includeSelf) yield return (TNode)current;

            while (current.Parent != null)
            {
                current = current.Parent;
                yield return (TNode)current;
            }
        }

        public static TNode FindCommonAncestor<TNode>(TNode a, TNode b) where TNode : class, ITreeNode
        {
            TNode[] ancestorsA = Ancestors<TNode>(a, true).ToArray();
            TNode[] ancestorsB = Ancestors<TNode>(b, true).ToArray();

            foreach (TNode ancestor in ancestorsB)
            {
                if (ancestorsA.Contains(ancestor))
                {
                    return ancestor;
                }
            }

            return null;
        }

        public static int CountAncestors<TNode>(TNode leaf) where TNode : class, ITreeNode
        {
            ITreeNode current = leaf.Parent;
            int       count   = 0;
            while (current != null)
            {
                count++;
                current = current.Parent;
            }

            return count;
        }
    }
}