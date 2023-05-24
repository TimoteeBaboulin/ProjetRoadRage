using System;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obstacles{
	public class StaticTreeArray : MonoBehaviour{
		public static StaticTreeArray Instance;
		public static Transform Transform => Instance.transform;

		[SerializeField] private GameObject[] _trees;

		private void Awake(){
			if (Instance==null) Instance = this;
		}

		public static GameObject GenerateTree(){
			return Instance.LocalGenerateTree();
		}

		private GameObject LocalGenerateTree(){
			return PoolingManager.Instance.CreatePrefab(_trees[Random.Range(0, _trees.Length)]);
		}
	}
}