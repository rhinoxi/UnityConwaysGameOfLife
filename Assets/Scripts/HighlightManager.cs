using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public static void HighlightByLocalPosition(List<Point> positions) {
        int hlNodesLen = hlNodes.Count;
        int count = 0;
        foreach (Point p in positions) {
            if (count >= hlNodesLen) {
                GenHlNode(new Vector3(p.x, p.y));
            } else {
                hlNodes[count].transform.localPosition = new Vector3(p.x, p.y);
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
        GameObject node = PrefabUtility.InstantiatePrefab(instance.NodePrefab, instance.hlRoot.transform) as GameObject;
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
