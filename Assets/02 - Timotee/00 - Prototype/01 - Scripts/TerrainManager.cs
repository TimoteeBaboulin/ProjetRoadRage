using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TerrainManager : MonoBehaviour{
	public static TerrainManager Instance;

	[SerializeField] private GameObject _terrainPrefab;
	
	[SerializeField] private ModuleObstacle[] _obstaclePrefabs;

	private List<GameObject> _map = new();
	private List<ModuleObstacle> _obstacles = new();
	[SerializeField] private float _speed;
	[SerializeField] private int _renderDistance;

	private bool _paused;
	private float _obstacleTotalLength;

	private void Awake(){
		if (Instance != null) Destroy(gameObject);
		else Instance = this;

		TerrainPoolManager.Instance.AddToPool(_map);
	}

	private void Start(){
		InitialiseTerrain();
		InitialiseObstacles();
	}

	private void OnEnable(){
		GameManager.OnPause += (paused) => { _paused = paused; };

		TerrainEnd.OnDisable += SpawnTerrain;

		ObstacleEnd.OnDisable += SpawnObstacle;
	}

	private void InitialiseTerrain(){
		int terrainLength = 0;
		while ( terrainLength<_renderDistance ){
			GameObject terrain = TerrainPoolManager.Instance.CreatePrefab(_terrainPrefab);
			_map.Add(terrain);
			terrain.transform.parent = transform;
			terrain.transform.position = new Vector3(0, 0, terrainLength + 50);
			terrainLength += 100;
		}
	}

	private void InitialiseObstacles(){
		float obstacleLength = 50;
		while ( obstacleLength < _renderDistance ){
			ModuleObstacle obstacle =
				TerrainPoolManager.Instance.CreatePrefab(
					_obstaclePrefabs[Random.Range((int)0, _obstaclePrefabs.Length)]).GetComponent<ModuleObstacle>();
			_obstacles.Add(obstacle);
			obstacleLength += obstacle.Length;
			obstacle.transform.position = new Vector3(0, 0, obstacleLength + 50);
		}
	}
	
	private void SpawnObstacle(ModuleObstacle oldObstacle){
		ModuleObstacle obstacle =
			TerrainPoolManager.Instance.CreatePrefab(_obstaclePrefabs[Random.Range((int)0, _obstaclePrefabs.Length)]).GetComponent<ModuleObstacle>();
		
		if (!_obstacles.Contains(obstacle))
			_obstacles.Add(obstacle);

		_obstacleTotalLength -= oldObstacle.Length - obstacle.Length;
		
		_obstacles.Sort((obj, obj2) => obj.transform.position.z > obj2.transform.position.z ? -1 : 1);
		Vector3 obstaclePosition = _obstacles.First().End.transform.position;
		obstaclePosition.z += 50;

		obstacle.transform.position = obstaclePosition;
	}
	private void SpawnTerrain(){
		GameObject terrain = TerrainPoolManager.Instance.CreatePrefab(_terrainPrefab);
		if (!_map.Contains(terrain)){
			_map.Add(terrain);
			terrain.transform.parent = transform;
		}
		
		_map.Sort((obj, obj2) => obj.transform.position.z > obj2.transform.position.z ? -1 : 1);
		Vector3 terrainPosition = _map.First().transform.position;
		terrainPosition.z += 100;

		terrain.transform.position = terrainPosition;
	}

	private void Update(){
		if (_paused) return;
		foreach ( var tile in _map ){
			Vector3 tilePosition = tile.transform.position;
			tilePosition.z -= _speed * Time.deltaTime;
			tile.transform.position = tilePosition;
		}
		foreach ( var tile in _obstacles ){
			Vector3 tilePosition = tile.transform.position;
			tilePosition.z -= _speed * Time.deltaTime;
			tile.transform.position = tilePosition;
		}
	}
}