using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Managers{
	public class CameraManager : MonoBehaviour{
		public static CameraManager Instance;

		[SerializeField] private AnimationCurve _fovCurve;
		[SerializeField] private float _baseFOV;
		[SerializeField] private float _maxFOV;
		private Animator _animator;

		private Vector3 _basePosition;
		private Quaternion _baseRotation;

		private Camera _camera;
		private float _fovCountdown;

		private Vector3 _menuPosition;
		private Quaternion _menuRotation;

		private Transform _transform;
		public static Camera Camera => Instance._camera;

		private void Awake(){
			if (Instance!=null) return;
			Instance = this;
			_fovCountdown = 0;
			_camera = GetComponent<Camera>();
			_transform = transform;
			_menuPosition = _transform.position;
			_menuRotation = _transform.rotation;
		}

		public void Reset(){
			_transform.position = _menuPosition;
			_transform.rotation = _menuRotation;
			_fovCountdown = 0;
			StopAllCoroutines();
			_camera.fieldOfView = _baseFOV;
		}

		private void OnEnable(){
			GameManager.OnRestart += Reset;
		}

		private void OnDisable(){
			GameManager.OnRestart -= Reset;
		}

		public void SetCameraBaseTransform(){
			_basePosition = _transform.position;
			_baseRotation = _transform.rotation;
		}

		public static void ShakeCamera(float duration){
			Instance.StartCameraShake(duration);
		}

		private void StartCameraShake(float duration){
			transform.DOShakePosition(duration).OnComplete(() => transform.position = _basePosition);
		}

		public static void DangerCamera(){
			Instance.StartDangerCamera();
		}

		private void StartDangerCamera(){
			if (_fovCountdown > 0.2f)
				_fovCountdown = PoliceManager.DangerTime;
			else
				StartCoroutine(DangerFOVCoroutine());
		}

		private IEnumerator DangerFOVCoroutine(){
			var time = PoliceManager.DangerTime;
			_fovCountdown = time;

			while(_fovCountdown > 0){
				yield return null;
				if (GameManager.Paused || !GameManager.GameRunning) continue;
				_fovCountdown -= Time.deltaTime;

				_camera.fieldOfView = Mathf.Lerp(_baseFOV, _maxFOV, _fovCurve.Evaluate(1 - _fovCountdown / time));
			}
		}

		public static void ResetFOV(){
			Instance.LocalResetFOV();
		}

		private void LocalResetFOV(){
			StopAllCoroutines();
			StartCoroutine(ResetFOVCoroutine(0.5f));
		}

		private IEnumerator ResetFOVCoroutine(float time){
			float baseFOV = _camera.fieldOfView;
			float timer = 0;

			while(timer < time){
				_camera.fieldOfView = Mathf.Lerp(baseFOV, _baseFOV, timer / time);
				yield return null;
				timer += Time.deltaTime;
			}

			_camera.fieldOfView = _baseFOV;
		}
	}
}