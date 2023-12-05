using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerManager;
    private StageController stageController;
    private GameObject canvas;

    private static readonly int[] slidersPosx = new int[] { 960, 500, 350, 300 };
    private static readonly int[] slidersGridSpace = new int[] { 300, 800, 500, 330 };

    public List<PlayerInput> playerList = new();
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    [SerializeField] InputAction joinAction;
    [SerializeField] InputAction leaveAction;

    private void Awake() {
        playerManager = GetComponent<PlayerInputManager>();
        stageController = GetComponent<StageController>();
        canvas = GameObject.Find("Canvas");

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
        // modify sliders layout when adding player
        GameObject sliders = canvas.transform.Find("SettingMenu").Find("Sliders").gameObject;
        int n = playerList.Count;
        Vector3 pos = sliders.GetComponent<RectTransform>().position;
        pos.x = slidersPosx[n - 1];
        sliders.GetComponent<RectTransform>().position = pos;
        Vector2 spacing = sliders.GetComponent<GridLayoutGroup>().spacing;
        spacing.x = slidersGridSpace[n - 1];
        sliders.GetComponent<GridLayoutGroup>().spacing = spacing;
        // attach new slider to sliders
        GameObject newSlider = Instantiate(Resources.Load<GameObject>("Canvas/Slider"));
        newSlider.transform.parent = sliders.transform;
        newSlider.GetComponent<CameraSlider>().SetCamera(playerInput.gameObject);
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

    public void Unregisterplayer(PlayerInput playerInput) {
        playerList.Remove(playerInput);
        stageController.playerObjects.Remove(playerInput.gameObject);
        CameraMovement virtualCamera = playerInput.gameObject.transform.Find("Camera").GetComponent<CameraMovement>();
        if (virtualCamera.transparentObject != null) {
            Destroy(virtualCamera.transparentObject);
        }
        virtualCamera.Disable();
        virtualCamera.enabled = false;
        playerInput.gameObject.transform.Find("Camera").GetComponent<MouseControlFollowCamera>().enabled = false;
        playerInput.gameObject.GetComponent<Player>().Disable(Player.State.STOP);
        Destroy(playerInput.gameObject);
    }

    public void DisableJoinAction() {
        joinAction.Disable();
    }

    public void EnableJoinAction() {
        joinAction.Enable();
    }
}
