using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers{
	public class TerrainManager : MonoBehaviour{
		public static TerrainManager Instance;


		[SerializeField] private int _renderDistance;

		//Speed Increase Relevent
		[Header("Speed Settings")]
		[SerializeField]
		private float _speed;

		[SerializeField] private float _minSpeed;
		[SerializeField] private float _maxSpeed;
		[SerializeField] private AnimationCurve _speedIncreaseCurve;
		[SerializeField] private float _timeToReachMaxSpeed;
		private bool _maxSpeedReached;
		private float _obstacleTotalLength;

		private bool _paused;
		private float _roadworkLength;

		//Roadwork critical fields
		private int _roadworkSide;
		private float _roadworkTotalLength;

		private float _timer;

		public static float SpeedIncrease =>
			Instance._speedIncreaseCurve.Evaluate(Instance._timer / Instance._timeToReachMaxSpeed);

		public float Speed => _speed;

		private void Awake(){
			if (Instance!=null) Destroy(gameObject);
			else Instance = this;

			_map = new List<GameObject>();
			_obstacles = new List<ModuleObstacle>();
			_roadworks = new List<GameObject>();

			TerrainPoolManager.Instance.AddToPool(_map);
		}

		private void Start(){
			InitialiseTerrain();
		}

		private void Update(){
			if (_paused) return;

			var distance = Time.deltaTime * _speed;
			Vector3 tilePosition;

			foreach(var tile in _map){
				tilePosition = tile.transform.position;
				tilePosition.z -= distance;
				tile.transform.position = tilePosition;
			}

			if (!GameManager.GameRunning) return;

			HandleSpeed();

			_obstacleTotalLength -= distance;
			if (_roadworkTotalLength > 0) _roadworkTotalLength -= distance;

			for (var x = 0; x < _roadworks.Count; x++){
				var tile = _roadworks[x];
				tilePosition = tile.transform.position;
				tilePosition.z -= distance;
				tile.transform.position = tilePosition;
			}

			if (_roadworkTotalLength < _renderDistance) SpawnRoadwork();

			for (var x = 0; x < _obstacles.Count; x++){
				var tile = _obstacles[x];
				var tileTransform = tile.transform;
				tilePosition = tileTransform.position;
				tilePosition.z -= distance;
				tileTransform.position = tilePosition;
			}

			if (_obstacleTotalLength < _renderDistance) SpawnObstacle();
		}

		private void OnEnable(){
			GameManager.OnPause += paused => { _paused = paused; };
			GameManager.OnGameStart += StartGame;
			GameManager.OnRestart += Restart;

			TerrainEnd.OnDisable += SpawnTerrain;
		}

		private void OnDisable(){
			GameManager.OnGameStart -= StartGame;
			GameManager.OnRestart -= Restart;

			TerrainEnd.OnDisable -= SpawnTerrain;
		}

		private void InitialiseTerrain(){
			var terrainLength = 0;
			while(terrainLength < _renderDistance){
				var terrain = TerrainPoolManager.Instance.CreatePrefab(_terrainPrefab);
				_map.Add(terrain);
				terrain.transform.parent = transform;
				terrain.transform.position = new Vector3(0, 0, terrainLength + 50);
				terrainLength += 100;
			}
		}

		private void InitialiseObstacles(){
			_obstacleTotalLength = 400;
			// while ( _obstacleTotalLength < _renderDistance ){
			// 	SpawnObstacle();
			// }
		}

		private void StartGame(){
			_timer = 0;
			InitialiseObstacles();
		}

		private void Restart(){
			_timer = 0;
			_speed = _minSpeed;
			_maxSpeedReached = false;
			_obstacleTotalLength = 0;
			_roadworkTotalLength = 0;
			_roadworkLength = 0;
			DisableObstacles();
		}

		private void SpawnTerrain(){
			var terrain = TerrainPoolManager.Instance.CreatePrefab(_terrainPrefab);
			if (!_map.Contains(terrain)){
				_map.Add(terrain);
				terrain.transform.parent = _terrainParent;
			}

			_map.Sort((obj, obj2) => obj.transform.position.z > obj2.transform.position.z ? -1 : 1);
			var terrainPosition = _map.First().transform.position;
			terrainPosition.z += 100;

			terrain.transform.position = terrainPosition;
		}

		private void SpawnObstacle(){
			if (_obstacleTotalLength <= _roadworkTotalLength){
				InstantiateObstacle(true);
				return;
			}

			if (_roadworkLength==0 && Random.Range(0f, 1f) < 0.1f){
				StartRoadwork();
				InstantiateObstacle(true);
				return;
			}

			InstantiateObstacle(false);
		}

		private ModuleObstacle InstantiateObstacle(bool isRoadwork){
			ModuleObstacle obstacle;
			Vector3 scale;

			switch (isRoadwork){
				case true:
					obstacle = TerrainPoolManager.Instance
						.CreatePrefab(_roadworkObstaclePrefabs[Random.Range(0, _roadworkObstaclePrefabs.Length)]);
					scale = new Vector3(_roadworkSide, 1, 1);
					break;
				case false:
					obstacle =
						TerrainPoolManager.Instance
							.CreatePrefab(_obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)]);
					scale = Vector3.one;
					break;
			}

			if (!_obstacles.Contains(obstacle)){
				_obstacles.Add(obstacle);
				obstacle.transform.parent = _obstacleParent;
			}

			var obstaclePosition = new Vector3(0, 0, _obstacleTotalLength + 50);

			var obstacleTransform = obstacle.transform;
			obstacleTransform.position = obstaclePosition;
			obstacleTransform.localScale = scale;
			_obstacleTotalLength = obstacle.End.transform.position.z;

			return obstacle;
		}

		private void SpawnRoadwork(){
			if (_roadworkLength==0) return;

			var random = Random.Range(0f, 1f);
			if (random <= 0.2f || _roadworkLength >= 500){
				InstantiateRoadwork(RoadworkPhase.End);

				_roadworkLength = 0;
				_roadworkTotalLength += 50;
				return;
			}

			InstantiateRoadwork(RoadworkPhase.Middle);

			_roadworkLength += 50;
			_roadworkTotalLength += 50;
		}

		private void StartRoadwork(){
			_roadworkSide = Random.Range(0, 2)==0 ? -1 : 1;
			_roadworkTotalLength = _obstacleTotalLength;

			InstantiateRoadwork(RoadworkPhase.Beginning);

			_roadworkLength = 50;
			_roadworkTotalLength = _obstacleTotalLength + 50;
		}

		private void InstantiateRoadwork(RoadworkPhase phase){
			GameObject roadworkPrefab;
			Vector3 position;
			Vector3 scale;

			switch (phase){
				case RoadworkPhase.Beginning:
					roadworkPrefab = _roadworkStartPrefab;
					break;
				case RoadworkPhase.Middle:
					roadworkPrefab = _roadworkPrefab;
					break;
				case RoadworkPhase.End:
					roadworkPrefab = _roadworkEndPrefab;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
			}

			position = new Vector3(_roadworkSide * GameManager.LaneWidth, 0, _roadworkTotalLength + 50);
			scale = new Vector3(_roadworkSide, 1, 1);

			var roadwork = TerrainPoolManager.Instance.CreatePrefab(roadworkPrefab);
			if (!_roadworks.Contains(roadwork)){
				roadwork.transform.parent = _roadworkParent;
				_roadworks.Add(roadwork);
			}

			roadwork.transform.position = position;
			roadwork.transform.localScale = scale;
		}

		private void DisableObstacles(){
			foreach(var obstacle in _obstacles.Where(obj => obj.gameObject.activeSelf))
				obstacle.gameObject.SetActive(false);
			foreach(var obstacle in _roadworks.Where(obj => obj.gameObject.activeSelf))
				obstacle.gameObject.SetActive(false);
		}

		private void HandleSpeed(){
			if (_maxSpeedReached) return;

			_timer += Time.deltaTime;
			if (_timer > _timeToReachMaxSpeed){
				_maxSpeedReached = true;
				_speed = _maxSpeed;
				return;
			}

			_speed = Mathf.Lerp(_minSpeed, _maxSpeed, _speedIncreaseCurve.Evaluate(_timer / _timeToReachMaxSpeed));
		}

	#region Prefabs
		[SerializeField] private GameObject _terrainPrefab;
		[SerializeField] private ModuleObstacle[] _obstaclePrefabs;
		[SerializeField] private ModuleObstacle[] _roadworkObstaclePrefabs;

		[Header("Roadwork Prefabs")]
		[SerializeField]
		private GameObject _roadworkStartPrefab;

		[SerializeField] private GameObject _roadworkPrefab;
		[SerializeField] private GameObject _roadworkEndPrefab;
	#endregion

	#region MapGameObjects
		private List<GameObject> _map;
		private List<ModuleObstacle> _obstacles;
		private List<GameObject> _roadworks;
	#endregion

	#region Transforms
		[SerializeField] private Transform _terrainParent;
		[SerializeField] private Transform _obstacleParent;
		[SerializeField] private Transform _roadworkParent;
	#endregion
	}

	public enum RoadworkPhase{
		Beginning,
		Middle,
		End
	}
}