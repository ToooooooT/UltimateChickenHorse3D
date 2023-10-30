using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    private GameObject transparentObject;
    private Color invalidColor = new(1.0f, 0.0f, 0.0f, 0.05f);
    private Color validColor = new(0.0f, 1.0f, 0.0f, 0.05f);
    private float rotationX = 0;
    private float rotationY = 0;
    public bool isAddingObject = false;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float sensitive = 0.2f;
        float x1=-100, x2=100, y1=-20, y2=100, z1=-100, z2=100;
        if (!isAddingObject) {
            rotationX -= mouseY * sensitive * 5;
            rotationY += mouseX * sensitive * 5;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }

        
        if (Input.GetKey(KeyCode.W)) {
            transform.position += 2*sensitive * transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += 2*sensitive * -transform.forward;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position += sensitive * -transform.right;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += sensitive * transform.right;
        }
        if (Input.GetKey(KeyCode.Space)) {
            transform.position += new Vector3(0, sensitive, 0);
        }
        if (Input.GetKey(KeyCode.LeftControl)) {
            transform.position += new Vector3(0, -sensitive, 0);
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
        if (Input.GetKeyDown(KeyCode.Z) && !isAddingObject) {
            isAddingObject = true;
            TransparentObject(PrimitiveType.Cube);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            if (isAddingObject && PlacingIsValid()) {
                CreateObject(PrimitiveType.Cube); 
                isAddingObject = false;
            }
        }
        if (isAddingObject) {
            AddingObject(mouseX, mouseY, 5.0f);
        }
    }

    private void TransparentObject(PrimitiveType type) {
        transparentObject = GameObject.CreatePrimitive(type);
        transparentObject.name = "transparent cube";
        
    }
    
    private void CreateObject(PrimitiveType type) {
        var newObject = GameObject.CreatePrimitive(type);
        newObject.name = "new";
        newObject.transform.SetPositionAndRotation(transparentObject.transform.position, transparentObject.transform.rotation);
        Destroy(transparentObject);
    }

    private bool PlacingIsValid() {
        // not finished
        if (transparentObject.transform.position.x > 0)
            return false;
        return true;
    }

    private void AddingObject(float mouseX, float mouseY, float sensitive, float x1=-100, float x2=100, float y1=-20, float y2=100, float z1=-100, float z2=100) {
        transparentObject.transform.position = transform.position + 25.0f* transform.forward;
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

        if (Input.GetMouseButton(0)) {
            //mouseY = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive;

            float horizontalRotationAngle = mouseX * sensitive;
            horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);
        } else if (Input.GetMouseButton(1)) {
            mouseX = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive;

            float horizontalRotationAngle = mouseX * sensitive;
            horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            //transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);

        } else {
            rotationX -= mouseY ;
            rotationY += mouseX ;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }

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
}
