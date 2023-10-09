using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    public GameObject transparentObject;
    private float rotationX = 0;
    private float rotationY = 0;
    //private float objectRotationX = 0;
    //private float objectRotationY = 0;
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
            rotationY = Mathf.Clamp(rotationY, -90, 90);

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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isAddingObject = true;
            TransparentObject(PrimitiveType.Cube);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (isAddingObject)
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
    private void AddingObject(float mouseX, float mouseY, float sensitive)
    {
        transparentObject.transform.position = transform.position + 25.0f * transform.forward;


        if (Input.GetMouseButton(0))
        {
            //mouseY = 0;
            // ?取?像机的前向向量
            Vector3 cameraForward = -transform.right;

            // ?算物体?垂直?旋?的角度
            float verticalRotationAngle = -mouseY * sensitive;

            // ?算物体?水平?旋?的角度，?其限制在-90度到90度之?
            float horizontalRotationAngle = mouseX * sensitive;
            horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            // ?算旋?后的前向方向
            Vector3 transparentObjectForward = transparentObject.transform.forward;
            transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            // 使用LookRotation函??置物体的旋?
            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);
        }
        else if (Input.GetMouseButton(1))
        {
            mouseX = 0;
            // ?取?像机的前向向量
            Vector3 cameraForward = -transform.right;

            // ?算物体?垂直?旋?的角度
            float verticalRotationAngle = -mouseY * sensitive;

            // ?算物体?水平?旋?的角度，?其限制在-90度到90度之?
            float horizontalRotationAngle = mouseX * sensitive;
            horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            // ?算旋?后的前向方向
            Vector3 transparentObjectForward = transparentObject.transform.forward;
            //transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            // 使用LookRotation函??置物体的旋?
            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);

        }
            /*
                objectRotationX -= mouseY;
                objectRotationY += mouseX;
                objectRotationY = Mathf.Clamp(objectRotationY, -90, 90);

                transparentObject.transform.rotation = Quaternion.Euler(objectRotationX, objectRotationY, 0.0f);
                */
        
        else
        {
            rotationX -= mouseY ;
            rotationY += mouseX ;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }
    }

}
