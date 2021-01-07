using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundEndScreen : MonoBehaviour
{
    public TextMeshProUGUI txt_round_n_of_k;
    public TextMeshProUGUI txt_countdownTimer;

    private void Start()
    {
        FindObjectOfType<GameManager>().scoreBoardPanel.SetActive(true);
        txt_countdownTimer.transform.parent.gameObject.SetActive(true);
        txt_round_n_of_k.transform.parent.gameObject.SetActive(true);

        int currentRound = GameSettings.gameMode.RoundLimit - GameSettings.gameMode.RoundsLeftToPlay;
        txt_round_n_of_k.text = $"Runde {currentRound} von {GameSettings.gameMode.RoundLimit}";
        txt_countdownTimer.text = "5";

        StartCoroutine(CountDownTimer());
    }

    IEnumerator CountDownTimer()
    {
        Time.timeScale = 0f;
        for (int i = 4; i > 0; i--)
        {
            txt_countdownTimer.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        Time.timeScale = 1f;
        FindObjectOfType<GameManager>().scoreBoardPanel.SetActive(false);
        GameManager.Instance.StartNextRound();
        gameObject.SetActive(false);
    }

}
