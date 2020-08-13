using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string[] OnHover();
    void OnHoverEnd();
    bool Interact(int action);
}
