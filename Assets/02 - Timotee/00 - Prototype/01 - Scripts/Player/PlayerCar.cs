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

		[SerializeField] private AnimationCurve _laneChangeCurve;
		[SerializeField] private ParticleSystem _coinParticleSystem;

		private float _dangerCountdown;
		private bool _paused;

		private GameObject _snowplow;

		private float _snowplowCountdown;
		private float _currentLanePosition{
			get{
				var laneWidth = GameManager.LaneWidth;
				return-laneWidth + _currentPath * laneWidth;
			}
		}

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

		public void PlayCoinParticles(){
			// if (_coinParticleSystem.isPlaying) return;
			// Instantiate(_coinParticleSystem, transform.position, transform.rotation).Play();
			_coinParticleSystem.Play();
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
			Turn(data.Side == SwipeSide.Left);
		}

		private void TurnRight(){
			Turn(false);
		}

		private void TurnLeft(){
			Turn(true);
		}

		private void Turn(bool left){
			if (_paused || !GameManager.GameRunning || (_currentPath==0 && left) || (_currentPath==2 && !left)) return;
			if (!CanMove){
				_inputBuffer.AddInput(left ? _leftTurnInput : _rightTurnInput);
				return;
			}
			
			_inputBuffer.Clear();
			
			_previousPath = _currentPath;
			if (left) _currentPath--;
			else _currentPath++;
			
			var laneChangeTime = LaneChangeTime;
			var animationName = "Turn " + (left ? "Left" : "Right");
			_animator.Play(animationName, 1);
			_animator.speed = 1 / laneChangeTime;

			_tweener = transform.DOMoveX(_currentLanePosition, laneChangeTime).SetEase(_laneChangeCurve)
				.OnComplete(OnCanMove);
		}

		private void OnCanMove(){
			if (_inputBuffer.TryGetInput(out var input))
				input.OnActivate?.Invoke();
			else
				_animator.speed = 1;
			
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
			CameraManager.ResetFOV();
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
			transform.position = new Vector3(0, transform.position.y, 0);
			
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