/*
Copyright <2023> <HuangJuanLr>

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the “Software”), to deal in
the Software without restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SGP
{
    public class ScaleOffsetDrawer : MaterialPropertyDrawer
    {
        private string _texName = "";
        GUIContent tillingTex = new GUIContent("Tiling", "Tiling");
        GUIContent offsetTex = new GUIContent("Offset", "Offset");

        public ScaleOffsetDrawer(string extraPropName)
        {
            this._texName = extraPropName;
        }

        public override void OnGUI(Rect position, MaterialProperty property, string label, MaterialEditor materialEditor)
        {
            string texName = _texName;
            string scaleOffsetName = texName + "_ST";
            Material mat = materialEditor.target as Material;
            Vector4 scaleOffset = Vector4.one;

            if (mat.HasProperty(scaleOffsetName)) scaleOffset = mat.GetVector(scaleOffsetName);
            
            property.vectorValue = scaleOffset;

            Vector4 vector = property.vectorValue;
            Vector2 tilling = new Vector2(vector.x, vector.y);
            Vector2 offset = new Vector2(vector.z, vector.w);

            // Referred from Unity Source Code
            float labelWidth = EditorGUIUtility.labelWidth;
            float controlStartX = position.x + labelWidth;
            float labelStartX = position.x + EditorGUI.indentLevel;

            // Tiling Rect
            Rect labelRect = new Rect(labelStartX, position.y, labelWidth, position.height);
            Rect valueRect = new Rect(controlStartX, position.y, position.width - labelWidth, position.height);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            {
                // Tilling GUI
                EditorGUI.PrefixLabel(labelRect, tillingTex);
                tilling = EditorGUI.Vector2Field(valueRect, GUIContent.none, tilling);

                // Offset Rect
                labelRect.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                valueRect.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                // Offset GUI
                EditorGUI.PrefixLabel(labelRect, offsetTex);
                offset = EditorGUI.Vector2Field(valueRect, GUIContent.none, offset);
                
                EditorGUILayout.LabelField("");
            }
            EditorGUILayout.EndVertical();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(mat, "MainTex ST Changed");
                Vector4 newScaleOffset = new Vector4(tilling.x, tilling.y, offset.x, offset.y);
                
                var materials = materialEditor.targets.OfType<Material>().ToArray();
                
                foreach (var targetMaterial in materials)
                {
                    if (targetMaterial.HasProperty(texName))
                    {
                        targetMaterial.SetTextureOffset(texName, offset);
                        targetMaterial.SetTextureScale(texName, tilling);
                        
                        targetMaterial.SetVector(property.name, newScaleOffset);
                    }
                }
                
                // if (mat.HasProperty(texName))
                // {
                //     mat.SetTextureOffset(texName, offset);
                //     mat.SetTextureScale(texName, tilling);
                // }
                
                // var allProps = MaterialEditor.GetMaterialProperties(materialEditor.targets);
                //
                // foreach (var targetProp in allProps)
                // {
                //     if (targetProp.name == property.name)
                //     {
                //         // targetProp.vectorValue = newScaleOffset;
                //         Debug.Log(targetProp);
                //     }
                // }
                // property.vectorValue = newScaleOffset;
            }
        }
    }
}
