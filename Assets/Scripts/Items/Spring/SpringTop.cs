using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringTop : MonoBehaviour
{
    private bool touchPlayer;
    private Player player;
    private bool stay;
    public Vector3 normal;

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
            player = other.GetComponent<Player>();
            touchPlayer = true;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!stay && other.gameObject.CompareTag("Player")) {
            player = other.GetComponent<Player>();
            touchPlayer = true;
            stay = true;
        } else if (other.gameObject.CompareTag("Player")) {
            touchPlayer = false;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            touchPlayer = false;
            stay = false;
        }
    }
}
