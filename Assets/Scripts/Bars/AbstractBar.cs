using System;
using System.Collections;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Bars {
    public abstract class AbstractBar : MonoBehaviour, IHealth {

        // abstract classe des bars, a modifier pour modifier tout les comportementes des bars

        #region Fields

        

        public Slider slider;
        public Slider damageBar;
        public float speed = 0.0125f;
        private bool _hasChanged;
        protected ushort MaxValue;
        public ushort curValue;
        protected float RestoreDelay;
        private bool _gain = true;
        #endregion

        #region MonoBehaviours

        public void Start() {
            MaxValue = (ushort)slider.maxValue;
            damageBar.maxValue = MaxValue;
            curValue = MaxValue;
            _hasChanged = true;
        }

        // Update is called once per frame
        public void Update() {
            /* if(curValue < 0) { // not needed anymore
                curValue = 0;
            } */
            if (RestoreDelay > 0 && _gain && !IsMax) StartCoroutine(Gain());
            
            if (_hasChanged) {
                slider.value = curValue;
                _hasChanged = false;
            }

            if (Math.Abs(slider.value - damageBar.value) > 0.01) {
                damageBar.value = Mathf.Lerp(damageBar.value, curValue, speed);
            }
        }
        #endregion
        
        public bool CanTakeDamages(ushort damages) => damages <= curValue;

        public bool TryTakeDamages(ushort damages) {
            if (damages <= curValue) {
                TakeDamages(damages);
                return true;
            }
            else return false;
        }

        public void TakeDamages(ushort damages) {
            // value should be checked to be positive, or else use a uint
            _hasChanged = true;
            if (damages >= curValue)
                curValue = 0;
            else curValue -= damages;
        }

        // tofix ??
        public void Heal(ushort heal) {
            _hasChanged = true;
            if (heal + curValue > MaxValue)
                curValue = MaxValue;
            else curValue += heal;
        }

        public void ChangeMaxValue(ushort newMax) {
            if (newMax < MaxValue) {
                curValue = Math.Clamp(curValue, (ushort)0, newMax);
                _hasChanged = true;
            }
            MaxValue = newMax;
            slider.maxValue = MaxValue;
            damageBar.maxValue = MaxValue;
        }

        public bool IsMax => curValue == MaxValue;

        IEnumerator Gain() {
            Heal(1);
            _gain = false;
            yield return new WaitForSeconds(RestoreDelay);
            _gain = true;
        }
    }
}