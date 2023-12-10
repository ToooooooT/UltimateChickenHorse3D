using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    public float countdownDuration = 5f;
    private float countdownTimer;
    public int[] playersInPavilion;

    public TextMeshProUGUI countdownText;
    public int totalPlayersCount = 1;
    public bool flgNextStage;

    void Start() {
        ResetState();
    }

    void Update() {
        CountTotalPlayers();

        if (CheckPlayersAllInPavilion() && !flgNextStage) {
            countdownTimer -= Time.deltaTime;

            if (countdownTimer <= 0f) {
                countdownTimer = 0f;
                flgNextStage = true;
                countdownText.enabled = false;
            } else {
                countdownText.enabled = true;
                UpdateCountdownText();
            }
        } else {
            countdownText.enabled = false;
            countdownTimer = 5f;
        }
    }

    public void ResetState() {
        playersInPavilion = new int[transform.childCount];
        countdownTimer = countdownDuration;
        countdownText.enabled = false;
        flgNextStage = false;
        UpdateCountdownText();
    }


    private void CountTotalPlayers() {
        totalPlayersCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private bool CheckPlayersAllInPavilion() {
        int total = 0;
        foreach (int count in playersInPavilion) {
            total += count;
        }
        return total == totalPlayersCount && totalPlayersCount != 0;
    }

    private void UpdateCountdownText() {
        if (countdownText != null) {
            countdownText.text = "GAME WILL START IN\n" + Mathf.FloorToInt(countdownTimer).ToString();
        }
    }
}
