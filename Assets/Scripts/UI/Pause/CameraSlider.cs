using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSlider : MonoBehaviour
{
    private CameraMovement virtualCamera;
    private MouseControlFollowCamera followCamera;
    private Slider placeRotateCameraSlider;
    private Slider placeRotateObjectSlider;
    private Slider placeZoomCameraSlider;
    private Slider placeMoveCameraSlider;
    private Slider playRotateSlider;
    private Slider playZoomSlider;

    private void Awake() {
        placeRotateCameraSlider = transform.Find("PlaceRotateCamera").gameObject.GetComponent<Slider>();
        placeZoomCameraSlider = transform.Find("PlaceZoomCamera").gameObject.GetComponent<Slider>();
        placeMoveCameraSlider = transform.Find("PlaceMoveCamera").gameObject.GetComponent<Slider>();
        placeRotateObjectSlider = transform.Find("PlaceRotateObject").gameObject.GetComponent<Slider>();
        playRotateSlider = transform.Find("PlayRotateCamera").gameObject.GetComponent<Slider>();
        playZoomSlider = transform.Find("PlayZoomCamera").gameObject.GetComponent<Slider>();
        playRotateSlider.value = (1 - 0.5f) / (4 - 0.5f);
        playZoomSlider.value = (1 - 0.3f) / (3 - 0.3f);
    }

    public void PlaceRotateCamera() {
        if (virtualCamera != null) {
            virtualCamera.AdjustSensitiveRotateCamera(placeRotateCameraSlider.value);
        }
    }

    public void PlaceZoomCamera() {
        if (virtualCamera != null) {
            virtualCamera.AdjustSensitiveZoomCamera(placeZoomCameraSlider.value);
        }
    }

    public void PlaceMoveCamera() {
        if (virtualCamera != null) {
            virtualCamera.AdjustSensitiveMoveCamera(placeMoveCameraSlider.value);
        }
    }

    public void PlaceRotateObject() {
        if (virtualCamera != null) {
            virtualCamera.AdjustSensitiveRotateObject(placeRotateObjectSlider.value);
        }
    }

    public void PlayRotateCamera() {
        if (followCamera != null) {
            followCamera.AdjustSensitiveRotate(playRotateSlider.value);
        }
    }

    public void PlayZoomCamera() {
        if (followCamera != null) {
            followCamera.AdjustSensitiveZoom(playZoomSlider.value);
        }
    }

    public void SetCamera(GameObject player) {
        virtualCamera = player.transform.Find("Camera").GetComponent<CameraMovement>();
        followCamera = player.transform.Find("Camera").GetComponent<MouseControlFollowCamera>();
    }
}
