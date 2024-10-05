
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class NetworkScoreManager : NetworkBehaviour
{
    private int _highScore;

    Dictionary<ulong, int> _playerScoresDict = new Dictionary<ulong, int>();

    public Action _scoreUpdated;
    public Action _highScoreUpdated;

    [System.Serializable]
    public class ScoreInfo
    {
        public ulong _id;
        public string _name;
        public int _score;
    }

    public void OnStart()
    {
        _highScore = 0;
    }

    public void PopulateScore(NetworkObject playerObject)
    {
        if (IsServer)
        {
            _playerScoresDict.Add(playerObject.OwnerClientId, 0);
            GetScoreClientRpc();
            SetHighScore();
        }
    }

    public void FetchHighScore()
    {
        if (IsServer)
        {
            SetHighScoreClientRpc(_highScore);
        }
    }

    [ClientRpc]
    public void SetHighScoreClientRpc(int highscore)
    {
        NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<NetworkUIManager>().UpdateHighScore(highscore);
    }

    public void FetchScore(NetworkObject playerObject)
    {
        if (IsServer)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerObject.OwnerClientId }
                }
            };
            SetPlayerScoreClientRpc(_playerScoresDict[playerObject.OwnerClientId], clientRpcParams);
        }
    }

    [ClientRpc]
    public void SetPlayerScoreClientRpc(int score, ClientRpcParams clientRpcParams = default)
    {
        NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<NetworkUIManager>().UpdateScore(score);
    }

    [ClientRpc]
    public void GetHighScoreClientRpc()
    {
        _highScoreUpdated?.Invoke();
        //NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<NetworkUIManager>().UpdateHighScore(highScore);
    }

    [ClientRpc]
    public void GetScoreClientRpc()
    {
        _scoreUpdated?.Invoke();
        //NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<NetworkUIManager>().UpdateScore(score);
    }

    public void IncrementScore(ulong id)
    {
        if (IsServer)
        {
            _playerScoresDict[id]++;
            SetHighScore();
            GetScoreClientRpc();
        }
    }

    public void SetHighScore()
    {
        //PlayerPrefs.SetInt("HighScore", _highScore);
        if (IsServer)
        {
            _highScore = _playerScoresDict.Values.Max();
            GetHighScoreClientRpc();
        }
    }

    //Ending Sequence
    public void GetHighScore()
    {
        ScoreInfo temp = new ScoreInfo();
        temp._id = _playerScoresDict.FirstOrDefault(x => x.Value == _highScore).Key;
        temp._name = ConnectionNotificationManager.Singleton._playerNameDict[temp._id];
        temp._score = _highScore;
        ShowGameEndUIClientRPC(JsonUtility.ToJson(temp));
    }

    [ClientRpc]
    public void ShowGameEndUIClientRPC(string winnerInfo)
    {
        ScoreInfo info = JsonUtility.FromJson<ScoreInfo>(winnerInfo);
        NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<NetworkUIManager>().GameWin(info._name, info._score);
    }
}
