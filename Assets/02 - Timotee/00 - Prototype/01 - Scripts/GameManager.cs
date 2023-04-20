using System;
using UnityEngine;

public class GameManager : MonoBehaviour{
	public static GameManager Instance;

#region Events
	public static event Action<bool> OnPause;
	public static event Action OnGameLost;
	public static event Action OnRestart;
#endregion

	public static bool Paused => Instance._paused;
	private bool _paused;

	private void Awake(){
		if (Instance != null) Destroy(gameObject);
		else Instance = this;
	}

	public static void StartPause(){
		Instance._paused = true;
		OnPause?.Invoke(Paused);
	}

	public static void StopPause(){
		Instance._paused = false;
		OnPause?.Invoke(Paused);
	}

	public static void LoseGame(){
		OnGameLost?.Invoke();
		StartPause();
	}

	public static void RestartGame(){
		OnRestart?.Invoke();
	}
}