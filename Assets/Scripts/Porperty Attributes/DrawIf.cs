using System;
using System.ComponentModel;
using UnityEngine;

namespace Porperty_Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | 
                    AttributeTargets.Class | AttributeTargets.Struct , AllowMultiple = true)]
    public class DrawIf : PropertyAttribute
    {
        public string boolName;
        public bool HideInInspector = false;

        public DrawIf(string boolName)
        {
            this.boolName = boolName;
            HideInInspector = false;
        }

        public DrawIf(string boolName, bool hideInInspector)
        {
            this.boolName = boolName;
            HideInInspector = hideInInspector;
        }
    }
}