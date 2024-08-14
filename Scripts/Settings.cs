using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject MainMenu, How2Play;
    [SerializeField] GameObject[] gameControlsSlides;

    [SerializeField] AudioSource Music;
    [SerializeField] AudioSource[] SFX;

    [SerializeField] float sfxVolume = 0.5f, musicVolume = 0.5f;
    [SerializeField] int numberOfSlides = 3, tutorialIndex = 0;

    [SerializeField] Slider musicSlider, sfxSlider;

    void Start()
    {
        BackToMainMenu();
    }

    void Update()
    {
        if (How2Play.activeSelf)
        {
            SetPrefSettings();

            for (int i = 0; i < gameControlsSlides.Length; ++i)
            {
                if (i == tutorialIndex)
                    gameControlsSlides[i].SetActive(true);
                else
                    gameControlsSlides[i].SetActive(false);
            }
        } else
            {
                UsePrefSettings();

                for (int i = 0; i < gameControlsSlides.Length; ++i)
                    gameControlsSlides[i].SetActive(false);
            }

        // Set the sound volume
        Music.volume = musicVolume;
        for (int i = 0; i < SFX.Length; ++i)
        {
            SFX[i].volume = sfxVolume;
        }
    }

    public void Next()
    {
        ++tutorialIndex;
        if (tutorialIndex >= numberOfSlides)
        {
            tutorialIndex = 0;
        }
    }

    public void Prev()
    {
        --tutorialIndex;
        if (tutorialIndex < 0)
        {
            tutorialIndex = numberOfSlides - 1;
        }
    }

    // Player Pref Settings
    public void UsePrefSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
    }

    public void SetPrefSettings()
    {
        musicVolume = musicSlider.value;
        sfxVolume = sfxSlider.value;

        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    // Screen Transitions
    public void BackToMainMenu()
    {
        MainMenu.SetActive(true);
        How2Play.SetActive(false);
    }

    public void ShowGameControls()
    {
        MainMenu.SetActive(false);

        // Set the Sliders before how2play gets enabled
        // and set the sliders accordingly to not undo
        // player sound settings
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.5f);
        
        How2Play.SetActive(true);
    }
}//EndScript