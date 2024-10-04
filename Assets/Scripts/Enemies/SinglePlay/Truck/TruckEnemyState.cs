public abstract class TruckEnemyState
{

    protected TruckEnemyController _enemy;

    public TruckEnemyState(TruckEnemyController enemy)
    {
        this._enemy = enemy;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();


}
