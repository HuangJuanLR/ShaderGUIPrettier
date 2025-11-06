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
using UnityEngine.Rendering;
using UnityEngine;

namespace SGP
{
    public class QueueOffsetDrawer : MaterialPropertyDrawer
    {
        private SurfaceType surfaceType;
        
        int renderQueue;
        bool alphaClip;

        public override void OnGUI(Rect position, MaterialProperty property, String label, MaterialEditor materialEditor)
        {
            Material mat = materialEditor.target as Material;

            if (mat.HasProperty("_Surface"))
            {
                surfaceType = (SurfaceType)mat.GetFloat("_Surface");
            }

            if (mat.HasProperty("_AlphaClip"))
            {
                alphaClip = mat.GetFloat("_AlphaClip") == 1.0;
            }
            
            

            float value = property.floatValue;

            EditorGUI.BeginChangeCheck();

            value = (float)EditorGUI.IntSlider(position, label, (int)value, -50, 50);

            if(EditorGUI.EndChangeCheck())
            {
                property.floatValue = value;
                
                if(surfaceType == SurfaceType.Opaque)
                {
                    if(alphaClip)
                    {
                        renderQueue = (int)RenderQueue.AlphaTest + (int)value;
                    }
                    else
                    {
                        renderQueue = (int)RenderQueue.Geometry + (int)value;
                    }
                    
                }
                else
                {
                    renderQueue = (int)RenderQueue.Transparent + (int)value;
                }
                mat.renderQueue = renderQueue;
            }
        }
    }
}
