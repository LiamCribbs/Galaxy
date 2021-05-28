using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public new LineRenderer renderer;

    public int hyperlanes;
    public List<Star> connectedStars = new List<Star>(2);

    public new string name;

    public float mass;
    public float radius;
}