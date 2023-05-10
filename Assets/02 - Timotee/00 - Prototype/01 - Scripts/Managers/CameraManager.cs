using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Managers{
	public class CameraManager : MonoBehaviour{
		public static CameraManager Instance;
		public static Camera Camera => Instance._camera;
		
		private Camera _camera;

		private Vector3 _basePosition;
		private Quaternion _baseRotation;

		private Vector3 _menuPosition;
		private Quaternion _menuRotation;

		private Transform _transform;
		private Animator _animator;

		[SerializeField] private AnimationCurve _fovCurve;
		[SerializeField] private float _baseFOV;
		[SerializeField] private float _maxFOV;
		private float _fovCountdown;
		
		private void Awake(){
			if (Instance != null) return;
			Instance = this;
			_fovCountdown = 0;
			_camera = GetComponent<Camera>();
			_transform = transform;
			_menuPosition = _transform.position;
			_menuRotation = _transform.rotation;
		}

		private void OnEnable(){
			GameManager.OnRestart += ResetCameraPosition;
		}

		private void OnDisable(){
			GameManager.OnRestart -= ResetCameraPosition;
		}

		public void ResetCameraPosition(){
			_transform.position = _menuPosition;
			_transform.rotation = _menuRotation;
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
			float time = PoliceManager.DangerTime;
			_fovCountdown = time;

			while(_fovCountdown > 0){
				yield return null;
				if (GameManager.Paused || ! GameManager.GameRunning) continue;
				_fovCountdown -= Time.deltaTime;
				
				_camera.fieldOfView = Mathf.Lerp(_baseFOV, _maxFOV, _fovCurve.Evaluate(1 - (_fovCountdown / time)));
			}
		}

		private void ZoomOut(){
			
		}
	}
}