using System;
using Managers;
using Player;
using UI;
using UnityEngine;

namespace PowerUps{
	public class Coin : PickUp{
		[SerializeField] private float _baseScoreValue;
		[SerializeField] private GameObject _coinModel;

		public override void Activate(PlayerCar player){
			ScoreManager.AddScore(_baseScoreValue * (1 + TerrainManager.SpeedIncrease));
			player.PlayCoinParticles();
			_coinModel.SetActive(false);
		}

		private void OnEnable(){
			_coinModel.SetActive(true);
		}
	}
}