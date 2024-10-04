using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class NetworkUIManager : NetworkBehaviour
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
    [SerializeField] private GameObject _txtGameWinner;
    [SerializeField] private GameObject _restartButton;

    public bool _isReady { get; private set; }
    private GameObject _uiElements;

    public override void OnNetworkSpawn()
    {
        enabled = false;
        _isReady = false;
    }

    public void GameStarted()
    {
        if (IsOwner)
        {
            enabled = true;
            _txtHealth = GameObject.FindWithTag("HealthTxt").GetComponent<TMP_Text>();
            _txtScore = GameObject.FindWithTag("ScoreTxt").GetComponent<TMP_Text>();
            _highScoretxt = GameObject.FindWithTag("HighScoreTxt").GetComponent<TMP_Text>();
            _txtSecondChance = GameObject.FindWithTag("UndoTxt").GetComponent<TMP_Text>();
            _txtLevel = GameObject.FindWithTag("LevelTxt").GetComponent<TMP_Text>();
            _txtBomb = GameObject.FindWithTag("BombSprites").GetComponent<TMP_Text>();
            _txtRocket = GameObject.FindWithTag("RocketSprites").GetComponent<TMP_Text>();
            _uiElements = GameObject.FindWithTag("EndingUI");
            _txtGameOver = _uiElements.transform.GetChild(0).gameObject;
            _txtGameWin = _uiElements.transform.GetChild(1).gameObject;
            _txtGameWinner = _uiElements.transform.GetChild(2).gameObject;
            _restartButton = _uiElements.transform.GetChild(3).gameObject;
            _isReady = true;
        }
    }

    public void GameOver()
    {
        if (IsOwner)
        {
            _txtGameOver.SetActive(true);
            if (IsServer)
            {
                _restartButton.SetActive(true);
            }
        }
    }

    public void GameWin(string name, int score)
    {
        if (IsOwner)
        {
            _txtGameWin.SetActive(true);
            _txtGameWinner.SetActive(true);
            _txtGameWinner.GetComponent<TMP_Text>().text = $"{name} scores {score}";
            if (IsServer)
            {
                _restartButton.SetActive(true);
            }
        }
    }

    public void UpdateHealth(float currentHealth)
    {
        if (IsOwner)
        {
            _txtHealth.SetText(currentHealth.ToString());
        }
    }

    public void UpdateBulletNum(int num)
    {
        if (IsOwner)
        {
            _txtBomb.text = "";
            for (int i = 0; i < num; i++)
            {
                _txtBomb.text += "<sprite=0>";
            }
        }
    }

    public void UpdateRocketNum(int num)
    {
        if (IsOwner)
        {
            _txtRocket.text = "";
            for (int i = 0; i < num; i++)
            {
                _txtRocket.text += "<sprite=1>";
            }
        }
    }

    public void UpdateSecondChance(int num)
    {
        if (IsOwner)
        {
            _txtSecondChance.SetText(num.ToString());
        }
    }

    public void UpdateScore(int score)
    {
        if (IsOwner)
        {
            _txtScore.SetText(score.ToString());
        }
    }

    public void UpdateLevel(string level)
    {
        if (IsOwner)
        {
            _txtLevel.SetText(level);
        }
    }

    public void UpdateHighScore(int highScore)
    {
        if (IsOwner)
        {
            _highScoretxt.SetText(highScore.ToString());
        }
    }
}
