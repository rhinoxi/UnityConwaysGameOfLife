﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {
    public GameObject NodePrefab;
    public Color hlColor;
    public GameObject hlRoot;

    public static List<GameObject> hlNodes;
    private Vector3 mouseWorldPos;
    private SpriteRenderer sprite;
    private float xpos;
    private float ypos;
    private static HighlightManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    void Start() {
        hlNodes = new List<GameObject>();
        GenHlNode();
    }

    public static void ResetHl() {
        HighlightByLocalPosition(new List<List<int>> { new List<int> { 0, 0 } });
    }

    public static void HighlightByLocalPosition(List<List<int>> positions) {
        int hlNodesLen = hlNodes.Count;
        int count = 0;
        foreach (List<int> pos in positions) {
            if (count >= hlNodesLen) {
                GenHlNode(new Vector3(pos[0], pos[1]));
            } else {
                hlNodes[count].transform.localPosition = new Vector3(pos[0], pos[1]);
            }
            ++count;
        }

        for (int i=count; i<hlNodesLen; ++i) {
            Destroy(hlNodes[hlNodes.Count - 1]);
            hlNodes.RemoveAt(hlNodes.Count - 1);
        }
    }

    private static void GenHlNode() {
        GenHlNode(new Vector3(0, 0, 0));
    }

    private static void GenHlNode(Vector3 pos) {
        GameObject node = Instantiate(instance.NodePrefab, instance.hlRoot.transform, false);
        node.GetComponent<SpriteRenderer>().color = instance.hlColor;
        node.transform.localPosition = pos;
        Debug.Log(node);
        Debug.Log(hlNodes);
        hlNodes.Add(node);
    }


    // Update is called once per frame
    void Update()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (GridManager.NormalizeXPosition(mouseWorldPos.x, out xpos) && GridManager.NormalizeYPosition(mouseWorldPos.y, out ypos)) {
            transform.position = new Vector3(xpos, ypos, transform.position.z);
            hlRoot.SetActive(true);
        } else {
            hlRoot.SetActive(false);
        }
    }
}
