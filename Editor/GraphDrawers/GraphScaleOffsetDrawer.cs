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
using UnityEditor;
using UnityEngine;

namespace SGP
{
	public class GraphScaleOffsetDrawer : PropertiesDrawer
	{
		GUIContent tilingLabel = new GUIContent("Tiling", "Tiling");
		GUIContent offsetLabel = new GUIContent("Offset", "Offset");
		
		public override void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;
			
			// var property = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, name);
			var property = findProperty(name);
			Rect position = EditorGUILayout.GetControlRect();

			Vector4 scaleOffset = property.vectorValue;

			Vector2 tilling = new Vector2(scaleOffset.x, scaleOffset.y);
			Vector2 offset = new Vector2(scaleOffset.z, scaleOffset.w);

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
				EditorGUI.PrefixLabel(labelRect, tilingLabel);
				tilling = EditorGUI.Vector2Field(valueRect, GUIContent.none, tilling);

				// Offset Rect
				labelRect.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				valueRect.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				// Offset GUI
				EditorGUI.PrefixLabel(labelRect, offsetLabel);
				offset = EditorGUI.Vector2Field(valueRect, GUIContent.none, offset);
				
				// EditorGUILayout.LabelField("");
				EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);
			}
			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(material, "Change Scale Offset");
				Vector4 newScaleOffset = new Vector4(tilling.x, tilling.y, offset.x, offset.y);
				property.vectorValue = newScaleOffset;
			}

			
		}
	}
}