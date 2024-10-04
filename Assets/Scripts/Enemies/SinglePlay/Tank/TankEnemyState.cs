public abstract class TankEnemyState
{

    protected TankEnemyController _enemy;

    public TankEnemyState(TankEnemyController enemy)
    {
        this._enemy = enemy;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();


}
