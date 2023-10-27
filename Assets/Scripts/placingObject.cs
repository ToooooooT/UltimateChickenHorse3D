using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    public GameObject transparentObject;
    public Color invalidColor = new Color(1.0f, 0.0f, 0.0f, 0.05f);
    public Color validColor = new Color(0.0f, 1.0f, 0.0f, 0.05f);
    private float rotationX = 0;
    private float rotationY = 0;
    bool isAddingObject = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float sensitive = 0.2f;

        if (!isAddingObject)
        {
            rotationX -= mouseY * sensitive * 5;
            rotationY += mouseX * sensitive * 5;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }

        
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += 2*sensitive * transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += 2*sensitive * -transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += sensitive * -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += sensitive * transform.right;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += new Vector3(0, sensitive, 0);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += new Vector3(0, -sensitive, 0);
        }
        if (transform.position.x < -20)
        {
            transform.position = new Vector3(-20, transform.position.y, transform.position.z);
        }
        if (transform.position.x > 20)
        {
            transform.position = new Vector3(20, transform.position.y, transform.position.z);
        }
        if (transform.position.z < -20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -20);
        }
        if (transform.position.z > 20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 20);
        }
        if (transform.position.y < 5)
        {
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
        }
        if (transform.position.y > 45)
        {
            transform.position = new Vector3(transform.position.x, 45, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.Z) && !isAddingObject)
        {
            isAddingObject = true;
            TransparentObject(PrimitiveType.Cube);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (isAddingObject && placingIsValid())
            {
                CreateObject(PrimitiveType.Cube); 
                isAddingObject = false;
            }
        }
        if (isAddingObject)
        {
            AddingObject(mouseX, mouseY, 5.0f);
        }
    }

    private void TransparentObject(PrimitiveType type)
    {
        transparentObject = GameObject.CreatePrimitive(type);
        transparentObject.name = "transparent cube";
        
    }
    
    private void CreateObject(PrimitiveType type)
    {
        var newObject = GameObject.CreatePrimitive(type);
        newObject.name = "new";
        newObject.transform.position = transparentObject.transform.position;
        newObject.transform.rotation = transparentObject.transform.rotation;
        Destroy(transparentObject);
    }
    private bool placingIsValid()
    {
        // not finished
        if (transparentObject.transform.position.x > 0)
            return false;
        return true;
    }
    private void AddingObject(float mouseX, float mouseY, float sensitive)
    {
        transparentObject.transform.position = transform.position + 25.0f* transform.forward;
        if (transparentObject.transform.position.x < -25)
        {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((-25f - transparentObject.transform.position.x) / transform.forward.x) * transform.forward;
        }
        if (transparentObject.transform.position.x > 25)
        {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((25f - transparentObject.transform.position.x) / transform.forward.x) * transform.forward;
        }
        if (transparentObject.transform.position.y < 0)
        {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((0f - transparentObject.transform.position.y) / transform.forward.y) * transform.forward;
        }
        if (transparentObject.transform.position.y > 50)
        {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((50f - transparentObject.transform.position.y) / transform.forward.y) * transform.forward;
        }
        if (transparentObject.transform.position.z < -25)
        {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((-25f - transparentObject.transform.position.z) / transform.forward.z) * transform.forward;
        }
        if (transparentObject.transform.position.z > 25)
        {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((23f - transparentObject.transform.position.z) / transform.forward.z) * transform.forward;
        }

        if (Input.GetMouseButton(0))
        {
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
        }
        else if (Input.GetMouseButton(1))
        {
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

        }
        else
        {
            rotationX -= mouseY ;
            rotationY += mouseX ;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }
        if (placingIsValid())
        {
            Renderer renderer = transparentObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = validColor;
            }
        }
        else
        {
            Renderer renderer = transparentObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = invalidColor;
            }
        }
    }

}
