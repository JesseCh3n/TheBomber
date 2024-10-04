using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkProjectilePlayerInteract : NetworkBehaviour
{
    [SerializeField] float _damage;
    [SerializeField] public float _shootVelocity;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject obj = other.gameObject;
            var clientId = obj.GetComponentInParent<NetworkObject>().OwnerClientId;
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };
            DeductHealthClientRpc(clientRpcParams);
            DespawnProjectile();
        }
    }

    [ClientRpc]
    public void DeductHealthClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Health damageable = NetworkManager.LocalClient.PlayerObject.GetComponentInChildren<Health>();
        damageable.DeductHealth(_damage);
    }

    public void DespawnProjectile()
    {
        if (IsServer)
        {
            NetworkObject obj = this.gameObject.GetComponent<NetworkObject>();
            GameObject prefab = NetworkGameManager.GetInstance().GetSpawner()._bombPrefab;
            NetworkObjectPool.Singleton.ReturnNetworkObject(obj, prefab);
            if (obj.IsSpawned)
            {
                obj.Despawn(false);
            }
        }
    }
}
