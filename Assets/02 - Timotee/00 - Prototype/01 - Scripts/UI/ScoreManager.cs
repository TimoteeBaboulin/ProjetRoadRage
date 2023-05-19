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
		}
		
		private void Update(){
			if (GameManager.Paused || !GameManager.GameRunning) return;

			_score += TerrainManager.Instance.Speed * Time.deltaTime;
		}
	}
}