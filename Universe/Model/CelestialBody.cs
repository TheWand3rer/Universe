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
        
        public static (string, int, string) Location(string location)
        {
            string[] target = location.Split('/');
            string   system = target[0];
            int      star   = target[1][0] - 65;
            string   body   = target[2];

            return (system, star, body);
        }

        public virtual void SetParentBody(CelestialBody parentBody)
        {
            ParentBody = parentBody;
            if (ParentBody!=null)
                OrbitState.SetAttractor(parentBody);
        }

        public bool HasOrbiter(string orbiterName)
        {
            return Orbiters.ContainsKey(orbiterName);
        }


#if UNITY_EDITOR
        protected virtual void CopyValues()
        {
        }
#endif
    }
}