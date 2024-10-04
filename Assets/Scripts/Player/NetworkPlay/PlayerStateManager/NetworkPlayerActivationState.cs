using UnityEngine;

public class NetworkPlayerActivationState : NetworkPlayerState
{
    public NetworkPlayerActivationState(NetworkPlayerManager player) : base(player) { }

    public override void OnStateEnter()
    {
        ActivatePlayer();
        //_playerManager.ActivatePlayerServerRpc();
        Debug.Log("Player entering activation state");
    }

    public override void OnStateUpdate()
    {
        if (_playerManager.GetComponentInChildren<NetworkPlayerInfo>().enabled == true)
        {
            _playerManager.ChangeState(new NetworkPlayerInitiationState(_playerManager));
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Player leaving activation state");
    }

    private void ActivatePlayer()
    {
        Debug.Log("Client is here 5");
        GameObject playerMiniMapChild;
        GameObject playerChild;
        GameObject playerInfoChild;
        CharacterController controller;
        CapsuleCollider collider;
        NetworkPlayerInput playerInput;
        NetworkPlayerController playerController;
        NetworkPlayerMovement playerMovement;
        NetworkPlayerRotation playerRotation;
        NetworkPlayerShoot playerShoot;
        NetworkPlayerInfo playerInfo;
        LookAtCamera playerInfoRot;
        Health playerHealth;
        playerMiniMapChild = _playerManager.transform.GetChild(0).gameObject;
        playerChild = _playerManager.transform.GetChild(1).gameObject;
        playerInfoChild = _playerManager.transform.GetChild(2).gameObject;
        if (_playerManager.CheckOwnership())
        {
            Camera.main.gameObject.SetActive(false);
            playerChild.GetComponentInChildren<Camera>().enabled = true;
            playerChild.GetComponentInChildren<Camera>().tag = "MainCamera";
            playerMiniMapChild.GetComponentInChildren<Camera>().enabled = true;
            playerMiniMapChild.GetComponent<NetworkMiniCameraController>().enabled = true;
            _playerManager.GetComponentInChildren<AudioListener>().enabled = true;
            playerInput = playerChild.GetComponent<NetworkPlayerInput>();
            playerInput.enabled = true;
            playerInput._isDiabled = false;
            playerController = playerChild.GetComponent<NetworkPlayerController>();
            playerController.enabled = true;

            playerShoot = playerChild.GetComponent<NetworkPlayerShoot>();
            playerShoot.enabled = true;
            playerHealth = playerChild.GetComponent<Health>();
            playerHealth.enabled = true;


        }

        playerInfo = playerInfoChild.GetComponent<NetworkPlayerInfo>();
        playerInfo.enabled = true;
        playerInfoRot = playerInfoChild.GetComponent<LookAtCamera>();
        playerInfoRot.enabled = true;

        controller = playerChild.GetComponent<CharacterController>();
        controller.enabled = true;
        collider = playerChild.GetComponent<CapsuleCollider>();
        collider.enabled = true;
        playerMovement = playerChild.GetComponent<NetworkPlayerMovement>();
        playerMovement.enabled = true;
        playerRotation = playerChild.GetComponent<NetworkPlayerRotation>();
        playerRotation.enabled = true;

        _playerManager.AddPlayer();
    }
}
