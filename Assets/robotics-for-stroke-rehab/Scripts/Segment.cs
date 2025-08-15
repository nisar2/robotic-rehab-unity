using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Segment
{
    public List<Point> points = new List<Point>();
    public Target StartTarget;
    public Target EndTarget;
}
