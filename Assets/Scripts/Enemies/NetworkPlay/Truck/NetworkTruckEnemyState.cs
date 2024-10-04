public abstract class NetworkTruckEnemyState
{

    protected NetworkTruckEnemyController _enemy;

    public NetworkTruckEnemyState(NetworkTruckEnemyController enemy)
    {
        this._enemy = enemy;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();

}
