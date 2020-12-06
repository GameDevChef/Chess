using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessUIManager : MonoBehaviour
{
	[SerializeField] private GameObject UIParent;
	[SerializeField] private Button restartButton;
	[SerializeField] private Text finishText;

	internal void HideUI()
	{
		UIParent.SetActive(false);
	}

	internal void OnGameFinished(string winner)
	{
		UIParent.SetActive(true);
		finishText.text = string.Format("{0} won", winner);
	}
}
