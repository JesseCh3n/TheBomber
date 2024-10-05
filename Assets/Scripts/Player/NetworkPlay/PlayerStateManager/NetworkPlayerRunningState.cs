using UnityEngine;

public class NetworkPlayerRunningState : NetworkPlayerState
{
    public NetworkPlayerRunningState(NetworkPlayerManager player) : base(player) { }

    public override void OnStateEnter()
    {
        if (_playerManager.CheckOwnership())
        {
            _playerManager.gameObject.GetComponentInChildren<NetworkPlayerController>().OnStart();
        }

        Debug.Log("Player entering running state");
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateExit()
    {
        Debug.Log("Player leaving running state");
    }
}
