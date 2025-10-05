// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using Unity.Properties;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public abstract class CelestialBody : ICelestialBody, IAttractor, IEnumerable<CelestialBody>
    {
        private OrbitalData orbitalData;

        public Attributes Attributes { get; protected internal set; }

        public CelestialBody this[string key] => _Orbiters[key];

        public CelestialBody this[int index] => _Orbiters.Values.ElementAt(index);

        public CelestialBody ParentBody { get; internal set; }

        [CreateProperty] public CelestialBodyType Type => Attributes.Type;
        public GravitationalParameter Mu => GravitationalParameter.FromMass(PhysicalData.Mass);

        public IEnumerable<CelestialBody> Hierarchy => Tree.PreOrderVisit(this).Cast<CelestialBody>();

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

        public int Index { get; protected internal set; }

        public int OrbiterCount => _Orbiters.Count;

        public int SiblingsCount => Siblings.Count();

        [CreateProperty]
        public OrbitalData OrbitalData
        {
            get => orbitalData;
            set
            {
                if (value == null)
                {
                    return;
                }

                orbitalData = value;
                if (ParentBody != null)
                {
                    OrbitState = OrbitState.FromOrbitalElements(orbitalData, ParentBody);
                }
                else
                {
                    OrbitState = OrbitState.FromOrbitalElements(orbitalData);
                }
#if UNITY_EDITOR
                CopyOrbitalValues();
#endif
            }
        }

        [CreateProperty] public OrbitState OrbitState { get; private set; }

        [CreateProperty] public virtual PhysicalData PhysicalData { get; set; }

        public Star ParentStar => Tree.FindAncestor<Star>(this);

        [CreateProperty] public StarSystem StarSystem { get; internal set; }

        [CreateProperty] public virtual string FullName => Name;

        public string Name { get; set; }

        protected Dictionary<string, CelestialBody> _Orbiters { get; }

        protected CelestialBody(string name, CelestialBodyType type)
        {
            Assert.IsFalse(string.IsNullOrEmpty(name));
            Name       = name;
            Attributes = new Attributes { [nameof(Type)] = type.ToString() };

            _Orbiters = new Dictionary<string, CelestialBody>();
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

        public IEnumerator<CelestialBody> GetEnumerator() => Orbiters.GetEnumerator();

        public Length DistanceTo(CelestialBody body)
        {
            Assert.IsNotNull(body, nameof(body));
            Vector3d from = OrbitState?.Position ?? Vector3d.zero;
            Vector3d to   = body.OrbitState.Position;
            double   d    = Vector3d.Distance(from, to);
            return Length.FromMeters(d);
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

            while (pathStack.Count > 0)
            {
                path += $"{pathStack.Pop()}/";
            }

            return path[..^1];
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
            Assert.IsNotNull(newOrbiters, nameof(Orbiters));

            foreach (CelestialBody orbiter in newOrbiters)
            {
                AddOrbiter(orbiter);
            }
        }

        /// <summary>
        ///     Sets the parent body of this <see cref="CelestialBody" />.
        ///     Also causes this parent body to become an attractor of this one.
        /// </summary>
        /// <param name="parentBody"></param>
        public virtual void SetParentBody(CelestialBody parentBody)
        {
            ParentBody = parentBody;
            if (ParentBody != null && OrbitState != null)
            {
                OrbitState.SetAttractor(parentBody);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


#if UNITY_EDITOR

        #region Unity_Editor

#if UNITY_EDITOR

        [Serializable]
        public struct OrbitalDataValues
        {
            public string AxialTilt;
            public string Eccentricity;
            public string Inclination;
            public string OrbitalPeriod;
            public string SemiMajorAxis;
            public string SiderealRotation;
        }

        public OrbitalDataValues orbitalDataValues;

        protected void CopyOrbitalValues()
        {
            orbitalDataValues.SemiMajorAxis    = OrbitalData.SemiMajorAxis.AstronomicalUnits.ToString("0.00 au");
            orbitalDataValues.Eccentricity     = OrbitalData.Eccentricity.Value.ToString("0.00");
            orbitalDataValues.SiderealRotation = OrbitalData.SiderealRotationPeriod.Days.ToString("0.00 d");
            orbitalDataValues.Inclination      = OrbitalData.Inclination.Degrees.ToString("0.00 °");
            orbitalDataValues.AxialTilt        = OrbitalData.AxialTilt.Degrees.ToString("0.00 °");
            orbitalDataValues.OrbitalPeriod = OrbitalData.Period.Years365 < 0.1f
                ? OrbitalData.Period.Days.ToString("0.00 d")
                : OrbitalData.Period.Years365.ToString("0.00 y");
        }
#endif

        #endregion

#endif

        #region ITreeNode

        IEnumerable<ITreeNode> ITreeNode.Children => Orbiters;

        ITreeNode ITreeNode.Parent => ParentBody;
        ITreeNode ITreeNode.this[string name] => this[name];

        #endregion

        #region IAttractor

        Mass IAttractor.Mass => PhysicalData.Mass;

        #endregion
    }
}