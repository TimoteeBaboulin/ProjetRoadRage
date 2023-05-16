using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Managers;
using Player;
using PowerUps;
using UnityEngine;

namespace UI{
	public class UIPowerUpSlotManager : MonoBehaviour{
		public static UIPowerUpSlotManager Instance;

		[SerializeField] private UIPowerUpSlot[] _slots;

		private Dictionary<PowerUp, UIPowerUpSlot> _activeSlots;

		private void Awake(){
			if (Instance!=null){
				Destroy(gameObject);
				return;
			}

			Instance = this;
			_activeSlots = new Dictionary<PowerUp, UIPowerUpSlot>();
		}

		private void OnEnable(){
			PlayerCar.OnPowerUp += PlayPowerUp;

			GameManager.OnRestart += DisableSlots;
		}

		private void OnDisable(){
			PlayerCar.OnPowerUp -= PlayPowerUp;

			GameManager.OnRestart -= DisableSlots;
		}

		public void SlotDeactivate(UIPowerUpSlot slot){
			if (!_activeSlots.ContainsValue(slot))
				// throw new IndexOutOfRangeException("Can't find slot in Active list.");
				return;

			if (!_activeSlots.TryGetValue(slot.PowerUp, out var activeSlot) || slot!=activeSlot)
				throw new InvalidDataException("Data seems to have been corrupted");

			_activeSlots.Remove(slot.PowerUp);
		}

		private void DisableSlots(){
			foreach(var slot in _activeSlots.Values) slot.gameObject.SetActive(false);

			_activeSlots.Clear();
		}

		private void PlayPowerUp(PowerUp powerUp){
			if (_activeSlots.TryGetValue(powerUp, out var slot)){
				slot.Refresh();
				return;
			}

			slot = _slots.First(obj => !obj.gameObject.activeSelf);
			if (slot==null)
				throw new NullReferenceException("No slot found");

			slot.PowerUp = powerUp;
			slot.gameObject.SetActive(true);
			_activeSlots.Add(powerUp, slot);
		}
	}
}