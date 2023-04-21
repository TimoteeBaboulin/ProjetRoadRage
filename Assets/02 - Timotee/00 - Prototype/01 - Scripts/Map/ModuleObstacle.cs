using UnityEngine;

public class ModuleObstacle : MonoBehaviour{
	public ObstacleEnd End;
	public float Length => End.transform.localPosition.z + 50;
}