using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;
using Toggle = UnityEngine.UIElements.Toggle;

public class OptionsControl : AbstractUIControl
{
    private ScrollView _scrollView;
    private Button _backButton;
    private Button _applyButton;
    private ListSelectorElement _screenModeSelector;
    private ListSelectorElement _resolutionSelector;
    private ListSelectorElement _framerateSelector;
    private Toggle _vSync;
    private Slider _masterAudio;
    private Slider _musicAudio;
    private Slider _soundAudio;
    private ListSelectorElement _graphicProfileSelector;
    private ListSelectorElement _antiAliasingSelector;
    private Slider _renderScale;
    private ListSelectorElement _shadowResolutionSelector;
    private ListSelectorElement _shadowDistanceSelector;
    private ListSelectorElement _textureQualitySelector;

    [SerializeField] 
    private UIControlRoot uiControlRoot;
    
    [SerializeField] 
    private AudioMixer audioMixer;
    
    private List<string> _resList = new List<string>();
    private List<string> _refreshList = new List<string>();
    private bool _areSettingChanged;
    private bool _updatingAfterApply;
    
    void Start()
    {
        InitializeElements();
        InitializeNavPath();
        defaultFocus = _screenModeSelector.RootElement();
        
        rootElement.RegisterCallback<GeometryChangedEvent>(OnVisibilityChange);
    }

    protected override void Update()
    {
        base.Update();
        
        if(_areSettingChanged && !_applyButton.enabledInHierarchy)
            _applyButton.SetEnabled(true);
        else if (!_areSettingChanged && _applyButton.enabledInHierarchy)
        {
            _backButton.Focus();
            lastFocus = _backButton;
            _applyButton.SetEnabled(false);
        }
    }

    private void OnVisibilityChange(GeometryChangedEvent evt)
    {
        UpdateView();
    }

