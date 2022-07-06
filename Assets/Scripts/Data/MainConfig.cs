using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "newMainConfig", menuName = "MainConfig")]
public class MainConfig : ScriptableObject
{
    public PlanetData[] Planets;
}

[Serializable]
public class PlanetData
{
    public PlanetOrbit planetOrbit;
    public int radius;
}
