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
            ChangePlayerSkill(other.gameObject);
        }
    }
    private void ChangePlayerSkill(GameObject player)
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript.skillData == null) {
            ChooseSkill(player, -1);
        }
        else {
            ChooseSkill(player.gameObject, playerScript.skillData.skillNum);
        }
    }
    private void ChooseSkill(GameObject player, int skillNum)
    {
        Player playerScript = player.gameObject.GetComponent<Player>();
        string[] skillNames = new SkillReader().GetSkillNames();
        int newSkillNum = (skillNum + 1) % skillNames.Length;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++) {
            Data skillData = players[i].GetComponent<Player>().skillData;
            if (skillData != null && skillData.skillNum == newSkillNum) {
                ChooseSkill(player, newSkillNum);
                return;
            }
        }
        playerScript.ChangeSkill(skillNames[newSkillNum]);
        playerScript.skillData.skillNum = newSkillNum;
    }
}
