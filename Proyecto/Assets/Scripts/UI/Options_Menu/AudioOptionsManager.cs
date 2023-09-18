using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioOptionsManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    // Valores de los sliders del menú opciones
    [SerializeField] Slider generalVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    [SerializeField] Slider uIVolumeSlider;
    [SerializeField] Slider dialogueVolumeSlider;
    TextMeshProUGUI generalVolume_Slider_Text;
    TextMeshProUGUI musicVolume_Slider_Text;
    TextMeshProUGUI effectsVolume_Slider_Text;
    TextMeshProUGUI uIVolume_Slider_Text;
    TextMeshProUGUI dialogueVolume_Slider_Text;

    [SerializeField] float defaultMasterVolume = 0f;
    [SerializeField] float defaultSfxVolume = 0f;
    [SerializeField] float defaultMusicVolume = 0f; 
    [SerializeField] float defaultUiVolume = 0f;
    [SerializeField] float defaultVoiceVolume = 0f;

    void Awake()
    {
        GetReferences();
    }
    void Start()
    {
        InitializeVariables();

        SetGeneralVolume(PlayerPrefs.GetFloat("masterVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        SetSfxVolume(PlayerPrefs.GetFloat("sfxVolume"));
        SetUiVolume(PlayerPrefs.GetFloat("uiVolume"));
        SetDialogueVolume(PlayerPrefs.GetFloat("voiceVolume"));
    }

    public void SetGeneralVolume(float volume)
    {
        volume = generalVolumeSlider.value;
        //audioMixer.SetFloat("masterVolume", volume);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
        //generalVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("masterVolume")).ToString();
        generalVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("masterVolume") * 100 / generalVolumeSlider.maxValue).ToString() + "%";
    }
    public void SetMusicVolume(float volume)
    {
        volume = musicVolumeSlider.value;
        //audioMixer.SetFloat("musicVolume", volume);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
        //musicVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("musicVolume")).ToString();
        musicVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("musicVolume") * 100 / musicVolumeSlider.maxValue).ToString() + "%";
    }
    public void SetSfxVolume(float volume)
    {
        volume = effectsVolumeSlider.value;
        //audioMixer.SetFloat("sfxVolume", volume);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
        //effectsVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("sfxVolume")).ToString();
        effectsVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("sfxVolume") * 100 / effectsVolumeSlider.maxValue).ToString() + "%";
    }
    public void SetUiVolume(float volume)
    {
        volume = uIVolumeSlider.value;
        //audioMixer.SetFloat("uiVolume", volume);
        audioMixer.SetFloat("uiVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("uiVolume", volume);
        //uIVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("uiVolume")).ToString();
        uIVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("uiVolume") * 100 / uIVolumeSlider.maxValue).ToString() + "%";
    }
    public void SetDialogueVolume(float volume)
    {
        volume = dialogueVolumeSlider.value;
        //audioMixer.SetFloat("voiceVolume", volume);
        audioMixer.SetFloat("voiceVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("voiceVolume", volume);
        dialogueVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("voiceVolume") * 100 / dialogueVolumeSlider.maxValue).ToString() + "%";
    }

    public void Reset_AudioOptions()
    {
        generalVolumeSlider.value = defaultMasterVolume;
        //audioMixer.SetFloat("masterVolume", defaultMasterVolume);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(defaultMasterVolume) * 20);
        PlayerPrefs.SetFloat("masterVolume", defaultMasterVolume);

        musicVolumeSlider.value = defaultMusicVolume;
        //audioMixer.SetFloat("musicVolume", defaultMusicVolume);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(defaultMusicVolume) * 20);
        PlayerPrefs.SetFloat("musicVolume", defaultMusicVolume);

        effectsVolumeSlider.value = defaultSfxVolume;
        //audioMixer.SetFloat("sfxVolume", defaultSfxVolume);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(defaultSfxVolume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", defaultSfxVolume);

        uIVolumeSlider.value = defaultUiVolume;
        //audioMixer.SetFloat("uiVolume", defaultUiVolume);
        audioMixer.SetFloat("uiVolume", Mathf.Log10(defaultUiVolume) * 20);
        PlayerPrefs.SetFloat("uiVolume", defaultUiVolume);

        dialogueVolumeSlider.value = defaultVoiceVolume;
        //audioMixer.SetFloat("voiceVolume", defaultVoiceVolume);
        audioMixer.SetFloat("voiceVolume", Mathf.Log10(defaultVoiceVolume) * 20);
        PlayerPrefs.SetFloat("voiceVolume", defaultVoiceVolume);
    }


    void GetReferences()
    {
        generalVolume_Slider_Text = generalVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        musicVolume_Slider_Text = musicVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        effectsVolume_Slider_Text = effectsVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        uIVolume_Slider_Text = uIVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        dialogueVolume_Slider_Text = dialogueVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
    }

    void InitializeVariables()
    {
        LoadAudioSettings();
        InitializeSliderPercentajes_Text();
    }
    void LoadAudioSettings()         // Paso los valores de las opciones al "PlayerPrefs" para q no se pierdan con el cambio de escena
    {
        // GENERAL //
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            generalVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        }
        else
        {
            //PlayerPrefs.SetFloat("masterVolume", defaultMasterVolume);

            //SetGeneralVolume(defaultMasterVolume);

            PlayerPrefs.SetFloat("masterVolume", Mathf.Log10(defaultMasterVolume) * 20);
            generalVolumeSlider.value = defaultMasterVolume;
        }
        // MUSIC //
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }
        else
        {
            //PlayerPrefs.SetFloat("musicVolume", defaultMusicVolume);

            //SetMusicVolume(defaultMusicVolume);

            PlayerPrefs.SetFloat("musicVolume", Mathf.Log10(defaultMusicVolume) * 20);
            musicVolumeSlider.value = defaultMusicVolume;
        }
        // SFX //
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            effectsVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            //PlayerPrefs.SetFloat("sfxVolume", defaultSfxVolume);

            //SetMusicVolume(defaultSfxVolume);

            PlayerPrefs.SetFloat("sfxVolume", Mathf.Log10(defaultSfxVolume) * 20);
            effectsVolumeSlider.value = defaultSfxVolume;
        }
        // UI //
        if (PlayerPrefs.HasKey("uiVolume"))
        {
            uIVolumeSlider.value = PlayerPrefs.GetFloat("uiVolume");
        }
        else
        {
            //PlayerPrefs.SetFloat("uiVolume", defaultUiVolume);

            //SetMusicVolume(defaultUiVolume);

            PlayerPrefs.SetFloat("uiVolume", Mathf.Log10(defaultUiVolume) * 20);
            uIVolumeSlider.value = defaultUiVolume;
        }
        // DIALOGUES //
        if (PlayerPrefs.HasKey("voiceVolume"))
        {
            dialogueVolumeSlider.value = PlayerPrefs.GetFloat("voiceVolume");
        }
        else
        {
            //PlayerPrefs.SetFloat("voiceVolume", defaultVoiceVolume);

            //SetMusicVolume(defaultVoiceVolume);

            PlayerPrefs.SetFloat("voiceVolume", Mathf.Log10(defaultVoiceVolume) * 20);
            dialogueVolumeSlider.value = defaultVoiceVolume;
        }
    }

    void InitializeSliderPercentajes_Text()
    {
        //generalVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("masterVolume")).ToString();
        //musicVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("musicVolume")).ToString();
        //effectsVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("sfxVolume")).ToString();
        //uIVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("uiVolume")).ToString();
        //dialogueVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("voiceVolume")).ToString();

        generalVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("masterVolume") * 100 / generalVolumeSlider.maxValue).ToString() + "%";
        musicVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("musicVolume") * 100 / musicVolumeSlider.maxValue).ToString() + "%";
        effectsVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("sfxVolume") * 100 / effectsVolumeSlider.maxValue).ToString() + "%";
        uIVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("uiVolume") * 100 / uIVolumeSlider.maxValue).ToString() + "%";
        dialogueVolume_Slider_Text.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("voiceVolume") * 100 / dialogueVolumeSlider.maxValue).ToString() + "%";
    }
}
