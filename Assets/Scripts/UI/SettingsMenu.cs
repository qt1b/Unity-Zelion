using System.Collections.Generic;
using System.Linq;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI {
    public class Settings : MonoBehaviour {
        private Resolution[] _resolutions;
        private int _lastBeforeFull;
        private int _maxRes;
        private int _index;
        public TMPro.TMP_Dropdown resDropdown;
        public AudioMixer audioMixer;
        public Slider masterSlider;
        private static float masterVolume;
        public Slider musicSlider;
        private static float musicVolume;
        public Slider sfxSlider;
        private static float sfxVolume;

        // Start is called before the first frame update
        public TMPro.TMP_Dropdown langDropdown;
        public TMP_Text SettingsTxt;
        public TMP_Text ResolutionTxt;
        public TMP_Text FullScreenTxt;
        public TMP_Text VolumeTxt;
        public TMP_Text MusicTxt;
        public TMP_Text SfxTxt;
        public TMP_Text LanguageTxt;
        public TMP_Text BackText;

        public bool IsTitleScreen;
        public TitleScreen TitleScreen;
        public PauseMenu PauseMenu;
        public GameObject PreviousMenu;
        
        public void Awake() {
            List<string> options = new List<string>();
            _resolutions = Screen.resolutions.Select(resolution => new Resolution() { width = resolution.width, height = resolution.height}).Distinct().ToArray();
            _index = 0;
            for (int i = 0; i < _resolutions.Length; i++) {
                string toadd = _resolutions[i].height + "x" + _resolutions[i].width;
                options.Add(toadd);
                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height) {
                    _index = i;
                }
            }
            resDropdown.options = _resolutions.Select(r => new TMP_Dropdown.OptionData(r.width + "x" + r.height)).ToList();
            resDropdown.value = _index;
            _lastBeforeFull = _index;
            _maxRes = _resolutions.Length - 1;
            
            langDropdown.value = GlobalVars.Language;
            langDropdown.options = new string[] { "English", "Français", "日本語"/*, "Italiano" */}.Select(s => new TMP_Dropdown.OptionData(s)).ToList();

            // ~= -6 db
            if (masterVolume == 0) masterVolume = 0.5f;
            masterSlider.value = masterVolume;
            SetMasterVolume(masterVolume);          
            if (musicVolume == 0) musicVolume = 0.5f;
            musicSlider.value = musicVolume;
            SetMusicVolume(musicVolume);          
            if (sfxVolume == 0) sfxVolume = 0.5f;
            sfxSlider.value = sfxVolume;
            SetSfxVolume(sfxVolume);

        }

        // sets the text depending on the language
        void Start() {
            SettingsTxt.text = TextValues.Settings;
            ResolutionTxt.text = TextValues.Resolution;
            FullScreenTxt.text = TextValues.FullScreen;
            VolumeTxt.text = TextValues.Volume;
            MusicTxt.text = TextValues.Music;
            SfxTxt.text = TextValues.Sfx;
            LanguageTxt.text = TextValues.Language;
            BackText.text = TextValues.Back;
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                PreviousMenu.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }

        public void SetFullscreen(bool val) {
            if (val) {
                _lastBeforeFull = _index;
                // on windows
                Screen.fullScreen = true;
                if (SystemInfo.operatingSystem.Contains("Windows")) {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                // on mac
                else {
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                }
                SetResolution(_maxRes);
            }
            else {
                Screen.fullScreen = false;
                //Debug.LogError("fullscreen : False");
                //Screen.fullScreenMode = FullScreenMode.Windowed;
                SetResolution(_lastBeforeFull);
            }
        }

        public void SetResolution(int resolutionIndex) {
            _index = resolutionIndex;
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetAudioMixer(string key,float value) {
            audioMixer.SetFloat(key, Mathf.Log10(value) * 20);
        }

        // is incomplete
        public void SetMasterVolume(float volume) {
            SetAudioMixer("Master",volume);
            masterVolume = volume;
        }

        public void SetMusicVolume(float volume) {
            SetAudioMixer("Music",volume);
            musicVolume = volume;
        }
        public void SetSfxVolume(float volume) {
            SetAudioMixer("Sfx", volume);
            sfxVolume = volume;
        }

        public void SetLang(int langIndex) {
            GlobalVars.Language = (byte)langIndex;
            Start();
            if (IsTitleScreen) TitleScreen.Start();
            else PauseMenu.Start();
        }
    }
}
