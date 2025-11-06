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
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SGP
{
	public class QuickSliderDrawer : MaterialPropertyDrawer
	{
		private float _min = 0.0f;
		private float _max = 1.0f;
		private string type = "float";
		
		private static Dictionary<string, Vector2> minMaxVectors = new Dictionary<string, Vector2>();

		private static Vector2 sliderMinMax;

		public QuickSliderDrawer(float extraProp1, float extraProp2)
		{
			this._min = extraProp1;
			this._max = extraProp2;
		}

		public QuickSliderDrawer()
		{
			
		}
		
		public QuickSliderDrawer(float extraProp1, float extraProp2, string type)
		{
			this._min = extraProp1;
			this._max = extraProp2;
			this.type = type.ToLower();
		}
		
		public override void OnGUI(Rect position, MaterialProperty property, String label, MaterialEditor materialEditor)
		{
			Material mat = materialEditor.target as Material;
			float floatValue = property.floatValue;

			GUIContent PrefixLabel = new GUIContent(label, label);
			
			// Referred from Unity Source Code
			float labelWidth = EditorGUIUtility.labelWidth;
			float labelStartX = position.x + EditorGUI.indentLevel;
			
			float sliderMinStartX = position.x + labelWidth;
			float sliderStartX = position.x + labelWidth + (position.width - labelWidth) * 0.1f;
			float sliderMaxStartX = position.x + labelWidth + (position.width - labelWidth) * 0.925f;

			// Tiling Rect
			Rect labelRect = new Rect(labelStartX, position.y, labelWidth, position.height);
			Rect sliderMinRect = new Rect(sliderMinStartX, position.y, (position.width - labelWidth) * 0.075f, position.height);
			Rect sliderRect = new Rect(sliderStartX, position.y, (position.width - labelWidth) * 0.8f, position.height);
			Rect sliderMaxRect = new Rect(sliderMaxStartX, position.y, (position.width - labelWidth) * 0.075f, position.height);
			
			string vector2Name = mat.GetHashCode() + "_" + property.name + "_" + "slider";
			if (!minMaxVectors.TryGetValue(vector2Name, out sliderMinMax))
			{
				sliderMinMax = new Vector2(_min, _max);
				minMaxVectors.Add(vector2Name, sliderMinMax);
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.PrefixLabel(labelRect, PrefixLabel);

				if (type == "float")
				{
					sliderMinMax.x = EditorGUI.FloatField(sliderMinRect, sliderMinMax.x);
				
					floatValue = EditorGUI.Slider(sliderRect, floatValue, sliderMinMax.x, sliderMinMax.y);
				
					sliderMinMax.y = EditorGUI.FloatField(sliderMaxRect, sliderMinMax.y);
				}
				else
				{
					int sliderMinInt = (int)Math.Round(sliderMinMax.x);
					int sliderMaxInt = (int)Math.Round(sliderMinMax.y);
					
					sliderMinMax.x = EditorGUI.IntField(sliderMinRect, sliderMinInt);

					int intValue = (int)Math.Round(floatValue);
					
					floatValue = (float)EditorGUI.IntSlider(sliderRect, intValue, sliderMinInt, sliderMaxInt);
				
					sliderMinMax.y = EditorGUI.IntField(sliderMaxRect, sliderMaxInt);
				}
				
				
			}
			EditorGUILayout.EndHorizontal();

			
			if (EditorGUI.EndChangeCheck())
			{
				property.floatValue = floatValue;
				
				var materials = materialEditor.targets.OfType<Material>().ToArray();

				foreach (var targetMaterial in materials)	
				{
					string curVector2Name = targetMaterial.GetHashCode() + "_" + property.name + "_" + "slider";

					minMaxVectors[curVector2Name] = sliderMinMax;
				}
			}

			// minMaxVectors[vector2Name] = sliderMinMax;
		}
	}
}