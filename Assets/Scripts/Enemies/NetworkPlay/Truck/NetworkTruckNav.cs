using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTruckNav
{
    private NetworkTruckEnemyController _truck;
    private Transform[] _patrollingPt;
    private int _currentPatrolingTarget;

    public NetworkTruckNav(NetworkTruckEnemyController truck, Transform[] pt)
    {
        _truck = truck;
        _patrollingPt = pt;
    }

    public void FreeRoaming()
    {
        if (_truck._agent.remainingDistance < 0.1f)
        {
            _currentPatrolingTarget = Random.Range(0, _patrollingPt.Length);
            _truck._agent.destination = _truck._patrolingPoint[_currentPatrolingTarget].position;
        }
    }

    public void Stop()
    {
        _truck._agent.isStopped = true;
    }
}
