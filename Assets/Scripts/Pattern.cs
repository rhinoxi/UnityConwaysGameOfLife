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
    public int i;
    public int j;

    public Point(int _i, int _j) {
        i = _i;
        j = _j;
    }
}
