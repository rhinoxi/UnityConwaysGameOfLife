using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropPattern : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public TextAsset patternFile;
    private Pattern p;
    private void Start() {
        p = JsonUtility.FromJson<Pattern>(patternFile.text);
        Normalize();
    }

    public void Normalize() {
        float centerX = 0;
        float centerY = 0;

        foreach (Point point in p.localPos) {
            point.x = GridManager.NormalizeToNodeSize(point.x);
            point.y = GridManager.NormalizeToNodeSize(point.y);
            centerX += point.x;
            centerY += point.y;
        }

        centerX /= p.localPos.Count;
        centerY /= p.localPos.Count;

        GridManager.NearestNodeX(centerX, out centerX);
        GridManager.NearestNodeY(centerY, out centerY);

        foreach (Point point in p.localPos) {
            point.x -= centerX;
            point.y -= centerY;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // Change Highlight Grid
        HighlightManager.HighlightByLocalPosition(p.localPos);
    }

    public void OnEndDrag(PointerEventData eventData) {
        foreach (GameObject node in HighlightManager.hlNodes) {
            GridManager.EnableNodeByPosition(node.transform.position.x, node.transform.position.y);
        }
        HighlightManager.ResetHl();
    }

    public void OnDrag(PointerEventData eventData) {
    }
}
