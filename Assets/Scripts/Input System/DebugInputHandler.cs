using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInputHandler : MonoBehaviour, IInputHandler
{
    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action onClick)
    {
        Debug.Log(string.Format("Clicked object {0} in position {1} with callback {2}", 
            selectedObject != null ? selectedObject.name.ToString() : "null",
            inputPosition, 
            (onClick !=null)));
    }  
}