using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonEnemyIdleState : CannonEnemyState
{

    public CannonEnemyIdleState(CannonEnemyController enemy) : base(enemy) { }

    public override void OnStateEnter()
    {
        //Debug.Log("Enemy is now Idling");
    }

    public override void OnStateExit()
    {
        //Debug.Log("Cannon enemy exiting idling state");
    }

    public override void OnStateUpdate()
    {
        //Debug.Log("Cannon enemy is idling");
        if (_enemy._playerShot)
        {
            _enemy.ChangeState(new CannonEnemyAttackState(_enemy));
        }
    }
}
