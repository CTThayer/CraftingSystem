using UnityEngine;

public class InventoryInput : MonoBehaviour
{
    [SerializeField] GameObject characterPanelGameObject;
    [SerializeField] GameObject equipmentPanelGameObject;
    [SerializeField] KeyCode[] toggleInventoryKeys;
    [SerializeField] KeyCode toggleEquipmentPanelKey;

    // Update is called once per frame
    void Update()
    {
        // Original Code from Kryzarel
        for (int i = 0; i < toggleInventoryKeys.Length; i++)
        {
            if (Input.GetKeyDown(toggleInventoryKeys[i]))
            {
                characterPanelGameObject.SetActive(!characterPanelGameObject.activeSelf);
                if (characterPanelGameObject.activeSelf)
                    ShowMouseCursor();
                else
                    HideMouseCursor();
                break;
            }
        }

        // Additionally, handle Escape to exit inventory
        if (characterPanelGameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            characterPanelGameObject.SetActive(false);
            HideMouseCursor();
        }

        if (Input.GetKeyDown(toggleEquipmentPanelKey))
        {
            if (!characterPanelGameObject.activeSelf)
            {
                characterPanelGameObject.SetActive(true);
                equipmentPanelGameObject.SetActive(true);
                ShowMouseCursor();
            }
            else
            {
                ToggleEquipmentPanel();
            }
        }
    }

    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleEquipmentPanel()
    {
        equipmentPanelGameObject.SetActive(!equipmentPanelGameObject.activeSelf);
    }

}
