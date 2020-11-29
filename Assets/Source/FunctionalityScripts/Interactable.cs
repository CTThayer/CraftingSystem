using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private int actionIndex;
    [SerializeField] private int defaultIndex;

    // For Debug purposes, setting these to be serialized. They can be non-serialized in build
    private ActionDelegate[] actions;
    [SerializeField] private string[] actionNames;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(actionIndex >= 0 && actionIndex < actions.Length);
        if (actionIndex < 0 || actionIndex > actions.Length)
            actionIndex = 0;

        Debug.Assert(defaultIndex >= 0 && defaultIndex < actions.Length);
        if (defaultIndex < 0 || defaultIndex > actions.Length)
            defaultIndex = 0;
    }

    public void Initialize()
    {
        ConfigureActions();
    }

    void OnValidate()
    {
        ConfigureActions();
    }

    /* Change Action Selection
     * Used to update the ActionIndex based on user input.
     * @Param i = an int for how many indices to increase the ActionIndex by.
     * Note: the input handler code needs to format any input into a valid int
     * from whatever format (likely float) that the input comes in as.
     * Returns: a string to be displayed in the UI for the name of the action.
     */
    public string ChangeActionSelectionBy(int i)
    {
        actionIndex = (actionIndex + i < actions.Length) ? actionIndex + i : 0;
        return actionNames[actionIndex];
    }

    public string GetCurrentActionName()
    {
        return actionNames[actionIndex];
    }

    public string[] GetAllActionNames()
    {
        return actionNames;
    }

    public string Interact(PlayerCharacter character)
    {
        return actions[actionIndex](character);
    }

    public string OnHoverEnter()
    {
        // TODO: Apply hover highlighting or other indication

        return this.name;
    }

    public void OnHoverExit()
    {
        // TODO: Remove hover highlighting or other indication

        actionIndex = defaultIndex;     // Reset to default action index

    }

    public void ConfigureActions()
    {
        List<ActionDelegate> actionsList = new List<ActionDelegate>();
        List<string> actionNamesList = new List<string>();
        foreach (Component component in GetComponents<Component>())
        {
            IActionable actionComponent = component as IActionable;
            if (actionComponent != null)
            {
                ActionDelegate[] actions = actionComponent.GetActions();
                string[] actionNames = actionComponent.GetActionNames();
                Debug.Assert(actions.Length == actionNames.Length);
                actionsList.AddRange(actions);
                actionNamesList.AddRange(actionNames);
            }
        }
        actions = actionsList.ToArray();
        actionNames = actionNamesList.ToArray();
    }
}
