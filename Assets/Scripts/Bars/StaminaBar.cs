using System.Collections;
using UnityEngine;

namespace Bars {
    public class StaminaBar : AbstractBar {
        private bool _gain = true;

        new void Start() {
            base.Start();
        }

        new void Update() {
            base.Update();
            if (_gain && !IsMax) {
                StartCoroutine(Gain());
            }
        }

        IEnumerator Gain() {
            base.Heal(1);
            _gain = false;
            yield return new WaitForSeconds(0.5f);
            _gain = true;
        }
    }
}
