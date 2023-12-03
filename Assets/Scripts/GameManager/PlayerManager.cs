using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerManager;
    private StageController stageController;

    public List<PlayerInput> playerList = new();
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    [SerializeField] InputAction joinAction;
    [SerializeField] InputAction leaveAction;

    private void Awake() {
        playerManager = GetComponent<PlayerInputManager>();
        stageController = GetComponent<StageController>();

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    void Start() {

    }

    void Update() {

    }

    void OnPlayerJoined(PlayerInput playerInput) {
        playerList.Add(playerInput);
        PlayerJoinedGame?.Invoke(playerInput);
        stageController.playerObjects.Add(playerInput.gameObject);
    }

    void OnPlayerLeft(PlayerInput playerInput) {

    }

    void JoinAction(InputAction.CallbackContext context) {
        playerManager.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    void LeaveAction(InputAction.CallbackContext context) {
        if (playerList.Count > 1) {
            foreach (var player in playerList) {
                foreach (var device in player.devices) {
                    if (device != null && context.control.device == device) {
                        Unregisterplayer(player);
                        return;
                    }
                }
            }
        }
    }

    void Unregisterplayer(PlayerInput playerInput) {
        playerList.Remove(playerInput);
        stageController.playerObjects.Remove(playerInput.gameObject);
        CameraMovement virtualCamera = playerInput.gameObject.transform.Find("Camera").GetComponent<CameraMovement>();
        if (virtualCamera.transparentObject != null) {
            Destroy(virtualCamera.transparentObject);
        }
        virtualCamera.Disable();
        virtualCamera.enabled = false;
        playerInput.gameObject.transform.Find("Camera").GetComponent<MouseControlFollowCamera>().enabled = false;
        Destroy(playerInput.gameObject);
    }

    public void DisableJoinAction() {
        joinAction.Disable();
    }

    public void EnableJoinAction() {
        joinAction.Enable();
    }
}
