using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {
    public GameObject NodePrefab;
    public Color hlColor;
    public GridManager grid;
    public GameObject hlRoot;

    public List<GameObject> hlNodes;
    private Vector3 mouseWorldPos;
    private SpriteRenderer sprite;
    private float xpos;
    private float ypos;

    void Start() {
        GenHlNode();
    }

    public void ResetHl() {
        Debug.Log("???");
        HighlightByLocalPosition(new List<List<int>> { new List<int> { 0, 0 } });
    }

    public void HighlightByLocalPosition(List<List<int>> positions) {
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

    private void GenHlNode() {
        GenHlNode(new Vector3(0, 0, 0));
    }

    private void GenHlNode(Vector3 pos) {
        GameObject node = Instantiate(NodePrefab, hlRoot.transform, false);
        node.GetComponent<SpriteRenderer>().color = hlColor;
        node.transform.localPosition = pos;
        hlNodes.Add(node);
    }


    // Update is called once per frame
    void Update()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (grid.NormalizeXPosition(mouseWorldPos.x, out xpos) && grid.NormalizeYPosition(mouseWorldPos.y, out ypos)) {
            transform.position = new Vector3(xpos, ypos, transform.position.z);
            hlRoot.SetActive(true);
        } else {
            hlRoot.SetActive(false);
        }
    }
}
