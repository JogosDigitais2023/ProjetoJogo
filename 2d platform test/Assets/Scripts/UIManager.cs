using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject pause;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Update()
    {
        PauseGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Debug.Log("exit");
        Application.Quit();
    }
    public void Options()
    {
        options.gameObject.SetActive(true);
    }

    public void CloseOptions()
    {
        options.gameObject.SetActive(false);
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume(float volume)
    {
        AudioManager.Instance.MusicVolume(musicSlider.value);
    }

    public void SFXVolume(float volume)
    {
        AudioManager.Instance.SFXVolume(sfxSlider.value);
    }

    public void PauseGame()
    {
        if (!SceneManager.GetActiveScene().Equals("Menu"))
        {
            if (!pause.gameObject.activeSelf)
            {
                Time.timeScale = 1f;
                if (Input.GetKeyDown("escape"))
                {
                    pause.gameObject.SetActive(true);
                }
            }
            else
            {
                Time.timeScale = 0f;
                if (Input.GetKeyDown("escape"))
                {
                    pause.gameObject.SetActive(false);
                    options.gameObject.SetActive(false);
                }
            }
        }
        
    }
    public void UnpauseGame()
    {
        Debug.Log("unpause");
        pause.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
    }
}
