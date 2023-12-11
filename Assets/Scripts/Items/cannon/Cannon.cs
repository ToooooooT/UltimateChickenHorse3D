using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : BaseItem
{
    private enum State { shooting, idle , placing};
    [SerializeField] private State state = State.placing;
    private const string FOLDERPATH = "cannon bomb";
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float countdown;
    [SerializeField] private float countdownTime;

    void Awake() {
        state = State.placing;
    }

    // Start is called before the first frame update
    void Start()
    {
        countdownTime = 10;
        rotateSpeed = 40;
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.placing) {
            ShootingMode();
        }
    }

    void ShootingMode()
    {
        if (state == State.shooting) {
            Generatebomb();
            state = State.idle;
            countdown = countdownTime;
        } else {
            countdown -= 10 * Time.deltaTime;
            if (countdown <= 0) {
                state = State.shooting;
            }
        }
        transform.Rotate(transform.up, rotateSpeed * Time.deltaTime);
    }

    public override void Initialize()
    {
        state = State.shooting;
    }

    public override void Reset()
    {
        state = State.placing;
    }

    public void Generatebomb()
    {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject newBomb = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/Round_shot"));
            newBomb.transform.position = transform.GetChild(i).position + 1.6f * transform.GetChild(i).up + 2f * transform.GetChild(i).forward;
            newBomb.transform.localScale = new Vector3(10f, 10f, 10f);
            CannonBomb BombScript = newBomb.GetComponent<CannonBomb>();
            BombScript.velocity = transform.GetChild(i).forward * 0.5f;
            BombScript.parentCannon = this.gameObject;
            Destroy(newBomb, 5f);
        }
    }
}
