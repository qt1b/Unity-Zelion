using System.Collections;
using UnityEngine;

namespace Bars {
    public class ManaBar : AbstractBar {
        new void Start() {
            base.Start();
            RestoreDelay = 3f;
        }
    }
}