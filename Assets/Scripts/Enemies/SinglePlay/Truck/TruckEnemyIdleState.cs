using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckEnemyIdleState : TruckEnemyState
{

    public TruckEnemyIdleState(TruckEnemyController enemy) : base(enemy) { }

    public override void OnStateEnter()
    {
        //Debug.Log("Enemy is now Idling");
    }

    public override void OnStateExit()
    {
        //Debug.Log("Truck enemy exiting idling state");
    }

    public override void OnStateUpdate()
    {
        if (_enemy._playerShot)
        {
            _enemy.ChangeState(new TruckEnemyRunningState(_enemy));
        }
    }
}
