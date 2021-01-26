using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenuController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private Canvas playerEquipmentCanvas;

    [SerializeField] private StorageUI _inventoryPanel;
    public StorageUI inventoryPanel { get => _inventoryPanel; }

    [SerializeField] private EquipmentPanel _equipmentPanel;
    public EquipmentPanel equipmentPanel { get => _equipmentPanel; }

    private bool _isEquipmentMenuOpen;

    public bool ignoreInput;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(playerCharacter != null);
        Debug.Assert(playerEquipmentCanvas != null);
        ignoreInput = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ignoreInput == false)
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                if (!_isEquipmentMenuOpen)
                    ActivateEquipmentMenu();
                else
                    DeactivateEquipmentMenu();
                return;
            }

            else if (Input.GetKeyUp(KeyCode.Escape) && _isEquipmentMenuOpen)
            {
                DeactivateEquipmentMenu();
                return;
            }
        }
    }

    public void ActivateEquipmentMenu()
    {
        playerCharacter.DeactivateCharacterInput();
        playerCharacter.DeactivateCharacterHUD();
        playerCharacter.DisableCameraController();
        playerEquipmentCanvas.gameObject.SetActive(true);
        inventoryPanel.SetStorage(playerCharacter.inventory);
        _isEquipmentMenuOpen = true;
    }

    public void DeactivateEquipmentMenu()
    {
        playerCharacter.ReactivateCharacterInput();
        playerCharacter.ReactivateCharacterHUD();
        playerCharacter.EnableCameraController();
        playerEquipmentCanvas.gameObject.SetActive(false);
        _isEquipmentMenuOpen = false;
    }

}
