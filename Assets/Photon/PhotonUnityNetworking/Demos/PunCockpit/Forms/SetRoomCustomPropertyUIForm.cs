﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetRoomCustomPropertyUIForm.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.PhotonUnityNetworking.Demos.PunCockpit.Forms
{
    /// <summary>
    /// Room custom properties UI Form.
    /// </summary>
    public class SetRoomCustomPropertyUIForm : MonoBehaviour
    {
        public InputField PropertyValueInput;

        [System.Serializable]
        public class OnSubmitEvent : UnityEvent<string> { }

        public OnSubmitEvent OnSubmit;

        public void Start()
        {

        }

        // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then StartChat.
        public void EndEditOnEnter()
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                this.SubmitForm();
            }
        }

        public void SubmitForm()
        {
            OnSubmit.Invoke(PropertyValueInput.text);
        }
    }
}