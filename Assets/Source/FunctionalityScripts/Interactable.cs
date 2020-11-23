using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string ObjectName;
    [SerializeField] private int ActionIndex;
    [SerializeField] private int DefaultIndex;
    ActionDelegate[] Actions;
    string[] ActionNames;

    // Start is called before the first frame update
    void Start()
    {
        if (ObjectName == null || ObjectName.Length < 1 || ObjectName == " ")
            ObjectName = this.gameObject.name;

        Debug.Assert(ActionIndex >= 0 && ActionIndex < Actions.Length);
        if (ActionIndex < 0 || ActionIndex > Actions.Length)
            ActionIndex = 0;

        Debug.Assert(DefaultIndex >= 0 && DefaultIndex < Actions.Length);
        if (DefaultIndex < 0 || DefaultIndex > Actions.Length)
            DefaultIndex = 0;
    }

    public void Initialize()
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
    public string ChangeActionSelection(int i)
    {
        ActionIndex = (ActionIndex + i < Actions.Length) ? ActionIndex + i : 0;
        return ActionNames[ActionIndex];
    }

    public string Interact()
    {
        return Actions[ActionIndex](this.gameObject);
    }

    public string OnHover()
    {
        // TODO: Apply hover highlighting or other indication

        return ObjectName;
    }

    public void OnHoverExit()
    {
        // TODO: Remove hover highlighting or other indication

        ActionIndex = DefaultIndex;     // Reset to default action index

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
        Actions = actionsList.ToArray();
        ActionNames = actionNamesList.ToArray();
    }
}
