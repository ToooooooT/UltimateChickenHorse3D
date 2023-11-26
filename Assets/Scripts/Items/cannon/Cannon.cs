using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private enum State { shooting, idle };
    private State state;
    private const string FOLDERPATH = "cannon bomb";
    public float countdown;
    public float countdownTime;
    // Start is called before the first frame update
    void Start()
    {
        state = State.idle;
        countdownTime = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (state == State.shooting) {
            Generatebomb();
            state = State.idle;
            countdown = countdownTime;
        }
        else {
            countdown -= 0.1f;
            if (countdown <= 0) {
                state = State.shooting;
            }
        }

    }
    public void Generatebomb()
    {
        GameObject newBomb = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/Round_shot"));
        newBomb.transform.position = transform.position + 1.6f*transform.up + 2f*transform.forward;
        newBomb.transform.localScale = new Vector3(3f,3f,3f);
        CannonBomb BombScript = newBomb.GetComponent<CannonBomb>();
        BombScript.velocity =  transform.forward;
        BombScript.parentCannon = this.gameObject;
    }
}
