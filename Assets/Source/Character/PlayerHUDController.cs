using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private Text currentObjectName;
    [SerializeField] private Text currentAction;
    [SerializeField] private Text actionOutput;

    public void OnFocusChange(Interactable focus)
    {
        if (focus != null)
        {
            currentObjectName.text = focus.name;
            currentAction.text = focus.GetCurrentActionName();
        }
        else
        {
            ClearSelectionHUD();
        }
    }

    public void OnActionSelectionChange(string actionName)
    {
        currentAction.text = actionName;
    }

    public void OnActionResponse(string actionResponse)
    {
        actionOutput.text = actionResponse;
    }

    public void ClearSelectionHUD()
    {
        currentObjectName.text = "";
        currentAction.text = "";
        actionOutput.text = "";
    }

}
