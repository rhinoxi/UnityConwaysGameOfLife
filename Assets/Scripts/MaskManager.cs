using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public GameObject NodePrefab;
    public Color maskColor;
    public GridManager grid;

    private Vector3 mouseWorldPos;
    void Start()
    {
        GameObject node = Instantiate(NodePrefab, transform, false);
        node.GetComponent<SpriteRenderer>().color = maskColor;
    }

    // Update is called once per frame
    void Update()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(grid.NormalizePosition(mouseWorldPos.x), grid.NormalizePosition(mouseWorldPos.y), transform.position.z);
    }
}
