﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GridManager grid;
    public float dragSpeed;
    public float zoomSpeed;
    // Start is called before the first frame update
    private float screenRatio;
    private float gridRatio;
    private float zoom;
    private float newZoom;
    private float newOrthographicSize;
    private Vector3 mouseOrigin;
    void Start()
    {
        Center();
        newOrthographicSize = Camera.main.orthographicSize;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            mouseOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        if (Input.GetMouseButton(1)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseOrigin;
            transform.Translate(pos * -1 * dragSpeed * Time.deltaTime, Space.Self);
        }

        newZoom = Input.GetAxis("Mouse ScrollWheel");
        if (newZoom != 0) {
            zoom = newZoom;
            newOrthographicSize = Camera.main.orthographicSize * (1 + -1 * zoom * zoomSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(Camera.main.orthographicSize - newOrthographicSize) > 1e-3) {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, newOrthographicSize, 0.2f);
        }

    }

    private void Center() {
        screenRatio = (float)Screen.width / Screen.height;
        gridRatio = (float)grid.columns / grid.rows;

        if (screenRatio > gridRatio) {
            Camera.main.orthographicSize = grid.rows / 2 * grid.upp;
        }
        else {
            Camera.main.orthographicSize = grid.columns / screenRatio / 2 * grid.upp;
        }

        Camera.main.transform.position = new Vector3((grid.columns / 2 - 0.5f) * grid.upp, (grid.rows / 2 - 0.5f) * grid.upp, -1);
    }

}
