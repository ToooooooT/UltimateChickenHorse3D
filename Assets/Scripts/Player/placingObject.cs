using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float sensitive_rotate;
    [SerializeField] private float sensitive_move;
    [SerializeField] private float sensitive_zoom;

    private GameObject transparentObject;
    private StageController stageController;
    private GameObject playerObject;
    private Color invalidColor = new(1.0f, 0.0f, 0.0f, 0.05f);
    private Color validColor = new(0.0f, 1.0f, 0.0f, 0.05f);
    private Dictionary<string, GameObject> name2object;
    private PlayerInputActions playerInputActions;
    private CinemachineVirtualCamera virtualCamera;
    private Camera camera_;
    // private Transform playerTransform;
    private float rotationX = 0;
    private float rotationY = 0;
    private float diviateX = 0;
    private float diviateZ = 0;
    private float distance;
    private bool isAddingObject = false;
    private const string FOLDERPATH = "Item";

    void Start() {
        stageController = GameObject.FindGameObjectWithTag("GameController").GetComponent<StageController>();
        playerInputActions = transform.parent.gameObject.GetComponent<Player>().GetPlayerInputActions();
        playerObject = transform.parent.gameObject;
        sensitive_rotate = 1.0f;
        sensitive_move = 0.5f;
        sensitive_zoom = 0.5f;
        distance = 25.0f;
        // load prefab for creating object
        name2object = new Dictionary<string, GameObject>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        camera_ = GetComponent<Camera>();
        LoadAllPrefabsInFolder();
        Disable();
    }

    void Update() {
        Vector2 inputVector = playerInputActions.PlaceObject.MoveCamera.ReadValue<Vector2>().normalized;
        RotateCamera();
        MoveCamera();
        ZoomCamera();
        string name = playerObject.GetComponent<Player>().GetItemName();
        if (name != null && !isAddingObject) {
            isAddingObject = true;
            TransparentObject();
        }
        if (name != null && isAddingObject) {
            AddingObject(inputVector.x, inputVector.y, 5.0f);
        }
    }

    private void RotateCamera() {
        Vector2 inputVector = playerInputActions.PlaceObject.RotateCamera.ReadValue<Vector2>().normalized;
        rotationX -= inputVector.y * sensitive_rotate;
        rotationY += inputVector.x * sensitive_rotate;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
    }

    private void MoveCamera() {
        float x1=-100, x2=100, y1=-20, y2=100, z1=-100, z2=100;
        Vector2 inputVector = playerInputActions.PlaceObject.MoveCamera.ReadValue<Vector2>().normalized;
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
        float inputValue = playerInputActions.PlaceObject.MoveCameraUpDown.ReadValue<float>();
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
        float scrollWheelInput = playerInputActions.PlaceObject.ZoomCamera.ReadValue<float>();
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
        playerInputActions.PlaceObject.Enable();
        playerInputActions.PlaceObject.Place.started += PlaceObject;
        virtualCamera.enabled = true;
        camera_.enabled = true;
    }

    public void Disable() {
        playerInputActions.PlaceObject.Disable();
        virtualCamera.enabled = false;
        camera_.enabled = false;
    }

    private void PlaceObject(InputAction.CallbackContext context) {
        if (isAddingObject && PlacingIsValid()) {
            CreateObject(); 
            isAddingObject = false;
        }
    }

    private void TransparentObject() {
        string name = playerObjects[0].GetComponent<Player>().GetItemName();
        transparentObject = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/" + name));
    }
    
    private void CreateObject() {
        string name = playerObject.GetComponent<Player>().GetItemName();
        GameObject obj = Instantiate(name2object[name], transparentObject.transform.position, transparentObject.transform.rotation);
        obj.name = name;
        stageController.items.Add(obj);
        Destroy(transparentObject);
        playerObject.GetComponent<Player>().RemoveItem();
    }

    private bool PlacingIsValid() {
        // not finished
        if (transparentObject.transform.position.x > 0)
            return false;
        return true;
    }

    private void AddingObject(float mouseX, float mouseY, float sensitive, float x1=-100, float x2=100, float y1=-20, float y2=100, float z1=-100, float z2=100) {
        MoveObject(x1, x2, y1, y2, z1, z2);
        RotateObject(mouseX, mouseY, sensitive);

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
        transparentObject.transform.position = transform.position + distance * transform.forward;
        if (transparentObject.transform.position.x < x1) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((x1 - transparentObject.transform.position.x) / transform.forward.x) * transform.forward;
        }
        if (transparentObject.transform.position.x > x2) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((x2 - transparentObject.transform.position.x) / transform.forward.x) * transform.forward;
        }
        if (transparentObject.transform.position.y < y1) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((y1 - transparentObject.transform.position.y) / transform.forward.y) * transform.forward;
        }
        if (transparentObject.transform.position.y > y2) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((y2 - transparentObject.transform.position.y) / transform.forward.y) * transform.forward;
        }
        if (transparentObject.transform.position.z < z1) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((z1 - transparentObject.transform.position.z) / transform.forward.z) * transform.forward;
        }
        if (transparentObject.transform.position.z > z2) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((z2 - transparentObject.transform.position.z) / transform.forward.z) * transform.forward;
        }
    }

    private void RotateObject(float mouseX, float mouseY, float sensitive) {
        if (Input.GetMouseButton(0)) {
            //mouseY = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive;

            float horizontalRotationAngle = mouseX * sensitive;
            //horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);
            Vector3 newRotation = transparentObject.transform.rotation.eulerAngles;
            diviateX = newRotation.x;
            diviateZ = newRotation.z;
            newRotation.x = newRotation.x - diviateX;
            newRotation.z = newRotation.z - diviateZ;
            transparentObject.transform.rotation = Quaternion.Euler(newRotation);
        } else if (Input.GetMouseButton(1)) {
            mouseX = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive;

            float horizontalRotationAngle = mouseX * sensitive;
            //horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            //transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);

            Vector3 newRotation = transparentObject.transform.rotation.eulerAngles;
            newRotation.x = newRotation.x - diviateX;
            newRotation.z = newRotation.z - diviateZ;
            transparentObject.transform.rotation = Quaternion.Euler(newRotation);

        } else {
            rotationX -= mouseY ;
            rotationY += mouseX ;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }

        ItemVisible(transparentObject, PlacingIsValid());
    }

    private void ItemVisible(GameObject item, bool visible)
    {

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
