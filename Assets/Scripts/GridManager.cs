using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager: MonoBehaviour
{

    public int rows = 10;
    public int columns = 10;
    public GameObject NodePrefab;
    public Transform GridRoot;

    public Color aliveColor;
    public Color deadColor;
    public float NodeSize; // units

    private GameObject[,] Grid;
    // private bool[,] isAlive;
    // private bool[,] nextIsAlive;
    private HashSet<string> aliveNodes;
    private HashSet<string> nextAliveNodes;
    private Dictionary<string, int> neighbourAliveCount;

    private float nodeZaxis = 2;

    private static GridManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    void Start()
    {
        GenGrid();
    }

    private void GenGrid() {
        Grid = new GameObject[rows, columns];
        // isAlive = new bool[rows, columns];
        // nextIsAlive = new bool[rows, columns];
        aliveNodes = new HashSet<string> { };
        nextAliveNodes = new HashSet<string> { };
        neighbourAliveCount = new Dictionary<string, int> { };
        for (int i=0; i<rows; ++i) {
            for (int j=0; j<columns; ++j) {
                GameObject node = Instantiate(NodePrefab, new Vector3(NodeSize * j, NodeSize * i, nodeZaxis), Quaternion.identity, GridRoot);
                node.GetComponent<SpriteRenderer>().color = deadColor;
                node.layer = 8;
                node.name = $"r{i}.c{j}";
                Grid[i, j] = node;
            }
        }
    }

    public static void ResetGrid() {
        List<string> aliveNodesList = new List<string>(instance.aliveNodes);
        foreach (string key in aliveNodesList) {
            instance.KeyToIndex(key, out var i, out var j);
            DisableNode(i, j);
        }
        instance.nextAliveNodes.Clear();
    }

    private string IndexToKey(int i, int j) {
        return $"{i}.{j}";
    }

    private void KeyToIndex(string key, out int i, out int j) {
        string[] ij = key.Split('.');
        i = int.Parse(ij[0]);
        j = int.Parse(ij[1]);
    }

    public static void EnableNode(int i, int j) {
        string key = instance.IndexToKey(i, j);
        if (!instance.aliveNodes.Contains(key)) {
            instance.Grid[i, j].GetComponent<SpriteRenderer>().color = instance.aliveColor;
            instance.aliveNodes.Add(key);

            instance.ChangeNeighbourCount(i, j, 1);
        }
    }

    public static void DisableNode(int i, int j) {
        string key = instance.IndexToKey(i, j);
        if (instance.aliveNodes.Contains(key)) {
            instance.Grid[i, j].GetComponent<SpriteRenderer>().color = instance.deadColor;
            instance.aliveNodes.Remove(key);

            instance.ChangeNeighbourCount(i, j, -1);
        }
    }

    private void ChangeNeighbourCount(int i, int j, int delta) {
        int newi, newj;
        for (int di = -1; di <= 1; ++di) {
            for (int dj = -1; dj <= 1; ++dj) {
                if (di == 0 && dj == 0) continue;

                newi = i + di;
                newj = j + dj;

                if (newi < 0) {
                    newi = rows - 1;
                } else if (newi == rows) {
                    newi = 0;
                }

                if (newj < 0) {
                    newj = columns - 1;
                } else if (newj == columns) {
                    newj = 0;
                }

                string key = IndexToKey(newi, newj);
                neighbourAliveCount.TryGetValue(key, out var count);
                neighbourAliveCount[key] = count + delta;
            }
        }

    }

    public static void ToggleNode(int i, int j) {
        if (instance.aliveNodes.Contains(instance.IndexToKey(i, j))) {
            DisableNode(i, j);
        } else {
            EnableNode(i, j);
        }
    }

    public static bool NormalizeXPosition(float x, out float xnor) {
        xnor = Mathf.FloorToInt(x / instance.NodeSize + 0.5f) * instance.NodeSize;
        return (xnor >= -instance.NodeSize / 2f && xnor < instance.NodeSize * (instance.columns - 0.5f));
    }

    public static bool NormalizeYPosition(float y, out float ynor) {
        ynor = Mathf.FloorToInt(y / instance.NodeSize + 0.5f) * instance.NodeSize;
        return (ynor >= -instance.NodeSize / 2f && ynor < instance.NodeSize * (instance.rows - 0.5f));
    }

    public static void EnableNodeByPosition(float x, float y) {
        int i;
        int j;
        if (PositionToGridIndex(x, y, out i, out j)) {
            EnableNode(i, j);
        }
    }

    public static bool PositionToGridIndex(float x, float y, out int i, out int j) {
        i = Mathf.FloorToInt(y / instance.NodeSize + 0.5f);
        j = Mathf.FloorToInt(x / instance.NodeSize + 0.5f);
        if (i >= 0 && i < instance.rows && j >= 0 && j < instance.columns) {
            return true;
        }
        i = 0;
        j = 0;
        return false;
    }

    public void RunGOL() {
        foreach (KeyValuePair<string, int> entry in neighbourAliveCount) {
            if (entry.Value == 3) {
                nextAliveNodes.Add(entry.Key);
            } else if (entry.Value == 2) {
                if (aliveNodes.Contains(entry.Key)) {
                    nextAliveNodes.Add(entry.Key);
                }
            }
        }

        List<string> toBeRemoved = new List<string>();
        foreach (string key in aliveNodes) {
            if (nextAliveNodes.Contains(key)) {
                nextAliveNodes.Remove(key);
            } else {
                toBeRemoved.Add(key);
            }
        }

        foreach (string key in toBeRemoved) {
            KeyToIndex(key, out var i, out var j);
            DisableNode(i, j);
        }

        foreach (string key in nextAliveNodes) {
            KeyToIndex(key, out var i, out var j);
            EnableNode(i, j);
        }

        nextAliveNodes.Clear();
    }
}
