using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIMuteButton : MonoBehaviour{
	private const float Zero = 0.0001f;
	
	[SerializeField] private string _parameterName;
	[SerializeField] private AudioMixer _mixer;
	private bool _mute = false;
	
	[SerializeField] private Image _image;

	public void Mute(){
		_mute = !_mute;
		if (_mute)
			_mixer.SetFloat(_parameterName, Mathf.Log(Zero) * 20);
		else{
			float volume = PlayerPrefs.GetFloat(_parameterName);
			_mixer.SetFloat(_parameterName, Mathf.Log(volume) * 20);
		}
		UpdateSprite();
	}

	private void UpdateSprite(){
		_image.sprite = UISpriteStaticArray.SoundSprites[_mute ? 1 : 0];
	}
}