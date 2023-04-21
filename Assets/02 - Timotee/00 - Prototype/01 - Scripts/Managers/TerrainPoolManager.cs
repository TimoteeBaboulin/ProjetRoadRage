using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainPoolManager : MonoBehaviour{
	public static TerrainPoolManager Instance;

	private readonly Dictionary<string, List<GameObject>> _pool = new();

	private void OnEnable(){
		if (Instance != null) Destroy(gameObject);
		else Instance = this;
	}

	public GameObject CreatePrefab(GameObject model){
		string key = model.name;
		List<GameObject> list;
		GameObject value;

		if (_pool.ContainsKey(key)){
			list = _pool[key];
			value = list.FirstOrDefault(obj => !obj.activeSelf);
			if (value != null){
				value.SetActive(true);
				Debug.Log("Saved 1 instantiation.");
				return value;
			}

			value = Instantiate(model);
			value.name = model.name;
			list.Add(value);

			return value;
		}

		list = new List<GameObject>();
		value = Instantiate(model);
		value.name = model.name;
		list.Add(value);
		_pool.Add(key, list);
		return value;
	}
	
	public ModuleObstacle CreatePrefab(ModuleObstacle model){
		string key = model.name;
		List<GameObject> list;
		ModuleObstacle value;

		if (_pool.ContainsKey(key)){
			list = _pool[key];
			GameObject tempValue = list.FirstOrDefault(obj => !obj.activeSelf);
			if (tempValue != null){
				tempValue.gameObject.SetActive(true);
				return tempValue.GetComponent<ModuleObstacle>();
			}

			value = Instantiate(model);
			value.name = model.name;
			list.Add(value.gameObject);

			return value;
		}

		list = new List<GameObject>();
		value = Instantiate(model);
		value.name = model.name;
		list.Add(value.gameObject);
		_pool.Add(key, list);
		return value;
	}

	public void AddToPool(List<GameObject> list){
		foreach ( var tile in list ){
			if (!_pool.ContainsKey(tile.name)){
				_pool.Add(tile.name, new List<GameObject>());
			}

			_pool[tile.name].Add(tile);
		}
	}

	public bool DisableInstance(GameObject instance){
		if (_pool.ContainsKey(instance.name) && _pool[instance.name].Contains(instance)){
			instance.SetActive(false);
			return true;
		}

		return false;
	}
}