﻿using System;
using UnityEngine;

public class RoadworkEnd : MonoBehaviour{
	public static event Action OnDisable;

	[SerializeField] private GameObject _parent;

	private void Update(){
		if (transform.position.z >= 0) return;
		_parent.SetActive(false);
		OnDisable?.Invoke();
	}
}