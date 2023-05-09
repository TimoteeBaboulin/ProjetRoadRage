using System.Collections;
using Managers;
using PowerUps;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
	public class UIPowerUpSlot : MonoBehaviour{
		public PowerUp PowerUp{
			get => _powerUp;
			set{
				_powerUpIcon.sprite = value.Icon;
				_countdownTimer = value.Time;
				_powerUp = value;
			}
		}
		private PowerUp _powerUp;

		[SerializeField] private Image _powerUpIcon;
		[SerializeField] private Image _maskFill;

		[SerializeField] private float _countdownTimer;

		public void Refresh(){
			_countdownTimer = PowerUp.Time;
		}
		
		private void OnEnable(){
			StartCoroutine(BarCoroutine());
		}

		private IEnumerator BarCoroutine(){
			while(_countdownTimer > 0){
				yield return null;
				if (!GameManager.Paused)
					_countdownTimer -= Time.deltaTime;
				_maskFill.fillAmount = _countdownTimer / _powerUp.Time;
			}
			
			gameObject.SetActive(false);
			UIPowerUpSlotManager.Instance.SlotDeactivate(this);
		}

		private void OnDisable(){
			StopAllCoroutines();
		}
	}
}