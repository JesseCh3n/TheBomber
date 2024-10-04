using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemyIdleState : TankEnemyState
{
    //private int _currentPatrolingTarget = 0;
    public TankEnemyIdleState(TankEnemyController enemy) : base(enemy) { }

    public override void OnStateEnter()
    {
        _enemy._agent.enabled = true;
        //Debug.Log("Enemy is now Idling");
    }

    public override void OnStateExit()
    {
        _enemy._agent.enabled = false;
        //Debug.Log("Tank enemy exiting idling state");
    }

    public override void OnStateUpdate()
    {
        _enemy._navigation.FreeRoaming();
        if (_enemy._playerShot)
        {
            _enemy.ChangeState(new TankEnemyAttackState(_enemy));
        }
    }
}
