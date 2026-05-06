using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UHFPS.Runtime;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    [CustomEditor(typeof(LeversPuzzleOrderLights))]
    public class LeversPuzzleOrderLightsEditor : InspectorEditor<LeversPuzzleOrderLights>
    {
        private ReorderableList reorderableList;
        
        public override void OnEnable()
        {
            base.OnEnable();

            if (Properties["LeversPuzzle"].objectReferenceValue != null)
                Properties["OrderLights"].arraySize = Target.LeversPuzzle.Levers.Count;
            
            reorderableList = new ReorderableList(serializedObject, Properties["OrderLights"], true, true, true, true);
            reorderableList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.xMin += 12f;
                rect.y += 1f;
                
                string name = "OrderLight " + index;
                EditorGUI.PropertyField(rect, element, new GUIContent(name), true);
                //ReorderableList.defaultBehaviours.DrawElement(rect, element, null, isActive, isFocused, true, true);
            };
            reorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                if (element.isExpanded)
                {
                    float height = EditorGUI.GetPropertyHeight(element);
                    height += EditorGUIUtility.standardVerticalSpacing;
                    return height;
                }

                return EditorGUIUtility.singleLineHeight;
            };
            reorderableList.drawHeaderCallback += (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Order Lights");
            };
        }

        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Levers Puzzle Order Lights"), Target);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("A helper script that is used to display the number of lever interactions you need to validate the levers order.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();
            {
                EditorGUI.BeginChangeCheck();
                Properties.Draw("LeversPuzzle");
                if (EditorGUI.EndChangeCheck())
                {
                    LeversPuzzle leversPuzzle = (LeversPuzzle)Properties["LeversPuzzle"].objectReferenceValue;
                    if (leversPuzzle != null) Properties["OrderLights"].arraySize = leversPuzzle.Levers.Count;
                    else Properties["OrderLights"].arraySize = 0;
                }

                if (Properties["LeversPuzzle"].objectReferenceValue != null)
                {
                    EditorGUILayout.Space(1f);
                    reorderableList.DoLayoutList();
                }

                EditorGUILayout.Space();
                Properties.Draw("EmissionKeyword");
                using(new EditorGUI.DisabledGroupScope(true))
                {
                    Properties.Draw("OrderIndex");
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}