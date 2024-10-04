
public abstract class NetworkCannonEnemyState
{

    protected NetworkCannonEnemyController _enemy;

    public NetworkCannonEnemyState(NetworkCannonEnemyController enemy)
    {
        this._enemy = enemy;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();


}
