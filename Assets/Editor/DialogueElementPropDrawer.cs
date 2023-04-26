using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueElement))]
public class DialogueElementDrawer : PropertyDrawer
{
    // Override the OnGUI method to customize the inspector for DialogueElement
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the serialized properties for IsChoices, SentenceData, and Choices
        SerializedProperty isChoices = property.FindPropertyRelative("IsChoices");
        SerializedProperty sentenceData = property.FindPropertyRelative("SentenceData");
        SerializedProperty choices = property.FindPropertyRelative("Choices");

        // Draw the IsChoices property first
        Rect isChoicesRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(isChoicesRect, isChoices);

        // If IsChoices is true, only show the Choices property
        if (isChoices.boolValue)
        {
            Rect choicesRect = new Rect(position.x, isChoicesRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                        position.width, EditorGUI.GetPropertyHeight(choices));
            EditorGUI.PropertyField(choicesRect, choices, true);
        }
        // If IsChoices is false, only show the SentenceData property
        else
        {
            Rect sentenceDataRect = new Rect(position.x, isChoicesRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                             position.width, EditorGUI.GetPropertyHeight(sentenceData));
            EditorGUI.PropertyField(sentenceDataRect, sentenceData, true);
        }

        EditorGUI.EndProperty();
    }

    // Override the GetPropertyHeight method to calculate the total height of the property drawer
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty isChoices = property.FindPropertyRelative("IsChoices");
        SerializedProperty sentenceData = property.FindPropertyRelative("SentenceData");
        SerializedProperty choices = property.FindPropertyRelative("Choices");

        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (isChoices.boolValue)
        {
            height += EditorGUI.GetPropertyHeight(choices) + EditorGUIUtility.standardVerticalSpacing;
        }
        else
        {
            height += EditorGUI.GetPropertyHeight(sentenceData) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}
