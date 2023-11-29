using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float sensitive_rotate_camera;
    [SerializeField] private float sensitive_rotate_object;
    [SerializeField] private float sensitive_move;
    [SerializeField] private float sensitive_zoom;

    private GameObject transparentObject;
    // private StageController stageController;
    private GameObject playerObject;
    private Color invalidColor = new(1.0f, 0.0f, 0.0f, 0.05f);
    private Color validColor = new(0.0f, 1.0f, 0.0f, 0.05f);
    private Dictionary<string, GameObject> name2object;
    private InputActionMap placeObjectInputActionMap;
    private CinemachineVirtualCamera virtualCamera;
    private Camera camera_;
    private StageController stageController;
    private float rotationX = 0;
    private float rotationY = 0;
    private float diviateX = 0;
    private float diviateZ = 0;
    private float distance;
    private bool pressRotateHorizontal;
    private bool pressRotateVertical;

    private const string FOLDERPATH = "Item";

    void Start() {
        stageController = GameObject.FindGameObjectWithTag("GameController").GetComponent<StageController>();
        placeObjectInputActionMap = transform.parent.gameObject.GetComponent<Player>().GetPlaceObjectInputActionMap();
        playerObject = transform.parent.gameObject;
        transparentObject = null;
        sensitive_rotate_camera = 1.0f;
        sensitive_rotate_object = 1.0f;
        sensitive_move = 0.5f;
        sensitive_zoom = 0.3f;
        distance = 25.0f;
        pressRotateHorizontal = false;
        pressRotateVertical = false;
        // load prefab for creating object
        name2object = new Dictionary<string, GameObject>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        camera_ = GetComponent<Camera>();
        LoadAllPrefabsInFolder();
        Disable();
    }

    void Update() {
        if (camera_.enabled && virtualCamera.enabled) {
            MoveCamera();
            ZoomCamera();
            if (transparentObject == null) {
                TransparentObject();
            }
            AddingObject();
        }
    }

    private void RotateCamera() {
        Vector2 inputVector = placeObjectInputActionMap.FindAction("RotateCamera").ReadValue<Vector2>().normalized;
        rotationX -= inputVector.y * sensitive_rotate_camera;
        rotationY += inputVector.x * sensitive_rotate_camera;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
    }

    private void MoveCamera() {
        float x1=-100, x2=100, y1=-20, y2=100, z1=-100, z2=100;
        Vector2 inputVector = placeObjectInputActionMap.FindAction("MoveCamera").ReadValue<Vector2>().normalized;
        if (inputVector.y > 0) {
            transform.position += sensitive_move * transform.forward;
        } else if (inputVector.y < 0) {
            transform.position -= sensitive_move * transform.forward;
        }
        if (inputVector.x > 0) {
            transform.position += sensitive_move * transform.right;
        } else if (inputVector.x < 0) {
            transform.position -= sensitive_move * transform.right;
        }
        float inputValue = placeObjectInputActionMap.FindAction("MoveCameraUpDown").ReadValue<float>();
        if (inputValue > 0) {
            transform.position += new Vector3(0, sensitive_move, 0);
        } else if (inputValue < 0) {
            transform.position -= new Vector3(0, sensitive_move, 0);
        }
        if (transform.position.x < x1) {
            transform.position = new Vector3(x1, transform.position.y, transform.position.z);
        }
        if (transform.position.x > x2) {
            transform.position = new Vector3(x2, transform.position.y, transform.position.z);
        }
        if (transform.position.z < z1) {
            transform.position = new Vector3(transform.position.x, transform.position.y, z1);
        }
        if (transform.position.z > z2) {
            transform.position = new Vector3(transform.position.x, transform.position.y, z2);
        }
        if (transform.position.y < y1) {
            transform.position = new Vector3(transform.position.x, y1, transform.position.z);
        }
        if (transform.position.y > y2) {
            transform.position = new Vector3(transform.position.x, y2, transform.position.z);
        }
    }

    private void ZoomCamera() {
        float scrollWheelInput = placeObjectInputActionMap.FindAction("ZoomCamera").ReadValue<float>();
        if (scrollWheelInput != 0) {
            if (scrollWheelInput > 0) {
                distance -= sensitive_zoom;
                distance = Mathf.Max(distance, 2.0f);
                if (distance > 2.0) {
                    transform.position = transform.position + transform.forward * sensitive_zoom;
                }
            } else if (scrollWheelInput < 0) {
                distance += sensitive_zoom;
                transform.position = transform.position - transform.forward * sensitive_zoom;
            }
        }
    }

    public void Enable() {
        placeObjectInputActionMap.Enable();
        placeObjectInputActionMap.FindAction("Place").started += PlaceObject;
        InputAction rotateObjectHorizontalAction = placeObjectInputActionMap.FindAction("rotateObjectHorizontal");
        rotateObjectHorizontalAction.performed += ctx => pressRotateHorizontal = true;
        rotateObjectHorizontalAction.canceled += ctx => pressRotateHorizontal = false;
        InputAction rotateObjectVerticalAction = placeObjectInputActionMap.FindAction("rotateObjectVertical");
        rotateObjectVerticalAction.performed += ctx => pressRotateVertical = true;
        rotateObjectVerticalAction.canceled += ctx => pressRotateVertical = false;
        virtualCamera.enabled = true;
        camera_.enabled = true;
    }

    public void Disable() {
        placeObjectInputActionMap.Disable();
        virtualCamera.enabled = false;
        camera_.enabled = false;
    }

    private void PlaceObject(InputAction.CallbackContext context) {
        if (PlacingIsValid()) {
            CreateObject(); 
            Disable();
        }
    }

    private void TransparentObject() {
        string name = playerObject.GetComponent<Player>().GetItemName();
        transparentObject = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/" + name));
    }
    
    private void CreateObject() {
        string name = playerObject.GetComponent<Player>().GetItemName();
        GameObject obj = Instantiate(name2object[name], transparentObject.transform.position, transparentObject.transform.rotation);
        obj.name = name;
        stageController.items.Add(obj);
        Destroy(transparentObject);
        transparentObject = null;
        playerObject.GetComponent<Player>().RemoveItem();
    }

    private bool PlacingIsValid() {
        // not finished
        if (transparentObject.transform.position.x > 0)
            return false;
        return true;
    }

    private void AddingObject(float x1=-100, float x2=100, float y1=-20, float y2=100, float z1=-100, float z2=100) {
        MoveObject(x1, x2, y1, y2, z1, z2);
        RotateObjectOrCamera();
        if (PlacingIsValid()) {
            if (transparentObject.TryGetComponent<Renderer>(out var renderer)) {
                renderer.material.color = validColor;
            }
        } else {
            if (transparentObject.TryGetComponent<Renderer>(out var renderer)) {
                renderer.material.color = invalidColor;
            }
        }
    }

    private void MoveObject(float x1=-100, float x2=100, float y1=-20, float y2=100, float z1=-100, float z2=100) {
        Transform transparentObjectTransform = transparentObject.transform;
        Vector3 forward = transform.forward;
        transparentObjectTransform.position = transform.position + distance * transform.forward;
        if (transparentObjectTransform.position.x < x1) {
            transparentObjectTransform.position -= Math.Abs((x1 - transparentObjectTransform.position.x) / forward.x) * forward;
        }
        if (transparentObjectTransform.position.x > x2) {
            transparentObjectTransform.position -= Math.Abs((x2 - transparentObjectTransform.position.x) / forward.x) * forward;
        }
        if (transparentObjectTransform.position.y < y1) {
            transparentObjectTransform.position -= Math.Abs((y1 - transparentObjectTransform.position.y) / forward.y) * forward;
        }
        if (transparentObjectTransform.position.y > y2) {
            transparentObjectTransform.position -= Math.Abs((y2 - transparentObjectTransform.position.y) / forward.y) * forward;
        }
        if (transparentObjectTransform.position.z < z1) {
            transparentObjectTransform.position -= Math.Abs((z1 - transparentObjectTransform.position.z) / forward.z) * forward;
        }
        if (transparentObjectTransform.position.z > z2) {
            transparentObjectTransform.position -= Math.Abs((z2 - transparentObjectTransform.position.z) / forward.z) * forward;
        }
    }

    private void RotateObjectOrCamera() {
        Vector2 inputVector = placeObjectInputActionMap.FindAction("RotateCamera").ReadValue<Vector2>().normalized;
        float mouseX = inputVector.x;
        float mouseY = inputVector.y;
        if (pressRotateHorizontal) {
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive_rotate_object;
            float horizontalRotationAngle = mouseX * sensitive_rotate_object;

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);
            Vector3 newRotation = transparentObject.transform.rotation.eulerAngles;
            diviateX = newRotation.x;
            diviateZ = newRotation.z;
            newRotation.x -= diviateX;
            newRotation.z -= diviateZ;
            transparentObject.transform.rotation = Quaternion.Euler(newRotation);
        } else if (pressRotateVertical) {
            mouseX = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive_rotate_object;
            float horizontalRotationAngle = mouseX * sensitive_rotate_object;

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);

            Vector3 newRotation = transparentObject.transform.rotation.eulerAngles;
            newRotation.x -= diviateX;
            newRotation.z -= diviateZ;
            transparentObject.transform.rotation = Quaternion.Euler(newRotation);

        } else {
            RotateCamera();
        }

        ItemVisible(transparentObject, PlacingIsValid());
    }

    private void ItemVisible(GameObject item, bool visible) {

        Transform parentTransform = item.transform;
        if (item.TryGetComponent<Renderer>(out var renderer))
            renderer.enabled = visible;
        for (int i = 0; i < parentTransform.childCount; i++) {
            Transform childTransform = parentTransform.GetChild(i);
            GameObject childObject = childTransform.gameObject;
            ItemVisible(childObject, visible);
        }
    }

    private void LoadAllPrefabsInFolder() {
        UnityEngine.Object[] loadedObjects = Resources.LoadAll(FOLDERPATH);
        foreach (UnityEngine.Object obj in loadedObjects) {
            if (obj is GameObject) {
                name2object[obj.name] = obj as GameObject;
            }
        }
    }
}
