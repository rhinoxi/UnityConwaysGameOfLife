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
        int centerI = 0;
        int centerJ = 0;

        foreach (Point point in p.localPos) {
            centerI += point.i;
            centerJ += point.j;
        }

        centerI /= p.localPos.Count;
        centerJ /= p.localPos.Count;

        foreach (Point point in p.localPos) {
            point.i -= centerI;
            point.j -= centerJ;
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

        int temp = 0;
        foreach (Point point in p.localPos) {
            temp = point.j;
            point.j = point.i;
            point.i = -temp;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            RotatePattern();
        }
    }
}
