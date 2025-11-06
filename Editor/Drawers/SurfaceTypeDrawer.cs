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

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SGP
{
    public class SurfaceTypeDrawer : MaterialPropertyDrawer
    {
        bool alphaClip = false;
        int renderQueue;
        bool zwrite = false;

        public override void OnGUI(Rect position, MaterialProperty property, string label, MaterialEditor materialEditor)
        {
            Material mat = materialEditor.target as Material;

            if(mat.HasProperty("_AlphaClip"))
            {
                alphaClip = mat.GetFloat("_AlphaClip") == 1.0;
            }
            renderQueue = mat.shader.renderQueue;


            float value = property.floatValue;

            EditorGUI.BeginChangeCheck();
            
            SurfaceType surfaceType = (SurfaceType)value;
            surfaceType = (SurfaceType)EditorGUI.EnumPopup(position, label, surfaceType);

            if (EditorGUI.EndChangeCheck())
            {  
                property.floatValue = (float)surfaceType;   
                if(surfaceType == SurfaceType.Opaque)
                {
                    if(alphaClip)
                    {
                        renderQueue = (int)RenderQueue.AlphaTest;
                        mat.SetOverrideTag("RenderType", "TransparentCutout");
                    }
                    else
                    {
                        renderQueue = (int)RenderQueue.Geometry;
                        mat.SetOverrideTag("RenderType", "Opaque");
                    }

                    SetMaterialBlendMode(mat, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
                    zwrite = true;
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.DisableKeyword("_REFRACTION");
                    mat.DisableKeyword("_CHROMATIC_ABERRATION");
                }
                else // SurfaceType.Transparent
                {
                    BlendMode blendMode = (BlendMode)mat.GetFloat("_Blend");

                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

                    switch(blendMode)
                    {
                        case BlendMode.Alpha:
                            SetMaterialBlendMode(mat, 
                                    UnityEngine.Rendering.BlendMode.SrcAlpha, UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        break;
                        case BlendMode.Premultiply:
                            SetMaterialBlendMode(mat, 
                                    UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                        case BlendMode.Additive:
                            SetMaterialBlendMode(mat, 
                                    UnityEngine.Rendering.BlendMode.SrcAlpha, UnityEngine.Rendering.BlendMode.One);
                        break;
                        case BlendMode.Multiply:
                            SetMaterialBlendMode(mat, 
                                    UnityEngine.Rendering.BlendMode.DstColor, UnityEngine.Rendering.BlendMode.Zero);
                            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    }

                    mat.SetOverrideTag("RenderType", "Transparent");
                    zwrite = mat.GetFloat("_ZWrite") == 1? true:false;
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    renderQueue = (int)RenderQueue.Transparent;

                }

                SetMaterialZWrite(mat, zwrite);
                mat.SetShaderPassEnabled("DepthOnly", zwrite);

                if(mat.HasProperty("_QueueOffset"))
                {
                    renderQueue += (int)mat.GetFloat("_QueueOffset");
                }

                mat.renderQueue = renderQueue;
                MaterialEditor.ApplyMaterialPropertyDrawers(mat);
            }
        }

        private void SetMaterialBlendMode(Material mat, UnityEngine.Rendering.BlendMode srcBlend, 
                                                    UnityEngine.Rendering.BlendMode dstBlend)
        {
            if(mat.HasProperty("_SrcBlend"))
            {
                mat.SetFloat("_SrcBlend", (float)srcBlend);
            }

            if(mat.HasProperty("_DstBlend"))
            {
                mat.SetFloat("_DstBlend", (float)dstBlend);
            }
        }

        private void SetMaterialZWrite(Material mat, bool zwrite)
        {
            if(mat.HasProperty("_ZWrite"))
            {
                mat.SetFloat("_ZWrite", zwrite? 1:0);
            }
        }
    }
}
