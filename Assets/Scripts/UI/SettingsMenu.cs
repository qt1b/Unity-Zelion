using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI {
    public class Settings : MonoBehaviour {
        private Resolution[] _resolutions;
        private int _lastBeforeFull;
        private int _maxRes;
        private List<string> _options = new List<string>();
        private int _index;
        public TMPro.TMP_Dropdown resDropdown;
        public AudioMixer audioMixer;
        //public Slider audioSlider;
        private float currentVolume;
        // Start is called before the first frame update
        void Start() {
            _resolutions = Screen.resolutions.Select(resolution => new Resolution() { width = resolution.width, height = resolution.height}).Distinct().ToArray();
            _index = 0;
            for (int i = 0; i < _resolutions.Length; i++) {
                string toadd = _resolutions[i].height + "x" + _resolutions[i].width;
                _options.Add(toadd);
                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height) {
                    _index = i;
                }
            }
            resDropdown.options = _resolutions.Select(r => new TMP_Dropdown.OptionData(r.width + "x" + r.height)).ToList();
            resDropdown.value = _index;
            _lastBeforeFull = _index;
            _maxRes = _resolutions.Length - 1;

            currentVolume = 0.5f;
            audioMixer.SetFloat("Master", currentVolume);
        }

        public void SetFullscreen(bool val) {
            if (val) {
                _lastBeforeFull = _index;
                // on windows
                Screen.fullScreen = true;
                /*if (SystemInfo.operatingSystem.Contains("Windows")) {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                // on mac
                else {
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                } */
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
            Screen.SetResolution(resolution.width, 
                resolution.height, Screen.fullScreen);
        }

        // is incomplete
        public void SetVolume(float volume) {
            audioMixer.SetFloat("Master", volume);
            currentVolume = volume;
        }
    }
}
