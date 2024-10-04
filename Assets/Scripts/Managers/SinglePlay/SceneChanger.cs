using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class SceneChanger : MonoBehaviour
{
    // Function to load a scene by name
    public void ChangeScene(string sceneName)
    {
        Debug.Log("hello");
        SceneManager.LoadScene(sceneName);
    }

    // Function to load a scene by index
    public void ChangeScene(int sceneIndex)
    {
        Debug.Log("hello");
        SceneManager.LoadScene(sceneIndex);
    }

    public void Restart()
    {
        AuthenticationService.Instance.SignOut(true);
        var foundManagerObjects = FindObjectsOfType<GameNetworkManager>();
        foreach (var item in foundManagerObjects)
        {
            item.DestroyObject();
            Destroy(item);
        }
        SceneManager.LoadScene("StartScene");
    }
}
