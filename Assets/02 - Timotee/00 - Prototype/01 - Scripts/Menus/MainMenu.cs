using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour{
    public Canvas MainMenuCanvas;

    public AudioMixer AudioMixer;
    public PlayableDirector TimelineDirector;
    public PlayableAsset StartGameTimeline;
    
    public void StartGame(){
        MainMenuCanvas.gameObject.SetActive(false);
        GameManager.StartGame();
        Debug.Log("Play");
        TimelineDirector.playableAsset = StartGameTimeline;
        TimelineDirector.Play();
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
}
