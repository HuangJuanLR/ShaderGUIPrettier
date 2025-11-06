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
	public class GraphTextureDrawer : PropertiesDrawer
	{
		private MaterialProperty extraProp;
		private bool invert = false;
		private bool conditional = false;

		public GraphTextureDrawer(MaterialProperty extraProp)
		{
			this.extraProp = extraProp;
			conditional = extraProp.displayName.Contains("&&") || 
										extraProp.displayName.Contains("&!");

			invert = extraProp.displayName.Contains("&!");
		}
		
		public GraphTextureDrawer()
		{
			
		}

		public override void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;
			
			// var property = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { material }, name);
			var property = findProperty(name);
			GUIContent label = new GUIContent(property.displayName);
			
			if (extraProp != null)
			{
				if (!FindKeyword(extraProp))
				{
					if (conditional)
					{
						if (invert? property.textureValue != null : property.textureValue == null)
						{
							materialEditor.TexturePropertySingleLine(label, property);
						}
						else
						{
							materialEditor.TexturePropertySingleLine(label, property, extraProp);
						}
					}
					else
					{
						materialEditor.TexturePropertySingleLine(label, property, extraProp);
					}
				}
				else
				{
					materialEditor.TexturePropertySingleLine(label, property);

					if (property.textureValue != null)
					{
						material.EnableKeyword(extraProp.name);
						extraProp.floatValue = 1.0f;
					}
					else
					{
						material.DisableKeyword(extraProp.name);
						extraProp.floatValue = 0.0f;
					}
				}
			}
			else
			{
				materialEditor.TexturePropertySingleLine(label, property);
			}
		}

		private static bool FindKeyword(MaterialProperty property)
		{
			string name = property.name;
			string upperName = name.ToUpper();

			for (int i = 0; i < name.Length; i++)
			{
				if (upperName[i] == name[i])
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return true;

		}
	}
}