using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitApplication()
    {
        //if (Application.isEditor)
        //    UnityEditor.EditorApplication.isPlaying = false;
        //else
            Application.Quit();
    }
}
