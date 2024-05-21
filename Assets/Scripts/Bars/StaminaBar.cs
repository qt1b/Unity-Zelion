using System.Collections;
using UnityEngine;

namespace Bars {
    public class StaminaBar : AbstractBar {
        new void Start() {
            base.Start();
            RestoreDelay = 1f;
        }
    }
}
