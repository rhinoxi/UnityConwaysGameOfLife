using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropPattern : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public HighlightManager hl;
    private List<List<int>> data;
    private void Awake() {
        data = new List<List<int>> { };
        data.Add(new List<int>{0, 0});
        data.Add(new List<int>{1, 0});
        data.Add(new List<int>{2, 0});
        data.Add(new List<int>{2, 1});
        data.Add(new List<int>{1, 2});
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // Change Highlight Grid
        hl.HighlightByLocalPosition(data);
    }

    public void OnEndDrag(PointerEventData eventData) {
        foreach (GameObject node in hl.hlNodes) {
            GridManager.EnableNodeByPosition(node.transform.position.x, node.transform.position.y);
        }
        hl.ResetHl();
    }

    public void OnDrag(PointerEventData eventData) {
    }
}
