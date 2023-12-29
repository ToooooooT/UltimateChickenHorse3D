using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Data
{
    public Data() { }
    public string skillName = "";
    public float cooldownTime = 0;
    public float castTime = 0;
    //Jump
    public bool jumpHigh = false;
    //dance invincible
    public bool invincible = false;
    public float dancingAngle = 0;
}
public class SkillReader : Data
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Data GetSkill(string skillName)
    {
        string[] skillNames = new string[] { "JumpHigh", "DanceInvincible", "Shoot", "Magnetic", "Hook", "Tack" };
        switch (skillName) {
            case "JumpHigh":return GetJumpHighData(skillName);
            case "DanceInvincible": return GetDanceInvincibleData(skillName);
            case "Shoot": return GetShootData(skillName);
            case "Magnetic": return GetMagneticData(skillName);
            case "Hook": return GetHookData(skillName);
            case "Tack": return GetTackData(skillName);
        }
        return GetJumpHighData("DanceInvincible");
        //return GetSkill(skillNames[(int) Random.Range(0, skillNames.Length-1)]);
    }

    private Data GetJumpHighData(string skillName)
    {
        Data skillData = new Data();
        skillData.skillName = skillName;
        skillData.cooldownTime = 0;
        return skillData;
    }
    private Data GetDanceInvincibleData(string skillName)
    {
        Data skillData = new Data();
        skillData.skillName = skillName;
        skillData.cooldownTime = 0;
        return skillData;
    }
    private Data GetShootData(string skillName)
    {
        Data skillData = new Data();
        skillData.skillName = skillName;
        skillData.cooldownTime = 10;
        return skillData;
    }
    private Data GetMagneticData(string skillName)
    {
        Data skillData = new Data();
        skillData.skillName = skillName;
        skillData.cooldownTime = 60;
        return skillData;
    }
    private Data GetHookData(string skillName)
    {
        Data skillData = new Data();
        skillData.skillName = skillName;
        skillData.cooldownTime = 30;
        return skillData;
    }
    private Data GetTackData(string skillName)
    {
        Data skillData = new Data();
        skillData.skillName = skillName;
        skillData.cooldownTime = 10;
        return skillData;
    }
}
