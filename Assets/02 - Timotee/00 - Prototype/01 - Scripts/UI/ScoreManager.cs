using Managers;
using UnityEngine;

namespace UI{
	public class ScoreManager : MonoBehaviour{
		public static ScoreManager Instance;

		public static float Score{
			get=> Instance._score;
			private set => Instance._score = value;
		}
		private float _score;

		public static float RawScore{
			get => Instance._rawScore;
			private set => Instance._rawScore = value;
		}
		private float _rawScore;

		public static int Coins{
			get => Instance._coins;
			set => Instance._coins = value;
		}
		private int _coins;
		
		public static int Crashes{
			get => Instance._crashes;
			set => Instance._crashes = value;
		}
		private int _crashes;

		private void Awake(){
			if (Instance!=null) return;
			
			Instance = this;
			GameManager.OnRestart += ResetScore;
		}

		public static void AddScore(float score){
			Score += score;
		}

		public static void ResetScore(){
			Score = 0;
			RawScore = 0;
			Crashes = 0;
			Coins = 0;
		}
		
		private void Update(){
			if (GameManager.Paused || !GameManager.GameRunning) return;

			_rawScore += TerrainManager.Instance.Speed * Time.deltaTime;
			_score += TerrainManager.Instance.Speed * Time.deltaTime;
		}
	}
}