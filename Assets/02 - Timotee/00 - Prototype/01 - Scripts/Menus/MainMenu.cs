using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour{
    public GameObject MainMenuParent;
    public Canvas MainMenuCanvas;

    public AudioMixer AudioMixer;
    public PlayableDirector TimelineDirector;
    public PlayableAsset StartGameTimeline;

    public GameObject GameOverMenu;
    
    public void StartGame(){
        MainMenuParent.SetActive(false);
        GameManager.StartGame();
        TimelineDirector.playableAsset = StartGameTimeline;
        TimelineDirector.Play();
    }

    private void OnEnable(){
        GameManager.OnGameLost += GameOver;
        GameManager.OnRestart += Restart;
    }

    private void OnDisable(){
        GameManager.OnGameLost -= GameOver;
        GameManager.OnRestart -= Restart;
    }

    public void ChangeVolumeBGM(float value){
        AudioMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
    }

    public void ChangeVolumeSFX(float value){
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    public void ChangeVolumeMaster(float value){
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
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
