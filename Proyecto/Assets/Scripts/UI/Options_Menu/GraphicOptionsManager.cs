using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class GraphicOptionsManager : MonoBehaviour
{    
    [SerializeField] TMP_Dropdown qualityDropdown;    // Calidad Visual        
    [SerializeField] RenderPipelineAsset[] qualityLevels;
    [SerializeField] TMP_Dropdown resolutionDropdown; // Resolución
    [SerializeField] TMP_Dropdown screenModeDropdown; // Fullscreen    
    [SerializeField] Slider brightnessSlider; // Brillo
    [SerializeField] Image brightnesspanel;
    [SerializeField] Slider fovSlider;
    [SerializeField] Slider fpsLimitSlider;
    [SerializeField] TMP_Dropdown vSyncDropdown;
    TextMeshProUGUI brightness_Slider_Text;
    TextMeshProUGUI fov_Slider_Text;
    TextMeshProUGUI fpsLimit_Slider_Text;
    TextMeshProUGUI vSync_Dropdown_Text;
    new Camera camera;

    Resolution[] resolutions;
    List<Resolution> filteredReslutions;
    float currentRefreshRate;
    int currentResolutionIndex = 0;
    protected float brightness;

    // Default options
    int screenMode_default = 0;
    int resolution_default;
    int quality_default;
    int fov_default = 90;
    float brightness_default = 0;
    int fpsLimit_default = 240;
    int defaultVSync_default = 1;

    int currentScreenMode;

    void Awake()
    {
        GetReferences();
    }
    void Start()
    {
        InitializeVariables();
        //Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        //QualitySettings.renderPipeline = qualityLevels[PlayerPrefs.GetInt("quality")];
        //ChangeScreenMode(PlayerPrefs.GetInt("screenMode"));
        //SetBrightness(PlayerPrefs.GetFloat("brightness"));
        //camera.fieldOfView = PlayerPrefs.GetInt("fov");
        //Application.targetFrameRate = PlayerPrefs.GetInt("fpsLimit");
        //QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync");
    }

    public void SetResolution(int value)
    {
        currentResolutionIndex = value;
        PlayerPrefs.SetInt("resolution", value);
        ApplyCurrentResolution();
        SetScreenMode(currentScreenMode);
    }
    void ApplyCurrentResolution() => ApplyResolution(resolutions[currentResolutionIndex]);

    void ApplyResolution(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", currentResolutionIndex);
    }

    /// Control visual
    public void SetQuality(int value)
    {
        value = qualityDropdown.value;
        PlayerPrefs.SetInt("quality", value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }
    public void SetScreenMode(int value)
    {
        value = screenModeDropdown.value;
        PlayerPrefs.SetInt("screenMode", value);
        ChangeScreenMode(value);
    }
    void ChangeScreenMode(int value)
    {
        currentScreenMode = value;
        if (value == 0) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        if (value == 1) Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        if (value == 2) Screen.fullScreenMode = FullScreenMode.Windowed;
    }
    public void SetBrightness(float value)
    {
        value = brightnessSlider.value;
        PlayerPrefs.SetFloat("brightness", value);
        brightnesspanel.color = new Color(brightnesspanel.color.r, brightnesspanel.color.g, brightnesspanel.color.b, value);
        brightness_Slider_Text.text = PlayerPrefs.GetFloat("brightness").ToString("F2");
    }
    public void SetFOV(int value)
    {
        value = (int)fovSlider.value;
        PlayerPrefs.SetInt("fov", value);
        camera.fieldOfView = value;
        fov_Slider_Text.text = PlayerPrefs.GetInt("fov").ToString();
    }
    public void SetFpsLimit(int value)
    {
        value = (int)fpsLimitSlider.value;
        PlayerPrefs.SetInt("fpsLimit", value);
        Application.targetFrameRate = value;
        fpsLimit_Slider_Text.text = PlayerPrefs.GetInt("fpsLimit").ToString();
    }
    public void EnableVSync(int value)
    {
        if (value == 0)     QualitySettings.vSyncCount = 0;
        if (value == 1)     QualitySettings.vSyncCount = 1;
        PlayerPrefs.SetInt("vSync", value);
    }

    int BoolToInt(bool val) => val ? 1 : 0;
    bool IntToBool(int val) => val == 1;

    void GetReferences()
    {
        camera = Camera.main;

        brightness_Slider_Text = brightnessSlider.GetComponentInChildren<TextMeshProUGUI>();
        fov_Slider_Text = fovSlider.GetComponentInChildren<TextMeshProUGUI>();
        fpsLimit_Slider_Text = fpsLimitSlider.GetComponentInChildren<TextMeshProUGUI>();
        vSync_Dropdown_Text = vSyncDropdown.GetComponentInChildren<TextMeshProUGUI>();
    }
    void InitializeVariables()
    {
        InitializeResolutionsDropdown();
        InitializeQualityDropdown();
        LoadGraphicSettings();
        InitializeSliderPercentajes_Text();
    }

    void InitializeResolutionsDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(resolutionOption);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolution_default = resolutions.Length;
    }

    void InitializeQualityDropdown()
    {
        quality_default = qualityLevels.Length;
    }

    void InitializeFrameRate()
    {
        filteredReslutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                //string resolutionOption = filteredReslutions[i].width + "x" + filteredReslutions[i].height + " " + filteredReslutions[i].refreshRate + " Hz";
                filteredReslutions.Add(resolutions[i]);
            }


        List<string> options = new List<string>();
        for (int i = 0; i < filteredReslutions.Count; i++)
        {
            string resolutionOption = filteredReslutions[i].width + "x" + filteredReslutions[i].height + " " + filteredReslutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);
            if (filteredReslutions[i].width == Screen.width && filteredReslutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    void LoadGraphicSettings()         // Paso los valores de las opciones al "PlayerPrefs" para q no se pierdan con el cambio de escena
    {
        if (PlayerPrefs.HasKey("screenMode"))
            screenModeDropdown.value = PlayerPrefs.GetInt("screenMode");
        else
        {
            PlayerPrefs.SetInt("screenMode", screenMode_default);
            screenModeDropdown.value = PlayerPrefs.GetInt("screenMode");
            currentScreenMode = PlayerPrefs.GetInt("screenMode");
        }
        //
        if (PlayerPrefs.HasKey("resolution"))
            resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
        else
        {
            PlayerPrefs.SetInt("resolution", resolution_default);
            resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
        }
        //
        if (PlayerPrefs.HasKey("quality"))
            qualityDropdown.value = PlayerPrefs.GetInt("quality");
        else
        {
            PlayerPrefs.SetInt("quality", quality_default);
            qualityDropdown.value = PlayerPrefs.GetInt("quality");
        }
        //
        if (PlayerPrefs.HasKey("fov"))
            fovSlider.value = PlayerPrefs.GetInt("fov");
        else
        {
            PlayerPrefs.SetInt("fov", fov_default);
            fovSlider.value = fov_default;
        }
        //
        if (PlayerPrefs.HasKey("brightness"))
            brightnessSlider.value = PlayerPrefs.GetFloat("brightness");
        else
        {
            PlayerPrefs.SetFloat("brightness", brightness_default);
            brightnessSlider.value = brightness_default;
        }
        //
        if (PlayerPrefs.HasKey("fpsLimit"))
            fpsLimitSlider.value = PlayerPrefs.GetInt("fpsLimit");
        else
        {
            PlayerPrefs.SetInt("fpsLimit", fpsLimit_default);
            fpsLimitSlider.value = fpsLimit_default;
        }
        //
        if (PlayerPrefs.HasKey("vSync"))
            vSyncDropdown.value = PlayerPrefs.GetInt("vSync");
        else
        {
            PlayerPrefs.SetInt("vSync", defaultVSync_default);
            vSyncDropdown.value = defaultVSync_default;
        }
        //
    }

    void InitializeSliderPercentajes_Text()
    {
        brightness_Slider_Text.text = PlayerPrefs.GetFloat("brightness").ToString("F2");
        fov_Slider_Text.text = PlayerPrefs.GetInt("fov").ToString();
        fpsLimit_Slider_Text.text = PlayerPrefs.GetInt("fpsLimit").ToString();
        //vSync_Dropdown_Text.text = PlayerPrefs.GetInt("vSync").ToString();
    }

    public void Reset_GraphicOptions()
    {
        PlayerPrefs.SetInt("screenMode", screenMode_default);
        screenModeDropdown.value = PlayerPrefs.GetInt("screenMode");

        PlayerPrefs.SetInt("resolution", resolution_default);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution");

        PlayerPrefs.SetInt("quality", quality_default);
        qualityDropdown.value = PlayerPrefs.GetInt("quality");

        PlayerPrefs.SetInt("fov", fov_default);
        fovSlider.value = fov_default;

        PlayerPrefs.SetFloat("brightness", brightness_default);
        brightnessSlider.value = brightness_default;

        PlayerPrefs.SetInt("fpsLimit", fpsLimit_default);
        fpsLimitSlider.value = fpsLimit_default;

        PlayerPrefs.SetInt("vSync", defaultVSync_default);
        vSyncDropdown.value = defaultVSync_default;
    }
}
