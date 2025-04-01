using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class SettingLoader : MonoBehaviour
{
    [SerializeField] 
    private AudioMixer audioMixer;
    
    void Awake()
    {
        //Only for testing
        //PlayerPrefs.DeleteAll();
        Debug.developerConsoleVisible = true;
        
        LoadGeneralSetting();
        LoadAudioSetting();
        LoadGraphicSetting();
    }
    
    private void LoadGeneralSetting()
    {
        if (PlayerPrefs.HasKey(Constants.ScreenMode))
        {
            FullScreenMode screenMode = Screen.fullScreenMode;
            if (PlayerPrefs.GetInt(Constants.ScreenMode) == 0)
            {
                screenMode = FullScreenMode.Windowed;
            }
            else if (PlayerPrefs.GetInt(Constants.ScreenMode) == 1)
            {
                screenMode = FullScreenMode.FullScreenWindow;
            }
            else if (PlayerPrefs.GetInt(Constants.ScreenMode) == 2)
            {
                screenMode = FullScreenMode.ExclusiveFullScreen;
            }
            
            int width = PlayerPrefs.GetInt(Constants.Width);
            int height = PlayerPrefs.GetInt(Constants.Height);
            int refresh = PlayerPrefs.GetInt(Constants.Refresh);
            Screen.SetResolution(width, height, screenMode);
            Application.targetFrameRate = PlayerPrefs.GetInt(Constants.Refresh);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
            print("General Setting Loaded");
        }
    }

    private void LoadAudioSetting()
    {
        if (PlayerPrefs.HasKey(Constants.MasterVolume))
        {
            audioMixer.SetFloat(Constants.MasterVolume, PlayerPrefs.GetFloat(Constants.MasterVolume));
            audioMixer.SetFloat(Constants.MusicVolume, PlayerPrefs.GetFloat(Constants.MusicVolume));
            audioMixer.SetFloat(Constants.SoundVolume, PlayerPrefs.GetFloat(Constants.SoundVolume));
            print("Audio Setting Loaded");
        }
    }

    private void LoadGraphicSetting()
    {
        if (PlayerPrefs.HasKey(Constants.QualityLevel))
        {
            switch (PlayerPrefs.GetInt(Constants.QualityLevel))
            {
                case 0: QualitySettings.SetQualityLevel(0);
                        QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
                        break;
                case 1: QualitySettings.SetQualityLevel(1); 
                        QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
                        break;
                case 2: QualitySettings.SetQualityLevel(2); 
                        QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
                        break;
                case 3: QualitySettings.SetQualityLevel(3); 
                        QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
                        break;
                case 4: UniversalRenderPipelineAsset urpAsset = QualitySettings.GetRenderPipelineAssetAt(4)  as UniversalRenderPipelineAsset;
                        urpAsset.renderScale = PlayerPrefs.GetFloat(Constants.RenderScale);
                        urpAsset.mainLightShadowmapResolution = PlayerPrefs.GetInt(Constants.ShadowResolution);
                        urpAsset.shadowDistance = PlayerPrefs.GetInt(Constants.ShadowDistance);
                        urpAsset.msaaSampleCount = PlayerPrefs.GetInt(Constants.AntiAliasing);
                        QualitySettings.SetQualityLevel(4);
                        QualitySettings.globalTextureMipmapLimit = PlayerPrefs.GetInt(Constants.TextureQuality);
                        QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
                        break;
            }
        }
    }
    
    void Update()
    {
        
    }
}