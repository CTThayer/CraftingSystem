using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate string ActionDelegate(GameObject caller);

public interface IActionable
{
    ActionDelegate[] GetActions();
    string[] GetActionNames();
}
