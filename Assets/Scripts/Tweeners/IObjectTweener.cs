using UnityEngine;

internal interface IObjectTweener
{
	void MoveTo(Transform transform, Vector3 targetPosition);
}