    protected override void InitializeElements()
    {
        _scrollView = rootElement.Q<ScrollView>("ScrollView");
        _backButton = rootElement.Q<Button>("BackButton");
        _applyButton = rootElement.Q<Button>("ApplyButton");
        _screenModeSelector = new ListSelectorElement(rootElement.Q<VisualElement>("ScreenModeSelector"));
        _resolutionSelector = new ListSelectorElement(rootElement.Q<VisualElement>("ResolutionSelector"));
        _framerateSelector = new ListSelectorElement(rootElement.Q<VisualElement>("FramerateSelector"));
        _vSync = rootElement.Q<Toggle>("VSync");
        _graphicProfileSelector = new ListSelectorElement(rootElement.Q<VisualElement>("GraphicProfileSelector"));
        _antiAliasingSelector = new ListSelectorElement(rootElement.Q<VisualElement>("AntiAliasingSelector"));
        _renderScale = rootElement.Q<Slider>("RenderScale");
        _shadowResolutionSelector = new ListSelectorElement(rootElement.Q<VisualElement>("ShadowResolutionSelector"));
        _shadowDistanceSelector = new ListSelectorElement(rootElement.Q<VisualElement>("ShadowDistanceSelector"));
        _textureQualitySelector = new ListSelectorElement(rootElement.Q<VisualElement>("TextureQualitySelector"));

        _masterAudio = rootElement.Q<Slider>("MasterAudio");
        _musicAudio = rootElement.Q<Slider>("MusicAudio");
        _soundAudio = rootElement.Q<Slider>("SoundAudio");
        
        _screenModeSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_screenModeSelector.RootElement());});
        _resolutionSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_resolutionSelector.RootElement());});
        _framerateSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_framerateSelector.RootElement());});
        _vSync.RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_vSync);});
        _masterAudio.RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_masterAudio);});
        _musicAudio.RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_musicAudio);});
        _soundAudio.RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_soundAudio);});
        _graphicProfileSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_graphicProfileSelector.RootElement());});
        _antiAliasingSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_antiAliasingSelector.RootElement());});
        _renderScale.RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_renderScale);});
        _shadowResolutionSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_shadowResolutionSelector.RootElement());});
        _shadowDistanceSelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_shadowDistanceSelector.RootElement());});
        _textureQualitySelector.RootElement().RegisterCallback<FocusInEvent>(evt => { _scrollView.ScrollTo(_textureQualitySelector.RootElement());});
        
        _backButton.clicked += uiControlRoot.GoToPrevious;
        _applyButton.clicked += Apply;
        
        _vSync.RegisterCallback<ChangeEvent<bool>>(OnToggleChange);
        _screenModeSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _resolutionSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _framerateSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _masterAudio.RegisterCallback<ChangeEvent<float>>(OnSliderChange);
        _musicAudio.RegisterCallback<ChangeEvent<float>>(OnSliderChange);
        _soundAudio.RegisterCallback<ChangeEvent<float>>(OnSliderChange);
        _graphicProfileSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _antiAliasingSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _renderScale.RegisterCallback<ChangeEvent<float>>(OnSliderChange);
        _shadowResolutionSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _shadowDistanceSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _textureQualitySelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnStringChanged);
        _graphicProfileSelector.RootElement().RegisterCallback<ChangeEvent<string>>(OnQualityLevelChange);

        InitializeScreenModeSelector();
        InitializeResolutions();
        InitializeFramerate();
        InitializeGraphicProfiles();
        InitializeAntiaAliasing();
        InitializeShadowResolution();
        InitializeShadowDistance();
        InitializeTextureQuality();
        UpdateView();
    }

    private void UpdateView()
    {
        SelectCurrentSetting();
        _areSettingChanged = false;
    }

    private void SelectCurrentSetting()
    {
        //General Screen setting
        if (PlayerPrefs.HasKey(Constants.ScreenMode))
        {
            switch (PlayerPrefs.GetInt(Constants.ScreenMode))
            {
                case 0: _screenModeSelector.SetValue(Constants.Windowed); break;
                case 1: _screenModeSelector.SetValue(Constants.WindowedNoBorder); break;
                case 2: _screenModeSelector.SetValue(Constants.Fullscreen); break;
            }
        }
        else
        {
            switch (Screen.fullScreenMode)
            {
                case FullScreenMode.Windowed: _screenModeSelector.SetValue(Constants.Windowed); break;
                case FullScreenMode.FullScreenWindow: _screenModeSelector.SetValue(Constants.WindowedNoBorder); break;
                case FullScreenMode.ExclusiveFullScreen: _screenModeSelector.SetValue(Constants.Fullscreen); break;
            } 
        }

        if (PlayerPrefs.HasKey(Constants.Width))
        {
            _resolutionSelector.SetValue(PlayerPrefs.GetInt(Constants.Width) + "x" + PlayerPrefs.GetInt(Constants.Height));
        }
        else
        {
            if (Screen.fullScreenMode == FullScreenMode.Windowed)
            {
                _resolutionSelector.SetValue(Screen.width + "x" + Screen.height);
            }
            else
            {
                _resolutionSelector.SetValue(Screen.currentResolution.width + "x" + Screen.currentResolution.height);
            }
        }
        
        
        #if UNITY_EDITOR
            _resolutionSelector.SetValue(Screen.currentResolution.width + "x" + Screen.currentResolution.height);
        #endif
        
        if(Application.targetFrameRate == -1)
            _framerateSelector.SetValue(Constants.RefreshUnlimited);
        else
            _framerateSelector.SetValue(Application.targetFrameRate + "");
        
        if (QualitySettings.vSyncCount == 0)
        {
            _vSync.value = false;
        }
        else
        {
            _vSync.value = true;
        }

        //Audio
        float audioVol;
        audioMixer.GetFloat(Constants.MasterVolume, out audioVol);
        _masterAudio.value = Mathf.Pow(10, audioVol / 20);
        
        audioMixer.GetFloat(Constants.MusicVolume, out audioVol);
        _musicAudio.value = Mathf.Pow(10, audioVol / 20);
        
        audioMixer.GetFloat(Constants.SoundVolume, out audioVol);
        _soundAudio.value = Mathf.Pow(10, audioVol / 20);
        
        //Graphics
        switch (QualitySettings.GetQualityLevel())
        {
            case 0: _graphicProfileSelector.SetValue(Constants.LowProfile); break;
            case 1: _graphicProfileSelector.SetValue(Constants.MediumProfile); break;
            case 2: _graphicProfileSelector.SetValue(Constants.HighProfile); break;
            case 3: _graphicProfileSelector.SetValue(Constants.UltraProfile); break;
            case 4: _graphicProfileSelector.SetValue(Constants.CustomProfile); break;
        }
        
        UniversalRenderPipelineAsset urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
        _renderScale.value = urpAsset.renderScale;
        
        print("Antialiasing is " + urpAsset.msaaSampleCount);
        switch (urpAsset.msaaSampleCount)
        {
            case 1: _antiAliasingSelector.SetValue(Constants.AntiAliasingDisable); break;
            case 2: _antiAliasingSelector.SetValue(Constants.AntiAliasingX2); break;
            case 4: _antiAliasingSelector.SetValue(Constants.AntiAliasingX4); break;
            case 8: _antiAliasingSelector.SetValue(Constants.AntiAliasingX8); break;
        }

        switch (urpAsset.mainLightShadowmapResolution)
        {
            case 512: _shadowResolutionSelector.SetValue(Constants.LowProfile); break;
            case 1024: _shadowResolutionSelector.SetValue(Constants.MediumProfile); break;
            case 2048: _shadowResolutionSelector.SetValue(Constants.HighProfile); break;
            case 4096: _shadowResolutionSelector.SetValue(Constants.UltraProfile); break;
        }

        switch (urpAsset.shadowDistance)
        {
            case 10: _shadowDistanceSelector.SetValue(Constants.LowProfile); break;
            case 25: _shadowDistanceSelector.SetValue(Constants.MediumProfile); break;
            case 40: _shadowDistanceSelector.SetValue(Constants.HighProfile); break;
            case 50: _shadowDistanceSelector.SetValue(Constants.UltraProfile); break;
        }
        
        switch (QualitySettings.globalTextureMipmapLimit)
        {
            case 2: _textureQualitySelector.SetValue(Constants.LowProfile); break;
            case 1: _textureQualitySelector.SetValue(Constants.MediumProfile); break;
            case 0: _textureQualitySelector.SetValue(Constants.HighProfile); break;
            
        }
    }
    
    private void InitializeScreenModeSelector()
    {
        print("InitializeScreenModeSelector");
        _screenModeSelector.RootElement().Q<Label>("Name").text = "Screen Mode";
        List<string> screenModeList = new List<string>();
        screenModeList.Add(Constants.Windowed);
        screenModeList.Add(Constants.WindowedNoBorder);
        screenModeList.Add(Constants.Fullscreen);
        _screenModeSelector.InitializeListSelector(screenModeList);
    }

    private void OnQualityLevelChange(ChangeEvent<string> evt)
    {
        print("***** QualityLevelChanged");
        if (evt.newValue.Equals(Constants.CustomProfile))
        {
            print("***** ******** Custom Profile");
            _antiAliasingSelector.RootElement().SetEnabled(true);
            _renderScale.SetEnabled(true);
            _shadowResolutionSelector.RootElement().SetEnabled(true);
            _shadowDistanceSelector.RootElement().SetEnabled(true);
            _textureQualitySelector.RootElement().SetEnabled(true);
        }
        else
        {
            print("***** ******** Standard Profile");
            _antiAliasingSelector.RootElement().SetEnabled(false);
            _renderScale.SetEnabled(false);
            _shadowResolutionSelector.RootElement().SetEnabled(false);
            _shadowDistanceSelector.RootElement().SetEnabled(false);
            _textureQualitySelector.RootElement().SetEnabled(false);
        }
    }

    private void OnToggleChange(ChangeEvent<bool> evt)
    {
        //print("Toggle state changed: " + evt.newValue);
        _framerateSelector.RootElement().SetEnabled(!evt.newValue);
        _areSettingChanged = true;
    }

    private void OnSliderChange(ChangeEvent<float> evt)
    {
        if(_updatingAfterApply)
            return;
        
        _areSettingChanged = true;
    }

    private void OnStringChanged(ChangeEvent<string> evt)
    {
        if(_updatingAfterApply)
            return;
        
        _areSettingChanged = true;
    }

    private void InitializeResolutions()
    {
        _resolutionSelector.RootElement().Q<Label>("Name").text = "Resolution";
        foreach (Resolution res in Screen.resolutions)
        {
            //print(res);
            string resString = res.width + "x" + res.height;
            
            if (!_resList.Contains(resString))
            { 
                _resList.Add(resString); 
            }
        }
        _resolutionSelector.InitializeListSelector(_resList);
    }

    private void InitializeFramerate()
    {
        _framerateSelector.RootElement().Q<Label>("Name").text = "Framerate";
        _refreshList.Add(Constants.RefreshUnlimited);
        foreach (Resolution res in Screen.resolutions)
        {
            string refreshString = res.refreshRate + "";
            
            if (!_refreshList.Contains(refreshString))
            { 
                _refreshList.Add(refreshString); 
            }
        }
        _framerateSelector.InitializeListSelector(_refreshList);
    }

    private void InitializeGraphicProfiles()
    {
        _graphicProfileSelector.RootElement().Q<Label>("Name").text = "Profile";
        List<string> profileList = new List<string>();
        profileList.Add(Constants.LowProfile);
        profileList.Add(Constants.MediumProfile);
        profileList.Add(Constants.HighProfile);
        profileList.Add(Constants.UltraProfile);
        profileList.Add(Constants.CustomProfile);
        _graphicProfileSelector.InitializeListSelector(profileList);
    }

    private void InitializeAntiaAliasing()
    {
        _antiAliasingSelector.RootElement().Q<Label>("Name").text = "AntiAliasing";
        List<string> valuesList = new List<string>();
        valuesList.Add(Constants.AntiAliasingDisable);
        valuesList.Add(Constants.AntiAliasingX2);
        valuesList.Add(Constants.AntiAliasingX4);
        valuesList.Add(Constants.AntiAliasingX8);
        _antiAliasingSelector.InitializeListSelector(valuesList);
    }
    
    private void InitializeShadowResolution()
    {
        _shadowResolutionSelector.RootElement().Q<Label>("Name").text = "Shadow Resolution";
        List<string> valuesList = new List<string>();
        valuesList.Add(Constants.LowProfile);
        valuesList.Add(Constants.MediumProfile);
        valuesList.Add(Constants.HighProfile);
        valuesList.Add(Constants.UltraProfile);
        _shadowResolutionSelector.InitializeListSelector(valuesList);
    }
    
    private void InitializeShadowDistance()
    {
        _shadowDistanceSelector.RootElement().Q<Label>("Name").text = "Shadow Distance";
        List<string> valuesList = new List<string>();
        valuesList.Add(Constants.LowProfile);
        valuesList.Add(Constants.MediumProfile);
        valuesList.Add(Constants.HighProfile);
        valuesList.Add(Constants.UltraProfile);
        _shadowDistanceSelector.InitializeListSelector(valuesList);
    }
    
    private void InitializeTextureQuality()
    {
        _textureQualitySelector.RootElement().Q<Label>("Name").text = "Texture Quality";
        List<string> valuesList = new List<string>();
        valuesList.Add(Constants.LowProfile);
        valuesList.Add(Constants.MediumProfile);
        valuesList.Add(Constants.HighProfile);
        _textureQualitySelector.InitializeListSelector(valuesList);
    }

    private void Apply()
    {
        SetScreenSetting();
        SetVSyncSetting();
        SetAudioSetting();
        SetGraphicProfileSetting();
        
        _updatingAfterApply = true;
        UpdateView();
        
        StartCoroutine(WaitOneFrameToCheckOnChange());
    }

    private IEnumerator WaitOneFrameToCheckOnChange()
    {
        yield return new WaitForEndOfFrame();
        _updatingAfterApply = false;
    }

    private void SetScreenSetting()
    {
        string[] res = _resolutionSelector.SelectedValue().Split("x");
        int width = int.Parse(res[0]);
        int height = int.Parse(res[1]);
        int refresh;
        if (_framerateSelector.SelectedValue().Equals(Constants.RefreshUnlimited))
            refresh = -1;
        else
            refresh = int.Parse(_framerateSelector.SelectedValue());
        FullScreenMode screenMode = FullScreenMode.ExclusiveFullScreen;
        
        PlayerPrefs.SetInt(Constants.Width, width);
        PlayerPrefs.SetInt(Constants.Height, height);
        PlayerPrefs.SetInt(Constants.Refresh, refresh);
        switch (_screenModeSelector.SelectedValue())
        {
            case "Windowed": screenMode = FullScreenMode.Windowed; PlayerPrefs.SetInt(Constants.ScreenMode, 0); break;
            case "Windowed no border": screenMode = FullScreenMode.FullScreenWindow; PlayerPrefs.SetInt(Constants.ScreenMode, 1); break;
            case "Fullscreen": screenMode = FullScreenMode.ExclusiveFullScreen; PlayerPrefs.SetInt(Constants.ScreenMode, 2);break;
        }
        PlayerPrefs.Save();
        
        Screen.SetResolution(width, height, screenMode);
        Application.targetFrameRate = refresh;
        print(width + "x" + height + ": " + refresh);
    }

    private void SetVSyncSetting()
    {
        if (_vSync.value)
        {
            PlayerPrefs.SetInt(Constants.VSync, 1);
            PlayerPrefs.Save();
            QualitySettings.vSyncCount = 1;
            print("VSync is " + QualitySettings.vSyncCount);
            return;
        }
        QualitySettings.vSyncCount = 0;
        PlayerPrefs.SetInt(Constants.VSync, 0);
        PlayerPrefs.Save();
        print("VSync is " + QualitySettings.vSyncCount);
    }

    private void SetAudioSetting()
    {
        audioMixer.SetFloat(Constants.MasterVolume, Mathf.Log10(_masterAudio.value) * 20);
        PlayerPrefs.SetFloat(Constants.MasterVolume, Mathf.Log10(_masterAudio.value) * 20);
        
        audioMixer.SetFloat(Constants.MusicVolume, Mathf.Log10(_musicAudio.value) * 20);
        PlayerPrefs.SetFloat(Constants.MusicVolume, Mathf.Log10(_musicAudio.value) * 20);
        
        audioMixer.SetFloat(Constants.SoundVolume, Mathf.Log10(_soundAudio.value) * 20);
        PlayerPrefs.SetFloat(Constants.SoundVolume, Mathf.Log10(_soundAudio.value) * 20);
        
        PlayerPrefs.Save();
        audioMixer.GetFloat(Constants.MasterVolume, out float master);
        audioMixer.GetFloat(Constants.MusicVolume, out float music);
        audioMixer.GetFloat(Constants.SoundVolume, out float sound);
        
        print("MasterVolume = " + master + " | MusicVolume = " + music + " | SoundVolume = " + sound);
    }

    private void SetGraphicProfileSetting()
    {
        if (_graphicProfileSelector.SelectedValue().Equals(Constants.LowProfile))
        {
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
            PlayerPrefs.SetInt(Constants.QualityLevel, 0);
        }
        else if (_graphicProfileSelector.SelectedValue().Equals(Constants.MediumProfile))
        {
            QualitySettings.SetQualityLevel(1);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
            PlayerPrefs.SetInt(Constants.QualityLevel, 1);
        }
        else if (_graphicProfileSelector.SelectedValue().Equals(Constants.HighProfile))
        {
            QualitySettings.SetQualityLevel(2);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
            PlayerPrefs.SetInt(Constants.QualityLevel, 2);
        }
        else if (_graphicProfileSelector.SelectedValue().Equals(Constants.UltraProfile))
        {
            QualitySettings.SetQualityLevel(3);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
            PlayerPrefs.SetInt(Constants.QualityLevel, 3);
        }
        else if (_graphicProfileSelector.SelectedValue().Equals(Constants.CustomProfile))
        {
            QualitySettings.SetQualityLevel(4);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(Constants.VSync);
            PlayerPrefs.SetInt(Constants.QualityLevel, 4);
            
            UniversalRenderPipelineAsset urpAsset = QualitySettings.GetRenderPipelineAssetAt(4) as UniversalRenderPipelineAsset;
            
            SetAntiAliasingSetting(urpAsset);
            SetRenderScaleSetting(urpAsset);
            SetShadowResolutionSetting(urpAsset);
            SetShadowDistance(urpAsset);
            SetTextureQuality();
            QualitySettings.renderPipeline = urpAsset;
            
            print("******* Custom Profile setted **********");
        }
        
        PlayerPrefs.Save();
    }

    private void SetAntiAliasingSetting(UniversalRenderPipelineAsset urpAsset)
    {
        if (_antiAliasingSelector.SelectedValue().Equals(Constants.AntiAliasingDisable))
        {
            urpAsset.msaaSampleCount = 1;
            PlayerPrefs.SetInt(Constants.AntiAliasing, 1);
        }
        else if (_antiAliasingSelector.SelectedValue().Equals(Constants.AntiAliasingX2))
        {
            urpAsset.msaaSampleCount = 2;
            PlayerPrefs.SetInt(Constants.AntiAliasing, 2);
        }
        else if (_antiAliasingSelector.SelectedValue().Equals(Constants.AntiAliasingX4))
        {
            urpAsset.msaaSampleCount = 4;
            PlayerPrefs.SetInt(Constants.AntiAliasing, 4);
        }
        else if (_antiAliasingSelector.SelectedValue().Equals(Constants.AntiAliasingX8))
        {
            urpAsset.msaaSampleCount = 8;
            PlayerPrefs.SetInt(Constants.AntiAliasing, 8);
        }
        
        PlayerPrefs.Save();
    }

    private void SetRenderScaleSetting(UniversalRenderPipelineAsset urpAsset)
    {
        urpAsset.renderScale = _renderScale.value;
        PlayerPrefs.SetFloat(Constants.RenderScale, _renderScale.value);
        PlayerPrefs.Save();
    }

    private void SetShadowResolutionSetting(UniversalRenderPipelineAsset urpAsset)
    {
        if (_shadowResolutionSelector.SelectedValue().Equals(Constants.LowProfile))
        {
            urpAsset.mainLightShadowmapResolution = 512;
            PlayerPrefs.SetInt(Constants.ShadowResolution, 512);
        }
        else if (_shadowResolutionSelector.SelectedValue().Equals(Constants.MediumProfile))
        {
            urpAsset.mainLightShadowmapResolution = 1024;
            PlayerPrefs.SetInt(Constants.ShadowResolution, 1024);
        }
        else if (_shadowResolutionSelector.SelectedValue().Equals(Constants.HighProfile))
        {
            urpAsset.mainLightShadowmapResolution = 2048;
            PlayerPrefs.SetInt(Constants.ShadowResolution, 2048);
        }
        else if (_shadowResolutionSelector.SelectedValue().Equals(Constants.UltraProfile))
        {
            urpAsset.mainLightShadowmapResolution = 4096;
            PlayerPrefs.SetInt(Constants.ShadowResolution, 4096);
        }
        
        PlayerPrefs.Save();
    }

    private void SetShadowDistance(UniversalRenderPipelineAsset urpAsset)
    {
        if (_shadowDistanceSelector.SelectedValue().Equals(Constants.LowProfile))
        {
            urpAsset.shadowDistance = 10;
            PlayerPrefs.SetInt(Constants.ShadowDistance, 10);
        }
        else if (_shadowDistanceSelector.SelectedValue().Equals(Constants.MediumProfile))
        {
            urpAsset.shadowDistance = 25;
            PlayerPrefs.SetInt(Constants.ShadowDistance, 25);
        }
        else if (_shadowDistanceSelector.SelectedValue().Equals(Constants.HighProfile))
        {
            urpAsset.shadowDistance = 40;
            PlayerPrefs.SetInt(Constants.ShadowDistance, 40);
        }
        else if (_shadowDistanceSelector.SelectedValue().Equals(Constants.UltraProfile))
        {
            urpAsset.shadowDistance = 50;
            PlayerPrefs.SetInt(Constants.ShadowDistance, 50);
        }
        
        PlayerPrefs.Save();
    }

    private void SetTextureQuality()
    {
        if (_textureQualitySelector.SelectedValue().Equals(Constants.LowProfile))
        {
            QualitySettings.globalTextureMipmapLimit = 2;
            PlayerPrefs.SetInt(Constants.TextureQuality, 2);
        }
        else if (_textureQualitySelector.SelectedValue().Equals(Constants.MediumProfile))
        {
            QualitySettings.globalTextureMipmapLimit = 1;
            PlayerPrefs.SetInt(Constants.TextureQuality, 1);
        }
        else if (_textureQualitySelector.SelectedValue().Equals(Constants.HighProfile))
        {
            QualitySettings.globalTextureMipmapLimit = 0;
            PlayerPrefs.SetInt(Constants.TextureQuality, 0);
        }
        
        PlayerPrefs.Save();
    }

    protected override void InitializeNavPath()
    {
        SetupNavPath(_backButton, null, _screenModeSelector.RootElement());
        SetupNavPath(_screenModeSelector.RootElement(), _backButton, _resolutionSelector.RootElement());
        SetupNavPath(_resolutionSelector.RootElement(), _screenModeSelector.RootElement(), _framerateSelector.RootElement());
        SetupNavPath(_framerateSelector.RootElement(), _resolutionSelector.RootElement(), _vSync);
        SetupNavPath(_vSync, _framerateSelector.RootElement() , _masterAudio);
        SetupNavPath(_masterAudio, _vSync, _musicAudio);
        SetupNavPath(_musicAudio, _masterAudio, _soundAudio);
        SetupNavPath(_soundAudio, _musicAudio, _graphicProfileSelector.RootElement());
        SetupNavPath(_graphicProfileSelector.RootElement(), _soundAudio, _antiAliasingSelector.RootElement());
        SetupNavPath(_antiAliasingSelector.RootElement(), _graphicProfileSelector.RootElement(), _renderScale);
        SetupNavPath(_renderScale, _antiAliasingSelector.RootElement(), _shadowResolutionSelector.RootElement());
        SetupNavPath(_shadowResolutionSelector.RootElement(), _renderScale, _shadowDistanceSelector.RootElement());
        SetupNavPath(_shadowDistanceSelector.RootElement(), _shadowResolutionSelector.RootElement(), _textureQualitySelector.RootElement());
        SetupNavPath(_textureQualitySelector.RootElement(), _shadowDistanceSelector.RootElement(), _applyButton);
        
        SetupNavPath(_applyButton, _textureQualitySelector.RootElement());
    }
}