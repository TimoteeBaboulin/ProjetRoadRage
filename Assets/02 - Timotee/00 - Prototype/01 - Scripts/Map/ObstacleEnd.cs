using System;
using UnityEngine;

public class ObstacleEnd : MonoBehaviour{
	public static event Action<ModuleObstacle> OnDisable;

	[SerializeField] private ModuleObstacle _parent;

	private void Update(){
		if (!(transform.position.z < 0)) return;
		_parent.gameObject.SetActive(false);
		OnDisable?.Invoke(_parent);
	}
}