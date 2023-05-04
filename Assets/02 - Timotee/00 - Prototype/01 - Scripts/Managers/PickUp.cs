using System;
using UnityEngine;

[Serializable]
public class PickUp : MonoBehaviour{
	[SerializeField] protected float _powerUpTime;
	
	public virtual void Activate(PlayerCar player){ }
}