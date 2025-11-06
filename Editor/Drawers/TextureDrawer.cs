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
using UnityEditor;
using UnityEngine;

namespace SGP
{
    public class TextureDrawer : MaterialPropertyDrawer
    {
        private string extraPropName = "";
        Color targetColor;
        private bool conditional = false;
        private bool isHDR = false;
        private bool invert = false;

        private string keyword;

        public TextureDrawer()
        {
            
        }

        public TextureDrawer(string extraPropName)
        {
            if (extraPropName.Contains("#"))
            {
                conditional = true;
                this.extraPropName = extraPropName.Substring(1).Trim();

                if (extraPropName.Contains("##"))
                {
                    invert = true;
                    this.extraPropName = extraPropName.Substring(2).Trim();
                }
            }
            else
            {
                this.extraPropName = extraPropName;
                keyword = extraPropName;
            }
            
        }
        
        public TextureDrawer(string extraPropName, string extraParm)
        {
            if (extraPropName.Contains("#"))
            {
                conditional = true;
                this.extraPropName = extraPropName.Substring(1).Trim();
                
                if (extraPropName.Contains("##"))
                {
                    invert = true;
                    this.extraPropName = extraPropName.Substring(2).Trim();
                }
            }
            else
            {
                this.extraPropName = extraPropName;
            }

            if (extraParm.ToLower() == "hdr")
            {
                this.isHDR = extraParm.ToLower() == "hdr";
            }
            else
            {
                keyword = extraParm;
                this.isHDR = false;
            }
            
        }

        public TextureDrawer(string extraParm1, string extraParm2, string extraParm3)
        {
            if (extraParm1.Contains("#"))
            {
                conditional = true;
                this.extraPropName = extraParm1.Substring(1).Trim();

                if (extraPropName.Contains("##"))
                {
                    invert = true;
                    this.extraPropName = extraParm1.Substring(2).Trim();
                }
            }
            else
            {
                this.extraPropName = extraParm1;
            }
        
            keyword = extraParm2;
            this.isHDR = extraParm3.ToLower() == "hdr";
            
        }

        public override void OnGUI(Rect position, MaterialProperty property, String label, MaterialEditor materialEditor)
        {
            var value = property.textureValue;

            Material mat = materialEditor.target as Material;
            
            EditorGUILayout.Space(-EditorGUIUtility.standardVerticalSpacing - EditorGUIUtility.singleLineHeight);
            
            EditorGUI.BeginChangeCheck();
            // EditorGUI.showMixedValue = true;
            
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent prefixLabel = new GUIContent(label);

                if (mat.HasProperty(extraPropName) && !isHDR)
                {
                    // var targetColorProp = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { mat }, extraPropName);

                    var allProps = MaterialEditor.GetMaterialProperties(materialEditor.targets);

                    foreach (var targetProp in allProps)
                    {
                        if (targetProp.name == extraPropName)
                        {
                            if (!conditional)
                            {
                                materialEditor.TexturePropertySingleLine(prefixLabel, property, targetProp);
                            }
                            else
                            {
                                if (invert? property.textureValue != null : property.textureValue == null)
                                {
                                    materialEditor.TexturePropertySingleLine(prefixLabel, property);
                                }
                                else
                                {
                                    materialEditor.TexturePropertySingleLine(prefixLabel, property, targetProp);
                                }
                            }
                        }
                    }
                    
                    // if (!conditional)
                    // {
                    //     materialEditor.TexturePropertySingleLine(prefixLabel, property, targetColorProp);
                    // }
                    // else
                    // {
                    //     if (invert? property.textureValue != null : property.textureValue == null)
                    //     {
                    //         materialEditor.TexturePropertySingleLine(prefixLabel, property);
                    //     }
                    //     else
                    //     {
                    //         materialEditor.TexturePropertySingleLine(prefixLabel, property, targetColorProp);
                    //     }
                    // }

                    
                }
                else if (mat.HasProperty(extraPropName) && isHDR)
                {
                    targetColor = mat.GetColor(extraPropName);
                    
                    materialEditor.TexturePropertySingleLine(prefixLabel, property);
                    
                    Rect colorFieldRect = MaterialEditor.GetRectAfterLabelWidth(position);
                    GUIContent colorLabel = new GUIContent();
                    
                    // var targetColorProp = MaterialEditor.GetMaterialProperty(new UnityEngine.Object[] { mat }, extraPropName);
                    
                    if (!conditional)
                    {
                        targetColor = EditorGUI.ColorField(colorFieldRect, colorLabel, targetColor, true, true, true);
                    }
                    else
                    {
                        if (invert? property.textureValue == null : property.textureValue != null)
                        {
                            targetColor = EditorGUI.ColorField(colorFieldRect, colorLabel, targetColor, true, true, true);
                        }
                    }
                    
                    
                    EditorGUILayout.Space(0);
                }
                else
                {
                    materialEditor.TexturePropertySingleLine(prefixLabel, property);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(mat, "Texture Changed");

                if (mat.HasProperty(extraPropName) && isHDR)
                {
                    var allProps = MaterialEditor.GetMaterialProperties(materialEditor.targets);

                    foreach (var targetProp in allProps)
                    {
                        if (targetProp.name == extraPropName)
                        {
                            // mat.SetColor(extraPropName, targetColor);
                            targetProp.colorValue = targetColor;
                        }
                    }
                    
                }
                
                // If keyword is specified, enable/disable it based on texture value
                if (keyword != null && !mat.HasProperty(keyword))
                {
                    var materials = materialEditor.targets.OfType<Material>().ToArray();

                    foreach (var targetMaterial in materials)
                    {
                        if (property.textureValue != null)
                        {
                            targetMaterial.EnableKeyword(keyword);
                        }
                        else
                        {
                            targetMaterial.DisableKeyword(keyword);
                        }
                    }
                    
                    // if (property.textureValue != null)
                    // {
                    //     mat.EnableKeyword(keyword);
                    // }
                    // else
                    // {
                    //     mat.DisableKeyword(keyword);
                    // }
                    
                }
            }
            // EditorGUI.showMixedValue = false;
        }
    }
}
