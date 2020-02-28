using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
    public GameObject savePanel;
    public GameObject loadPanel;
    public TextMeshProUGUI generationText;
    public TextMeshProUGUI aliveNodesText;
    public TMP_InputField sceneName;
    public Button saveButton;
    public ToggleGroup sceneToggleGroup;
    public RectTransform sceneList;
    public GameObject sceneItemPrefab;

    private string savedSceneRootPath;
    private HashSet<string> savedScenes;
    private Camera mainCamera;
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
        mainCamera = Camera.main;
        Interval = 1;

        float scrollBarWidth = patternBar.GetComponent<RectTransform>().sizeDelta.x;
        float scrollContentWidth = patternBar.content.sizeDelta.x;
        float scrollContentCount = patternBar.content.childCount;

        scrollDelta = scrollContentWidth / scrollContentCount / (scrollContentWidth - scrollBarWidth);

        savedSceneRootPath = Path.Combine(Application.persistentDataPath, "Scenes");
        if (!Directory.Exists(savedSceneRootPath)) {
            Directory.CreateDirectory(savedSceneRootPath);
        }

        savedScenes = new HashSet<string>();
        RefreshSceneList();
    }

    private void RefreshSceneList() {
        foreach (RectTransform t in sceneList) {
            GameObject.Destroy(t.gameObject);
        }
        savedScenes.Clear();

        string cleanFileName;

        foreach (string file in Directory.GetFiles(savedSceneRootPath)) {
            cleanFileName = Path.GetFileNameWithoutExtension(file);
            savedScenes.Add(cleanFileName);
            GameObject sceneItem = PrefabUtility.InstantiatePrefab(sceneItemPrefab) as GameObject;
            sceneItem.transform.SetParent(sceneList, false);
            sceneItem.GetComponentInChildren<TextMeshProUGUI>().text = cleanFileName;
            sceneItem.GetComponent<Toggle>().group = sceneToggleGroup;
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !mouseOverUI) {
            Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
        UpdateStats();
    }

    private void UpdateStats() {
        generationText.text = grid.gen.ToString();
        aliveNodesText.text = grid.AliveNodesCount.ToString();
    }

    IEnumerator RunGOL() {
        while (true) {
            grid.RunGOL();
            UpdateStats();
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

    public void ShowSave() {
        if (grid.AliveNodesCount == 0) {
            return;
        }

        savePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void CheckUniqueScene() {
        if (sceneName.text.Length == 0 || savedScenes.Contains(sceneName.text)) {
            saveButton.interactable = false;
        } else {
            saveButton.interactable = true;
        }

    }

    public void CloseSave() {
        sceneName.text = "";
        savePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void SaveScene() {
        if (grid.AliveNodesCount == 0 || savedScenes.Contains(sceneName.text)) {
            CloseSave();
            return;
        }

        Pattern scene = grid.CurrentGridToPattern(sceneName.text);
        string json = JsonUtility.ToJson(scene);
        string path = Path.Combine(savedSceneRootPath, sceneName.text);

        File.WriteAllText(path, json);
        CloseSave();
        RefreshSceneList();
    }

    public void ShowLoad() {
        loadPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private string ReadFile(string path) {
        StreamReader sr = new StreamReader(path);
        string content = sr.ReadToEnd();
        sr.Close();
        return content;
    }

    public void LoadScene() {
        Toggle selected = sceneToggleGroup.ActiveToggles().FirstOrDefault();
        if (selected != null) {
            // Read data from json
            string file = Path.Combine(savedSceneRootPath, selected.GetComponentInChildren<TextMeshProUGUI>().text);
            Pattern p = JsonUtility.FromJson<Pattern>(ReadFile(file));

            // Reset Grid
            GridManager.ResetGrid();
            // Enable Node
            foreach (Point point in p.localPos) {
                GridManager.PositionToGridIndex(point.x, point.y, out var i, out var j);
                GridManager.EnableNode(i, j);
            }
        }
        CloseLoad();
    }

    public void CloseLoad() {
        loadPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void DeleteScene() {
        Toggle selected = sceneToggleGroup.ActiveToggles().FirstOrDefault();
        if (selected != null) {
            // Read data from json
            string file = Path.Combine(savedSceneRootPath, selected.GetComponentInChildren<TextMeshProUGUI>().text);
            File.Delete(file);
        }
        RefreshSceneList();
    }
}
