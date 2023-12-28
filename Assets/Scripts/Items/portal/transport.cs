using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    private enum State { enable, unable };
    private State state; 

    private float countdown;
    private GameObject magic;
    private GameObject[] portals;
    private Color validColor = new(0.0f, 1.0f, 0.0f, 0.05f);

    void Start() {
        state = State.unable;
        magic = transform.Find("Magic").gameObject;
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
                Renderer cubeRenderer = GetComponent<Renderer>();
                int newLayerMask = 1 << 1;
                cubeRenderer.renderingLayerMask = (uint)newLayerMask;
                if (TryGetComponent<Renderer>(out var renderer)) {
                    renderer.material.color = validColor;
                }
            } else if (state == State.enable) {
                state = State.unable;
                Renderer cubeRenderer = GetComponent<Renderer>();
                int newLayerMask = 0;
                cubeRenderer.renderingLayerMask = (uint)newLayerMask;
            }
            countdown = Random.Range(8f, 10f);
        }
        countdown -= 0.01f;
    }

    public void StartParticleSystem() {
        magic.SetActive(true);
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
            Renderer chosenCubeRenderer = chosenPortalScript.GetComponent<Renderer>();
            int chosenNewLayerMask = 0;
            chosenCubeRenderer.renderingLayerMask = (uint)chosenNewLayerMask;
            chosenPortalScript.StartParticleSystem();

            state = State.unable;
            countdown = Random.Range(8f, 10f);
            Renderer myCubeRenderer = GetComponent<Renderer>();
            int myNewLayerMask = 0;
            myCubeRenderer.renderingLayerMask = (uint)myNewLayerMask;
            StartParticleSystem();

            if (other.CompareTag("Player")) {
                Player otherscript = other.GetComponent<Player>();
                otherscript.ModifyPosition(enablePortals[choose].transform.position);
            } else if(!other.CompareTag("Wall")) {
                other.transform.position = enablePortals[choose].transform.position;
            }
            
        }
    }
}
