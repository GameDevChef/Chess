using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
	[SerializeField] private Material freeSquareMaterial;
	[SerializeField] private Material enemySquareMaterial;
	[SerializeField] private GameObject selectorPrefab;
	private List<GameObject> instantiatedSelectors = new List<GameObject>();

	public void ShowSelection(Dictionary<Vector3, bool> squareData)
	{
		ClearSelection();
		foreach (var data in squareData)
		{
			GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity);
			instantiatedSelectors.Add(selector);
			foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
			{
				setter.SetSingleMaterial(data.Value ? freeSquareMaterial : enemySquareMaterial);
			}
		}
	}

	public void ClearSelection()
	{
		for (int i = 0; i < instantiatedSelectors.Count; i++)
		{
			Destroy(instantiatedSelectors[i]);
		}
	}
}