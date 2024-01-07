using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpToStage : MonoBehaviour
{
    private string[] stageNames;
    private int nextStageId;
    private Countdown countdown;
    private int[] playersInPavilion;
    private AudioManager audioManager;
    public CanvasGroup fadeCanvasGroup;
    public TextMeshProUGUI nextStageText;
    public bool flag;
    private bool canChangeState = true;

    // Start is called before the first frame update
    void Start() {
        countdown = GetComponent<Countdown>();
        GetStagesNames();
        nextStageId = 0;
        nextStageText.enabled = false;
        flag = false;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update() {
        if (!flag && countdown.flgNextStage == true && canChangeState) {
            canChangeState = false;
            playersInPavilion = countdown.playersInPavilion;
            playersGetSkill();
            GetNextStageId();
            nextStageText.enabled = true;
            nextStageText.text = stageNames[nextStageId];
            StartCoroutine(FadeInOut());
        }
    }
    void playersGetSkill()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++) {
            players[i].GetComponent<Player>().ChangeSkill();
        }
    }
    IEnumerator FadeInOut() {
        float fadeSpeed = 0.5f;
        float volumeRatio = 1.0f;
        float originalVolume = audioManager.Volume;
        // fade out
        while (fadeCanvasGroup.alpha < 1.0f) {
            fadeCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            volumeRatio -= Time.deltaTime * fadeSpeed;
            audioManager.Volume = originalVolume * volumeRatio;
            yield return null;
        }

        // TeleportPlayers();
        audioManager.Volume = 0;
        flag = true;
        transform.parent.position = new Vector3(0, -1000, 0);
        yield return new WaitForSeconds(2.0f);

        // fade in
        while (fadeCanvasGroup.alpha > 0.0f) {
            fadeCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            volumeRatio += Time.deltaTime * fadeSpeed;
            volumeRatio = Mathf.Clamp01(volumeRatio);
            audioManager.Volume = originalVolume * volumeRatio;
            yield return null;
        }

        audioManager.Volume = originalVolume;
        countdown.ResetState();
        transform.parent.gameObject.SetActive(false);
        nextStageText.enabled = false;
        flag = false;
        canChangeState = true;
    }

    private void GetStagesNames() {
        stageNames = new string[transform.childCount];
        int i = 0;

        foreach (Transform child in transform) {
            stageNames[i] = child.name;
            i++;
        }
    }

    private void GetNextStageId() {
        int maxCount = FindMaxPlayerCount();
        nextStageId = GetRandomIndexWithMaxCount(maxCount);
    }

    private int FindMaxPlayerCount() {
        int maxCount = 0;

        for (int i = 0; i < playersInPavilion.Length; i++) {
            if (playersInPavilion[i] > maxCount) {
                maxCount = playersInPavilion[i];
            }
        }

        return maxCount;
    }

    private int GetRandomIndexWithMaxCount(int maxCount) {
        List<int> maxIndices = new List<int>();

        for (int i = 0; i < playersInPavilion.Length; i++) {
            if (playersInPavilion[i] == maxCount) {
                maxIndices.Add(i);
            }
        }

        int randomIndex = maxIndices[Random.Range(0, maxIndices.Count)];
        return randomIndex;
    }

    public string GetChoosenStageName() {
        return stageNames[nextStageId];
    }
}
