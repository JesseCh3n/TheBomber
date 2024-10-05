using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;


public class SceneUIManager : NetworkBehaviour
{
    // Start is called before the first frame update
    public void ChangeScene(string sceneName)
    {
        if (IsServer)
        {
            Debug.Log("hello");
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    public void RestartScene()
    {
        if (IsServer)
        {
            RestartSceneClientRpc();
        }
    }

    [ClientRpc]
    public void RestartSceneClientRpc()
    {
        Disconnect();
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
    }

    private void Disconnect()
    {
        NetworkObjectPool.Singleton.OnNetworkDespawn();
        NetworkManager.Singleton.Shutdown();
        AuthenticationService.Instance.SignOut(true);
        if (NetworkManager.Singleton != null)
        {
            GameNetworkManager.GetInstance().DestroyObject();
            Destroy(GameNetworkManager.GetInstance().gameObject);
        }
    }
}
