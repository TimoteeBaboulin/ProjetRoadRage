using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    [SerializeField] private GameObject[] _terrainPrefabs;
    
    [SerializeField] private List<GameObject> _terrainTiles = new();
    [SerializeField] private float _speed;

    private bool _paused;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        
        TerrainPoolManager.Instance.AddToPool(_terrainTiles);
    }

    private void OnEnable()
    {
        GameManager.OnPause += (paused) => { _paused = paused; };
        
        TerrainEnd.OnDisable += () =>
        {
            GameObject terrain = TerrainPoolManager.Instance.CreatePrefab(_terrainPrefabs[Random.Range((int)0, _terrainPrefabs.Length)]);
            if (!_terrainTiles.Contains(terrain))
                _terrainTiles.Add(terrain);
            terrain.transform.parent = transform;
            _terrainTiles.Sort((obj, obj2) =>
            {
                return obj.transform.position.z > obj2.transform.position.z ? -1 : 1;
            });
            Vector3 terrainPosition = _terrainTiles.First().transform.position;
            terrainPosition.z += 100;

            terrain.transform.position = terrainPosition;
        };
    }

    private void Update()
    {
        if (_paused) return;
        foreach (var tile in _terrainTiles)
        {
            Vector3 tilePosition = tile.transform.position;
            tilePosition.z -= _speed * Time.deltaTime;
            tile.transform.position = tilePosition;
        }
    }
}