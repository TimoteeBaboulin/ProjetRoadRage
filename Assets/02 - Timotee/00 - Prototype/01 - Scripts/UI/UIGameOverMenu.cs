using System;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;

public class UIGameOverMenu : MonoBehaviour{
	[SerializeField] private float _scoreSpeed;

	[SerializeField] private TextMeshProUGUI _scoreTMP;
	[SerializeField] private TextMeshProUGUI _coinsTMP;
	[SerializeField] private TextMeshProUGUI _crashesTMP;

	private void OnEnable(){
		StartCoroutine(LoadScore());
	}

	private IEnumerator LoadScore(){
		_scoreTMP.text = "0";
		_coinsTMP.text = "";
		_crashesTMP.text = "";

		int scoreGoal = Mathf.FloorToInt(ScoreManager.RawScore);
		float score = 0;
		
		while(score < scoreGoal){
			_scoreTMP.text = Mathf.FloorToInt(score).ToString();
			yield return null;

			score += Time.deltaTime * _scoreSpeed;
		}

		_scoreTMP.text = scoreGoal.ToString();
		score = scoreGoal;

		yield return new WaitForSeconds(.1f);
		int coins = ScoreManager.Coins;
		_coinsTMP.text = coins.ToString();
		yield return new WaitForSeconds(.1f);

		scoreGoal += ScoreManager.Coins * 100;
		while(score < scoreGoal){
			_scoreTMP.text = Mathf.FloorToInt(score).ToString();
			yield return null;

			score += Time.deltaTime * _scoreSpeed;
		}
		
		_scoreTMP.text = scoreGoal.ToString();
		score = scoreGoal;

		yield return new WaitForSeconds(.1f);
		coins = ScoreManager.Crashes;
		_crashesTMP.text = coins.ToString();
		yield return new WaitForSeconds(.1f);

		scoreGoal += ScoreManager.Crashes * 400;
		while(score < scoreGoal){
			_scoreTMP.text = Mathf.FloorToInt(score).ToString();
			yield return null;

			score += Time.deltaTime * _scoreSpeed;
		}
		
		_scoreTMP.text = scoreGoal.ToString();
	}
}