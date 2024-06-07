using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI {
    public class Settings : MonoBehaviour {
        private Resolution[] _resolutions;
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

            currentVolume = 0.5f;
            audioMixer.SetFloat("Volume", currentVolume);
        }

        public void SetFullscreen(bool val) {
            if (val) {
                Screen.SetResolution(_resolutions.Last().width,_resolutions.Last().height,FullScreenMode.FullScreenWindow);
            }
            else {
                // tip : keep the last resolution before fullscreen ?
                Screen.fullScreen = false;
            }
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, 
                resolution.height, Screen.fullScreen);
        }

        // is incomplete
        public void SetVolume(float volume) {
            audioMixer.SetFloat("Volume", volume);
            currentVolume = volume;
        }
    }
}
