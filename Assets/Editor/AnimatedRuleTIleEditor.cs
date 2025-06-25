using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AnimatedRuleTile))]
public class AnimatedRuleTileEditor : Editor
{
    private SerializedProperty rulesProp;
    private SerializedProperty speedProp;
    private SerializedProperty colliderTypeProp;

    private ReorderableList rulesList;

    private static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(-1, 1, 0),   // Top-Left
        new Vector3Int(0, 1, 0),    // Top
        new Vector3Int(1, 1, 0),    // Top-Right
        new Vector3Int(-1, 0, 0),   // Left
        new Vector3Int(0, 0, 0),    // Center (not used for neighbors)
        new Vector3Int(1, 0, 0),    // Right
        new Vector3Int(-1, -1, 0),  // Bottom-Left
        new Vector3Int(0, -1, 0),   // Bottom
        new Vector3Int(1, -1, 0)    // Bottom-Right
    };

    private readonly string[] conditionLabels = new string[] { "?", "✓", "✗" };

    private void OnEnable()
    {
        colliderTypeProp = serializedObject.FindProperty("colliderType");
        rulesProp = serializedObject.FindProperty("rules");
        speedProp = serializedObject.FindProperty("Speed");

        rulesList = new ReorderableList(serializedObject, rulesProp, true, true, true, true);

        rulesList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Rules");
        };

        rulesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty ruleProp = rulesProp.GetArrayElementAtIndex(index);
            SerializedProperty neighborsProp = ruleProp.FindPropertyRelative("neighbors");
            SerializedProperty spritesProp = ruleProp.FindPropertyRelative("sprites");
            SerializedProperty animationProp = ruleProp.FindPropertyRelative("animation");

            // Ensure neighbors array size is exactly 8
            if (neighborsProp.arraySize != 8)
            {
                neighborsProp.ClearArray();
                for (int i = 0; i < 8; i++)
                    neighborsProp.InsertArrayElementAtIndex(i);

                for (int i = 0; i < 8; i++)
                {
                    neighborsProp.GetArrayElementAtIndex(i).FindPropertyRelative("offset").vector3IntValue = neighborOffsets[i];
                    neighborsProp.GetArrayElementAtIndex(i).FindPropertyRelative("condition").enumValueIndex = 0; // DontCare
                }
            }

            float y = rect.y + 2;

            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), $"Rule {index + 1}", EditorStyles.boldLabel);
            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUI.GetPropertyHeight(spritesProp, true)), spritesProp, new GUIContent("Sprites"), true);
            y += EditorGUI.GetPropertyHeight(spritesProp, true) + 4;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUI.GetPropertyHeight(animationProp, true)), animationProp, new GUIContent("Animation"), true);
            y += EditorGUI.GetPropertyHeight(animationProp, true) + 6;

            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), "Neighbors:");
            y += EditorGUIUtility.singleLineHeight + 2;

            const int buttonSize = 30;
            const int padding = 4;
            int startX = (int)rect.x;

            int neighborIndex = 0;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Rect buttonRect = new Rect(startX + col * (buttonSize + padding), y + row * (buttonSize + padding), buttonSize, buttonSize);

                    if (row == 1 && col == 1)
                    {
                        // Center tile - draw label only
                        GUIStyle centerStyle = new GUIStyle(EditorStyles.label);
                        centerStyle.alignment = TextAnchor.MiddleCenter;
                        centerStyle.fontSize = 20;
                        EditorGUI.LabelField(buttonRect, "●", centerStyle);
                    }
                    else
                    {
                        SerializedProperty neighborProp = neighborsProp.GetArrayElementAtIndex(neighborIndex);
                        SerializedProperty conditionProp = neighborProp.FindPropertyRelative("condition");

                        int condition = conditionProp.enumValueIndex;
                        Color originalColor = GUI.backgroundColor;
                        switch (condition)
                        {
                            case 0: GUI.backgroundColor = Color.gray; break;    // DontCare
                            case 1: GUI.backgroundColor = Color.green; break;   // This
                            case 2: GUI.backgroundColor = Color.red; break;     // NotThis
                        }

                        if (GUI.Button(buttonRect, conditionLabels[condition]))
                        {
                            condition = (condition + 1) % conditionLabels.Length;
                            conditionProp.enumValueIndex = condition;
                        }
                        GUI.backgroundColor = originalColor;

                        neighborIndex++;
                    }
                }
            }
        };

        rulesList.elementHeightCallback = (int index) =>
        {
            SerializedProperty ruleProp = rulesProp.GetArrayElementAtIndex(index);
            SerializedProperty spritesProp = ruleProp.FindPropertyRelative("sprites");
            SerializedProperty animationProp = ruleProp.FindPropertyRelative("animation");


            float height = EditorGUIUtility.singleLineHeight + 4; // Label
            height += EditorGUI.GetPropertyHeight(spritesProp, true) + 4;
            height += EditorGUI.GetPropertyHeight(animationProp, true) + 6;
            height += 110; // Space for neighbors grid approx
            height += 20;
            return height;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(speedProp);
        EditorGUILayout.PropertyField(colliderTypeProp, new GUIContent("Collider Type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultSprite"), new GUIContent("Default Sprite"));

        EditorGUILayout.Space();

        rulesList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
