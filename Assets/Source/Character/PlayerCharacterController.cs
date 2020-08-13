using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    /* Data Members */
    [SerializeField] private Camera Cam;

    GameObject CurrentHit;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(Cam != null);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }

    private void CheckForInteractables()
    {
        RaycastHit Hit;
        // NOTE: Check if transform.forward is actually what we want, this might need to be offset by some height value.
        if (Physics.Raycast(this.transform.position, Cam.transform.forward, out Hit, 1.5f)) 
        {
            if (Hit.transform.gameObject != CurrentHit)
            {
                // Update previous object that was hit to switch off any active functionality
                if (CurrentHit != null && CurrentHit.GetComponent<IInteractable>() != null)
                {
                    CurrentHit.GetComponent<IInteractable>().OnHoverEnd();
                }

                // Update CurrentHit to refer to current hit
                CurrentHit = Hit.transform.gameObject;

                // If the latest hit is an interactable, call OnHover() and update UI
                if (CurrentHit != null && CurrentHit.GetComponent<IInteractable>() != null)
                {
                    string[] ActionNames = CurrentHit.GetComponent<IInteractable>().OnHover();
                    // TODO: Update UI elements
                }
            }
        }
    }

    private void HandleInteractionInput()
    {

    }

}
