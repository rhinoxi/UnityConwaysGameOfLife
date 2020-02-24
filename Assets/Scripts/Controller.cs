using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GridManager grid;
    public GameObject StartIcon;
    public GameObject PauseIcon;

    public static float Interval {
        get {
            return _interval;
        }
        set {
            _interval = Mathf.Clamp(value, 0.1f, 2);
        }
    }

    private static float _interval;
    private bool isRunning;
    public static bool mouseOverUI;

    private Coroutine co;
    private void Start() {
        Interval = 1;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !mouseOverUI) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int i;
            int j;
            if (GridManager.PositionToGridIndex(pos.x, pos.y, out i, out j)) {
                GridManager.ToggleNode(i, j);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Toggle();
        }
        
    }

    public void Pause() {
        StopCoroutine(co);
        isRunning = false;
        StartIcon.SetActive(true);
        PauseIcon.SetActive(false);
    }

    public void Resume() {
        co = StartCoroutine(RunGOL());
        isRunning = true;
        StartIcon.SetActive(false);
        PauseIcon.SetActive(true);
    }

    public void Toggle() {
        if (isRunning) {
            Pause();
        } else {
            Resume();
        }
    }

    public void ResetGrid() {
        GridManager.ResetGrid();
    }

    IEnumerator RunGOL() {
        while (true) {
            grid.RunGOL();
            yield return new WaitForSeconds(Interval);
        }
    }
    public void SetSpeed(float speed) {
        Interval = 1f / speed;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseOverUI = false;
    }
}
