using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager: MonoBehaviour
{

    public int rows = 10;
    public int columns = 10;
    public GameObject NodePrefab;
    public Transform GridRoot;

    private GameObject[,] Grid;
    // private bool[,] isAlive;
    // private bool[,] nextIsAlive;
    private HashSet<string> aliveNodes;
    private HashSet<string> nextAliveNodes;
    private Dictionary<string, int> neighbourAliveCount;

    public Color aliveColor;
    public Color deadColor;
    // Start is called before the first frame update
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
                GameObject node = Instantiate(NodePrefab, new Vector3(j, i, 1), Quaternion.identity, GridRoot);
                node.layer = 8;
                node.name = $"r{i}.c{j}";
                Grid[i, j] = node;
            }
        }
    }

    private string IndexToKey(int i, int j) {
        return $"{i}.{j}";
    }

    private void KeyToIndex(string key, out int i, out int j) {
        string[] ij = key.Split('.');
        i = int.Parse(ij[0]);
        j = int.Parse(ij[1]);
    }

    public void DisableNode(int i, int j) {
        Grid[i, j].GetComponent<SpriteRenderer>().color = deadColor;
        // isAlive[i, j] = false;
        aliveNodes.Remove(IndexToKey(i, j));

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
                neighbourAliveCount[key] = count - 1;
            }
        }
    }

    public void EnableNode(int i, int j) {
        Grid[i, j].GetComponent<SpriteRenderer>().color = aliveColor;
        // isAlive[i, j] = false;
        aliveNodes.Add(IndexToKey(i, j));

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
                neighbourAliveCount[key] = count + 1;
            }
        }
    }

    public void ToggleNode(int i, int j) {
        if (aliveNodes.Contains(IndexToKey(i, j))) {
            DisableNode(i, j);
        } else {
            EnableNode(i, j);
        }
    }

    public bool PositionToGridIndex(float x, float y, out int i, out int j) {
        i = Mathf.FloorToInt(y + 0.5f);
        j = Mathf.FloorToInt(x + 0.5f);
        if (i >= 0 && i < rows && j >= 0 && j < columns) {
            return true;
        }
        i = 0;
        j = 0;
        return false;
    }

    private void PrintHashSet(HashSet<string> n) {
        string s = "";
        foreach (string key in n) {
            s += key;
            s += " ";
        }
        Debug.Log(s);
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
