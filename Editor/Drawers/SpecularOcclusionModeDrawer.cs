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
using UnityEditor;
using UnityEngine;

namespace SGP
{
	public class SpecularOcclusionModeDrawer : MaterialPropertyDrawer
	{
		public override void OnGUI(Rect position, MaterialProperty property, String label, MaterialEditor materialEditor)
		{
			Material material = materialEditor.target as Material;
			float value = property.floatValue;
			SpecOcclusionMode display = (SpecOcclusionMode)value;

			EditorGUI.BeginChangeCheck();
			
			display = (SpecOcclusionMode)EditorGUI.EnumPopup(position, label, display);
			value = (float)display;

			

			if (EditorGUI.EndChangeCheck())
			{
				property.floatValue = value;
				switch(display)
				{
					case SpecOcclusionMode.Off:
							material.DisableKeyword("_AO_SPECULAR_OCCLUSION");
							material.DisableKeyword("_BENTNORMAL_SPECULAR_OCCLUSION");
							material.DisableKeyword("_GI_SPECULAR_OCCLUSION");
							break;
					case SpecOcclusionMode.FromAmbientOcclusion:
						material.DisableKeyword("_BENTNORMAL_SPECULAR_OCCLUSION");
						material.DisableKeyword("_GI_SPECULAR_OCCLUSION");
						material.EnableKeyword("_AO_SPECULAR_OCCLUSION");
						break;
					case SpecOcclusionMode.FromBentNormalsAndAO:
						material.DisableKeyword("_AO_SPECULAR_OCCLUSION");
						material.DisableKeyword("_GI_SPECULAR_OCCLUSION");
						material.EnableKeyword("_BENTNORMAL_SPECULAR_OCCLUSION");
						break;
					case SpecOcclusionMode.FromGI:
						material.DisableKeyword("_AO_SPECULAR_OCCLUSION");
						material.DisableKeyword("_BENTNORMAL_SPECULAR_OCCLUSION");
						material.EnableKeyword("_GI_SPECULAR_OCCLUSION");
						break;
				}
			}
		}
	}
}