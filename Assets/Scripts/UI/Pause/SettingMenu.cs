using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public void Exit() {
        gameObject.SetActive(false);
        transform.parent.Find("PauseMenu").gameObject.SetActive(true);
    }

    public void Activate(bool isPlaying) {
        transform.Find("FollowSliders").gameObject.SetActive(isPlaying);
        transform.Find("VirtualSliders").gameObject.SetActive(!isPlaying);
    }
}
