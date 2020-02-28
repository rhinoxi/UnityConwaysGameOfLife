using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Pattern
{
    public string name;
    public List<Point> localPos;

    public Pattern(string _name, List<Point> _localPos = null) {
        name = _name;
        localPos = new List<Point>();
        if (_localPos != null) {
            localPos = _localPos;
        }
    }
}

[Serializable]
public class Point
{
    public float x;
    public float y;

    public Point(float i, float j) {
        x = i;
        y = j;
    }
}
