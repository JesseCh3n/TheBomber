using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTruckEnemyRunningState : NetworkTruckEnemyState
{

    public NetworkTruckEnemyRunningState(NetworkTruckEnemyController enemy) : base(enemy) { }

    public override void OnStateEnter()
    {
        _enemy._agent.enabled = true;
        //Debug.Log("Enemy is now Running");
    }

    public override void OnStateExit()
    {
        //Debug.Log("Truck enemy exiting running state");
    }

    public override void OnStateUpdate()
    {
        _enemy._navigation.FreeRoaming();
    }
}
