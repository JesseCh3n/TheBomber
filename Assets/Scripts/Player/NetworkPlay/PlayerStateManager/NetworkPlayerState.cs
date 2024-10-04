public abstract class NetworkPlayerState
{

    protected NetworkPlayerManager _playerManager;

    public NetworkPlayerState(NetworkPlayerManager player)
    {
        this._playerManager = player;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();


}
