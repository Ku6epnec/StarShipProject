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

    public Color groundColor;

    [Range(0.0f, 5.0f)] public float radiusRing;
}
