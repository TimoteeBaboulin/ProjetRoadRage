using System.Collections;
using DG.Tweening;
using Managers;
using Player;
using UnityEngine;

namespace Obstacles{
	public class PoliceCar : MonoBehaviour{
		[SerializeField] private float _closeDistance;
		[SerializeField] private float _farDistance;

		[SerializeField] private AnimationCurve _dangerDistanceCurve;
		private float _countdown;
		private PlayerCar _player;

		private void Start(){
			_player = PlayerCar.Player;

			var position = transform.position;
			position.z = _farDistance;
			transform.position = position;
		}

		private void OnEnable(){
			PlayerCar.OnLineChange += ChangeLane;
			PlayerCar.OnSideContact += SideContact;

			GameManager.OnRestart += ResetLane;
		}

		private void OnDisable(){
			PlayerCar.OnLineChange -= ChangeLane;
			PlayerCar.OnSideContact -= SideContact;

			GameManager.OnRestart -= ResetLane;
		}

		private void ResetLane(){
			_countdown = 0;
			transform.DOMoveX(0, 0.001f);
		}

		private void SideContact(bool arrested){
			if (!arrested) GetCloser();
		}

		private void GetCloser(){
			if (_countdown > 0)
				_countdown = PoliceManager.DangerTime;
			else
				StartCoroutine(GetCloserCoroutine());
		}

		private IEnumerator GetCloserCoroutine(){
			var time = PoliceManager.DangerTime;
			_countdown = time;
			Vector3 position;

			while(_countdown > 0){
				yield return null;
				if (GameManager.Paused || !GameManager.GameRunning) continue;
				_countdown -= Time.deltaTime;

				position = transform.position;
				position.z = Mathf.Lerp(_farDistance, _closeDistance,
					PoliceManager.DangerCurve.Evaluate(1 - _countdown / time));
				transform.position = position;
			}

			_countdown = 0;
			position = transform.position;
			position.z = _farDistance;
			transform.position = position;
		}

		private void ChangeLane(int lane){
			var time = Mathf.Abs(_player.transform.position.z - transform.position.z) / TerrainManager.Instance.Speed;
			time -= PlayerCar.LaneChangeTime;

			StartCoroutine(ChangeLaneCoroutine(time, lane));
		}

		private IEnumerator ChangeLaneCoroutine(float time, int lane){
			var countdown = time;

			while(countdown > 0){
				yield return null;
				if (GameManager.Paused || !GameManager.GameRunning) continue;

				countdown -= Time.deltaTime;
			}

			transform.DOMoveX(-3 + lane * 3, PlayerCar.LaneChangeTime);
		}
	}
}