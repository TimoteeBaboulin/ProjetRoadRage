using System.Collections.Generic;
using System.Linq;
using Map;
using Obstacles;
using UnityEditor;
using UnityEngine;

namespace Managers{
	public class PoolingManager : MonoBehaviour{
		public static PoolingManager Instance;

		private readonly Dictionary<string, List<GameObject>> _pool = new();
		private readonly Dictionary<string, List<CarPieceHandler>> _carPiecePool = new();
		private readonly Dictionary<string, List<ModuleObstacle>> _obstaclePool = new();

		private void OnEnable(){
			if (Instance!=null) Destroy(gameObject);
			else Instance = this;
		}

		public GameObject CreatePrefab(GameObject model){
			var key = model.name;
			List<GameObject> list;
			GameObject value;

			if (_pool.ContainsKey(key)){
				list = _pool[key];
				value = list.FirstOrDefault(obj => !obj.activeSelf);
				if (value!=null){
					value.SetActive(true);
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
			var key = model.name;
			List<ModuleObstacle> list;
			ModuleObstacle value;

			if (_obstaclePool.ContainsKey(key)){
				list = _obstaclePool[key];
				var tempValue = list.FirstOrDefault(obj => !obj.gameObject.activeSelf);
				if (tempValue !=null){
					tempValue.gameObject.SetActive(true);
					return tempValue;
				}

				value = Instantiate(model);
				value.name = model.name;
				list.Add(value);

				return value;
			}

			list = new List<ModuleObstacle>();
			value = Instantiate(model);
			value.name = model.name;
			list.Add(value);
			_obstaclePool.Add(key, list);
			return value;
		}

		public CarPieceHandler CreatePrefab(CarPieceHandler model){
			var key = model.name;
			List<CarPieceHandler> list;
			CarPieceHandler value;

			if (_carPiecePool.TryGetValue(key, out list)){
				value = list.FirstOrDefault(obj => !obj.gameObject.activeSelf);
				if (value!=null){
					value.gameObject.SetActive(true);
					return value;
				}

				value = Instantiate(model);
				value.name = model.name;
				list.Add(value);

				return value;
			}

			list = new List<CarPieceHandler>();
			value = Instantiate(model);
			value.name = model.name;
			list.Add(value);
			_carPiecePool.Add(key, list);
			return value;
		}

		public void AddToPool(List<GameObject> list){
			foreach(var tile in list){
				if (!_pool.ContainsKey(tile.name)) _pool.Add(tile.name, new List<GameObject>());

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
}