using UnityEngine;

public class PickUpMovement : MonoBehaviour{
	[SerializeField] private float _bonusSpeed;

	private Vector3 _baseLocalPosition;
	private float _speed;

	private void Awake(){
		_baseLocalPosition = transform.localPosition;
	}

	private void OnEnable(){
		transform.localPosition = _baseLocalPosition;
		transform.rotation = Quaternion.identity;
		_speed = TerrainManager.Instance.Speed * _bonusSpeed;
	}

	private void Update(){
		if (GameManager.Paused || transform.position.z > 100) return;
		
		Vector3 position = transform.position;
		position.z -= _speed * Time.deltaTime;
		transform.position = position;
	}
}