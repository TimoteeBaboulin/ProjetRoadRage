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
		public static PlayerCar Player;


		[SerializeField] private Transform _front;

		[SerializeField] private ParticleSystem _smoke;

		private float _dangerCountdown;
		private bool _paused;

		private GameObject _snowplow;

		private float _snowplowCountdown;

		private TweenerCore<Vector3, Vector3, VectorOptions> _tweener;

		public static float LaneChangeTime => 0.2f / (1 + TerrainManager.SpeedIncrease);
		private bool CanMove => _tweener==null || !_tweener.IsActive() || _tweener.IsComplete();
		
		#region events
			public static event Action<PowerUp> OnPowerUp;
			public static event Action<int> OnLineChange;
			public static event Action<bool> OnSideContact;
		#endregion

		#region Suspensions
			private Animator _animator;
			private float _suspensionCountdown;
		#endregion

		#region Movement
			private int _currentPath;
			private InputBuffer _inputBuffer;
			private int _previousPath;
		#endregion

		private void Awake(){
			_inputBuffer = GetComponent<InputBuffer>();
			_inputBuffer ??= gameObject.AddComponent<InputBuffer>();
			if (Player!=null) return;
			Player = this;
		}

		//Initialise fields
		private void Start(){
			_currentPath = 1;
			_previousPath = 1;
			_rightTurnInput = new CustomInput(TurnRight);
			_leftTurnInput = new CustomInput(TurnLeft);
			_animator = GetComponentInChildren<Animator>();
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

			_animator.Play("SuspensionsOn", 0);
			_suspensionCountdown = time;
			StartCoroutine(SuspensionCoroutine());
		}

		private IEnumerator SuspensionCoroutine(){
			while(_suspensionCountdown > 0){
				yield return null;
				if (GameManager.Paused) continue;
				_suspensionCountdown -= Time.deltaTime;
			}

			_animator.Play("SuspensionsOff", 0);
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
			var laneChangeTime = LaneChangeTime;
			_animator.PlayInFixedTime("Turn Right", 1, laneChangeTime);
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, laneChangeTime)
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
			var laneChangeTime = LaneChangeTime;
			_animator.PlayInFixedTime("Turn Left", 1, laneChangeTime);
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, laneChangeTime)
				.OnComplete(OnCanMove);
		}

		private void OnCanMove(){
			if (_inputBuffer.TryGetInput(out var input))
				input.OnActivate?.Invoke();

			OnLineChange?.Invoke(_currentPath);
			Debug.Log("Move Done");
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
			Lost();
			GetComponent<AudioSource>().Play();
			_smoke.Play();
		}

		private void Lost(){
			GameManager.LoseGame();

			_tweener.Kill();
			CameraManager.ShakeCamera(1);
			MusicManager.FadeOut();
			_animator.speed = 0;
		}

		private void Arrested(){
			Lost();
			TimelineManager.Arrest();
		}

		private void Restart(){
			//Clear smoke effect
			_smoke.Stop();
			_smoke.Clear();
			
			//Reset Position
			_currentPath = 1;
			transform.position = new Vector3(0, 1, 0);
			
			//Restart the music
			MusicManager.FadeIn();
			
			//Remove the Snow Plow if it exists
			Destroy(_snowplow);
			
			//Resets Animations
			_animator.PlayInFixedTime("SuspensionsOff", 0, 0.01f);
			_animator.speed = 1;
		}

		private void Pause(bool paused){
			_paused = paused;
		}
	}
}