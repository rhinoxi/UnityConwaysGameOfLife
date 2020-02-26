using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GridManager grid;
    public GameObject StartIcon;
    public GameObject PauseIcon;
    public ScrollRect patternBar;
    public GameObject rulePanel;
    public TextMeshProUGUI generationText;
    public TextMeshProUGUI aliveNodesText;

    private float scrollDelta;
    private float scrollDeltaTotal = 0;
    private Coroutine scrollCoroutine;
    private float scrollSpeed = 0.0f;
    private float scrollTime = 0.3f;

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

        float scrollBarWidth = patternBar.GetComponent<RectTransform>().sizeDelta.x;
        float scrollContentWidth = patternBar.content.sizeDelta.x;
        float scrollContentCount = patternBar.content.childCount;

        scrollDelta = scrollContentWidth / scrollContentCount / (scrollContentWidth - scrollBarWidth);
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
        generationText.text = grid.gen.ToString();
        aliveNodesText.text = grid.AliveNodesCount.ToString();
    }

    IEnumerator RunGOL() {
        while (true) {
            grid.RunGOL();
            generationText.text = grid.gen.ToString();
            aliveNodesText.text = grid.AliveNodesCount.ToString();
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

    public void ScrollLeft() {
        ScrollPatternBar(true);
    }

    public void ScrollRight() {
        ScrollPatternBar(false);
    }

    public void ScrollPatternBar(bool scrollLeft) {
        if (scrollLeft) {
            scrollDeltaTotal = -scrollDelta;
        } else {
            scrollDeltaTotal = scrollDelta;
        }

        if (scrollCoroutine != null) {
            StopCoroutine(scrollCoroutine);
        }
        scrollCoroutine = StartCoroutine(ScrollSmoothly());

    }

    IEnumerator ScrollSmoothly() {
        float final = Mathf.Clamp01(patternBar.horizontalNormalizedPosition + scrollDeltaTotal);
        while (Mathf.Abs(patternBar.horizontalNormalizedPosition - final) > 1e-3) {
            patternBar.horizontalNormalizedPosition = Mathf.SmoothDamp(patternBar.horizontalNormalizedPosition, final, ref scrollSpeed, scrollTime);
            yield return null;
        }
        scrollDeltaTotal = 0;
        yield break;
    }

    public void ShowRule() {
        rulePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseRule() {
        rulePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
