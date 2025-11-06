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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SGP
{
	public class FeatureFolderDrawer : FolderDrawer
	{
		private static Dictionary<string, bool> toggles = new Dictionary<string, bool>();

		private string label;
		private string keyword;

		public override void Init(string data)
		{
			var dataArray = data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (dataArray.Length < 2)
			{
				throw new Exception($"Specify a label and a shader feature");
			}

			label = dataArray[0].Trim();
			keyword = dataArray[1].Trim();
		}
		

		public override void Draw(MaterialEditor editor, Material material, Func<string, MaterialProperty> findProperty)
		{
			var active = material.IsKeywordEnabled(keyword);
			
			Material mat = editor.target as Material;

			EditorGUILayout.BeginVertical(folderStyle);

			GUI.backgroundColor = ShaderGUIStyle.labelColor;
			
			bool toggle = true;

			string toggleName = material.GetHashCode() + "_" + label;
			
			if (!toggles.TryGetValue(toggleName, out var storedToggle))
			{
				toggles.Add(toggleName, toggle);
			}
			else
			{
				toggle = storedToggle;
			}
			
			toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, label, ShaderGUIStyle.FeatureFolderHeader);

			GUI.backgroundColor = active? ShaderGUIStyle.checkColor : ShaderGUIStyle.labelColor ;
			
			
			var rect = GUILayoutUtility.GetLastRect();
			rect.x += rect.width + 15;
			rect.width = 15;

			EditorGUI.BeginChangeCheck();
			
			active = EditorGUI.Toggle(rect, active);
			
			var materials = editor.targets.OfType<Material>().ToArray();

			if (EditorGUI.EndChangeCheck())
			{
				
				
				foreach (var targetMaterial in materials)
				{
					if (targetMaterial == null) continue;
					
					if (active)
					{
						targetMaterial.EnableKeyword(keyword);
					}
					else
					{
						targetMaterial.DisableKeyword(keyword);
					}
					
					if (targetMaterial.HasProperty(keyword))
					{
						targetMaterial.SetFloat(keyword, active? 1.0f:0.0f);
					}
				}
				
				// if (active)
				// {
				// 	material.EnableKeyword(keyword);
				// }
				// else
				// {
				// 	material.DisableKeyword(keyword);
				// }
				//
				// if (material.HasProperty(keyword))
				// {
				// 	material.SetFloat(keyword, active? 1.0f:0.0f);
				// }
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			foreach (var targetMaterial in materials)
			{
				string curToggleName = targetMaterial.GetHashCode() + "_" + label;
				toggles[curToggleName] = toggle;
			}
			// toggles[toggleName] = toggle;

			GUI.backgroundColor = ShaderGUIStyle.folderColor;

			if (toggle)
			{
				EditorGUILayout.BeginVertical(ShaderGUIStyle.Folder);

				for (var i = 0; i < drawers.Count; i++)
				{
					drawers[i].Draw(editor, material, findProperty);
				}

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();
		}
	}
}