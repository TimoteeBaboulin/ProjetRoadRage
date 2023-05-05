using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI{
	public class UICutoutMask : Image{
		public override Material materialForRendering{
			get{
				var material = new Material(base.materialForRendering);
				material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
				return material;
			}
		}
	}

	public class UICutoutManager : MonoBehaviour{
		public static UICutoutManager Instance;

		private Animator _animator;

		private void Awake(){
			if (Instance==null) Instance = this;
			else
				Destroy(gameObject);
			_animator = GetComponent<Animator>();
		}
	}
}