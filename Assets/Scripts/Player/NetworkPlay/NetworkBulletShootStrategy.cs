using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkBulletShootStrategy : NetworkBehaviour, IshootStrategy
{
    [SerializeField]
    private NetworkPlayerShoot _playerShoot;
    [SerializeField]
    private Transform _shootPoint;

    public void Shoot()
    {
        if (_playerShoot.GetBulletNum() > 0)
        {
            GetBulletServerRpc(_playerShoot.GetShootVelocity(), _shootPoint.position, _shootPoint.rotation);
            _playerShoot.DeductBulletNum();
        }
    }

    public void Undo()
    {
        _playerShoot.DeductUndoChance();
        _playerShoot.IncreaseBulletNum();
    }

    [ServerRpc]
    public void GetBulletServerRpc(float velocity, Vector3 pos, Quaternion rot, ServerRpcParams serverRpcParams = default)
    {
        NetworkObject pooledBullet = NetworkObjectPool.Singleton.GetNetworkObject(NetworkGameManager.GetInstance().GetSpawner()._bulletPrefab, pos, rot);
        if (!pooledBullet.IsSpawned)
        {
            pooledBullet.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        }
        else
        {
            pooledBullet.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
        }

        Rigidbody bulletRB = pooledBullet.GetComponent<Rigidbody>();
        //pooledBullet.GetComponent<NetworkProjectileEnemyInteract>()._clientID = serverRpcParams.Receive.SenderClientId;
        pooledBullet.GetComponent<NetworkProjectileEnemyInteract>()._prefab = NetworkGameManager.GetInstance().GetSpawner()._bulletPrefab;
        bulletRB.AddForce(bulletRB.transform.forward * (Mathf.Max(0, velocity) + pooledBullet.GetComponent<NetworkProjectileEnemyInteract>()._shootVelocity), ForceMode.VelocityChange);
        StartCoroutine(DestroyBullet(pooledBullet));
    }

    private IEnumerator DestroyBullet(NetworkObject obj)
    {
        yield return new WaitForSeconds(5f);
        if(obj.gameObject.activeSelf == true)
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(obj, NetworkGameManager.GetInstance().GetSpawner()._bulletPrefab);
            if (obj.IsSpawned)
            {
                obj.Despawn(false);
            }
        }
    }
}
