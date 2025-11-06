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
	public class GraphVector2Drawer : PropertiesDrawer
	{
		public GraphVector2Drawer(string displayName)
		{
			this.displayName = displayName;
		}
		
		public override void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;
			
			// var property = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, name);
			var property = findProperty(name);
			Vector2 vector2 = property.vectorValue;
			GUIContent label = new GUIContent(displayName);
			EditorGUI.BeginChangeCheck();
			vector2 = EditorGUILayout.Vector2Field(label, vector2);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(material, "Change Vector2");
				property.vectorValue = vector2;
			}

			
		}
	}
	
	public class GraphVector3Drawer : PropertiesDrawer
	{
		public GraphVector3Drawer(string displayName)
		{
			this.displayName = displayName;
		}
		
		public override void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;
			
			// var property = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, name);
			var property = findProperty(name);
			Vector3 vector3 = property.vectorValue;
			GUIContent label = new GUIContent(displayName);
			EditorGUI.BeginChangeCheck();
			vector3 = EditorGUILayout.Vector3Field(label, vector3);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(material, "Change Vector3");
				property.vectorValue = vector3;
			}

			
		}
	}
	
	public class GraphVector4Drawer : PropertiesDrawer
	{
		public GraphVector4Drawer(string displayName)
		{
			this.displayName = displayName;
		}
		
		public override void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;
			
			// var property = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, name);
			var property = findProperty(name);
			Vector4 vector4 = property.vectorValue;
			GUIContent label = new GUIContent(displayName);
			EditorGUI.BeginChangeCheck();
			vector4 = EditorGUILayout.Vector4Field(label, vector4);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(material, "Change Vector4");
				property.vectorValue = vector4;
			}

			
		}
	}
}