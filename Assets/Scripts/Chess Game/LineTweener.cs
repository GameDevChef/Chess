using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] private float movementSpeed;
    public void MoveTo(Transform transform, Vector3 targetPosition)
    {
        StartCoroutine(MoveToCoroutine(transform, targetPosition));
    }

    private IEnumerator MoveToCoroutine(Transform transform, Vector3 targetPosition)
    {
        while (Vector3.Distance(targetPosition, transform.position) >= float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);
            Debug.LogError(" CO");
            yield return null;
        }
        Debug.LogError("Ended CO");
    }
}
