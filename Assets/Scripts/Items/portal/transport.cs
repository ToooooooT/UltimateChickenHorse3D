using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public enum State { enable, unable };
    public State state; 
    public float countdown;

    private GameObject[] portals;

    void Start() {
        state = State.unable;
        countdown = 0;
    }

    void Update() {
        portals = GameObject.FindGameObjectsWithTag("portal");
        if (portals.Length < 2) {
            state = State.unable;
            countdown = 0;
        } else if(countdown <= 0) {
            if (state == State.unable) {
                state = State.enable;
                SetParticleSystem(true);
            } else if (state == State.enable) {
                state = State.unable;
                SetParticleSystem(false);
            }
            countdown = Random.Range(8f, 10f);
        }
        countdown -= Time.deltaTime;
    }

    private void SetParticleSystem(bool active) {
        transform.Find("Rays").gameObject.SetActive(active);
        transform.Find("Rays1").gameObject.SetActive(active);
    }

    private void OnTriggerEnter(Collider other) {
        portals = GameObject.FindGameObjectsWithTag("portal");
        List<GameObject> enablePortals = new();
        for (int i = 0; i < portals.Length; i++) {
            if (portals[i] != gameObject) {
                Transport portalScript = portals[i].GetComponent<Transport>();
                if (portalScript != null && portalScript.state == State.enable) {
                    enablePortals.Add(portals[i]);
                }
            }
        }
        if(enablePortals.Count > 0) {
            int choose = Random.Range(0, enablePortals.Count);
            Transport chosenPortalScript = enablePortals[choose].GetComponent<Transport>();
            
            chosenPortalScript.state = State.unable;
            chosenPortalScript.countdown = Random.Range(8f, 10f);
            chosenPortalScript.SetParticleSystem(false);

            state = State.unable;
            countdown = Random.Range(8f, 10f);
            SetParticleSystem(false);

            if (other.CompareTag("Player")) {
                Player otherscript = other.GetComponent<Player>();
                otherscript.ModifyPosition(enablePortals[choose].transform.position);
            } else if(!other.CompareTag("Wall")) {
                other.transform.position = enablePortals[choose].transform.position;
            }
            
        }
    }
}
