using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartSceneUIManager1 : NetworkBehaviour
{
    [SerializeField] GameObject _runningCanvas;
    [SerializeField] GameObject _SinglePlayBtn;
    [SerializeField] GameObject _CoopPlayBtn;
    [SerializeField] GameObject _StartPlayBtn;
    [SerializeField] GameObject _PlayerNameTxt;
    [SerializeField] GameObject _JoinCodeInputTxt;
    [SerializeField] GameObject _HostBtn;
    [SerializeField] GameObject _ClientBtn;
    [SerializeField] GameObject _JoinCodeBtn;
    [SerializeField] GameObject _PlayerIDTxt;
    [SerializeField] GameObject _StatusTxt;

    //private bool _clientStarted = false;

    public void Awake()
    {
        _runningCanvas.SetActive(false);
        _SinglePlayBtn.SetActive(false);
        _CoopPlayBtn.SetActive(false);
        _StartPlayBtn.SetActive(false);
        _PlayerNameTxt.SetActive(true);
        _JoinCodeInputTxt.SetActive(true);
        _HostBtn.SetActive(true);
        _ClientBtn.SetActive(true);
        _JoinCodeBtn.SetActive(true);
        _PlayerIDTxt.SetActive(true);
        _StatusTxt.SetActive(true);
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

    public void GameStart()
    {
        DeactivateCanvasClientRpc();
        if (IsServer)
        {
            NetworkGameManager.GetInstance().GameStart1();
        }
    }

    [ClientRpc]
    public void DeactivateCanvasClientRpc()
    {
        Debug.Log("Client is here 1");
        _PlayerNameTxt.SetActive(false);
        _JoinCodeInputTxt.SetActive(false);
        _JoinCodeBtn.SetActive(false);
        _PlayerIDTxt.SetActive(false);
        _StatusTxt.SetActive(false);
        _StartPlayBtn.SetActive(false);
        _runningCanvas.SetActive(true);
    }
}
