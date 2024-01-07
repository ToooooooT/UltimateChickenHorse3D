using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : BaseItem
{
    private enum State { IDLE, ROTATE };

    [SerializeField] private float rotateSpeed;

    private Vector3 origin_position;
    private Quaternion origin_rotation;
    private State state;
    private Transform pirateCoin;
    private Vector3 localScale;
    private ParticleSystem lightGlow;
    private AudioManager audioManager;

    void Awake() {
        state = State.IDLE;
        pirateCoin = transform.Find("PirateCoin");
        pirateCoin.GetComponent<MeshCollider>().enabled = false;
        lightGlow = transform.Find("LightGlow").GetComponent<ParticleSystem>();
    }

    void Start() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        rotateSpeed = 100.0f;
    }

    void Update() {
        if (state == State.ROTATE) {
            Rotate();
            if (transform.parent != null && 
            transform.parent.GetComponent<Player>().GetState() == Player.State.WIN) {
                audioManager.PlaySE("getMoney");
                transform.Find("Explode").gameObject.SetActive(true);
                transform.Find("PirateCoin").gameObject.SetActive(false);
                lightGlow.Stop();
                state = State.IDLE;
            }
        }
    }

    private void Rotate() {
        Vector3 rotation = pirateCoin.eulerAngles;
        rotation.z += Time.deltaTime * rotateSpeed;
        pirateCoin.rotation = Quaternion.Euler(rotation);
    }

    public override void Initialize() {
        state = State.ROTATE;
        lightGlow.Play();
        pirateCoin.GetComponent<MeshCollider>().enabled = true;
        localScale = transform.localScale;
        origin_position = transform.position;
        origin_rotation = transform.rotation;
    }

    public override void Reset() {
        state = State.IDLE;
        lightGlow.Stop();
        transform.localScale = localScale;
        transform.parent = null;
        pirateCoin.GetComponent<MeshCollider>().enabled = true;
        transform.SetPositionAndRotation(origin_position, origin_rotation);
    }

    public bool IsRotate() {
        return state == State.ROTATE;
    }
}
