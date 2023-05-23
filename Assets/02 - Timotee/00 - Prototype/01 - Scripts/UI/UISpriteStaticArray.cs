using UnityEngine;

public class UISpriteStaticArray : MonoBehaviour{
	public static UISpriteStaticArray Instance;

	public static Sprite[] SoundSprites => Instance._soundSprites;
	[SerializeField] private Sprite[] _soundSprites;

	private void Awake(){
		if (Instance==null)
			Instance = this;
	}
}