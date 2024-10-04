using UnityEngine;

public class NetworkPlayerInitiationState : NetworkPlayerState
{
    public NetworkPlayerInitiationState(NetworkPlayerManager player) : base(player) { }

    public override void OnStateEnter()
    {
        if (_playerManager.CheckOwnership())
        {
            _playerManager.gameObject.GetComponentInChildren<NetworkPlayerController>().SetPlayer();
        }

        Debug.Log("Player entering initiation state");
    }

    public override void OnStateUpdate()
    {
        if (_playerManager.CheckHealth())
        {
            _playerManager.ChangeState(new NetworkPlayerRunningState(_playerManager));
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Player leaving initiation state");
    }
}

