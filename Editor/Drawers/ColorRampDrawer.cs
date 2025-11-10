/*
Copyright <2025> <HuangJuanLr>

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UseIndexFromEndExpression

namespace SGP
{
    public class ColorRampDrawer : MaterialPropertyDrawer
    {
        private static readonly Dictionary<string, Gradient> m_GradientCache = new Dictionary<string, Gradient>();
        
        private Gradient m_Gradient;

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            Material mat = editor.target as Material;
            
            string cacheKey = mat.GetInstanceID() + "_" + prop.name;
            bool needsReload = false;

            if (!m_GradientCache.TryGetValue(cacheKey, out m_Gradient))
            {
                needsReload = true;
                m_Gradient = new Gradient();
            }
            else
            {
                int materialColorCount = mat.GetInt(prop.name + "ColorCount");
                materialColorCount = Mathf.Clamp(materialColorCount, 2, 8);

                if (m_Gradient.colorKeys.Length != materialColorCount)
                {
                    needsReload = true;
                    m_Gradient = new Gradient();
                }
            }

            if (needsReload)
            {
                LoadGradientFromMaterial(mat, prop.name);
                m_GradientCache[cacheKey] = m_Gradient;
            }
            
            EditorGUI.BeginChangeCheck();
            
            GUIContent labelContent = new GUIContent(label);
            Gradient newGradient = EditorGUI.GradientField(position, labelContent, m_Gradient, true);
            
            if (EditorGUI.EndChangeCheck())
            {
                var materials = editor.targets.OfType<Material>().ToArray();
                foreach (var targetMat in materials)
                {
                    Undo.RegisterCompleteObjectUndo(targetMat, "Change Color Ramp");
                }

                m_Gradient = newGradient;
                m_GradientCache[cacheKey] = m_Gradient;
                
                SaveGradientToMaterial(mat, prop.name);

                foreach (var targetMat in materials)
                {
                    if(targetMat != mat)
                    {
                        string targetCacheKey = targetMat.GetInstanceID() + "_" + prop.name;
                        m_GradientCache[targetCacheKey] = m_Gradient;
                        SaveGradientToMaterial(targetMat, prop.name);
                    }
                }
            }
        }

        private void LoadGradientFromMaterial(Material mat, string propName)
        {
            string colorCountPropName = propName + "ColorCount";
            string alphaCountPropName = propName + "AlphaCount";
    
            int colorCount = 2;
            int alphaCount = 2;
    
            if (mat.HasProperty(colorCountPropName))
            {
                colorCount = mat.GetInt(colorCountPropName);
            }
            if (mat.HasProperty(alphaCountPropName))
            {
                alphaCount = mat.GetInt(alphaCountPropName);
            }
            
            colorCount = Mathf.Clamp(colorCount, 2, 8);
            alphaCount = Mathf.Clamp(alphaCount, 2, 8);
            
            List<GradientColorKey> colorKeys = new List<GradientColorKey>();
            for (int i = 0; i < colorCount; i++)
            {
                string colorPropName = $"{propName}Color{i}";
                if (mat.HasProperty(colorPropName))
                {
                    Color color = mat.GetColor(colorPropName);
                    colorKeys.Add(new GradientColorKey(color, color.a));
                }
            }

            List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();
            for (int i = 0; i < alphaCount; i++)
            {
                string alphaPropName = $"{propName}Alpha{i*2}";
                if (mat.HasProperty(alphaPropName))
                {
                    Vector4 alphaData = mat.GetVector(alphaPropName);
                    alphaKeys.Add(new GradientAlphaKey(alphaData.x, alphaData.y));
                    if (i * 2 + 1 < alphaCount)
                    {
                        alphaKeys.Add(new GradientAlphaKey(alphaData.z, alphaData.w));
                    }
                }
            }

            if (colorKeys.Count >= 2 && alphaKeys.Count >= 2)
            {
                m_Gradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            }
        }

        private void SaveGradientToMaterial(Material mat, string propName)
        {
            GradientColorKey[] colorKeys = m_Gradient.colorKeys;
            GradientAlphaKey[] alphaKeys = m_Gradient.alphaKeys;

            string colorCountPropName = propName + "ColorCount";
            int newColorCount = Mathf.Min(colorKeys.Length, 8);
            if (mat.HasProperty(colorCountPropName))
            {
                mat.SetInt(colorCountPropName, newColorCount);
            }

            string alphaCountPropName = propName + "AlphaCount";
            int newAlphaCount = Mathf.Min(alphaKeys.Length, 8);
            if (mat.HasProperty(alphaCountPropName))
            {
                mat.SetInt(alphaCountPropName, newAlphaCount);
            }

            for (int i = 0; i < colorKeys.Length; i++)
            {
                string colorPropName = $"{propName}Color{i}";
                if (mat.HasProperty(colorPropName))
                {
                    if (i < colorKeys.Length)
                    {
                        Color color = colorKeys[i].color;
                        color.a = colorKeys[i].time;
                        mat.SetColor(colorPropName, color);
                    }
                    else
                    {
                        // Not using ^1 to avoid compatability issue
                        Color color = colorKeys[colorKeys.Length - 1].color;
                        color.a = colorKeys[colorKeys.Length - 1].time;
                        mat.SetColor(colorPropName, color);
                    }
                }
            }

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                string alphaPropName = $"{propName}Alpha{i*2}";
                if (mat.HasProperty(alphaPropName))
                {
                    int index0 = i * 2;
                    int index1 = i * 2 + 1;
                    
                    Vector4 alphaData = Vector4.zero;
                    if (index0 < alphaKeys.Length)
                    {
                        alphaData.x = alphaKeys[index0].alpha;
                        alphaData.y = alphaKeys[index0].time;
                    }
                    else if(alphaKeys.Length > 0)
                    {
                        alphaData.x = alphaKeys[alphaKeys.Length - 1].alpha;
                        alphaData.y = 1.0f;
                    }

                    if (index1 < alphaKeys.Length)
                    {
                        alphaData.z = alphaKeys[index1].alpha;
                        alphaData.w = alphaKeys[index1].time;
                    }
                    else if (alphaKeys.Length > 0)
                    {
                        alphaData.z = alphaKeys[alphaKeys.Length - 1].alpha;
                        alphaData.w = 1.0f;
                    }
                    mat.SetVector(alphaPropName, alphaData);
                }
            }
        }
    }
}