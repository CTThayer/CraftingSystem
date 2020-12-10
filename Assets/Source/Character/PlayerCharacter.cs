using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Actors.AnimationControllers;

public class PlayerCharacter : MonoBehaviour
{
    // World Interaction Controller
    [SerializeField] private InteractionController _interactionController;
    public InteractionController interactionController { get => _interactionController; }

    // Character Movement and Camera,
    [SerializeField] private MotionController characterMotionController;
    [SerializeField] private GameObject _playerCamObj;
    public GameObject playerCamObj { get => _playerCamObj; }

    // Character Panel (Inventory & Equipment)
    [SerializeField] private Storage _inventory;
    public Storage inventory { get => _inventory; }

    [SerializeField] private EquipmentPanel _equipmentPanel;
    public EquipmentPanel equipmentPanel { get => _equipmentPanel; }

    [SerializeField] private EquipmentMenuController _equipmentMenu;

    // Character HUD Canvas
    [SerializeField] private Canvas playerHUD;

    // TODO: Add character attributes here (i.e. stats, skills, etc)
    // TODO: Add character resources manager here (health, stamina, mana, etc.)

    // Private Variables
    private bool _isInputActive;
    public bool isInputActive { get => _isInputActive; }

    // Start is called before the first frame update
    void Start()
    {
        if (_interactionController == null)
            _interactionController = GetComponent<InteractionController>();
        if (characterMotionController == null)
            characterMotionController = GetComponent<MotionController>();
        if (_playerCamObj == null)
            _playerCamObj = _interactionController.characterCamera.gameObject;
        if (_equipmentMenu == null)
            _equipmentMenu = GetComponent<EquipmentMenuController>();

        Debug.Assert(characterMotionController != null);
        Debug.Assert(_interactionController != null);
        Debug.Assert(_playerCamObj != null);
        Debug.Assert(_equipmentMenu != null);

        _isInputActive = true;
    }


    public void DeactivateCharacterInput()
    {
        characterMotionController.InputSource.IsEnabled = false;
        _interactionController.isInteractionActive = false;
        _isInputActive = false;
    }

    public void ReactivateCharacterInput()
    {
        characterMotionController.InputSource.IsEnabled = true;
        _interactionController.isInteractionActive = true;
        _isInputActive = true;
    }

    public void DeactivateCharacterCamera()
    {
        _playerCamObj.GetComponentInChildren<Camera>().enabled = false;
        _playerCamObj.SetActive(false);
    }

    public void ReactivateCharacterCamera()
    {
        _playerCamObj.SetActive(true);
        _playerCamObj.GetComponentInChildren<Camera>().enabled = true;
    }

    public void DeactivateCharacterHUD()
    {
        playerHUD.enabled = false; ;
    }

    public void ReactivateCharacterHUD()
    {
        playerHUD.enabled = true;
    }

    public void DisableCameraController()
    {
        com.ootii.Cameras.CameraController cc = 
            playerCamObj.GetComponent<com.ootii.Cameras.CameraController>();
        cc.enabled = false;
    }

    public void EnableCameraController()
    {
        com.ootii.Cameras.CameraController cc =
            playerCamObj.GetComponent<com.ootii.Cameras.CameraController>();
        cc.enabled = true;
    }

    public void DeactivateEquipmentMenu()
    {
        _equipmentMenu.ignoreInput = true;
    }

    public void ReactivateEquipmentMenu()
    {
        _equipmentMenu.ignoreInput = false;
    }
}
