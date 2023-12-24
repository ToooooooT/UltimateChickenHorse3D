using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    private GameObject[] Fireballs;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Fireballs = GameObject.FindGameObjectsWithTag("FireBall");
        for (int i = 0; i < Fireballs.Length; i++) {
            FireBall FireBallScript = Fireballs[i].GetComponent<FireBall>();
            Fireballs[i].transform.position = Fireballs[i].transform.position + Time.deltaTime * 100 * FireBallScript.velocity;
            FireBallScript.velocity += 2 * Time.deltaTime * new Vector3(0,-1,0);
            CheckCollision(Fireballs[i], FireBallScript);
        }
    }

    void CheckCollision(GameObject Fireball, FireBall FireBallScript)
    {
        Collider FireBallCollider = Fireball.GetComponent<Collider>();
        if (FireBallCollider != null) {
            if (Physics.SphereCast(Fireball.transform.position, 1.1f, FireBallScript.velocity, out RaycastHit hitInfo, 1f)) {
                GameObject collidedObject = hitInfo.collider.gameObject;
                FireBall FireBall = Fireball.GetComponent<FireBall>();
                if (collidedObject != FireBall.parentFireBallShooter) {
                    if (collidedObject.TryGetComponent<Player>(out var player) && 
                    player.state == Player.State.GAME) {
                        collidedObject.transform.Find("PlayerVisual").GetComponent<PlayerAnimator>().SetDead();
                    }
                    if (collidedObject.CompareTag("Wall")) {
                        Destroy(Fireball);
                    }
                }
            }
        }
    }
}
