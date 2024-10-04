using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTankNav
{
    private NetworkTankEnemyController _tank;
    private Transform[] _patrollingPt;
    private int _currentPatrolingTarget;

    public NetworkTankNav(NetworkTankEnemyController tank, Transform[] pt)
    {
        _tank = tank;
        _patrollingPt = pt;
    }

    public void FreeRoaming()
    {
        if (_tank._agent.remainingDistance < 0.1f)
        {
            _currentPatrolingTarget = Random.Range(0, _patrollingPt.Length);
            _tank._agent.destination = _tank._patrolingPoint[_currentPatrolingTarget].position;
        }
    }
}
