using UnityEngine;

/// <summary>
/// Specifies the type of message that a HelpBox can display.
/// </summary>
public enum HelpBoxMessageType {
    None,
    Info,
    Warning,
    Error
}

/// <summary>
/// Defines a HelpBox attribute that can be used to display informational messages in the Unity Inspector.
/// </summary>
public class HelpBoxAttribute : PropertyAttribute {
    public string text;
    public HelpBoxMessageType messageType;

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpBoxAttribute" /> class.
    /// </summary>
    /// <param name="text">The text to display in the help box.</param>
    /// <param name="messageType">The type of message. Default is None.</param>
    public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None) {
        this.text = text;
        this.messageType = messageType;
    }
}