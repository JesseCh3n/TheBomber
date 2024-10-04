using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("GamePlay")]
    [SerializeField] private TMP_Text _txtHealth;
    [SerializeField] private TMP_Text _txtScore;
    [SerializeField] private TMP_Text _highScoretxt;
    [SerializeField] private TMP_Text _txtSecondChance;
    [SerializeField] private TMP_Text _txtLevel;
    [SerializeField] private TMP_Text _txtBomb;
    [SerializeField] private TMP_Text _txtRocket;
    [SerializeField] private GameObject _txtGameOver;
    [SerializeField] private GameObject _txtGameWin;
    [SerializeField] private GameObject _restartButton;


    public void GameStarted()
    {
        _txtGameOver.SetActive(false);
        _txtGameWin.SetActive(false);
        _restartButton.SetActive(false);
    }

    public void GameOver()
    {
        _txtGameOver.SetActive(true);
        _restartButton.SetActive(true);
    }
    public void GameWin()
    {
        _txtGameWin.SetActive(true);
        _restartButton.SetActive(true);
    }

    public void UpdateHealth(float currentHealth)
    {
        _txtHealth.SetText(currentHealth.ToString());
    }

    public void UpdateBulletNum(int num)
    {
        _txtBomb.text = "";
        for (int i = 0; i < num; i++)
        {
            _txtBomb.text += "<sprite=0>";
        }
    }

    public void UpdateRocketNum(int num)
    {
        _txtRocket.text = "";
        for (int i = 0; i < num; i++)
        {
            _txtRocket.text += "<sprite=1>";
        }
    }

    public void UpdateSecondChance(int num)
    {
        _txtSecondChance.SetText(num.ToString());
    }

    public void UpdateScore()
    {
        _txtScore.SetText(GameManager.GetInstance().GetScoreManager().GetScore().ToString());
    }

    public void UpdateLevel()
    {
        _txtLevel.SetText(GameManager.GetInstance().GetCurrentLevel().lvl._levelName.ToString());
    }

    public void UpdateHighScore()
    {
        _highScoretxt.SetText(GameManager.GetInstance().GetScoreManager().GetHighScore().ToString());
    }


}