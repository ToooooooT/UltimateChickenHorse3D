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
    public CanvasGroup fadeCanvasGroup;
    public TextMeshProUGUI nextStageText;

    // Start is called before the first frame update
    void Start() {
        countdown = GetComponent<Countdown>();
        GetStagesNames();
        nextStageId = 0;
        nextStageText.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (countdown && countdown.flgNextStage == true) {
            playersInPavilion = countdown.playersInPavilion;
            // countdown.enabled = false;
            countdown = null;
            GetNextStageId();
            Debug.Log("nextStageId: " + nextStageId);
            nextStageText.enabled = true;
            nextStageText.text = stageNames[nextStageId];
            StartCoroutine(FadeInOut());
        }
    }

    IEnumerator FadeInOut() {
        float fadeSpeed = 0.5f;

        // fade out
        while (fadeCanvasGroup.alpha < 1.0f) {
            fadeCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        TeleportPlayers();
        yield return new WaitForSeconds(2.0f);

        // fade in
        while (fadeCanvasGroup.alpha > 0.0f) {
            fadeCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        nextStageText.enabled = false;
        Destroy(transform.parent.gameObject);
    }

    private void TeleportPlayers() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<Player>().ModifyPosition(new Vector3(0, 1000 * (nextStageId + 1), 0));
            Debug.Log(player.name + " teleport to " + player.transform.position);
        }
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
}
