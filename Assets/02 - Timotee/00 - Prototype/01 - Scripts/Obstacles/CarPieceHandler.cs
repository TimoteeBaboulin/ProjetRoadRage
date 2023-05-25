using UnityEngine;

namespace Obstacles{
	public class CarPieceHandler : MonoBehaviour{
		[SerializeField] private MeshRenderer _renderer;

		public void SetColor(Color color){
			foreach(var material in _renderer.materials){
				material.SetColor("_Carosserie", color);
			}
		}
	}
}