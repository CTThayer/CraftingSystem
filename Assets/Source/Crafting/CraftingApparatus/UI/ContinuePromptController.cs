using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinuePromptController : MonoBehaviour
{
    [SerializeField] private Button noButton;
    [SerializeField] private Button yesButton;

    public delegate void YesNoDelegate();
    YesNoDelegate noDelegate;
    YesNoDelegate yesDelegate;

    // Start is called before the first frame update
    void Start()
    {
        if (noButton == null)
            noButton = transform.Find("No_Button").GetComponent<Button>();
        if (yesButton == null)
            yesButton = transform.Find("Yes_Button").GetComponent<Button>();
        Debug.Assert(noButton != null);
        Debug.Assert(yesButton != null);

        noButton.onClick.AddListener(No);
        yesButton.onClick.AddListener(Yes);
    }

    public void Yes()
    {
        yesDelegate();
    }

    public void No()
    {
        noDelegate();
    }

    public void SetYesButtonCallback(YesNoDelegate yesCallback)
    {
        yesDelegate = yesCallback;
    }

    public void SetNoButtonCallback(YesNoDelegate noCallback)
    {
        noDelegate = noCallback;
    }

}
