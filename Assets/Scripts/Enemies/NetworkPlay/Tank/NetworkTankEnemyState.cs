public abstract class NetworkTankEnemyState
{

    protected NetworkTankEnemyController _enemy;

    public NetworkTankEnemyState(NetworkTankEnemyController enemy)
    {
        this._enemy = enemy;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();


}
