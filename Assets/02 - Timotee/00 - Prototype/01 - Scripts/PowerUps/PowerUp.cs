using UnityEngine;

namespace PowerUps{
	[CreateAssetMenu(menuName = "Create PowerUp", fileName = "PowerUp", order = 0)]
	public class PowerUp : ScriptableObject{
		public Sprite Icon;
		public float Time;
	}
}