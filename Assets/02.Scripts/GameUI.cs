using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public Text txtScore;
    // 表示分数的变量
    private int totScore = 0;

    void Start()
    {
        DispScore(0);
    }

    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";

        // 保存分数
        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
}
