using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBombController : MonoBehaviour
{
    private GameObject[] cannonBombs;
    private Collider firstCollider;
    // Start is called before the first frame update
    void Start()
    {
        firstCollider = null;
    }

    // Update is called once per frame
    void Update()
    {
        cannonBombs = GameObject.FindGameObjectsWithTag("CannonBomb");
        for (int i = 0; i < cannonBombs.Length; i++) {
            CannonBomb cannonBombScript = cannonBombs[i].GetComponent<CannonBomb>();
            cannonBombs[i].transform.position = cannonBombs[i].transform.position + cannonBombScript.velocity;
            CheckCollision(cannonBombs[i]);
            CheckOutOfBound(cannonBombs[i]);
        }
    }
    void CheckOutOfBound(GameObject bomb)
    {
        if (bomb.transform.position.x < -100 || bomb.transform.position.x > 100 || bomb.transform.position.y < -100 || bomb.transform.position.y > 100 || bomb.transform.position.z < -100 || bomb.transform.position.z > 100) {
            DestroyBomb(bomb);
        }
    }
    void CheckCollision(GameObject bomb)
    {
        Collider bombCollider = bomb.GetComponent<Collider>();
        if (bombCollider != null) {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(bomb.transform.position, bomb.transform.forward), out hitInfo, 1f)) {
                GameObject collidedObject = hitInfo.collider.gameObject;
                CannonBomb bombScript = bomb.GetComponent<CannonBomb>();
                if (collidedObject != bombScript.parentCannon) {
                    if (collidedObject.TryGetComponent<Player>(out var player) && player.state == Player.State.GAME) {
                        player.state = Player.State.LOSE;
                    }
                    DestroyBomb(bomb);
                }
            }
        }
    }
    void DestroyBomb(GameObject bomb)
    {
        Destroy(bomb);
    }
}
