using Managers;
using TMPro;
using UnityEngine;

namespace UI{
	public class UIScoreCounter : MonoBehaviour{
		[SerializeField] private TextMeshProUGUI _scoreTmp;

		private float _score;

		private void Update(){
			if (GameManager.Paused) return;

			_score += TerrainManager.Instance.Speed * Time.deltaTime;
			WriteScore();
		}

		private void OnEnable(){
			_score = 0;
			WriteScore();
		}

		private void CarThrashed(){ }

		private void WriteScore(){
			_scoreTmp.text = Mathf.FloorToInt(_score).ToString().PadLeft(9, '0');
		}
	}
}