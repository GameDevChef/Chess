using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInputHandler : MonoBehaviour, IInputHandler
{
    [SerializeField] private AudioClip clip;
    private AudioSource source;
    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action onClick)
    {
        throw new NotImplementedException();
    }
}
