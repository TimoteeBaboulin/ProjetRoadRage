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

		[SerializeField] private ParticleSystem[] _impacts;
		[SerializeField] private ParticleSystem _snowPlowParticles;

		//Audio Sources
		[SerializeField] private AudioSource _frontImpactAudio;
		[SerializeField] private AudioSource _sideImpactAudio;
		[SerializeField] private AudioSource _policeAudio;
		[SerializeField] private AudioSource _engineAudio;
		[SerializeField] private AudioSource _pickUpAudio;

		[SerializeField] private AudioClip _poofOfSmokeClip;
		[SerializeField] private AudioClip _suspensionOnClip;
		[SerializeField] private AudioClip _suspensionOffClip;
		
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
			_engineAudio.volume = 0;
			_engineAudio.DOFade(1,.5f);
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
			if (_dangerCountdown > 0){
				if (Time.deltaTime >= _dangerCountdown)
					_policeAudio.Stop();
				_dangerCountdown -= Time.deltaTime;
				
			}
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
			_coinParticleSystem.Play();
		}
		
		public void GogoGadgetSuspension(float time = 10){
			if (_suspensionCountdown > 0){
				_suspensionCountdown = time;
				return;
			}

			_pickUpAudio.clip = _suspensionOnClip;
			_pickUpAudio.Play();
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

			_pickUpAudio.clip = _suspensionOffClip;
			_pickUpAudio.Play();
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

			_pickUpAudio.clip = _poofOfSmokeClip;
			_pickUpAudio.Play();
			_snowPlowParticles.Play();
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
		private void HandleHitboxContact(HitboxType type, int id){
			PlayImpactParticle(id);
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
			if (_snowplow!=null && _tweener.ElapsedPercentage(false) <= 0.1f)
				return;
			
			_sideImpactAudio.Play();
			
			if (_dangerCountdown <= 0 || _dangerCountdown <= PoliceManager.DangerTime * 0.15f){
				_policeAudio.Play();
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

		private void PlayImpactParticle(int hitbox){
			if (hitbox is > 2 or < 0) return;
			
			_impacts[hitbox].Play();
		}

		//Game Events
		private void Die(){
			Lost();
			_frontImpactAudio.Play();
			_smoke.Play();
		}

		private void Lost(){
			GameManager.LoseGame();

			_engineAudio.DOFade(0, .5f);
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
			
			//Stop the police audio from carrying over to the reset
			_policeAudio.Stop();
			_engineAudio.DOFade(1,.5f);
			
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
			_dangerCountdown = 0;
		}

		private void Pause(bool paused){
			_paused = paused;
		}
	}
}