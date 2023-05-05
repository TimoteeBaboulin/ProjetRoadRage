﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers{
	public class InputManager : MonoBehaviour{
		public static InputManager Instance;

		//Swipe Data and Fields
		[SerializeField] private float _minimalSwipeSpeed;

		private Camera _camera;
		private Vector2 _endSwipe;
		private MainControls _mainControls;
		private Vector2 _startSwipe;

		private void Awake(){
			if (Instance!=null){
				Destroy(gameObject);
				return;
			}

			Instance = this;
			_mainControls = new MainControls();
			_camera = Camera.main;
		}

		private void Start(){
			_mainControls.Touch.PrimaryTouch.started += ctx => StartSwipe(ctx);
			_mainControls.Touch.PrimaryTouch.canceled += ctx => EndSwipe(ctx);
			_camera = Camera.main;
			gameObject.SetActive(false);
		}

		private void OnEnable(){
			_mainControls.Enable();
		}

		private void OnDisable(){
			_mainControls.Disable();
		}

		/// <summary>
		///     Return whether or not it's going right
		/// </summary>
		public static event Action<SwipeData> OnSwipe;

		private void StartSwipe(InputAction.CallbackContext context){
			_startSwipe = _mainControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
		}

		private void EndSwipe(InputAction.CallbackContext context){
			_endSwipe = _mainControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();

			var normalizedStart = _camera.ScreenToViewportPoint(_startSwipe);
			var normalizedEnd = _camera.ScreenToViewportPoint(_endSwipe);

			SwipeData data;

			if (normalizedEnd.x > normalizedStart.x + 0.1f){
				data = new SwipeData(SwipeSide.Right, normalizedStart, normalizedEnd);
				OnSwipe?.Invoke(data);
			} else if (normalizedEnd.x < normalizedStart.x - 0.1f){
				data = new SwipeData(SwipeSide.Left, normalizedStart, normalizedEnd);
				OnSwipe?.Invoke(data);
			}
		}
	}

	public enum SwipeSide{
		Right,
		Left
	}

	public struct SwipeData{
		public SwipeSide Side;
		public Vector3 NormalisedStart;
		public Vector3 EndStart;

		public SwipeData(SwipeSide side, Vector3 normalisedStart, Vector3 endStart){
			Side = side;
			NormalisedStart = normalisedStart;
			EndStart = endStart;
		}
	}
}