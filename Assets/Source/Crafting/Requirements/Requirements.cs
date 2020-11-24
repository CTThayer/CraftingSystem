using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Requirements : MonoBehaviour
{
    public abstract bool ValidateConfiguration(out string output);
}
