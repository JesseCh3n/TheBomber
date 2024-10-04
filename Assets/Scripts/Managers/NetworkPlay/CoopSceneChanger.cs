using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class CoopSceneChanger : NetworkBehaviour
{
    [SerializeField] public NetworkManager _manager;

    // Start is called before the first frame update
    public void ChangeScene(string sceneName)
    {
        if (IsServer)
        {
            _manager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

}
