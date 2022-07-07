using Mirror;

public class PlanetSpawner
{
    public static void Spawn(MainConfig planetSpawnSettings)
    {
        var planets = planetSpawnSettings.Planets;
        foreach(var planetSettings in planets)
        {
            var planetOrbit = PlanetOrbit.Instantiate(planetSettings.planetOrbit);
            planetOrbit.name = planetSettings.planetOrbit.name;
            NetworkServer.Spawn(planetOrbit.gameObject);
            planetOrbit.Init(planetSettings);
        }
    }
}
