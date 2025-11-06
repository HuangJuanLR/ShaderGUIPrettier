/*
Copyright <2024> <HuangJuanLr>

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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SGP
{
	public class GraphRemappingDrawer : PropertiesDrawer
	{
		private static Dictionary<string, Vector2> remappingMinMaxVectors = new Dictionary<string, Vector2>();
		
		private static Vector2 remappingMinMax;

		public GraphRemappingDrawer(string displayName)
		{
			this.displayName = displayName;
		}
		
		public override void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;
			
			// var property = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, name);
			var property = findProperty(name);
			var shader = material.shader;
			
			var vectorValue = property.vectorValue;
			string vector2Name = material.GetHashCode() + "_" + property.name + "_" + "remapping";
			
			int index = shader.FindPropertyIndex(property.name);
			Vector2 defaultVector = shader.GetPropertyDefaultVectorValue(index);
			if (!remappingMinMaxVectors.TryGetValue(vector2Name, out remappingMinMax))
			{
				remappingMinMax = defaultVector;
				remappingMinMaxVectors.Add(vector2Name, remappingMinMax);
			}
			
			Rect position = EditorGUILayout.GetControlRect();
			GUIContent label = new GUIContent(displayName);
			
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
				
				EditorGUI.PrefixLabel(labelRect, label);
				remappingMinMax.x = EditorGUI.FloatField(sliderMinRect, remappingMinMax.x);
				
				EditorGUI.MinMaxSlider(sliderRect, 
									ref vectorValue.x, ref vectorValue.y, 
							   		 remappingMinMax.x, remappingMinMax.y);
				
				remappingMinMax.y = EditorGUI.FloatField(sliderMaxRect, remappingMinMax.y);
			}
			EditorGUILayout.EndHorizontal();
			
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(material, "Change Remapping");
				property.vectorValue = vectorValue;
				
				var materials = materialEditor.targets.OfType<Material>().ToArray();

				foreach (var targetMaterial in materials)
				{
					string curVector2Name = targetMaterial.GetHashCode() + "_" + property.name + "_" + "remapping";

					remappingMinMaxVectors[curVector2Name] = remappingMinMax;
				}
			}
			
			// remappingMinMaxVectors[vector2Name] = remappingMinMax;
				
			
		}
	}
}