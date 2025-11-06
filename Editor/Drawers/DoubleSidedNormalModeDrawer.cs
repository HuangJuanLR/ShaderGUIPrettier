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
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;

namespace SGP
{
    public class DoubleSidedNormalModeDrawer : MaterialPropertyDrawer
    {
        private RenderFace renderFace;
        
        public override void OnGUI(Rect position, MaterialProperty property, string label, MaterialEditor materialEditor)
        {
            float value = property.floatValue;
            Material mat = materialEditor.target as Material;
            if(mat.HasProperty("_Cull"))
            {
                renderFace = (RenderFace)mat.GetFloat("_Cull");

                if(renderFace == RenderFace.Both)
                {
                    EditorGUI.BeginChangeCheck();
                    value = EditorGUI.Popup(position, label, (int)value, Enum.GetNames(typeof(DoubleSidedNormalMode)));

                    if(EditorGUI.EndChangeCheck())
                    {
                        property.floatValue = value;
                        
                        DoubleSidedNormalMode doubleSidedNormalMode = (DoubleSidedNormalMode)value;
                        switch(doubleSidedNormalMode)
                        {
                            case DoubleSidedNormalMode.Mirror: // Mirror mode (in tangent space)
                                mat.SetVector("_DoubleSidedConstants", new Vector4(1.0f, 1.0f, -1.0f, 0.0f));
                                break;
                            case DoubleSidedNormalMode.Flip: // Flip mode (in tangent space)
                                mat.SetVector("_DoubleSidedConstants", new Vector4(-1.0f, -1.0f, -1.0f, 0.0f));
                                break;
                            case DoubleSidedNormalMode.None: // None mode (in tangent space)
                                mat.SetVector("_DoubleSidedConstants", new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
                                break;
                        }
                    }
                }
            }

        }
    }
}