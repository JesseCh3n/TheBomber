using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : MonoBehaviour
{
    [SerializeField] private int _connectionMax = 2;
    private bool _clientJoined = false;

    [Header("Connection UI")]
    [SerializeField] private GameObject _btnClient;
    [SerializeField] private GameObject _btnHost;
    [SerializeField] private TMP_Text _playerIDTxt;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private TMP_Text _joinCodeTxt;
    [SerializeField] private TMP_InputField _joinCodeIF;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private GameObject _startBtn;

    private static GameNetworkManager _instance;
    public static GameNetworkManager GetInstance()
    {
        return _instance;
    }

    private string _joinCode;
    private string _playerID;
    private bool _clientAuthenticated = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    private async void Start()
    {
        await AuthenticatePlayer();
    }
    private async Task AuthenticatePlayer()
    {
        // NetworkManager.Singleton.StartHost();
        try
        {
            await UnityServices.InitializeAsync();//Initialize Unity Services
            await AuthenticationService.Instance.SignInAnonymouslyAsync();//Sign in the user Anonymously
            _playerID = AuthenticationService.Instance.PlayerId;
            _playerIDTxt.text = $"Player ID: {_playerID}";
            Debug.Log($"client authentication success - {_playerID}");
            _clientAuthenticated = true;

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    IEnumerator ConfigureGetCodeAndJoinHost()
    {
        var allocateAndGetCode = AllocateRelayServerAndGetCode(_connectionMax);
        while (!allocateAndGetCode.IsCompleted)
        {
            yield return null;
        }
        if (allocateAndGetCode.IsFaulted)
        {
            Debug.LogError($"Cannot start the servre due to an exception {allocateAndGetCode.Exception.Message}");
            yield break;
        }
        var relayServerData = allocateAndGetCode.Result;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
        _joinCodeIF.gameObject.SetActive(true);
        _joinCodeIF.text = _joinCode;
        _joinCodeTxt.text = _joinCode;
        _statusText.text = "Joined as Host";
        _startBtn.SetActive(true);
        //StartCoroutine(WaitForClientJoin());
    }

    private async Task<RelayServerData> AllocateRelayServerAndGetCode(int maxConnection, string region = null)
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection, region);
        }
        catch (Exception e)
        {
            Debug.Log($"Relay allocation request failed - {e}");
            throw;
        }

        Debug.Log($"Server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Server : {allocation.AllocationId}");

        try
        {
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.Log($"Unable to create a join code {e}");
        }
        return new RelayServerData(allocation, "wss");
    }


    IEnumerator ConfigureUseCodeJoinClient(string joinCode)
    {
        var joinAllocationFromCode = JoinRelayServerWithCode(joinCode);
        while (!joinAllocationFromCode.IsCompleted)
        {
            yield return null;
        }
        if (joinAllocationFromCode.IsFaulted)
        {
            Debug.LogError($"Cannot join the relay due to an exception {joinAllocationFromCode.Exception.Message}");
        }

        var relayServerData = joinAllocationFromCode.Result;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
        _statusText.text = "Joined As Client";
        _clientJoined = true;
    }

    JoinAllocation joinAllocation;
    public async Task<RelayServerData> JoinRelayServerWithCode(string joinCode)
    {
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log($"Relay allocation join request failed {e}");
        }
        Debug.Log($"Client : {joinAllocation.ConnectionData[0]} {joinAllocation.ConnectionData[1]}");
        Debug.Log($"Server : {joinAllocation.HostConnectionData[0]} {joinAllocation.HostConnectionData[1]}");
        Debug.Log($"client: {joinAllocation.AllocationId}");

        return new RelayServerData(joinAllocation, "wss");
    }
    public void StartHost()
    {
        if (!_clientAuthenticated)
        {
            Debug.Log("client is not authenticated please try again");
            return;
        }
        StartCoroutine(ConfigureGetCodeAndJoinHost());
        _btnClient.gameObject.SetActive(false);
        _btnHost.gameObject.SetActive(false);
        _joinCodeIF.gameObject.SetActive(false);
        Debug.Log("Host Started");
    }
    public void StartClient()
    {
        _statusText.text = "Joined As Client";
        if (!_clientAuthenticated)
        {
            Debug.Log("Client is not authenticated, please try again!");
        }
        if (_joinCodeIF.text.Length == 0)
        {
            Debug.Log("Enter a proper join code");
            _statusText.text = "Enter a proper join code";
        }
        Debug.Log(_joinCodeIF.text);
        StartCoroutine(ConfigureUseCodeJoinClient(_joinCodeIF.text));
        _btnClient.gameObject.SetActive(false);
        _btnHost.gameObject.SetActive(false);
        _joinCodeIF.gameObject.SetActive(false);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Server Started");
    }


    public string GetPlayerName()
    {
        if (_playerName != null)
        {
            return _playerName.text.ToString();
        }
        else return "Player Name";
    }

    public void DestroyObject()
    {
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
    }

    private IEnumerator WaitForClientJoin()
    {
        yield return new WaitUntil(() => _clientJoined == true);
        Debug.Log(_clientJoined);
        Debug.Log("Client Joined");
        _startBtn.SetActive(true);
    }

}

