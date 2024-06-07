using Porperty_Attributes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            HorizontalLineAttribute attr = attribute as HorizontalLineAttribute;
            return Mathf.Max(attr!.Padding, attr.Thickness);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HorizontalLineAttribute attr = attribute as HorizontalLineAttribute;
            position.height = attr!.Thickness;
            position.y += attr.Padding * .5f;
            
            EditorGUI.DrawRect(position, EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, 1f) : new Color(.7f, .7f, .7f, 1f));
        }
    }

    [CustomPropertyDrawer(typeof(DrawIf))]
    public class DrawIfDrawer : DecoratorDrawer
    {
        public override 
    }
}