using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBombController : MonoBehaviour
{
    private GameObject[] cannonBombs;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cannonBombs = GameObject.FindGameObjectsWithTag("CannonBomb");
        for (int i = 0; i < cannonBombs.Length; i++) {
            CannonBomb cannonBombScript = cannonBombs[i].GetComponent<CannonBomb>();
            cannonBombs[i].transform.position = cannonBombs[i].transform.position + Time.deltaTime * 40 * cannonBombScript.velocity;
            CheckCollision(cannonBombs[i], cannonBombScript);
        }
    }

    void CheckCollision(GameObject bomb, CannonBomb cannonBombScript)
    {
        Collider bombCollider = bomb.GetComponent<Collider>();
        if (bombCollider != null) {
            if (Physics.SphereCast(bomb.transform.position, 1.1f, cannonBombScript.velocity, out RaycastHit hitInfo, 1f)) {
                GameObject collidedObject = hitInfo.collider.gameObject;
                CannonBomb bombScript = bomb.GetComponent<CannonBomb>();
                if (collidedObject != bombScript.parentCannon) {
                    if (collidedObject.TryGetComponent<Player>(out var player) && 
                    player.GetState() == Player.State.GAME) {
                        player.SetDead();
                    }
                    if (collidedObject.CompareTag("Wall")) {
                        Destroy(bomb);
                    }
                }
            }
        }
    }
}
