using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueElement))]
public class DialogueElementDrawer : PropertyDrawer
{
    // Override the OnGUI method to customize the inspector for DialogueElement
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the serialized properties for IsChoices, SentenceData, and Choices, IsChecker, Checker
        SerializedProperty isChoices = property.FindPropertyRelative("IsChoices");
        SerializedProperty isChecker = property.FindPropertyRelative("IsChecker");
        SerializedProperty sentenceData = property.FindPropertyRelative("SentenceData");
        SerializedProperty choices = property.FindPropertyRelative("Choices");
        SerializedProperty checker = property.FindPropertyRelative("CheckerAnswer");

        // Draw the IsChoices property first
        Rect isChoicesRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(isChoicesRect, isChoices);

        // Draw the IsChecker property first - copy IsChoice
        Rect isCheckerRect = new Rect(position.x, isChoicesRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                      position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(isCheckerRect, isChecker);

        // If IsChoices is true, only show the Choices property
        if (isChoices.boolValue)
        {
            Rect choicesRect = new Rect(position.x, isCheckerRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                        position.width, EditorGUI.GetPropertyHeight(choices));
            EditorGUI.PropertyField(choicesRect, choices, true);
        }
        // If IsChecker is true and Is Choice is false, only show the Checker property
        else if (isChecker.boolValue)
        {
            Rect checkerRect = new Rect(position.x, isCheckerRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                        position.width, EditorGUI.GetPropertyHeight(checker));
            EditorGUI.PropertyField(checkerRect, checker, true);
        }
        // If IsChoices and IsChecker is false, only show the SentenceData property
        else
        {
            Rect sentenceDataRect = new Rect(position.x, isCheckerRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                             position.width, EditorGUI.GetPropertyHeight(sentenceData));
            EditorGUI.PropertyField(sentenceDataRect, sentenceData, true);
        }

        EditorGUI.EndProperty();
    }

    // Override the GetPropertyHeight method to calculate the total height of the property drawer
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty isChoices = property.FindPropertyRelative("IsChoices");
        SerializedProperty isChecker = property.FindPropertyRelative("IsChecker");
        SerializedProperty sentenceData = property.FindPropertyRelative("SentenceData");
        SerializedProperty choices = property.FindPropertyRelative("Choices");
        SerializedProperty checker = property.FindPropertyRelative("CheckerAnswer");

        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (isChoices.boolValue)
        {
            height += EditorGUI.GetPropertyHeight(choices) + EditorGUI.GetPropertyHeight(isChecker) + EditorGUIUtility.standardVerticalSpacing;
        }
        else if (isChecker.boolValue)
        {
            height += EditorGUI.GetPropertyHeight(checker) + EditorGUI.GetPropertyHeight(isChecker) + EditorGUIUtility.standardVerticalSpacing;
        }
        else
        {
            height += EditorGUI.GetPropertyHeight(sentenceData) + EditorGUI.GetPropertyHeight(isChecker) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}
