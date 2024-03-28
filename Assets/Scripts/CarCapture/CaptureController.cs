using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CaptureController : MonoBehaviour
{
    public Camera cam;
    public float camFOV = 3f;
    public Vector3 camPosition = Vector3.zero;
    public GameObject carHolder;
    public string fileName = "Noname";


    [ContextMenu("Take Capture")]
    void TakeCapture() {
        if (cam != null) {
            ScreenCapture.CaptureScreenshot(fileName + ".png");
        }
    }

    [ContextMenu("Next Car")]
    void NextCar() {
        camPosition += new Vector3(-28.28427f, 0, 0);
    }
    [ContextMenu("Previous Car")]
    void PreviousCar() {
        camPosition += new Vector3(28.28427f, 0, 0);
    }
    void Update() {
        cam.orthographicSize = camFOV;
        carHolder.transform.localPosition = camPosition;
    }
}