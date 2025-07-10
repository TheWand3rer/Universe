using System.Linq;
using UnitsNet;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits
{
    public class SkyMapper
    {
        private double elapsedTime;

        /// <summary>
        /// Local coordinate frame.
        /// </summary>
        public (Vector3d east, Vector3d north, Vector3d up) Frame { get; private set; }

        public Angle SiderealRotationObserver => ObserverBody.OrbitState.SiderealRotation;

        public CelestialBody ClosestBody { get; }

        public CelestialBody ObserverBody { get; }

        public Quaterniond ObserverPhysicalRotation { get; private set; }

        public Quaterniond WorldToObserverLocalRotation { get; private set; }

        public Star Primary { get; }

        public Vector3d ObserverLocalPosition { get; }

        public Vector3d ObserverPositionSSF { get; private set; }

        public Vector3d SpinAxis { get; private set; }

        public SkyMapper(StarSystem system, CelestialBody observerBody, double latitude, double longitude)
            : this(system, observerBody, new GeoCoordinates(latitude, longitude)) { }

        public SkyMapper(StarSystem system, CelestialBody observerBody, GeoCoordinates coordinates)
        {
            ObserverBody = observerBody;
            CelestialBody[] celestialBodies = system.Hierarchy.OrderBy(s => s.DistanceTo(observerBody)).ToArray();
            ClosestBody = celestialBodies.First(body => body.Type == CelestialBodyType.Planet && body != observerBody);
            Primary     = (Star)celestialBodies.First(body => body.Type == CelestialBodyType.Star);

            ObserverLocalPosition = coordinates.ToCartesian(observerBody.PhysicalData.Radius.Meters);
            DetermineInitialRotationAngle();
        }

        public void Update(double elapsedTime)
        {
            this.elapsedTime = elapsedTime;

            OrbitState state = ObserverBody.OrbitState;
            state.Rotate(elapsedTime);

            SpinAxis                 = state.AngularMomentum.normalized.ToXZYd();
            ObserverPhysicalRotation = Quaterniond.AngleAxis(SiderealRotationObserver.Degrees, SpinAxis);
            Frame                    = BuildLocalFrame();

            Quaterniond observerLocalToWorldRotation = Quaterniond.LookRotation(Frame.north, Frame.up);
            WorldToObserverLocalRotation = Quaterniond.Inverse(observerLocalToWorldRotation);
        }

        /// <summary>
        /// Returns the rotation quaternion for the planet's axis rotation,
        /// according to how much time has passed since the last <see cref="Update"/> call.
        /// </summary>
        /// <param name="planet">The planet to rotate.</param>
        /// <param name="elapsedTime">How much time has passed.</param>
        /// <returns>The rotation quaternion.</returns>
        public Quaterniond CalculateSiderealPlanetRotation(Planet planet)
        {
            planet.OrbitState.Rotate(elapsedTime);
            double      siderealRotation  = planet.OrbitState.SiderealRotation.Degrees;
            Vector3d    spinAxisSSF       = planet.OrbitState.AngularMomentum.normalized.ToXZYd();
            Quaterniond planetRotationSSF = Quaterniond.AngleAxis(siderealRotation, spinAxisSSF.normalized);
            return WorldToObserverLocalRotation * planetRotationSSF;
        }

        /// <summary>
        /// Returns a vector pointing from the origin to where this celestial body should be located in the sky.
        /// </summary>
        /// <param name="celestialBody">The celestial body.</param>
        /// <returns>The direction vector.</returns>
        public Vector3d TransformCelestialBody(CelestialBody celestialBody)
        {
            Vector3d sunDir = TransformSystemToSurface(celestialBody, ObserverPositionSSF, WorldToObserverLocalRotation);
            return sunDir;
        }

        private (Vector3d east, Vector3d north, Vector3d up) BuildLocalFrame()
        {
            Vector3d observerSystemPositionSSF = ObserverBody.OrbitState.Position.ToXZYd();
            ObserverPositionSSF = observerSystemPositionSSF + ObserverPhysicalRotation * ObserverLocalPosition;
            Vector3d localUp    = (ObserverPositionSSF - observerSystemPositionSSF).normalized;
            Vector3d localNorth = Vector3d.ProjectOnPlane(SpinAxis, localUp).normalized;

            // Fallback if near pole
            if (localNorth.magnitude < 1e-3)
            {
                localNorth = Vector3d.ProjectOnPlane(Vector3d.forward, localUp).normalized;
            }

            Vector3d localEast = Vector3d.Cross(localUp, localNorth).normalized;

            return (localEast, localNorth, localUp);
        }

        private void DetermineInitialRotationAngle()
        {
            SpinAxis = ObserverBody.OrbitState.AngularMomentum.normalized.ToXZYd();
            Vector3d observerSSF           = ObserverBody.OrbitState.Position.ToXZYd();
            Vector3d closestBodySSF        = ClosestBody.OrbitState.Position.ToXZYd();
            Vector3d observedDir           = (closestBodySSF - observerSSF).normalized;
            Vector3d referenceDirectionSSF = Vector3d.ProjectOnPlane(Vector3d.right, SpinAxis).normalized;
            Vector3d equatorDir            = Vector3d.ProjectOnPlane(observedDir, SpinAxis).normalized;
            double   initialAngleDegrees   = Vector3d.SignedAngle(referenceDirectionSSF, equatorDir, SpinAxis);
            ObserverBody.OrbitState.sra = initialAngleDegrees.ToRadians();
        }

        private static Vector3d TransformSystemToSurface(
            CelestialBody celestialBody,
            Vector3d observerLocalPositionSSF,
            Quaterniond worldToObserverLocalRotation)
        {
            Vector3d celestialBodySSF = celestialBody.OrbitState?.Position.ToXZYd() ?? Vector3d.zero;
            Vector3d observedDir      = (celestialBodySSF - observerLocalPositionSSF).normalized;
            Vector3d observedDirLocal = worldToObserverLocalRotation * observedDir;

            return observedDirLocal;
        }
    }
}