using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkRocketShootStrategy : NetworkBehaviour, IshootStrategy
{
    [SerializeField]
    private NetworkPlayerShoot _playerShoot;
    [SerializeField]
    private Transform _shootPoint;

    public void Shoot()
    {
        if (_playerShoot.GetRocketNum() > 0)
        {
            GetRocketServerRpc(_playerShoot.GetShootVelocity(), _shootPoint.position, _shootPoint.rotation);
            _playerShoot.DeductRocketNum();
        }
    }

    public void Undo()
    {
        _playerShoot.DeductUndoChance();
        _playerShoot.IncreaseRocketNum();
    }

    [ServerRpc]
    public void GetRocketServerRpc(float velocity, Vector3 pos, Quaternion rot, ServerRpcParams serverRpcParams = default)
    {
        NetworkObject pooledRocket = NetworkObjectPool.Singleton.GetNetworkObject(NetworkGameManager.GetInstance().GetSpawner()._rocketPrefab, pos, rot);
        if (!pooledRocket.IsSpawned)
        {
            pooledRocket.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        }
        else
        {
            pooledRocket.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
        }

        Rigidbody rocketRB = pooledRocket.GetComponent<Rigidbody>();
        //pooledRocket.GetComponent<NetworkProjectileEnemyInteract>()._clientID = serverRpcParams.Receive.SenderClientId;
        pooledRocket.GetComponent<NetworkProjectileEnemyInteract>()._prefab = NetworkGameManager.GetInstance().GetSpawner()._rocketPrefab;
        rocketRB.AddForce(rocketRB.transform.forward * (Mathf.Max(0, velocity) + pooledRocket.GetComponent<NetworkProjectileEnemyInteract>()._shootVelocity), ForceMode.VelocityChange);
        StartCoroutine(DestroyRocket(pooledRocket));
    }

    private IEnumerator DestroyRocket(NetworkObject obj)
    {
        yield return new WaitForSeconds(5f);
        if (obj.gameObject.activeSelf == true)
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(obj, NetworkGameManager.GetInstance().GetSpawner()._rocketPrefab);
            if (obj.IsSpawned)
            {
                obj.Despawn(false);
            }
        }
    }
}
