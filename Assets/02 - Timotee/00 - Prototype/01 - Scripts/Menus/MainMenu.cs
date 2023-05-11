using System;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Menus{
	public class MainMenu : MonoBehaviour{
		public GameObject MainMenuParent;
		public Canvas MainMenuCanvas;

		public AudioMixer AudioMixer;
		public PlayableDirector TimelineDirector;
		public PlayableAsset StartGameTimeline;

		public string MasterVolumePlayerPrefName;
		public string BGMVolumePlayerPrefName;
		public string SFXVolumePlayerPrefName;

		public Slider MasterVolumeSlider;
		public Slider BGMVolumeSlider;
		public Slider SFXVolumeSlider;
		
		public GameObject GameOverMenu;

		private void Start(){
			if (PlayerPrefs.HasKey(MasterVolumePlayerPrefName))
				LoadVolumes();
			else
				InitializeVolumes();
		}

		private void OnEnable(){
			GameManager.OnGameLost += GameOver;
			GameManager.OnRestart += Restart;
		}

		private void InitializeVolumes(){
			PlayerPrefs.SetFloat(MasterVolumePlayerPrefName, 1);
			PlayerPrefs.SetFloat(BGMVolumePlayerPrefName, 1);
			PlayerPrefs.SetFloat(SFXVolumePlayerPrefName, 1);
		}
		
		private void LoadVolumes(){
			float master, sfx, bgm;
			
			master = PlayerPrefs.GetFloat(MasterVolumePlayerPrefName);
			bgm = PlayerPrefs.GetFloat(BGMVolumePlayerPrefName);
			sfx = PlayerPrefs.GetFloat(SFXVolumePlayerPrefName);

			MasterVolumeSlider.value = master;
			BGMVolumeSlider.value = bgm;
			SFXVolumeSlider.value = sfx;
			
			AudioMixer.SetFloat("MasterVolume", Mathf.Log10(master) * 20);
			AudioMixer.SetFloat("BGMVolume", Mathf.Log10(bgm) * 20);
			AudioMixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);
		}

		private void OnDisable(){
			GameManager.OnGameLost -= GameOver;
			GameManager.OnRestart -= Restart;
			
			PlayerPrefs.Save();
		}

		public void StartGame(){
			MainMenuParent.SetActive(false);
			GameManager.StartGame();
			TimelineDirector.playableAsset = StartGameTimeline;
			TimelineDirector.Play();
		}

		public void ChangeVolumeMaster(float value){
			AudioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
			PlayerPrefs.SetFloat(MasterVolumePlayerPrefName, value);
		}
		
		public void ChangeVolumeBGM(float value){
			AudioMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
			PlayerPrefs.SetFloat(BGMVolumePlayerPrefName, value);
		}

		public void ChangeVolumeSFX(float value){
			AudioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
			PlayerPrefs.SetFloat(SFXVolumePlayerPrefName, value);
		}

		public void ExitGame(){
			#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
			#else
            Application.Quit();
			#endif
		}

		public void GameOver(){
			GameOverMenu.SetActive(true);
		}

		public void Restart(){
			MainMenuParent.SetActive(true);
			GameOverMenu.SetActive(false);
		}
	}
}