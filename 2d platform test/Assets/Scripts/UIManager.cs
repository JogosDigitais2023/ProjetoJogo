using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image imageMage;
    [SerializeField] private Image imageSlinger;
    [SerializeField] private Image imageWarrior;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject options;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void SetSelecterCharacter(PlayerController player)
    {
        // switch (characterState)
        // {
        //     case player.CharacterState.mage:
        //         imageMage.color = new Color(0,1,0);
        //         imageSlinger.color = new Color(1,1,1);
        //         imageWarrior.color = new Color(1,1,1);
        //         break;
        //     case player.CharacterState.slinger:
        //         imageMage.color = new Color(1,1,1);
        //         imageSlinger.color = new Color(0,1,0);
        //         imageWarrior.color = new Color(1,1,1);
        //         break;
        //     case player.CharacterState.warrior:
        //         imageMage.color = new Color(1,1,1);
        //         imageSlinger.color = new Color(0,1,0);
        //         imageWarrior.color = new Color(1,1,1);
        //         break;
        // }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
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

}
