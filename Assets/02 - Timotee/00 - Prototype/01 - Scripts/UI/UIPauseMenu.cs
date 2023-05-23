using TMPro;
using UI;
using UnityEngine;

public class UIPauseMenu : MonoBehaviour{
	[SerializeField] private TextMeshProUGUI _score;
	[SerializeField] private TextMeshProUGUI _coins;
	[SerializeField] private TextMeshProUGUI _crashes;
	
	private void OnEnable(){
		_score.text = Mathf.FloorToInt(ScoreManager.Score).ToString();
		_coins.text = ScoreManager.Coins.ToString();
		_crashes.text = ScoreManager.Crashes.ToString();
	}
}