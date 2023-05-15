using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obstacles{
	public class StaticCarArray : MonoBehaviour{
		public static StaticCarArray Instance;

		[SerializeField] private GameObject[] _frontPrefabs;
		[SerializeField] private GameObject[] _backPrefabs;
		
		private void Awake(){
			if (Instance==null) Instance = this;
		}

		public static GameObject[] GenerateParts(){
			return Instance.LocalGeneratePart();
		}

		private GameObject[] LocalGeneratePart(){
			GameObject[] parts = new GameObject[2];

			parts[0] = TerrainPoolManager.Instance.CreatePrefab(_frontPrefabs[Random.Range(0, _frontPrefabs.Length)]);
			parts[1] = TerrainPoolManager.Instance.CreatePrefab(_backPrefabs[Random.Range(0, _backPrefabs.Length)]);
			return parts;
		}
	}
}