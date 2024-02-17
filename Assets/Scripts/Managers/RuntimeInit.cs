using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RuntimeInit
{
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		GameManager gameManager = Resources.Load<GameManager>("Game Manager");
		gameManager = Object.Instantiate(gameManager);
		Object.DontDestroyOnLoad(gameManager.gameObject);
	}
}
