public abstract class CannonEnemyState
{

    protected CannonEnemyController _enemy;

    public CannonEnemyState(CannonEnemyController enemy)
    {
        this._enemy = enemy;
    }


    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();


}
