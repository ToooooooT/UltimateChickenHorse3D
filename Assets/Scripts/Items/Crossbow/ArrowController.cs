using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private GameObject[] Arrows;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Arrows = GameObject.FindGameObjectsWithTag("Arrow");
        for (int i = 0; i < Arrows.Length; i++) {
            Arrow ArrowScript = Arrows[i].GetComponent<Arrow>();
            Arrows[i].transform.position = Arrows[i].transform.position + Time.deltaTime * 80 * ArrowScript.velocity;
            CheckCollision(Arrows[i], ArrowScript);
        }
    }

    void CheckCollision(GameObject arrow, Arrow arrowScript)
    {
        Collider arrowCollider = arrow.GetComponent<Collider>();
        if (arrowCollider != null) {
            if (Physics.SphereCast(arrow.transform.position, 0.3f, arrowScript.velocity, out RaycastHit hitInfo, 1f)) {
                GameObject collidedObject = hitInfo.collider.gameObject;
                Arrow newArrowScript = arrow.GetComponent<Arrow>();
                if (collidedObject != newArrowScript.parentCrossbow) {
                    if (collidedObject.TryGetComponent<Player>(out var player) && 
                    player.state == Player.State.GAME) {
                        player.SetDead();
                    }
                    if (collidedObject.CompareTag("Wall")) {
                        Destroy(arrow);
                    }
                }
            }
        }
    }
}
