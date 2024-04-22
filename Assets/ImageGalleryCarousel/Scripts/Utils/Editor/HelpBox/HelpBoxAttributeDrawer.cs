using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A property drawer for the <see cref="HelpBoxAttribute" /> that creates a custom GUI element in the Unity Inspector.
/// This drawer handles the layout and rendering of help boxes with adjustable text and message type.
/// </summary>
[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxAttributeDrawer : DecoratorDrawer {
    /// <summary>
    /// Calculates the height of the help box depending on the content and the current GUI style.
    /// </summary>
    /// <returns>The height of the GUI element.</returns>
    public override float GetHeight() {
        try {
            var helpBoxAttribute = attribute as HelpBoxAttribute;
            if (helpBoxAttribute == null) {
                return base.GetHeight();
            }

            var helpBoxStyle = GUI.skin != null ? GUI.skin.GetStyle("helpbox") : null;
            if (helpBoxStyle == null) {
                return base.GetHeight();
            }

            return Mathf.Max(40f,
                helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 4);
        } catch (ArgumentException) {
            // Handle Unity 2022.2 bug by returning default value.
            return 3 * EditorGUIUtility.singleLineHeight;
        }
    }

    /// <summary>
    /// Renders the HelpBox GUI in the inspector window.
    /// </summary>
    /// <param name="position">The rectangle on the screen to use for the help box.</param>
    public override void OnGUI(Rect position) {
        var helpBoxAttribute = attribute as HelpBoxAttribute;
        if (helpBoxAttribute == null) {
            return;
        }

        EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
    }

    /// <summary>
    /// Converts a <see cref="HelpBoxMessageType" /> to a <see cref="MessageType" /> used by Unity's EditorGUI.
    /// </summary>
    /// <param name="helpBoxMessageType">The type of message to display.</param>
    /// <returns>The MessageType equivalent.</returns>
    private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType) {
        switch (helpBoxMessageType) {
            default:
            case HelpBoxMessageType.None: return MessageType.None;
            case HelpBoxMessageType.Info: return MessageType.Info;
            case HelpBoxMessageType.Warning: return MessageType.Warning;
            case HelpBoxMessageType.Error: return MessageType.Error;
        }
    }
}