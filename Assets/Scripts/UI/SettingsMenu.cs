using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI {
    public class Settings : MonoBehaviour {
        private Resolution[] _resolutions;
        private List<string> _options = new List<string>();
        private int _index;
        public TMPro.TMP_Dropdown resDropdown;
        // Start is called before the first frame update
        void Start() {
            _resolutions = Screen.resolutions.Select(resolution => new Resolution() { width = resolution.width, height = resolution.height}).Distinct().ToArray();
            _index = 0;
            for (  int i = 0; i < _resolutions.Length; i++) {
                string toadd = _resolutions[i].height + "x" + _resolutions[i].width;
                _options.Add(toadd);
                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height) {
                    _index = i;
                }
            }
            resDropdown.value = _index;
        }

        public void SetFullscreen(bool val) => Screen.fullScreen = val;
    }
}
