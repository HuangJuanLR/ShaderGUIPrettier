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

using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SGP
{
    public class RemappingDrawer : MaterialPropertyDrawer
    {
        private static Dictionary<string, Vector2> remappingMinMaxVectors = new Dictionary<string, Vector2>();
		
        private static Vector2 remappingMinMax;

        private static Vector4 vectorValue;
        
        public override void OnGUI(Rect position, MaterialProperty property, String label, MaterialEditor materialEditor)
        {
            Material mat = materialEditor.target as Material;
            var shader = mat.shader;
            
            string vector2Name = mat.GetHashCode() + "_" + property.name + "_" + "remapping";
            
            string minPropName = property.name + "Min";
            string maxPropName = property.name + "Max";
            float min = 0;
            float max = 0;
            
            GUIContent prefixLabel = new GUIContent(label, label);
            
            // Referred from Unity Source Code
            float labelWidth = EditorGUIUtility.labelWidth;
            float labelStartX = position.x + EditorGUI.indentLevel;
            
            float remainWidth = position.width - labelWidth;
			     
            float sliderMinStartX = position.x + labelWidth;
            float sliderStartX = position.x + labelWidth + remainWidth * 0.075f;
            float sliderMaxStartX = position.x + labelWidth + remainWidth * 0.95f;
        
            // Tiling Rect
            Rect labelRect = new Rect(labelStartX, position.y, labelWidth, position.height);
            Rect sliderMinRect = new Rect(sliderMinStartX, position.y, remainWidth * 0.05f, position.height);
            Rect sliderRect = new Rect(sliderStartX, position.y, remainWidth * 0.85f, position.height);
            Rect sliderMaxRect = new Rect(sliderMaxStartX, position.y, remainWidth * 0.05f, position.height);
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                
                if (property.type == MaterialProperty.PropType.Float)
                {
                    min = mat.GetFloat(minPropName);
                    max = mat.GetFloat(maxPropName);
                    
                    int minIndex = shader.FindPropertyIndex(minPropName);
                    int maxIndex = shader.FindPropertyIndex(maxPropName);
                    float defaultMin = shader.GetPropertyDefaultFloatValue(minIndex);
                    float defaultMax = shader.GetPropertyDefaultFloatValue(maxIndex);
                    
                    if (!remappingMinMaxVectors.TryGetValue(vector2Name, out remappingMinMax))
                    {
                        remappingMinMax = new Vector2(defaultMin, defaultMax);
                        remappingMinMaxVectors.Add(vector2Name, remappingMinMax);
                    }
                    
                    EditorGUI.PrefixLabel(labelRect, prefixLabel);
                    remappingMinMax.x = EditorGUI.FloatField(sliderMinRect, remappingMinMax.x);
                    EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, remappingMinMax.x, remappingMinMax.y);
                    remappingMinMax.y = EditorGUI.FloatField(sliderMaxRect, remappingMinMax.y);
                }
                else if (property.type == MaterialProperty.PropType.Vector)
                {
                    vectorValue = property.vectorValue;
                    
                    int index = shader.FindPropertyIndex(property.name);
                    Vector2 defaultVector = shader.GetPropertyDefaultVectorValue(index);
                    if (!remappingMinMaxVectors.TryGetValue(vector2Name, out remappingMinMax))
                    {
                        remappingMinMax = defaultVector;
                        remappingMinMaxVectors.Add(vector2Name, remappingMinMax);
                    }
                
                    EditorGUI.PrefixLabel(labelRect, prefixLabel);
                    remappingMinMax.x = EditorGUI.FloatField(sliderMinRect, remappingMinMax.x);
					
                    EditorGUI.MinMaxSlider(sliderRect, 
                        ref vectorValue.x, ref vectorValue.y, 
                        remappingMinMax.x, remappingMinMax.y);
					
                    remappingMinMax.y = EditorGUI.FloatField(sliderMaxRect, remappingMinMax.y);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(mat, "Change Remapping");
                
                var materials = materialEditor.targets.OfType<Material>().ToArray();
                
                if (property.type == MaterialProperty.PropType.Float)
                {
                    foreach (var targetMaterial in materials)
                    {
                        targetMaterial.SetFloat(minPropName, min);
                        targetMaterial.SetFloat(maxPropName, max);

                        string curVector2Name = targetMaterial.GetHashCode() + "_" + property.name + "_" + "remapping";

                        remappingMinMaxVectors[curVector2Name] = remappingMinMax;
                    }

                    // mat.SetFloat(minPropName, min);
                    // mat.SetFloat(maxPropName, max);
                }
                else if (property.type == MaterialProperty.PropType.Vector)
                {
                    var allProps = MaterialEditor.GetMaterialProperties(materialEditor.targets);

                    foreach (var targetProp in allProps)
                    {
                        if (targetProp.name == property.name)
                        {
                            targetProp.vectorValue = vectorValue;
                        }
                    }
                    
                    foreach (var targetMaterial in materials)
                    {
                        string curVector2Name = targetMaterial.GetHashCode() + "_" + property.name + "_" + "remapping";

                        remappingMinMaxVectors[curVector2Name] = remappingMinMax;
                    }
                    
                    // property.vectorValue = vectorValue;
                }
                // remappingMinMaxVectors[vector2Name] = remappingMinMax;
            }
        }
        
    }
}
