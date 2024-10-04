using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossNav
{
    private BossEnemyController _boss;
    private Transform[] _patrollingPt;
    private int _currentPatrolingTarget;

    public BossNav(BossEnemyController boss, Transform[] pt)
    {
        _boss = boss;
        _patrollingPt = pt;
    }

    public void FreeRoaming()
    {
        if (_boss._agent.remainingDistance < 0.1f)
        {
            _currentPatrolingTarget = Random.Range(0, _patrollingPt.Length);
            _boss._agent.destination = _boss._patrolingPoint[_currentPatrolingTarget].position;
        }
    }

}
