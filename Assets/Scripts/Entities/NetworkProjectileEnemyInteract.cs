using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkProjectileEnemyInteract : NetworkBehaviour
{
    [SerializeField] float _damage;
    [SerializeField] public float _shootVelocity;

    //public Action _onEnemyHit;
    public ulong _clientID;
    public GameObject _prefab;
    public NetworkObject _obj;
    public GameObject _enemy;


    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.GetComponent<NetworkObject>().OwnerClientId);
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemy = other.gameObject;
            DespawnEnemy();
            AddScore();
            ReduceEnemyNum();
            _obj = gameObject.GetComponent<NetworkObject>();
            DespawnProjectile();
        }
    }

    public void DespawnEnemy()
    {
        if (IsServer)
        {
            _enemy.GetComponent<IDestroyable>().Die();
        }
    }

    public void AddScore()
    {
        if (IsServer)
        {
            var clientId = gameObject.GetComponent<NetworkObject>().OwnerClientId;
            if (NetworkManager.ConnectedClients.ContainsKey(clientId))
            {
                NetworkGameManager.GetInstance().GetScoreManager().IncrementScore(clientId);
                Debug.Log("when hit, player id is " + clientId);
            }
        }
    }

    public void ReduceEnemyNum()
    {
        if (IsServer)
        {
            NetworkGameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum();
        }
    }

    public void DespawnProjectile()
    {
        if (IsServer)
        {
            StartCoroutine(DespawnCountDown());
        }
    }

    private IEnumerator DespawnCountDown()
    {
        yield return new WaitForSeconds(3f);
        NetworkObjectPool.Singleton.ReturnNetworkObject(_obj, _prefab);
        if (_obj.IsSpawned)
        {
            _obj.Despawn(false);
        }
    }

}
