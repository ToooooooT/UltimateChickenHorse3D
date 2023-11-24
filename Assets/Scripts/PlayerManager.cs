using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerManager;
    public List<PlayerInput> playerList = new List<PlayerInput>();
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    [SerializeField] InputAction joinAction;
    [SerializeField] InputAction leaveAction;

    private void Awake() {
        playerManager = GetComponent<PlayerInputManager>();

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    void Start() {
        // playerManager.JoinPlayer(0, -1, null);   
    }

    void Update() {
    }

    void OnPlayerJoined(PlayerInput playerInput) {
        Debug.Log("Player joined");
        playerList.Add(playerInput);
        if (PlayerJoinedGame != null) {
            PlayerJoinedGame(playerInput);
        }
    }

    void OnPlayerLeft(PlayerInput playerInput) {

    }

    void JoinAction(InputAction.CallbackContext context) {
        playerManager.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    void LeaveAction(InputAction.CallbackContext context) {

    }
}
