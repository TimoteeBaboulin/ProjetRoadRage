using System;
using UnityEngine;

namespace Managers{
	public class GameManager : MonoBehaviour{
		public static GameManager Instance;
		private bool _gameRunning;
		private bool _paused;

		public static bool Paused => Instance._paused;

		public static bool GameRunning => Instance._gameRunning;

		private void Awake(){
			if (Instance!=null){
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
			InputManager.Instance.gameObject.SetActive(false);
			OnGameLost?.Invoke();
			StartPause();
		}

		public static void RestartGame(){
			OnRestart?.Invoke();
			Instance._gameRunning = false;
			StopPause();
		}

		public static void StartGame(){
			InputManager.Instance.gameObject.SetActive(true);

			Instance._gameRunning = true;
			OnGameStart?.Invoke();
		}

	#region Events
		public static event Action<bool> OnPause;
		public static event Action OnGameLost;
		public static event Action OnRestart;
		public static event Action OnGameStart;
	#endregion
	}
}