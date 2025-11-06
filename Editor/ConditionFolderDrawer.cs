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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SGP
{
	public class ConditionFolderDrawer : IUniversalDrawer
	{
		private IUniversalDrawer container;

		private int level;
		private string data;
		private List<IUniversalDrawer> drawers;

		public int Level => level;
		
		protected string name;
		public string Name => name;

		public virtual bool Containable => true;
		
		public IUniversalDrawer Container => container;
		
		private bool isMatchedCondition;

		private string feature;
		private string condition;
		private bool enabled;

		private bool invert = false;
		
		public ConditionFolderDrawer()
		{
			drawers = new List<IUniversalDrawer>();
		}
		
		public void SetContainer(IUniversalDrawer container)
		{
			level = container.Level + 1;
			this.container = container;
		}

		public void Init(string data)
		{
			var dataArray = data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (dataArray.Length < 1)
			{
				throw new Exception("Specify at least one property");
			}

			if (dataArray.Length == 1)
			{
				feature = dataArray[0].Trim();
				condition = string.Empty;
				if (feature.Contains("!"))
				{
					feature = feature.Split(new[] { "!" }, StringSplitOptions.RemoveEmptyEntries)[0];
					invert = true;
				}
			}
			else if (dataArray.Length == 2)
			{
				feature = dataArray[0].Trim();
				condition = dataArray[1].Trim();
			}
		}
		
		public void Add(IUniversalDrawer drawer)
		{
			drawer.SetContainer(this);
			drawers.Add(drawer);
		}

		public void Draw(MaterialEditor editor, Material material, Func<string, MaterialProperty> findProperty)
		{
			Material mat = editor.target as Material;
			string targetFeature = feature;

			isMatchedCondition = false;
			
			if (condition == String.Empty)
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
				
				if(mat.HasProperty(targetFeature))
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

			if(isMatchedCondition)
			{
				GUI.backgroundColor = ShaderGUIStyle.labelColor;
				
				EditorGUILayout.BeginVertical(ShaderGUIStyle.Folder);
				{
					for (var i = 0; i < drawers.Count; i++)
					{
						drawers[i].Draw(editor, material, findProperty);
					}
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
            else if(!enabled && condition.ToLower() == "off")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

	}
}