using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string _levelName;
    public int _cannonNum;
    public int _tankNum;
    public int _truckNum;
    public int _bossNum;
    public int _totalBomb;
    public int _totalRocket;
    public int _secondChance;
    public float _playerSpeed;
    public float _playerHealth;
}
