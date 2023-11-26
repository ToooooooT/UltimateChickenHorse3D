using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringTop : MonoBehaviour
{
    private bool touchPlayer;
    private Player player;

    // Start is called before the first frame update
    void Start() {
        touchPlayer = false;
        player = null;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public bool TouchPlayer() {
        return touchPlayer;
    }

    public Player Player() {
        return player;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Touch player");
            player = other.GetComponent<Player>();
            touchPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            player = null;
            touchPlayer = false;
        }
    }
}
