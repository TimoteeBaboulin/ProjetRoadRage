using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Managers;
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

		[SerializeField] private Transform _front;

		[SerializeField] private ParticleSystem _smoke;

		private Camera _camera;
		private Vector3 _cameraBasePosition;
		private int _currentPath;
		private InputBuffer _inputBuffer;
		private Coroutine _killSnowPlowCoroutine;
		private bool _paused;
		private int _previousPath;

		private GameObject _snowplow;

		private TweenerCore<Vector3, Vector3, VectorOptions> _tweener;
		private bool CanMove => _tweener==null || !_tweener.IsActive() || _tweener.IsComplete();

		private void Awake(){
			_camera = Camera.main;
			_inputBuffer = GetComponent<InputBuffer>();
			_inputBuffer ??= gameObject.AddComponent<InputBuffer>();
		}

		//Initialise fields
		private void Start(){
			_currentPath = 1;
			_previousPath = 1;
			_cameraBasePosition = _camera.transform.position;
			_rightTurnInput = new CustomInput(TurnRight);
			_leftTurnInput = new CustomInput(TurnLeft);
		}

		//Editor only
		private void Update(){
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
			GameManager.OnGameLost += Die;
			GameManager.OnRestart += Restart;
		}

		private void OnDisable(){
			PlayerHitbox.OnContact -= HandleHitboxContact;
			InputManager.OnSwipe -= HandleSwipe;
			GameManager.OnPause -= Pause;
			GameManager.OnGameLost -= Die;
			GameManager.OnRestart -= Restart;
		}

		public void SpawnItemAtFront(GameObject spawn, float time = 15){
			if (_killSnowPlowCoroutine!=null){
				StopCoroutine(_killSnowPlowCoroutine);
				Destroy(_snowplow);
			}

			_snowplow = Instantiate(spawn, _front);
			_killSnowPlowCoroutine = StartCoroutine(DestroyObjectCoroutine(time));
		}

		private IEnumerator DestroyObjectCoroutine(float time){
			yield return new WaitForSeconds(time);
			Destroy(_snowplow);
			_killSnowPlowCoroutine = null;
		}

		//Controls
		private void HandleSwipe(SwipeData data){
			if (data.Side==SwipeSide.Left)
				TurnLeft();
			else
				TurnRight();
		}

		private void TurnRight(){
			if (_paused || _currentPath==2) return;
			if (!CanMove){
				_inputBuffer.AddInput(_rightTurnInput);
				return;
			}

			_previousPath = _currentPath;
			_currentPath++;
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.2f / (TerrainManager.Instance.Speed / 60))
				.OnComplete(OnCanMove);
		}

		private void TurnLeft(){
			if (_paused || _currentPath==0) return;
			if (!CanMove){
				_inputBuffer.AddInput(_leftTurnInput);
				return;
			}

			_previousPath = _currentPath;
			_currentPath--;
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.2f / (TerrainManager.Instance.Speed / 60))
				.OnComplete(OnCanMove);
		}

		private void OnCanMove(){
			if (_inputBuffer.TryGetInput(out var input))
				input.OnActivate?.Invoke();
		}

		//Hitboxes
		private void HandleHitboxContact(HitboxType type){
			if (type==HitboxType.Front || !_tweener.IsActive() || _tweener.ElapsedPercentage(false) >= 0.5f){
				if (_snowplow!=null) return;
				GameManager.LoseGame();
				return;
			}

			_tweener.Kill();
			_cameraBasePosition = _camera.transform.position;
			_camera.transform.DOShakePosition(0.1f).OnComplete(() => _camera.transform.position = _cameraBasePosition);
			_currentPath = _previousPath;
			_tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.05f).OnComplete(OnCanMove);
		}

		//Game Events
		private void Die(){
			_tweener.Kill();
			_cameraBasePosition = _camera.transform.position;
			_camera.transform.DOShakePosition(1).OnComplete(() => { _camera.transform.position = _cameraBasePosition; });
			MusicManager.FadeOut();
			GetComponent<AudioSource>().Play();
			_smoke.Play();
		}

		private void Restart(){
			_smoke.Stop();
			_smoke.Clear();
			_currentPath = 1;
			transform.position = new Vector3(0, 1, 0);
			_camera.GetComponent<Animator>().Play("SoundStart");
			Destroy(_snowplow);
		}

		private void Pause(bool paused){
			_paused = paused;
		}
	}
}