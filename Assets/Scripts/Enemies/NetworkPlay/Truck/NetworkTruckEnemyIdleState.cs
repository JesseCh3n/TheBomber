using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTruckEnemyIdleState : NetworkTruckEnemyState
{

    public NetworkTruckEnemyIdleState(NetworkTruckEnemyController enemy) : base(enemy) { }

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
            _enemy.ChangeState(new NetworkTruckEnemyRunningState(_enemy));
        }
    }
}
