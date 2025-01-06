using System.Collections.Generic;
using Unity.Properties;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

namespace VindemiatrixCollective.Universe.Model
{
    public abstract class CelestialBody : ICelestialBody
    {
        private OrbitalData orbitalData;

        protected Dictionary<string, CelestialBody> Orbiters { get; private set; }
        public Attributes Attributes { get; protected internal set; }

        public CelestialBody ParentBody { get; internal set; }

        public CelestialBodyType Type => Attributes.Type;
        public GravitationalParameter Mu => GravitationalParameter.FromMass(PhysicalData.Mass);
        public int Index { get; protected set; }

        [CreateProperty]
        public OrbitalData OrbitalData
        {
            get => orbitalData;
            set
            {
                this.orbitalData = value;
                if (ParentStar != null)
                    OrbitState = OrbitState.FromOrbitalElements(orbitalData, ParentStar);
                else 
                    OrbitState = OrbitState.FromOrbitalElements(orbitalData);
            }
        }

        [CreateProperty]
        public OrbitState OrbitState { get; private set; }

        [CreateProperty]
        public PhysicalData PhysicalData { get; set; }

        public Star ParentStar { get; internal set; }

        [CreateProperty]
        public StarSystem StarSystem { get; internal set; }

        [CreateProperty]
        public virtual string FullName => Name;

        public string Name { get; set; }


        /// <summary>
        /// Returns a string representing the location of this body in the Galaxy.
        /// The format used is StarSystemName/[0..9]starIndex/PlanetName.
        /// </summary>
        public abstract string Path { get; }

        protected virtual void AddOrbiter(CelestialBody orbiter)
        {
            orbiter.Index = Orbiters.Count;
            orbiter.StarSystem = StarSystem;
            orbiter.SetParentBody(this);
            
            Orbiters.Add(orbiter.Name, orbiter);
        }

        protected void AddOrbiters(IEnumerable<CelestialBody> newOrbiters)
        {
            foreach (CelestialBody orbiter in newOrbiters)
            {
                AddOrbiter(orbiter); 
            }
        }

        protected CelestialBody(string name, CelestialBodyType type)
        {
            this.Name = name;
            Attributes = new Attributes
            {
                [nameof(Type)] = type.ToString()
            };

            Orbiters = new Dictionary<string, CelestialBody>();
        }

        /// <summary>
        /// Splits the <see cref="Path"/> of this body as a tuple representing 
        /// the individual components.
        /// The first value is the name of the Star System as a string.
        /// The second is a 0-based index of the parent star.
        /// The third is the name of the planet.
        /// </summary>
        /// <param name="location">The Path.</param>
        /// <returns>The location tuple.</returns>
        public static (string, int, string) Location(string location)
        {
            string[] target = location.Split('/');
            string   system = target[0];
            int      star   = target[1][0] - 65;
            string   body   = target[2];

            return (system, star, body);
        }

        /// <summary>
        /// Sets the parent body of this <see cref="CelestialBody"/>.
        /// Also causes this parent body to become an attractor of this one.
        /// </summary>
        /// <param name="parentBody"></param>
        public virtual void SetParentBody(CelestialBody parentBody)
        {
            ParentBody = parentBody;
            if (ParentBody!=null)
                OrbitState.SetAttractor(parentBody);
        }

        /// <summary>
        /// Searches in the orbiters of this body for <see cref="orbiterName"/>. 
        /// </summary>
        /// <param name="orbiterName">The name of the <see cref="CelestialBody"/> to find.</param>
        /// <param name="recursive">If true, will also search descendants.</param>
        /// <returns></returns>

        public bool HasOrbiter(string orbiterName, bool recursive = false)
        {
            bool result = Orbiters.ContainsKey(orbiterName);
            if (recursive && !result)
            {
                foreach (CelestialBody body in Orbiters.Values)
                {
                    result = body.HasOrbiter(orbiterName, true);
                    if (result)
                        break;
                }
            }

            return result;
        }


#if UNITY_EDITOR
        protected virtual void CopyValues()
        {
        }
#endif
    }
}