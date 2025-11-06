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
	public class FolderDrawer : IUniversalDrawer
	{
		private IUniversalDrawer container;

		private int level;
		private string data;
		protected List<IUniversalDrawer> drawers;
		
		public int Level => level;
		
		public virtual bool Containable => true;
		
		public IUniversalDrawer Container => container;
		
		private static Dictionary<string, bool> toggles = new Dictionary<string, bool>();

		protected GUIStyle folderStyle;
		
		private string title = "";
		private string feature = "";
		private string condition = "";
		private bool enabled = false;

		private bool conditional = false;

		private bool alwaysOn = false;

		private bool invert = false;

		public FolderDrawer()
		{
			drawers = new List<IUniversalDrawer>();
		}
		
		public virtual void Init(string data)
		{
			this.data = data;

			string[] dataArray = data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (dataArray.Length == 3)
			{
				title = dataArray[0].Trim();
				feature = dataArray[1].Trim();
				condition = dataArray[2].Trim();
				conditional = true;
			}
			else if(dataArray.Length == 2)
			{
				title = dataArray[0].Trim();
				alwaysOn = CheckBoolean(dataArray[1].Trim());
				if (!alwaysOn)
				{
					feature = dataArray[1].Trim();
					condition = string.Empty;
					conditional = true;
					if (feature.Contains("!"))
					{
						feature = feature.Split(new[] { "!" }, StringSplitOptions.RemoveEmptyEntries)[0];
						invert = true;
					}
				}
			}
			else
			{
				title = dataArray[0].Trim();
				conditional = false;
			}
		}
		
		public void Add(IUniversalDrawer drawer)
		{
			drawer.SetContainer(this);
			drawers.Add(drawer);
		}
		
		public virtual void SetContainer(IUniversalDrawer container)
		{
			level = container.Level + 1;
			this.container = container;
			folderStyle = level == 1 ? ShaderGUIStyle.Folder : ShaderGUIStyle.NestedFolder;
		}

		public virtual void Draw(MaterialEditor editor, Material material, Func<string, MaterialProperty> findProperty)
		{
			Material mat = editor.target as Material;
			string targetFeature = feature;
		
			string toggleName = mat.GetHashCode() + "_" + data;;

			bool isMatchedCondition = false;
			if (!conditional)
			{
				isMatchedCondition = true;
			}
			else
			{
				if (condition == string.Empty)
				{
					enabled = mat.IsKeywordEnabled(targetFeature);
					isMatchedCondition = CheckCondition(enabled, "On");
					
					if (mat.HasProperty(targetFeature))
					{
						var prop = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, 
							targetFeature);
						
						if (prop.type == MaterialProperty.PropType.Texture)
						{
							isMatchedCondition = prop.textureValue != null;
						}
						else if (prop.type == MaterialProperty.PropType.Float)
						{
							// Rider suggestion of precision comparison
							isMatchedCondition = Math.Abs(prop.floatValue - 1.0f) < 0.01;
						}
					}
					
					if (invert) isMatchedCondition = !isMatchedCondition;
				}
				else
				{
					enabled = mat.IsKeywordEnabled(targetFeature);
					isMatchedCondition = CheckCondition(enabled, condition);
					
					if (mat.HasProperty(targetFeature))
					{
						var prop = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, 
							targetFeature);
						
						if (prop.type == MaterialProperty.PropType.Texture)
						{
							isMatchedCondition = prop.textureValue != null;
							if (condition.ToLower() == "off")
							{
								isMatchedCondition = !isMatchedCondition;
							}
							
						}
						else if (prop.type == MaterialProperty.PropType.Float)
						{
							float value = mat.GetFloat(targetFeature);
							if (condition.ToLower() == "off") condition = "0";
							if (condition.ToLower() == "on") condition = "1";
							float parsedCondition = float.Parse(condition);
							isMatchedCondition = value.Equals(parsedCondition);
						}
						
					}
					
					
				}
				
			}

			if (isMatchedCondition)
			{
				EditorGUILayout.BeginVertical(folderStyle);		
			
				GUI.backgroundColor = ShaderGUIStyle.labelColor;
				
				bool toggle = true;

				if (!toggles.TryGetValue(toggleName, out var storedToggle))
				{
					toggles.Add(toggleName, toggle);
				}
				else
				{
					toggle = storedToggle;
				}

				toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, title, 
										alwaysOn? ShaderGUIStyle.AlwaysOnHeader : ShaderGUIStyle.FolderHeader);
				
				EditorGUILayout.EndFoldoutHeaderGroup();
				
				var materials = editor.targets.OfType<Material>().ToArray();
				foreach (var targetMaterial in materials)
				{
					string curToggleName = targetMaterial.GetHashCode() + "_" + data;
					toggles[curToggleName] = toggle;
				}
				// toggles[toggleName] = toggle;

				GUI.backgroundColor = ShaderGUIStyle.folderColor;

				if (toggle || alwaysOn)
				{
					EditorGUILayout.BeginVertical(ShaderGUIStyle.Folder);
					{
						for (var i = 0; i < drawers.Count; i++)
						{
							drawers[i].Draw(editor, material, findProperty);
						}
					}
					EditorGUILayout.EndVertical();
				}
			
				EditorGUILayout.EndVertical();
			}
		}
		
		private bool CheckCondition(bool enabled, string condition)
		{
			if(enabled && condition.ToLower() == "on")
			{
				return true;
			}
			
			if(!enabled && condition.ToLower() == "off")
			{
				return true;
			}
			
			return false;
			
		}

		private bool CheckBoolean(string boolean)
		{
			if (boolean.ToLower() == "true")
			{
				return true;
			}
			else if (boolean.ToLower() == "false")
			{
				return false;
			}
			else
			{
				return false;
			}
		}
	}
}