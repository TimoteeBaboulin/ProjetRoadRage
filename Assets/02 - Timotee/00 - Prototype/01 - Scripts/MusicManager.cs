using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour{
	public static MusicManager Instance;

	[SerializeField] private AudioSource _source;
	[SerializeField] private AudioClip[] _gameMusics;
	[SerializeField] private AudioClip[] _menuMusics;

	[SerializeField] private float _fadeLength;

	private MusicType _musicType;
	private Coroutine _coroutine;

	private void Awake(){
		if (Instance != null) Destroy(gameObject);
		else Instance = this;
	}

	private void OnEnable(){
		_musicType = MusicType.Menu;
		_source.volume = 0;
		_coroutine = StartCoroutine(HandleMusics());
	}

	private void OnDisable(){
		StopCoroutine(_coroutine);
	}

	private void StartHandlingMusics(){
		StartCoroutine(HandleMusics());
	}

	private IEnumerator HandleMusics(){
		while(_source.time < _source.clip.length - _fadeLength){
			yield return null;
		}
		
		TransitionMusics();
	}

	private void TransitionMusics(){
		StopCoroutine(_coroutine);
		
		_source.DOFade(0, _fadeLength).onComplete = () => {
			_source.clip = ChoseRandomMusic(_musicType);
			_source.Play();
			_source.DOFade(1, _fadeLength).onComplete = () => {
				_coroutine = StartCoroutine(HandleMusics());
			};
		};
	}

	private static AudioClip ChoseRandomMusic(MusicType type){
		return type == MusicType.Main
			? Instance._gameMusics[Random.Range(0, Instance._gameMusics.Length)]
			: Instance._menuMusics[Random.Range(0, Instance._menuMusics.Length)];
	}
	
	public static void FadeIn(){
		Instance.MusicFadeIn();
	}
	public static void FadeOut(){
		Instance.MusicFadeOut();
	}

	private void MusicFadeIn(){
		_source.clip = ChoseRandomMusic(Instance._musicType);
		_source.Play();
		_source.DOPitch(1, _fadeLength);
		_source.DOFade(1, _fadeLength);
		StartHandlingMusics();
	}
	private void MusicFadeOut(){
		_source.DOPitch(0, _fadeLength);
	}
}

public enum MusicType{
	Main,
	Menu
}