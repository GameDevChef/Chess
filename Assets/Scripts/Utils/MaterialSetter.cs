using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialSetter : MonoBehaviour
{
	[SerializeField] private MeshRenderer _meshRenderer;
	private MeshRenderer meshRenderer
	{
		get
		{
			if (_meshRenderer == null)
				_meshRenderer = GetComponent<MeshRenderer>();
			return _meshRenderer;
		}
	}

	internal void SetSingleMaterial(Material selectedMaterial)
	{
		meshRenderer.material = selectedMaterial;
	}
}
