using System;
using System.Collections.Generic;
using UnityEngine;

public enum BufferType{
	First,
	Last
}

public class InputBuffer : MonoBehaviour{
	[SerializeField] private float _bufferLength;
	[SerializeField] private BufferType _bufferType;

	private List<CustomInput> _inputs;

	private void Awake(){
		_inputs = new List<CustomInput>();
	}

	private void Update(){
		if (_inputs.Count <= 0) return;
		for (var x = _inputs.Count - 1; x >= 0; x--){
			_inputs[x].Timer += Time.deltaTime;
			if (_inputs[x].Timer > _bufferLength)
				_inputs.RemoveAt(x);
		}
	}

	public void Clear(){
		_inputs.Clear();
	}
	
	public void AddInput(CustomInput input){
		_inputs.Add(input);
	}

	public void InsertInput(CustomInput input){
		_inputs.Insert(0, input);
	}

	public bool TryGetInput(out CustomInput input){
		if (_inputs.Count==0){
			input = null;
			return false;
		}

		switch (_bufferType){
			case BufferType.First:
				input = _inputs[0];
				break;
			case BufferType.Last:
				input = _inputs[^1];
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		_inputs.Remove(input);
		return true;
	}
}

public class CustomInput{
	public Action OnActivate;
	public float Timer;

	public CustomInput(Action action){
		OnActivate = action;
	}
}