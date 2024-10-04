using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkCannonEnemyIdleState : NetworkCannonEnemyState
{
    public NetworkCannonEnemyIdleState(NetworkCannonEnemyController enemy) : base(enemy) { }

    public override void OnStateEnter()
    {
        //Debug.Log("Enemy is now Idling");
    }

    public override void OnStateExit()
    {
        Debug.Log("Cannon enemy exiting idling state");
    }

    public override void OnStateUpdate()
    {
        //Debug.Log("Cannon enemy is idling");
        if (_enemy._playerShot)
        {
            _enemy.ChangeState(new NetworkCannonEnemyAttackState(_enemy));
        }
    }
}
