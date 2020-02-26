using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GridManager grid;
    public float dragSpeed;
    public float zoomSpeed;
    public float cameraSizePlus;
    // Start is called before the first frame update
    private float screenRatio;
    private float gridRatio;
    private float zoom;
    private float newZoom;
    private float newOrthographicSize;
    private Vector3 mouseOrigin;
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
        Center();
        newOrthographicSize = mainCamera.orthographicSize;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            mouseOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        if (Input.GetMouseButton(1)) {
            Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition) - mouseOrigin;
            transform.Translate(pos * -1 * dragSpeed * Time.deltaTime, Space.Self);
        }

        newZoom = Input.GetAxis("Mouse ScrollWheel");
        if (newZoom != 0) {
            zoom = newZoom;
            newOrthographicSize = mainCamera.orthographicSize * (1 + -1 * zoom * zoomSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(mainCamera.orthographicSize - newOrthographicSize) > 1e-3) {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthographicSize, 0.2f);
        }

    }

    private void Center() {
        screenRatio = (float)Screen.width / Screen.height;
        gridRatio = (float)grid.columns / grid.rows;

        if (screenRatio > gridRatio) {
            mainCamera.orthographicSize = GridManager.NormalizeToNodeSize(GridManager.Rows / 2) * (1 + cameraSizePlus);
        }
        else {
            mainCamera.orthographicSize = GridManager.NormalizeToNodeSize(GridManager.Columns / screenRatio / 2) * (1 + cameraSizePlus);
        }

        mainCamera.transform.position = new Vector3(GridManager.NormalizeToNodeSize(GridManager.Columns / 2 - 0.5f), GridManager.NormalizeToNodeSize(GridManager.Rows / 2 - 0.5f), -1);
    }

}
