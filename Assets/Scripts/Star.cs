using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public new Renderer renderer;
    public LineRenderer hyperlaneRenderer;

    public int hyperlanes;
    public List<Star> connectedStars = new List<Star>(2);

    public new string name;

    public float mass;
    public float radius;

    public int constellationIndex = -1;

    public SpectralClass spectralClass;
}

[System.Serializable]
public class SpectralClass
{
    public char type;
    [ColorUsage(true, true)] public Color color;
    public Vector2 massRange;
    public Vector2 radiusRange;
    public float size;
    public int spawnWeight;
}