using System.Linq;
using UnityEngine;
using UnityEditor;
using UHFPS.Runtime;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    [CustomPropertyDrawer(typeof(RendererMaterial))]
    public class RendererMaterialDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty meshRenderer = property.FindPropertyRelative("meshRenderer");
            SerializedProperty material = property.FindPropertyRelative("material");
            SerializedProperty materialIndex = property.FindPropertyRelative("materialIndex");

            Renderer meshRendererRef = meshRenderer.objectReferenceValue as Renderer;
            Material materialRef = material.objectReferenceValue as Material;

            EditorGUI.BeginProperty(position, label, property);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect contentRect = EditorGUI.IndentedRect(position);

            Rect lineRect = contentRect;
            lineRect.width = 2f;
            lineRect.height -= 4f;
            lineRect.x += 2f;
            lineRect.y += 2f;
            EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.5f, 1f));

            Rect row1 = contentRect;
            row1.height = EditorGUIUtility.singleLineHeight;
            row1.xMin += 8f;

            Rect fieldRect = EditorGUI.PrefixLabel(row1, label);
            EditorGUI.PropertyField(fieldRect, meshRenderer, GUIContent.none);

            using (new EditorGUI.DisabledGroupScope(meshRendererRef == null))
            {
                Rect row2 = contentRect;
                row2.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                row2.height = EditorGUIUtility.singleLineHeight;
                row2.xMin += 8f;

                Rect materialRect = EditorGUI.PrefixLabel(row2, new GUIContent("Material"));

                string[] materials = new string[0];
                string selected = meshRendererRef != null && materialRef != null ? materialRef.name : "";

                if (meshRendererRef != null)
                    materials = meshRendererRef.sharedMaterials.Select(x => x != null ? x.name : "<None>").ToArray();

                Vector2 defaultIconSize = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(new Vector2(14, 14));

                GUIContent baseText = EditorGUIUtility.TrTextContentWithIcon("Select Material", "Material Icon");
                EditorDrawing.DrawStringSelectPopup(materialRect, baseText, materials, selected, (str) =>
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        int index = ArrayUtility.IndexOf(materials, str);
                        if (index != -1)
                        {
                            materialIndex.intValue = index;
                            material.objectReferenceValue = meshRendererRef.sharedMaterials[index];
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                });

                EditorGUIUtility.SetIconSize(defaultIconSize);
            }

            EditorGUI.indentLevel = oldIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}