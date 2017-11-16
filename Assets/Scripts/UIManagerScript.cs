using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {

    [SerializeField]
    private Text soundText;
    [SerializeField]
    private GameObject points;
    [SerializeField]
    private GameObject restartDialoge;
    [SerializeField]
    private GameObject startDialoge;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private AudioManager audioManager;


    void Start()
    {
        gameController = Transform.FindObjectOfType<GameController>();
        soundText.text = PlayerPrefs.GetString("Music") != "no" ? "SOUND : ON" : "SOUND : OFF";
    }

   
    public void StartGame()
    {
        audioManager.Play(AudioState.BtnClick); 
        gameController.isGame = true;
        gameController.isShift = true;
        points.SetActive(true);
        startDialoge.SetActive(false);
    }
    

    public void RestartGame()
    {
        audioManager.Play(AudioState.BtnClick);
        gameController.isGame = true;
        gameController.isShift = true;
        restartDialoge.SetActive(false);
    }    


    public void OnMenuBtnClick()
    {
        audioManager.Play(AudioState.BtnClick);
        SceneManager.LoadScene(0);             
    }


    public void OnSoundBtnClick()
    {
        audioManager.Play(AudioState.BtnClick);
        soundText.text = soundText.text == "SOUND : OFF" ? "SOUND : ON" : "SOUND : OFF";
        if(PlayerPrefs.GetString("Music") != "no")
        {
            PlayerPrefs.SetString("Music", "no");
        }
        else
        {
            PlayerPrefs.SetString("Music", "yes");
        }
    }
}
