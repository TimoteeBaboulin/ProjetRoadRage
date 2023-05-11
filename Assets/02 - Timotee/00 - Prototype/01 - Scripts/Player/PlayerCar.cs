using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Managers;
using PowerUps;
using UnityEngine;

namespace Player{
	public enum Road{
		Left = 0,
		Middle = 1,
		Right = 2
	}

	public class PlayerCar : MonoBehaviour{
		private static CustomInput _rightTurnInput;
		private static CustomInput _leftTurnInput;

		public static float LaneChangeTime => 0.2f / (1 + TerrainManager.SpeedIncrease);
		public static PlayerCar Player;

	#region events
		public static event Action<PowerUp> OnPowerUp;
		public static event Action<int> OnLineChange;
		public static event Action<bool> OnSideContact;
	#endregion
		

		[SerializeField] private Transform _front;

		[SerializeField] private ParticleSystem _smoke;

	#region Suspensions
		private Animator _animator;
		private float _suspensionCountdown;
	#endregion

	#region Camera
		private Camera _camera;
		private Vector3 _cameraBasePosition;
	#endregion

	#region Movement
		private int _currentPath;
		private InputBuffer _inputBuffer;
		private int _previousPath;
	#endregion
		
		private float _snowplowCountdown;
		private bool _paused;

		private float _dangerCountdown;
		
		private GameObject _snowplow;

		private TweenerCore<Vector3, Vector3, VectorOptions> _tweener;
		private bool CanMove => _tweener==null || !_tweener.IsActive() || _tweener.IsComplete();

		private void Awake(){
			_inputBuffer = GetComponent<InputBuffer>();
			_inputBuffer ??= gameObject.AddComponent<InputBuffer>();
			_animator = GetComponent<Animator>();
			if (Player != null) return;
			Player = this;
		}

		//Initialise fields
		private void Start(){
			_camera = CameraManager.Camera;
			_currentPath = 1;
			_previousPath = 1;
			_cameraBasePosition = _camera.transform.position;
			_rightTurnInput = new CustomInput(TurnRight);
			_leftTurnInput = new CustomInput(TurnLeft);
			
		}

		//Editor only
		private void Update(){
			if (_dangerCountdown > 0)
				_dangerCountdown -= Time.deltaTime;
			#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.LeftArrow))
				TurnLeft();
			else if (Input.GetKeyDown(KeyCode.RightArrow))
				TurnRight();
			#endif
		}

		//Subscribe/Unsuscribe to events
		private void OnEnable(){
			PlayerHitbox.OnContact += HandleHitboxContact;
			InputManager.OnSwipe += HandleSwipe;
			GameManager.OnPause += Pause;
			GameManager.OnRestart += Restart;
		}

		private void OnDisable(){
			PlayerHitbox.OnContact -= HandleHitboxContact;
			InputManager.OnSwipe -= HandleSwipe;
			GameManager.OnPause -= Pause;
			GameManager.OnRestart -= Restart;
		}

		public void GogoGadgetSuspension(float time = 10){
			if (_suspensionCountdown > 0){
				_suspensionCountdown = time;
				return;
			}
			
			_animator.Play("GogoGadgetSuspension");
			_suspensionCountdown = time;
			StartCoroutine(SuspensionCoroutine());
		}

		private IEnumerator SuspensionCoroutine(){
			while(_suspensionCountdown > 0){
				yield return null;
				if (GameManager.Paused) continue;
				_suspensionCountdown -= Time.deltaTime;
			}
			_animator.Play("SuspensionRemake");
		}
		
		public void UsePowerUp(PowerUp powerUp){
			OnPowerUp?.Invoke(powerUp);
		}
		
		public void SpawnSnowPlow(GameObject spawn, float time = 15){
			if (_snowplowCountdown > 0){
				_snowplowCountdown = time;
				return;
			}

			_snowplowCountdown = time;
			_snowplow = Instantiate(spawn, _front);
			StartCoroutine(DestroySnowPlowCoroutine());
		}

		private IEnumerator DestroySnowPlowCoroutine(){
			while(_snowplowCountdown > 0){
				yield return null;
				if (GameManager.Paused) continue;
				_snowplowCountdown -= Time.deltaTime;
			}
			Destroy(_snowplow);
		}

		//Controls
		private void HandleSwipe(SwipeData data){
			if (data.Side==SwipeSide.Left)
				TurnLeft();
			else
				TurnRight();
		}

		private void TurnRight(){
			if (_paused || !GameManager.GameRunning || _currentPath==2) return;
			if (!CanMove){
				_inputBuffer.AddInput(_rightTurnInput);
				return;
			}

			_previousPath = _currentPath;
			_currentPath++;
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, LaneChangeTime)
				.OnComplete(OnCanMove);
		}

		private void TurnLeft(){
			if (_paused || !GameManager.GameRunning || _currentPath==0) return;
			if (!CanMove){
				_inputBuffer.AddInput(_leftTurnInput);
				return;
			}

			_previousPath = _currentPath;
			_currentPath--;
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, LaneChangeTime)
				.OnComplete(OnCanMove);
		}

		private void OnCanMove(){
			if (_inputBuffer.TryGetInput(out var input))
				input.OnActivate?.Invoke();
			
			OnLineChange?.Invoke(_currentPath);
		}

		//Hitboxes
		private void HandleHitboxContact(HitboxType type){
			if (type==HitboxType.Front || !_tweener.IsActive() || _tweener.ElapsedPercentage(false) >= 0.5f)
				FrontContact();
			else
				SideContact();
		}

		private void FrontContact(){
			if (_snowplow!=null) return;
			Die();
		}

		private void SideContact(){
			if (_dangerCountdown <= 0 || _dangerCountdown / PoliceManager.DangerTime > 0.85f){
				_currentPath = _previousPath;
				_tweener.Kill();
				_tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.05f).OnComplete(OnCanMove);
				
				CameraManager.ShakeCamera(0.1f);
				CameraManager.DangerCamera();

				_dangerCountdown = PoliceManager.DangerTime;
				OnSideContact?.Invoke(false);
				return;
			}
			
			Arrested();
			OnSideContact?.Invoke(true);
		}
		
		//Game Events
		private void Die(){
			GameManager.LoseGame();
			
			_tweener.Kill();
			CameraManager.ShakeCamera(1);
			MusicManager.FadeOut();
			GetComponent<AudioSource>().Play();
			_smoke.Play();
		}
		
		private void Arrested(){
			GameManager.LoseGame();
			
			_tweener.Kill();
			CameraManager.ShakeCamera(1);
			MusicManager.FadeOut();
			TimelineManager.Arrest();
		}

		private void Restart(){
			_smoke.Stop();
			_smoke.Clear();
			_currentPath = 1;
			transform.position = new Vector3(0, 1, 0);
			// _camera.GetComponent<Animator>().Play("SoundStart");
			MusicManager.FadeIn();
			Destroy(_snowplow);
			_animator.PlayInFixedTime("SuspensionRemake", 0, 0.01f);
		}

		private void Pause(bool paused){
			_paused = paused;
		}
	}
}