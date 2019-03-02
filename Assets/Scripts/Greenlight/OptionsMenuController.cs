using Scorch_SceneObject;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityStandardAssets.Characters.FirstPerson; //required to update mouse sensitivity
using System;
public class OptionsMenuController : MonoBehaviour
{
    //the sliders need to be serialized so that they can be updated in Start() with PlayerPrefs
    [SerializeField]
    Slider volumeMasterSlider, volumeSFXSlider, volumeVoiceSlider, volumeMusicSlider, volumeAtmoSlider, fovSlider, mouseSensSlider;

    [SerializeField]
    Text fovValueText, mouseSensText;
    [SerializeField]
    AudioMixer masterMixer;

    [SerializeField]
    Dropdown subtitlesDropdown;
    [SerializeField]
    Toggle subtitles;
    [SerializeField]
    Text subtitleText;
    private bool language;

    private bool inGame;
    private bool subtitleVisibility = false;

    private string subtitleLanguage;

    int testiterator = 0;
    // Use this for initialization
    void Start()
    {
        //Initialize subtitle visibility based on the previous selection in the drop down menu (Main Menu)
        int playerprefsub = PlayerPrefs.GetInt("language", 0); //If nothing's set Playerpref int is 0 -> no language
        if (playerprefsub == 0)
        {
            subtitleVisibility = false;
        }
        else
        {
            subtitleVisibility = true;
        }

        //Check if playerprefs exist and update sliders
        volumeMasterSlider.value = PlayerPrefs.GetFloat("VolumeMaster", 50f);
        volumeSFXSlider.value = PlayerPrefs.GetFloat("VolumeSFX", 50f);
        volumeVoiceSlider.value = PlayerPrefs.GetFloat("VolumeVoice", 50f);
        volumeMusicSlider.value = PlayerPrefs.GetFloat("VolumeMusic", 50f);
        volumeAtmoSlider.value = PlayerPrefs.GetFloat("VolumeAtmo", 50f);
        fovSlider.value = PlayerPrefs.GetFloat("FOV", 60f);

        float mouseSens = PlayerPrefs.GetFloat("MouseSens", 2f);
        mouseSens = Mathf.Round(100 * mouseSens) / 100;
        mouseSensSlider.value = mouseSens;

        //Subtitle Toggle
        subtitles.isOn = subtitleVisibility;

        fovValueText.text = fovSlider.value.ToString();

        string mouseSensString = mouseSensSlider.value.ToString();
        mouseSensText.text = AppendToNPlaces(mouseSensString, 4);

        Camera.main.fieldOfView = fovSlider.value;

    }


    public void SetMasterVolume(float newValue)
    {
        float mappedVal;
        mappedVal = ConvertToDecibel(newValue);

        PlayerPrefs.SetFloat("VolumeMaster", newValue);
        masterMixer.SetFloat("masterVol", mappedVal);
        Debug.Log("I belive the new master volume to be " + mappedVal.ToString());
    }


    public void SetSFXVolume(float newValue)
    {
        float mappedVal;
        mappedVal = ConvertToDecibel(newValue);

        PlayerPrefs.SetFloat("VolumeSFX", newValue);
        masterMixer.SetFloat("sfxVol", mappedVal);
        Debug.Log("I belive the new SFX Volume to be " + mappedVal.ToString());

    }

    public void SetVoiceVolume(float newValue)
    {
        float mappedVal;
        mappedVal = ConvertToDecibel(newValue);

        PlayerPrefs.SetFloat("VolumeVoice", newValue);
        masterMixer.SetFloat("voiceactingVol", mappedVal);
        Debug.Log("I belive the new voice volume to be " + mappedVal.ToString());
    }

    public void SetMusicVolume(float newValue)
    {
        float mappedVal;
        mappedVal = ConvertToDecibel(newValue);

        PlayerPrefs.SetFloat("VolumeMusic", newValue);
        masterMixer.SetFloat("musicVol", mappedVal);
        Debug.Log("I belive the new music volume to be " + mappedVal.ToString());
    }


    public void SetAmbientVolume(float newValue)
    {
        float mappedVal;
        mappedVal = ConvertToDecibel(newValue);

        PlayerPrefs.SetFloat("VolumeAtmo", newValue);
        masterMixer.SetFloat("ambientVol", mappedVal);
        Debug.Log("I belive the new ambient volume to be " + mappedVal.ToString());
    }
    public void SetFov(float newValue)
    {
        PlayerPrefs.SetFloat("FOV", newValue);
        Camera.main.fieldOfView = newValue; //changes the main camera's fov
        fovValueText.text = newValue.ToString();
        Debug.Log("I belive the new fov to be " + newValue.ToString());
    }

    public void SetMouseSensitivity(float newValue)
    {
        PlayerPrefs.SetFloat("MouseSens", newValue);

        //The following lines are terribly written but really fast (Optimization)
        if (Camera.main.transform.parent != null)
        {
            Camera.main.transform.parent.GetComponent<FirstPersonController>().m_MouseLook.XSensitivity = newValue;
            Camera.main.transform.parent.GetComponent<FirstPersonController>().m_MouseLook.YSensitivity = newValue;
        }
        if (newValue == 5)
        {
            mouseSensText.text = "FAST";
        }
        else
        {
            newValue = Mathf.Round(newValue * 100f) / 100f;
            string mouseSensString = newValue.ToString();
            mouseSensText.text = AppendToNPlaces(mouseSensString, 4);
        }

        Debug.Log("I belive the new mouse sens to be " + newValue.ToString());
    }

    public void SaveAllPlayerPrefs()
    {
        PlayerPrefs.Save();
        Debug.Log("All player prefernces were saved");
    }

    private float ConvertToDecibel(float normalizedValue) //Converts float value to dB Value -> Necessary for Audiomixer Slider
    {
        float decibelValue;
        decibelValue = -48f * (1f - normalizedValue);
        return decibelValue;
    }



    //Turn subtitles on and off 
    public void ToggleSubtitlesOnAndOff()
    {
        subtitleVisibility = subtitles.isOn;
        PlayerPrefs.SetInt("subtitles", ConvertBoolToInt(subtitleVisibility));// Subtitlevisibility when toggling

        if (subtitleText != null)
        {
            Color col = subtitleText.color;
            if (subtitleVisibility)
            {
                col.a = 1.0f;
            }
            else
            {

                col.a = 0.0f;

            }
            subtitleText.color = col;

        }
        //   Debug.Log("I believe that subtitles are available " + subtitleVisibility);
    }

    //Subtitle DropDown
    public void SetLanguage()
    {
        int language = subtitlesDropdown.value;
        PlayerPrefs.SetInt("language", language); //0 = none, 1 = english, etc

        switch (language)
        {
            case 0:
                subtitleLanguage = "none";
                break;
            case 1:
                subtitleLanguage = "english";
                break;
            case 2:
                subtitleLanguage = "german";
                break;
            case 3:
                subtitleLanguage = "spanish";
                break;
            default:
                subtitleLanguage = "none";
                break;
        }

        //if (Subtitles.Instance != null)
        //{
        //    Subtitles.Instance.SetLanguage(language);
        //}
    }


    private int ConvertBoolToInt(bool toggle) //Casting did not work. therefore an own function for it
    {
        if (toggle)
        { return 1; }
        else { return 0; }
    }


    //Appends a zeros to N places if shorter than n
    public string AppendToNPlaces(string str, int n)
    {
        while (str.Length < n)
        {
            str = str + "0";
        }

        return str;
    }
}
