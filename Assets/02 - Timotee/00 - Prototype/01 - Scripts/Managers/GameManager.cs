using System;
using UnityEngine;

public class GameManager : MonoBehaviour{
	public static GameManager Instance;

#region Events
	public static event Action<bool> OnPause;
	public static event Action OnGameLost;
	public static event Action OnRestart;
	public static event Action OnGameStart;
#endregion

	public static bool Paused => Instance._paused;
	private bool _paused;

	public static bool GameRunning => Instance._gameRunning;
	private bool _gameRunning;

	private void Awake(){
		if (Instance != null){
			Destroy(gameObject);
			return;
		}
		Instance = this;
		_gameRunning = false;
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
		StopPause();
	}

	public static void StartGame(){
		Instance._gameRunning = true;
		OnGameStart?.Invoke();
	}
}