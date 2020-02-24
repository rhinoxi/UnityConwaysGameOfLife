using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public GameObject NodePrefab;
    public Color maskColor;
    public GridManager grid;

    private Vector3 mouseWorldPos;
    private SpriteRenderer sprite;
    private float xpos;
    private float ypos;
    void Start()
    {
        GameObject node = Instantiate(NodePrefab, transform, false);
        sprite = node.GetComponent<SpriteRenderer>();
        sprite.color = maskColor;
    }

    // Update is called once per frame
    void Update()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (grid.NormalizeXPosition(mouseWorldPos.x, out xpos) && grid.NormalizeYPosition(mouseWorldPos.y, out ypos)) {
            transform.position = new Vector3(xpos, ypos, transform.position.z);
            sprite.enabled = true;
        } else {
            sprite.enabled = false;
        }
    }
}
