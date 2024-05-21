using System;
using UnityEngine;

namespace Bars {
    public class HealthBar : AbstractBar {
        private new void Start() {
            base.Start();
            RestoreDelay = 0f;
        }
    }
}