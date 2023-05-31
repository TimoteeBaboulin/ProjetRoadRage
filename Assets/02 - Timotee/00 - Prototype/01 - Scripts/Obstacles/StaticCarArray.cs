using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obstacles{
	public class StaticCarArray : MonoBehaviour{
		public static StaticCarArray Instance;
		public static GameObject GameObject => Instance.gameObject;

		[SerializeField] private CarPieceHandler[] _frontPrefabs;
		[SerializeField] private CarPieceHandler[] _backPrefabs;
		[SerializeField] private Color[] _colors;

		private void Awake(){
			if (Instance==null) Instance = this;
		}

		public static CarPieceHandler[] GenerateParts(){
			return Instance.LocalGeneratePart();
		}
		
		public static CarPieceHandler[] GenerateParts(out Color color){
			color = Instance.GenerateColorLocal();
			return Instance.LocalGeneratePart();
		}

		private CarPieceHandler[] LocalGeneratePart(){
			var parts = new CarPieceHandler[2];

			parts[0] = PoolingManager.Instance.CreatePrefab(_frontPrefabs[Random.Range(0, _frontPrefabs.Length)]);
			parts[1] = PoolingManager.Instance.CreatePrefab(_backPrefabs[Random.Range(0, _backPrefabs.Length)]);
			return parts;
		}

		public static Color GenerateColor(){
			return Instance.GenerateColorLocal();
		}
		
		private Color GenerateColorLocal(){
			return _colors[Random.Range(0, _colors.Length)];
		}
	}
}