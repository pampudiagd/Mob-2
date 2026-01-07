using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

//Note: This includes the pause menus as well as submenus,
//which are as separate objects so their visibility can be toggled on and off.

public class PauseMenu : MonoBehaviour
{

    //Define variables

    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;


    //Get audio mixers for volume sliders

    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    //Serialize Field for TMP resolution dropdown
    //Could also use UnityEngine.UI but this seems to be a bit better
    [SerializeField] public TMP_Dropdown resolutionDropdown;

    //Resolution containers and variables
    public Resolution[] allResolutions;
    public List<Resolution> filteredResolutions;

    public RefreshRate currentRefreshRate;
    public int currentResolutionIndex = 0;



    //Resume and Pause functions

    public void Resume()
    {
        //deactivate pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        //deactivate options menu
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(false);
        }

        //Timescale and pause variable are normal
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    //Load Menu (Options button)
    public void LoadMenu()
    {

        // Show Options Menu
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Options Menu UI is not assigned in LoadMenu()!");
        }

        // Hide Pause Menu
        pauseMenuUI.SetActive(false);
    }

    //Quit Game (Quit button)
    public void QuitGame()
    {
        Debug.Log("Qutting game (works outside Unity editor)");
        Application.Quit();
    }

    //Options Menu function: Back (return to Pause Menu)
    public void Back()
    {
        //Hide Options Menu
        optionsMenuUI.SetActive(false);

        //Show Pause Menu
        pauseMenuUI.SetActive(true);
    }

    //Options Menu function: Music slider
    public void SetMusicVolume(float musicVolume)
    {
        //set the music mixer exposed parameter "musicVolume" to
        //the float musicVolume which we take as input from the volume slider
        musicMixer.SetFloat("musicVolume", musicVolume);
    }

    //Options Menu function: SFX slider
    public void SetSFXVolume(float sfxVolume)
    {
        //set the sfx mixer exposed parameter "sfxVolume" to
        //the float sfxVolume which we take as input from the volume slider
        sfxMixer.SetFloat("sfxVolume", sfxVolume);
    }

    //Set resolution
    //This uses the resolution stuff in Start().
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height,true); //boolean = fullscreen
    }

    // S T A R T
    // Start is called before the first frame update
    void Start()
    {
        //Assign pause and options menu UI if null
        if (pauseMenuUI == null)
        {
            Debug.LogError("At Start(), pauseMenuUI is not assigned in the Inspector!");
            pauseMenuUI = GameObject.Find("PauseMenu");

            // If still null, log further details
            if (pauseMenuUI == null)
            {
                Debug.LogError("pauseMenuUI is still null after GameObject.Find('PauseMenu')");
            }
            else
            {
                Debug.Log("Now PauseMenu's assigned.");
            }
        }
        else
        {
            Debug.Log("At Start(), pauseMenuUI successfully assigned.");
        }

        if (optionsMenuUI == null)
        {
            Debug.LogError("At Start(), optionsMenuUI is not assigned in the Inspector!");
            optionsMenuUI = GameObject.Find("OptionsMenu");

            // If still null, log further details
            if (optionsMenuUI == null)
            {
                Debug.LogError("optionsMenuUI is still null after GameObject.Find('OptionsMenu')");
            }
            else
            {
                Debug.Log("Now OptionMenu's assigned.");
            }
        }

        else
        {
            Debug.Log("At Start(), optionsMenuUI successfully assigned.");
        }
        //End Debug

        //Get player's screen's resolutions + refresh rate
        allResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions(); //this clears default options
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        //Filter resolutions so only applicable ones are present
        //(none for unsupported refresh rates)
        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].refreshRateRatio.value == currentRefreshRate.value)
            {
                filteredResolutions.Add(allResolutions[i]);
            }
        }

        //Add all applicable resolutions to a list
        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            //The following string is what the player sees
            string resOptString = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + (int)filteredResolutions[i].refreshRateRatio.value + " Hz";
            resolutionOptions.Add(resOptString);

            //Also, dynamically choose current resolution
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        //Finally, add these resolution variables to the dropdown
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Start game in unpaused state
        Resume();
    }

    // U P D A T E
    // Update is called once per frame
    void Update()
    {
        // Only check for pauseMenuUI if it's still null
        if (pauseMenuUI == null)
        {
            Debug.LogWarning("pauseMenuUI is null in Update(). Attempting to reassign.");
            pauseMenuUI = GameObject.Find("PauseMenu");

            // If it is still null, log that we couldn't find the object
            if (pauseMenuUI == null)
            {
                Debug.LogError("pauseMenuUI is still null in Update() after GameObject.Find('PauseMenu')");
            }
        }

        //Pause or Unpause by hitting escape (for now, later: remappable key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}