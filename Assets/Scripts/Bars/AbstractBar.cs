using System;
using System.Collections;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Bars {
    public abstract class AbstractBar : MonoBehaviour, IHealth {

        // abstract classe des bars, a modifier pour modifier tout les comportementes des bars

        public Slider slider;
        public Slider damageBar;
        public float speed = 0.0125f;
        private bool _hasChanged;
        protected uint MaxValue;
        private uint _curValue;
        protected float RestoreDelay;
        private bool _gain = true;

        public void Start() {
            MaxValue = (uint)slider.maxValue;
            damageBar.maxValue = MaxValue;
            _curValue = MaxValue;
            _hasChanged = true;
        }

        // Update is called once per frame
        public void Update() {
            /* if(curValue < 0) { // not needed anymore
                curValue = 0;
            } */
            if (RestoreDelay > 0 && _gain && !IsMax) StartCoroutine(Gain());
            
            if (_hasChanged) {
                slider.value = _curValue;
                _hasChanged = false;
            }

            if (Math.Abs(slider.value - damageBar.value) > 0.01) {
                damageBar.value = Mathf.Lerp(damageBar.value, _curValue, speed);
            }

        }

        public uint GetValue() {
            return _curValue;
        }

        public bool CanTakeDamages(uint damages) => damages <= _curValue;

        public bool TryTakeDamages(uint damages) {
            if (damages <= _curValue) {
                TakeDamages(damages);
                return true;
            }
            else return false;
        }

        public void TakeDamages(uint damages) {
            // value should be checked to be positive, or else use a uint
            _hasChanged = true;
            if (damages >= _curValue)
                _curValue = 0;
            else _curValue -= damages;
        }

        // tofix ??
        public void Heal(uint heal) {
            _hasChanged = true;
            if (heal + _curValue > MaxValue)
                _curValue = MaxValue;
            else _curValue += heal;
        }

        public void ChangeMaxValue(uint newMax) {
            if (newMax < MaxValue) {
                _curValue = Math.Clamp(_curValue, 0, newMax);
                _hasChanged = true;
            }
            MaxValue = newMax;
            slider.maxValue = MaxValue;
            damageBar.maxValue = MaxValue;
        }

        public bool IsMax => _curValue == MaxValue;

        IEnumerator Gain() {
            Heal(1);
            _gain = false;
            yield return new WaitForSeconds(RestoreDelay);
            _gain = true;
        }
    }
}