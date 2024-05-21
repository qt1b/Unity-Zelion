using System.Collections;
using UnityEngine;

namespace Bars {
    public class StaminaBar : AbstractBar {
        private bool _gain = true;

        new void Start() {
            base.Start();
            RestoreDelay = 1f;
        }
    }
}
