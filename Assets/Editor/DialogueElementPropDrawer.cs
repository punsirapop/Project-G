using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueElement))]
public class DialogueElementDrawer : PropertyDrawer
{
    private string[] _boolOptions = new string[] { "False", "True" };

    private string[] _dataTypeOptions = new string[] { "Sentence", "Choice", "Checker" };

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

        // Draw the data type dropdown first
        Rect dataTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        int dataTypeIndex = isChoices.boolValue ? 1 : isChecker.boolValue ? 2 : 0;
        dataTypeIndex = EditorGUI.Popup(dataTypeRect, "Data Type", dataTypeIndex, _dataTypeOptions);
        isChoices.boolValue = dataTypeIndex == 1;
        isChecker.boolValue = dataTypeIndex == 2;

        // Draw the SentenceData property if "Sentence" is chosen
        if (!isChoices.boolValue && !isChecker.boolValue)
        {
            Rect sentenceDataRect = new Rect(position.x, dataTypeRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                        position.width, EditorGUI.GetPropertyHeight(sentenceData));
            EditorGUI.PropertyField(sentenceDataRect, sentenceData, true);
        }

        // Draw the Choices property if IsChoices is true
        if (isChoices.boolValue)
        {
            Rect choicesRect = new Rect(position.x, dataTypeRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                        position.width, EditorGUI.GetPropertyHeight(choices));
            EditorGUI.PropertyField(choicesRect, choices, true);
        }
        // Draw the Checker property if IsChecker is true
        else if (isChecker.boolValue)
        {
            Rect checkerRect = new Rect(position.x, dataTypeRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                                        position.width, EditorGUI.GetPropertyHeight(checker));
            EditorGUI.PropertyField(checkerRect, checker, true);
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

        if (!isChoices.boolValue && !isChecker.boolValue)
        {
            height += EditorGUI.GetPropertyHeight(sentenceData) + EditorGUIUtility.standardVerticalSpacing;
        }
        
        if (isChoices.boolValue)
        {
            height += EditorGUI.GetPropertyHeight(choices) + EditorGUI.GetPropertyHeight(isChecker) + EditorGUIUtility.standardVerticalSpacing;
        }
        else if (isChecker.boolValue)
        {
            height += EditorGUI.GetPropertyHeight(checker) + EditorGUI.GetPropertyHeight(isChecker) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}
