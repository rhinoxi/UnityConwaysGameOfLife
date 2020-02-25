using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropPattern : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public TextAsset patternFile;
    private Pattern p;
    private RectTransform patternImage;
    private void Start() {
        p = JsonUtility.FromJson<Pattern>(patternFile.text);
        Normalize();
        patternImage = GetComponent<RectTransform>();
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

    private void RotatePattern() {
        patternImage.Rotate(new Vector3(0, 0, -90));

        float temp = 0;
        foreach (Point point in p.localPos) {
            temp = point.x;
            point.x = point.y;
            point.y = -temp;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            RotatePattern();
        }
    }
}
