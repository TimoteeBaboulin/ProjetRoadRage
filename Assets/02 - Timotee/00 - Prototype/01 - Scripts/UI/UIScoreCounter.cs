using TMPro;
using UnityEngine;

namespace UI{
	public class UIScoreCounter : MonoBehaviour{
		[SerializeField] private TextMeshProUGUI _scoreTmp;

		private void Update(){
			WriteScore();
		}

		private void WriteScore(){
			_scoreTmp.text = Mathf.FloorToInt(ScoreManager.Score).ToString().PadLeft(9, '0');
		}
	}
}