﻿using Player;
using UnityEngine;

namespace PowerUps{
	public class SnowPlowPickUp : PickUp{
		[SerializeField] private GameObject _snowplowPrefab;

		public override void Activate(PlayerCar player){
			Destroy(gameObject);
			player.SpawnItemAtFront(_snowplowPrefab, _powerUpTime);
		}
	}
}