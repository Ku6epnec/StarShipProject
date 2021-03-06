using System.Collections.Generic;
using UnityEngine;
using Main;
using Data;
using Mirror;

public class Root : MonoBehaviour
{
    public MainConfig planetSpawnSettings;
    public SolarSystemNetworkManager networkManagerPrefab;
    public SpaceShipSettings spaceShipSettings;

    [Space]
    public List<Transform> spaceShipSpawnPoints;

    private void Start()
    {
        var networkManager = Instantiate(networkManagerPrefab);
        
        foreach(var spawnPoint in spaceShipSpawnPoints)
        NetworkManager.RegisterStartPosition(spawnPoint);

        networkManager.Init(Instantiate(spaceShipSettings));
        networkManager.OnServerStart += SpawnPlanets;
    }

    private void SpawnPlanets()
    {
        PlanetSpawner.Spawn(Instantiate(planetSpawnSettings));
    }
}
