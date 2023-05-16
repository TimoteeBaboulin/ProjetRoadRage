using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Managers;
using UnityEngine;

namespace Obstacles{
	public class PoliceCarProp : MonoBehaviour{
		[SerializeField] private float _basePosition;
		[SerializeField] private float _finalPosition;
		[SerializeField] private AnimationCurve _positionCurve;

		private TweenerCore<Vector3, Vector3, VectorOptions> _tweener;

		private void Reset(){
			_tweener.Kill();
			var position = transform.position;
			position.z = _basePosition;
			transform.position = position;
		}

		private void OnEnable(){
			GameManager.OnRestart += Reset;
		}

		private void OnDisable(){
			GameManager.OnRestart -= Reset;
		}

		public void GetCloser(){
			_tweener = transform.DOMoveZ(_finalPosition, 1.5f).SetEase(_positionCurve);
		}
	}
}