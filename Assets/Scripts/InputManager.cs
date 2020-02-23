using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager: MonoBehaviour
{
    public LayerMask layer;
    public GridManager grid;

    private bool isRunning;
    private Coroutine co;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int i;
            int j;
            if (grid.PositionToGridIndex(pos.x, pos.y, out i, out j)) {
                grid.ToggleNode(i, j);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isRunning) {
                StopCoroutine(co);
                isRunning = false;
            } else {
                co = StartCoroutine(RunGOL());
                isRunning = true;
            }
        }
        
    }

    IEnumerator RunGOL() {
        while (true) {
            grid.RunGOL();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
