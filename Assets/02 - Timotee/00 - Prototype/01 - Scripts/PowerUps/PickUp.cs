using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace PowerUps{
	[Serializable]
	public class PickUp : MonoBehaviour{
		[SerializeField] protected string _name;
		[SerializeField] protected PowerUp _powerUp;

		public virtual void Activate(PlayerCar player){
			
		}
	}
}