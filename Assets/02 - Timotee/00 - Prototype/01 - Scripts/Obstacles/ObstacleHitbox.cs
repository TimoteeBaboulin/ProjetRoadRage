using UnityEngine;

namespace Obstacles{
	public class ObstacleHitbox : MonoBehaviour{
		public ObstacleCar Car;

		private void OnEnable(){
			tag = "Obstacle";
		}
	}
}