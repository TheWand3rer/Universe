# Universe
<img src="Documentation/Images/Sine Fine Transfer.png"/>
`Universe` is a library used in the development of [Sine Fine](https://www.vindemiatrixcollective.com) a space exploration game. It provides methods and code to perform calculations useful in this genre of games. With code ported to C# from popular libraries like [poliastro](https://github.com/poliastro/poliastro), it can calculate the position of the planets given a certain date, or an interplanetary manoeuvre to travel to another planet!

## Features
These are the features currently available in `Universe`:

* A game agnostic "galactic" object model: for each celestial body, it only contains physical and orbital data, based on the [UnitsNet](https://github.com/angularsen/UnitsNet) library to avoid (minimise) confusion and mistakes.
* Uses `double` for internal calculations and can output in `float` for Unity.
* The model is supports Galaxies / (n-ary) Star Systems / Stars / Planets (and moons). Each orbiter is a `CelestialBody`.
* A Kepler simulation model that calculates the position of any object in the solar system given their orbital data and time from the [J2000 epoch](https://en.wikipedia.org/wiki/Epoch_(astronomy)#Julian_years_and_J2000).
* An interplanetary mission planner that calculates possible transfers given a set of launch windows.
* A set of functions to calculate mission profiles for *relativistic rockets*, to obtain the acceleration and cruise time for both the observer and the ship, and the total duration of the mission given speeds that approach fractions of c.
* (optional) custom deserialisation converters in the assembly `com.vindemiatrixcollective.universe.data`.
* Unit tests based on results from [poliastro](https://github.com/poliastro/poliastro) and other sources.

## Installation

1. Open the Unity Package Manager from Window > Package Manager
2. Click on the "+" button > Install package from git URL
3. Enter the following URL:

```
https://github.com/TheWand3rer/Universe.git?path=/Universe
```

## Creating a galaxy
The easiest way is to use the `com.vindemiatrixcollective.universe.data` package and deserialise the Solar system data. To do so, move the file [Systems.json](Documentation/Data/Systems.json) into your `Resources/Data` folder. Then, you can deserialise its contents by implementing a function like the following:

```cs
public Galaxy LoadSol(ref Galaxy galaxy, string path = "Data/systems")
{
    JsonSerializerSettings settings = new();
    Galaxy additionalData = DeserializeFile<Galaxy>(path,
        new CoreObjectConverter<Galaxy>(new GalaxyConverter()),
        new CoreObjectConverter<StarSystem>(new StarSystemConverter()),
        new CoreObjectConverter<Star>(new StarConverter()),
        new CoreObjectConverter<Planet>(new PlanetConverter()));
    foreach ((string key, StarSystem system) in additionalData.Systems)
    {
        galaxy[key] = system;
    }
}
```
then call it via:
```cs
Galaxy galaxy = new Galaxy("Milky Way");
LoadSol(ref galaxy);
```

### From code
```cs
Galaxy galaxy = new Galaxy("Milky Way");
StarSystem sol = new StarSystem("Sol");

// Star.Sol and Planet.Earth are included as examples with hardcoded values at J2000.
// You should create stars and planets with the appropriate constructors or factory methods
Star sun = Star.Sun;
Planet earth = Planet.Earth

galaxy.AddSystem(sol);
sun.AddPlanet(earth);
sol.AddStar(sun);
```

## Propagating orbyts
Define a solar system, then in your `Start` or other initialisation method:
```cs
// iterate on every body in the solar system
planet.OrbitState.Propagate(DateTime.Today);
```
Then, in your `Update` method:
```cs
float delta = speed * 86400 * Time.deltaTime;
planet.OrbitState.Propagate(delta);

// When you need to use the calculated position, in the gameobject representing your Planet:
transform.position = OrbitalMechanics.MetresToAu(planet.OrbitState.Position, unitsPerAU).ToXZY()
```
Remarks:
* Set speed as a multiplier to define how many in-game seconds correspond to a day: 1 day = 86400 seconds.
* Set unitsPerAU to define how many Unity units correspond to one Astronomical Unit (AU).
* `.toXZY()` is necessary because the calculations are computed using Y as up, to adapt to the original libraries.

## Interplanetary Transfer Plannet
TODO

## Relativity
TODO

