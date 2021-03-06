﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {
    public GameObject NodePrefab;
    public Color hlColor;
    public GameObject hlRoot;


    public static List<GameObject> hlNodes;
    private Camera mainCamera;
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
        mainCamera = Camera.main;
        hlNodes = new List<GameObject>();
        GenHlNode();
    }

    public static void ResetHl() {
        HighlightByLocalPosition(new List<Point> { new Point(0, 0) });
    }

    public static void HighlightByLocalPosition(List<Point> points) {
        int hlNodesLen = hlNodes.Count;
        int count = 0;
        foreach (Point p in points) {
            if (count >= hlNodesLen) {
                // TODO:
                GenHlNode(new Vector3(p.j, p.i));
            } else {
                hlNodes[count].transform.localPosition = new Vector3(p.j, p.i);
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
        GameObject node = Instantiate(instance.NodePrefab, instance.hlRoot.transform);
        node.GetComponent<SpriteRenderer>().color = instance.hlColor;
        node.transform.localPosition = pos;
        hlNodes.Add(node);
    }


    // Update is called once per frame
    void Update()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (GridManager.NearestNodeX(mouseWorldPos.x, out xpos) && GridManager.NearestNodeY(mouseWorldPos.y, out ypos)) {
            transform.position = new Vector3(xpos, ypos, transform.position.z);
            hlRoot.SetActive(true);
        } else {
            hlRoot.SetActive(false);
        }
    }
}
