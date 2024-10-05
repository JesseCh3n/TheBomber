using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class StartSceneUIManager1 : NetworkBehaviour
{
    [SerializeField] GameObject _runningCanvas;
    [SerializeField] GameObject _SinglePlayBtn;
    [SerializeField] GameObject _CoopPlayBtn;
    [SerializeField] GameObject _StartPlayBtn;
    [SerializeField] GameObject _TestPlayBtn;
    [SerializeField] GameObject _PlayerNameTxt;
    [SerializeField] GameObject _JoinCodeInputTxt;
    [SerializeField] GameObject _HostBtn;
    [SerializeField] GameObject _ClientBtn;
    [SerializeField] GameObject _JoinCodeBtn;
    [SerializeField] GameObject _PlayerIDTxt;
    [SerializeField] GameObject _StatusTxt;
    [SerializeField] GameObject _ClientJoinedTxt;

    //private bool _clientStarted = false;

    public void Awake()
    {
        _runningCanvas.SetActive(false);
        _SinglePlayBtn.SetActive(false);
        _CoopPlayBtn.SetActive(false);
        _StartPlayBtn.SetActive(false);
        _TestPlayBtn.SetActive(false);
        _PlayerNameTxt.SetActive(true);
        _JoinCodeInputTxt.SetActive(true);
        _HostBtn.SetActive(true);
        _ClientBtn.SetActive(true);
        _JoinCodeBtn.SetActive(true);
        _PlayerIDTxt.SetActive(true);
        _StatusTxt.SetActive(true);
        _ClientJoinedTxt.SetActive(false);
    }

    public void Update()
    {
        if(_ClientJoinedTxt.activeSelf == true)
        {
            if(ConnectionNotificationManager.Singleton._playerList.Count < 2)
            {
                _ClientJoinedTxt.GetComponent<TMP_Text>().text = "Waiting for client to join";
            } else if(ConnectionNotificationManager.Singleton._playerList.Count == 2)
            {
                _ClientJoinedTxt.GetComponent<TMP_Text>().text = "Client joined";
            }
        }
    }

    public void UIEnable()
    {
        _SinglePlayBtn.SetActive(false);
        _CoopPlayBtn.SetActive(false);
        _PlayerNameTxt.SetActive(true);
        _JoinCodeInputTxt.SetActive(true);
        _HostBtn.SetActive(true);
        _ClientBtn.SetActive(true);
        _JoinCodeBtn.SetActive(true);
        _PlayerIDTxt.SetActive(true);
        _StatusTxt.SetActive(true);
        _StartPlayBtn.SetActive(true);
    }

    public void TestGame()
    {
        if (ConnectionNotificationManager.Singleton._playerList.Count ==0) return;
        DeactivateCanvasClientRpc();
        if (IsServer)
        {
            NetworkGameManager.GetInstance().GameStart1();
        }
    }

    public void GameStart()
    {
        if (ConnectionNotificationManager.Singleton._playerList.Count <= 1) return;
        DeactivateCanvasClientRpc();
        if (IsServer)
        {
            NetworkGameManager.GetInstance().GameStart1();
        }
    }

    [ClientRpc]
    public void DeactivateCanvasClientRpc()
    {
        _PlayerNameTxt.SetActive(false);
        _JoinCodeInputTxt.SetActive(false);
        _JoinCodeBtn.SetActive(false);
        _PlayerIDTxt.SetActive(false);
        _ClientJoinedTxt.SetActive(false);
        _StatusTxt.SetActive(false);
        _StartPlayBtn.SetActive(false);
        _TestPlayBtn.SetActive(false);
        _runningCanvas.SetActive(true);
    }
}
