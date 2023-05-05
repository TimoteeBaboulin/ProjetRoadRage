using System;
using UnityEngine;

namespace Map{
	public class TerrainEnd : MonoBehaviour{
		[SerializeField] private GameObject _parent;

		private void Update(){
			if (!(transform.position.z < 0)) return;
			_parent.SetActive(false);
			OnDisable?.Invoke();
		}

		public static event Action OnDisable;
	}
}