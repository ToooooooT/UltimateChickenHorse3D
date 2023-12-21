using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : PlayerFollowObject
{
    private GameObject[] players;
    private Vector3[] movingVectors;
    private TireController tireControllerScript;
    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        movingVectors = new Vector3[players.Length];
        tireControllerScript = transform.parent.GetComponent<TireController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < players.Length; i++) {
            Vector3 objectPosition = players[i].transform.position;
            Vector3 datumPoint = transform.position + Vector3.Dot(transform.right, (objectPosition - transform.position)) * transform.right;
            Vector3 lookAt = objectPosition - datumPoint;
            Vector3 movingPosition = datumPoint + 
                (lookAt + Mathf.Tan(tireControllerScript.theta * Mathf.PI / 180) * 
                lookAt.magnitude * Vector3.Cross(transform.right, lookAt).normalized);
            //movingPosition = datumPoint ;
            movingVectors[i] = movingPosition - objectPosition;
        }
    }
    public override Vector3 GetDiffPosition(GameObject gameObject)
    {
        
        for(int i = 0; i < players.Length; i++) {
            if (players[i] == gameObject) {
                return movingVectors[i];
            }
        }
        return Vector3.zero;
    }
}
