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

namespace SGP
{
    public static class ShaderGUIStyle
    {
	    public static Color checkColor = new Color(0.4f, 0.6f, 0.2f, 1);
	    public static Color folderColor = new Color(0.85f, 0.85f, 0.85f, 1);
	    public static Color labelColor = new Color(0.85f, 0.85f, 0.85f, 1);
	    
	    public static readonly GUIContent workflowModeLabel = new GUIContent("Workflow Mode");
	    public static readonly GUIContent surfaceTypeLabel = new GUIContent("Surface Type");
	    public static readonly GUIContent blendModeLabel = new GUIContent("Blend Mode");
	    public static readonly GUIContent renderFaceLabel = new GUIContent("Render Face");
	    public static readonly GUIContent depthWriteLabel = new GUIContent("Depth Write");
	    public static readonly GUIContent depthTestLabel = new GUIContent("Depth Test");
	    public static readonly GUIContent alphaClipLabel = new GUIContent("Alpha Clipping");
	    public static readonly GUIContent castShadowLabel = new GUIContent("Cast Shadows");
	    public static readonly GUIContent receiveShadowLabel = new GUIContent("Receive Shadows");
	    public static readonly GUIContent alphaCutoffLabel = new GUIContent("Threshold");
	    public static readonly GUIContent queueOffsetLabel = new GUIContent("Sorting Priority");
	    public static readonly GUIContent queueControlLabel = new GUIContent("Queue Control");
	    public static readonly GUIContent specularHighlightsLabel = new GUIContent("Specular Highlights");
	    public static readonly GUIContent environmentReflectionsLabel = new GUIContent("Environment Reflections");

	    
        public static GUIStyle FolderHeader;
        public static GUIStyle FeatureFolderHeader;
        public static GUIStyle TextFolder;
        public static GUIStyle Folder;
        public static GUIStyle NestedFolder;
        public static GUIStyle AlwaysOnHeader;
		
        static ShaderGUIStyle()
        {
	        AlwaysOnHeader = new GUIStyle(EditorStyles.label)
	        {
		        padding = new RectOffset(5, 5, 2, 2),
		        margin = new RectOffset(17, 7, 0, 0),
		        fontSize = 14,
		        fontStyle = FontStyle.Bold,
		        wordWrap = false,
	        };
	        
			FolderHeader = new GUIStyle(EditorStyles.foldout)
			{
				padding = new RectOffset(20, 5, 2, 2),
				margin = new RectOffset(17, 7, 0, 0),
				fontSize = 14,
				fontStyle = FontStyle.Bold,
				wordWrap = false,
			};
			
			FeatureFolderHeader = new GUIStyle(EditorStyles.foldout)
			{
				padding = new RectOffset(20, 5, 2, 2),
				margin = new RectOffset(17, 40, 0, 0),
				fontSize = 14,
				fontStyle = FontStyle.Bold,
				wordWrap = false,
				
			};
			
			TextFolder = new GUIStyle(EditorStyles.helpBox)
			{
				padding = new RectOffset(5, 5, 5, 5),
				wordWrap = true,
				fontSize = GUI.skin.label.fontSize,
			};
			
			Folder = new GUIStyle(EditorStyles.helpBox)
			{
				padding = new RectOffset(5, 5, 5, 5),
				wordWrap = false,
			};
			
			NestedFolder = new GUIStyle(EditorStyles.helpBox)
			{
				padding = new RectOffset(5, 5, 5, 5),
				margin = new RectOffset(0, 0, 5, 5),
				wordWrap = false,
			};
        }
    }
}