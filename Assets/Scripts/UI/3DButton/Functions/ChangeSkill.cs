using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Player playerScript = other.gameObject.GetComponent<Player>();
            string[] skillNames = new SkillReader().GetSkillNames();
            playerScript.ChangeSkill(skillNames[Random.Range(0, skillNames.Length)]);
        }
    }
}
