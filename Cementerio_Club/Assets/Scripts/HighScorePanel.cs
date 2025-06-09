using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscorePanel : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Update()
    {
#if !UNITY_WEBGL
        if (Input.GetKey(KeyCode.F12))
        {
            Debug.Log("RESET");
            HighScoreManager.instance.ResetData();
            RefreshUI();
        }
#endif
    }

    private void RefreshUI()
    {
        HighScoreManager.ScoreList data = HighScoreManager.instance.HIGHSCORES;
        for (int i = 0; i < 10; i++)
        {
            names[i].text = data.scoreData[i].name;
            scores[i].text = data.scoreData[i].score.ToString("0000");
        }
    }
}
