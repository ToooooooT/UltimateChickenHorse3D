using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : BaseItem
{
    private enum State { shooting, idle, placing };
    [SerializeField] private State state = State.placing;
    private const string FOLDERPATH = "Fireball";
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float countdown;
    [SerializeField] private float countdownTime;

    void Awake()
    {
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
            GenerateFireBall();
            state = State.idle;
            countdown = countdownTime;
        }
        else {
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

    public void GenerateFireBall()
    {
        GameObject newFireball = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/Fireball"));
        newFireball.transform.position = transform.position + 3f * transform.up + 7f * transform.right;
        //newFireball.transform.localScale = new Vector3(10f, 10f, 10f);
        FireBall FireBallScript = newFireball.GetComponent<FireBall>();
        FireBallScript.velocity = transform.right * 0.5f;
        FireBallScript.parentFireBallShooter = this.gameObject;
        Destroy(newFireball, 5f);
    }
}
