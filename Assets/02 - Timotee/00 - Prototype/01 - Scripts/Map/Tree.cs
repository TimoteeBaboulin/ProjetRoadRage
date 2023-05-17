using UnityEngine;

namespace Obstacles{
	public class Tree : MonoBehaviour{
		private GameObject _tree;

		private void OnEnable(){
			if (_tree!=null){
				_tree.SetActive(false);
				_tree.transform.SetParent(StaticTreeArray.Transform);
			}

			_tree = StaticTreeArray.GenerateTree();
			_tree.SetActive(true);
			_tree.transform.SetParent(transform);
			_tree.transform.position = transform.position;

			var random = Random.Range(0f, 180f);
			_tree.transform.rotation = Quaternion.Euler(0, random, 0);
		}
	}
}