using UnityEngine;

public class NetworkPlayerAwakeState : NetworkPlayerState
{
    public NetworkPlayerAwakeState(NetworkPlayerManager player) : base(player) { }

    public override void OnStateEnter()
    {
        SetPlayerPos();
        Debug.Log("Player entering awake state");
    }

    public override void OnStateUpdate()
    {
        if (NetworkGameManager.GetInstance()._gameReady)
        {
            _playerManager.ChangeState(new NetworkPlayerActivationState(_playerManager));
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Player leaving awake state");
    }

    private void SetPlayerPos()
    {
        int randInt = Random.Range(0, NetworkGameManager.GetInstance().GetSpawner()._playerSpawnPoint.Length);
        Vector3 randomSpawnPosition = NetworkGameManager.GetInstance().GetSpawner()._playerSpawnPoint[randInt].position;
        Debug.Log("Network object " + _playerManager.transform.position);
        _playerManager.GetComponentInChildren<CharacterController>().transform.position = randomSpawnPosition;
        Debug.Log("Network transform " + _playerManager.transform.GetChild(1).transform.position);
    }

}

