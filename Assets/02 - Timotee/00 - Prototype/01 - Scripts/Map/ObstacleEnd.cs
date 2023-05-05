using System;
using UnityEngine;

namespace Map{
	public class ObstacleEnd : MonoBehaviour{
		[SerializeField] private ModuleObstacle _parent;

		private void Update(){
			if (!(transform.position.z < 0)) return;
			_parent.gameObject.SetActive(false);
			OnDisable?.Invoke(_parent);
		}

		public static event Action<ModuleObstacle> OnDisable;
	}
}