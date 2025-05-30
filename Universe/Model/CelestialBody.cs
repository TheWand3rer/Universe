#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitsNet;
using Unity.Properties;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public abstract class CelestialBody : ICelestialBody
    {
        private OrbitalData orbitalData;

        public Attributes Attributes { get; protected internal set; }

        public CelestialBody ParentBody { get; internal set; }

        [CreateProperty] public CelestialBodyType Type => Attributes.Type;
        public GravitationalParameter Mu => GravitationalParameter.FromMass(PhysicalData.Mass);

        public IEnumerable<CelestialBody> Hierarchy => PreOrderVisit(this);

        public IEnumerable<CelestialBody> Orbiters => _Orbiters.Values;

        public IEnumerable<CelestialBody> Siblings
        {
            get
            {
                List<CelestialBody> siblings = ParentBody != null ? ParentBody.Orbiters.ToList() : StarSystem.Orbiters.ToList();
                siblings.Remove(this);
                return siblings;
            }
        }

        public int Depth
        {
            get
            {
                int           depth = 0;
                CelestialBody body  = ParentBody;
                while (body != null)
                {
                    body = body.ParentBody;
                    depth++;
                }

                return depth;
            }
        }

        public int Index { get; protected set; }

        public int OrbiterCount => _Orbiters.Count;

        public int SiblingsCount => Siblings.Count();

        [CreateProperty]
        public OrbitalData OrbitalData
        {
            get => orbitalData;
            set
            {
                orbitalData = value;
                if (ParentBody != null)
                {
                    OrbitState = OrbitState.FromOrbitalElements(orbitalData, ParentBody);
                }
                else
                {
                    OrbitState = OrbitState.FromOrbitalElements(orbitalData);
                }
            }
        }

        [CreateProperty] public OrbitState OrbitState { get; private set; }

        [CreateProperty] public virtual PhysicalData PhysicalData { get; set; }

        public Star ParentStar
        {
            get
            {
                CelestialBody body       = this;
                Star          parentStar = null;
                while (body.ParentBody != null)
                {
                    body = body.ParentBody;
                    if (body is Star star)
                    {
                        parentStar = star;
                        break;
                    }
                }

                return parentStar;
            }
        }

        [CreateProperty] public StarSystem StarSystem { get; internal set; }

        [CreateProperty] public virtual string FullName => Name;

        public string Name { get; set; }
        protected Dictionary<string, CelestialBody> _Orbiters { get; }

        protected CelestialBody(string name, CelestialBodyType type)
        {
            Assert.IsFalse(string.IsNullOrEmpty(name));
            Name = name;
            Attributes = new Attributes
            {
                [nameof(Type)] = type.ToString()
            };

            _Orbiters = new Dictionary<string, CelestialBody>();
        }

        public virtual void AddOrbiter(CelestialBody orbiter)
        {
            orbiter.Index = _Orbiters.Count;
            orbiter.SetParentBody(this);

            _Orbiters.Add(orbiter.Name, orbiter);

            foreach (CelestialBody body in orbiter.Hierarchy)
            {
                body.StarSystem = StarSystem;
            }
        }

        public void AddOrbiters(IEnumerable<CelestialBody> newOrbiters)
        {
            foreach (CelestialBody orbiter in newOrbiters)
            {
                AddOrbiter(orbiter);
            }
        }

        /// <summary>
        ///     Returns a string representing the location of this body in the Galaxy.
        ///     The format used is StarSystemName/StarName/PlanetName.
        /// </summary>
        /// <returns>The path.</returns>
        public string GetPath()
        {
            Stack<string> pathStack = new();

            pathStack.Push(Name);

            CelestialBody body = ParentBody;
            while (body != null)
            {
                pathStack.Push(body.Name);
                body = body.ParentBody;
            }

            pathStack.Push(StarSystem.Name);

            string path = string.Empty;

            while (pathStack.Count > 0) path += $"{pathStack.Pop()}/";

            return path[..^1];
        }


        /// <summary>
        ///     Searches in the orbiters of this body for <see cref="orbiterName" />.
        /// </summary>
        /// <param name="orbiterName">The name of the <see cref="CelestialBody" /> to find.</param>
        /// <param name="recursive">If true, will also search descendants.</param>
        /// <returns></returns>
        public bool HasOrbiter(string orbiterName, bool recursive = false)
        {
            bool result = _Orbiters.ContainsKey(orbiterName);
            if (recursive && !result)
            {
                foreach (CelestialBody body in _Orbiters.Values)
                {
                    result = body.HasOrbiter(orbiterName, true);
                    if (result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Sets the parent body of this <see cref="CelestialBody" />.
        ///     Also causes this parent body to become an attractor of this one.
        /// </summary>
        /// <param name="parentBody"></param>
        public virtual void SetParentBody(CelestialBody parentBody)
        {
            ParentBody = parentBody;
            if (ParentBody != null)
            {
                OrbitState.SetAttractor(parentBody);
            }
        }


#if UNITY_EDITOR
        protected virtual void CopyValues() { }
#endif

        public static IEnumerable<CelestialBody> PreOrderVisit(CelestialBody root)
        {
            yield return root;

            foreach (CelestialBody orbiter in root.Orbiters)
            {
                foreach (CelestialBody child in PreOrderVisit(orbiter))
                {
                    yield return child;
                }
            }
        }


        public static IEnumerable<CelestialBody> LevelOrderVisit(CelestialBody root)
        {
            Queue<CelestialBody> queue = new();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                CelestialBody body = queue.Dequeue();
                yield return body;

                foreach (CelestialBody orbiter in body.Orbiters)
                {
                    queue.Enqueue(orbiter);
                }
            }
        }

        public static void VisitHierarchy<TBody>(TBody root, Action<TBody> callback, Func<CelestialBody, IEnumerable<CelestialBody>> visitAlgorithm = null)
            where TBody : CelestialBody
        {
            visitAlgorithm ??= PreOrderVisit;

            foreach (CelestialBody body in visitAlgorithm(root))
            {
                if (body is TBody tBody)
                {
                    callback?.Invoke(tBody);
                }
            }
        }

        public static string RenderTree(CelestialBody root, int indentLength = 2)
        {
            string indent = string.Empty;
            for (int i = 0; i < indentLength; i++)
            {
                indent += " ";
            }

            StringBuilder sb = new();

            PrintNode(sb, root, string.Empty, true);

            return sb.ToString();

            void PrintNode(StringBuilder sb, CelestialBody node, string currentIndent, bool isLast)
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

                CelestialBody[] orbiters = node.Orbiters.ToArray();
                for (int i = 0; i < orbiters.Length; i++)
                {
                    CelestialBody orbiter   = orbiters[i];
                    bool          lastChild = i == orbiters.Length - 1;
                    PrintNode(sb, orbiter, currentIndent + (isLast ? $"{indent}  " : $"│{indent} "), lastChild);
                }
            }
        }

        Mass IAttractor.Mass => PhysicalData.Mass;
    }
}