using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace PowerUps{
	[Serializable]
	public class PickUp : MonoBehaviour{
		[SerializeField] protected string _name;
		[SerializeField] protected Image _sprite;
		
		[SerializeField] protected float _powerUpTime;

		public virtual void Activate(PlayerCar player){
			
		}
	}
}