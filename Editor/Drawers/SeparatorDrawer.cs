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
    public class SeparatorDrawer : MaterialPropertyDrawer
    {
        private float height = 1.0f;
        
        public SeparatorDrawer(float height)
        {
            this.height = height;
        }
        
        public SeparatorDrawer()
        {
        }

        public override void OnGUI(Rect position, MaterialProperty property, String label,
            MaterialEditor materialEditor)
        {
            Rect rect = GUILayoutUtility.GetRect(1, height, GUILayout.ExpandWidth(true));
            rect.y = position.y;
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;

            Color lineColor = new Color(.1f, .1f, .1f, 1);
            EditorGUI.DrawRect(rect, lineColor);
        
            EditorGUILayout.Space(-EditorGUIUtility.standardVerticalSpacing - EditorGUIUtility.singleLineHeight);
        }
    }
}
