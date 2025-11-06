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

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine.Rendering;

namespace SGP
{
	internal static class ShaderKeywordStrings
	{
		public static readonly string _ALPHATEST_ON = "_ALPHATEST_ON";
		public static readonly string _SURFACE_TYPE_TRANSPARENT = "_SURFACE_TYPE_TRANSPARENT";
		public static readonly string _ALPHAPREMULTIPLY_ON = "_ALPHAPREMULTIPLY_ON";
		public static readonly string _ALPHAMODULATE_ON = "_ALPHAMODULATE_ON";
		public static readonly string _RECEIVE_SHADOWS_OFF = "_RECEIVE_SHADOWS_OFF";
	}
	internal static class Properties
	{
		public static readonly string WorkflowMode = "_WorkflowMode";
		public static readonly string SurfaceType = "_Surface";
		public static readonly string BlendMode = "_Blend";
		public static readonly string AlphaClip = "_AlphaClip";
		public static readonly string SrcBlend = "_SrcBlend";
		public static readonly string DstBlend = "_DstBlend";
		public static readonly string ZWrite = "_ZWrite";
		public static readonly string RenderFace = "_Cull";
		public static readonly string CastShadows = "_CastShadows";
		public static readonly string ReceiveShadows = "_ReceiveShadows";
		public static readonly string QueueOffset = "_QueueOffset";
		public static readonly string AlphaCutoff = "_Cutoff";
		public static readonly string ZTest = "_ZTest";
		public static readonly string ZWriteControl = "_ZWriteControl";
		public static readonly string QueueControl = "_QueueControl";
		
		public static readonly string SpecularHighlights = "_SpecularHighlights";
		public static readonly string EnvironmentReflections = "_EnvironmentReflections";

		public static readonly string Emission = "_EMISSION";
	}
	
	public class ShaderGUIPrettierBase : ShaderGUI
	{
		protected Material material;

		private static Dictionary<string, bool> toggles = new Dictionary<string, bool>();

		protected static bool isGraph = false;

		protected bool hasSurfaceOptions = false;

		private static readonly string[] surfaceOptionsProperties = new[]
		{
			Properties.WorkflowMode,
			Properties.SurfaceType,
			Properties.BlendMode,
			Properties.AlphaClip,
			Properties.SrcBlend,
			Properties.DstBlend,
			Properties.ZWrite,
			Properties.RenderFace,
			Properties.CastShadows,
			Properties.ReceiveShadows,
			Properties.AlphaCutoff,
			Properties.ZTest,
			Properties.ZWriteControl,
		};

        private MaterialProperty workflowModeProp;
        private MaterialProperty surfaceTypeProp;
        private MaterialProperty blendModeProp;
        private MaterialProperty alphaClipProp;
        private MaterialProperty renderFaceProp;
        private MaterialProperty depthWriteProp;
        private MaterialProperty depthTestProp;
        private MaterialProperty castShadowsProp;
        private MaterialProperty receiveShadowsProp;
        private MaterialProperty alphaCutoffProp;
        private MaterialProperty queueOffsetProp;
        private MaterialProperty specularHighlightsProp;
        private MaterialProperty environmentReflectionsProp;

        private void FindProperties(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (material == null)
                return;

            workflowModeProp = FindProperty(Properties.WorkflowMode, properties, false);
            surfaceTypeProp = FindProperty(Properties.SurfaceType, properties, false);
            blendModeProp = FindProperty(Properties.BlendMode, properties, false);
            alphaClipProp = FindProperty(Properties.AlphaClip, properties, false);
            renderFaceProp = FindProperty(Properties.RenderFace, properties, false);
            depthWriteProp = FindProperty(Properties.ZWriteControl, properties, false);
            depthTestProp = FindProperty(Properties.ZTest, properties, false);
            castShadowsProp = FindProperty(Properties.CastShadows, properties, false);
            receiveShadowsProp = FindProperty(Properties.ReceiveShadows, properties, false);
            alphaCutoffProp = FindProperty(Properties.AlphaCutoff, properties, false);
            queueOffsetProp = FindProperty(Properties.QueueOffset, properties, false);
            specularHighlightsProp = FindProperty(Properties.SpecularHighlights, properties, false);
            environmentReflectionsProp = FindProperty(Properties.EnvironmentReflections, properties, false);
        }
		
	    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	    {
		    material = (Material)materialEditor.target;
	        
	        Material mat = materialEditor.target as Material;
	        
	        FindProperties(materialEditor, properties);

	        MaterialValidation(material);
	    }
	    
	    protected void DrawAdvancedOptions(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
	    {
		    if (specularHighlightsProp != null && environmentReflectionsProp != null)
		    {
			    materialEditor.ShaderProperty(specularHighlightsProp, ShaderGUIStyle.specularHighlightsLabel);
			    materialEditor.ShaderProperty(environmentReflectionsProp, ShaderGUIStyle.environmentReflectionsLabel);
		    }
		    
		    bool autoQueueControl = GetAutoQueueControlSetting(material);
		    MaterialProperty queueControlProp = FindProperty(Properties.QueueControl, properties, false);
		    DrawPopupProperty(ShaderGUIStyle.queueControlLabel, queueControlProp, materialEditor, Enum.GetNames(typeof(QueueControl)));
		    if (autoQueueControl)
		    {
			    DrawQueueOffsetField(materialEditor);
		    }
		    else
		    {
			    if(UnityEngine.Rendering.SupportedRenderingFeatures.active.editableMaterialRenderQueue) 
				    materialEditor.RenderQueueField();
		    }
                
		    materialEditor.EnableInstancingField();
		    materialEditor.DoubleSidedGIField();
		    
		    if(material.IsKeywordEnabled(Properties.Emission))
				materialEditor.LightmapEmissionProperty();
	    }
	    
	    protected void DrawSurfaceOptions(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
	    {
		    if (hasSurfaceOptions)
		    {
			    DrawPopupProperty(ShaderGUIStyle.workflowModeLabel, workflowModeProp, materialEditor, Enum.GetNames(typeof(WorkflowMode)));
                
			    DrawPopupProperty(ShaderGUIStyle.surfaceTypeLabel, surfaceTypeProp, materialEditor, Enum.GetNames(typeof(SurfaceType)));
			    
			    if ((surfaceTypeProp != null) && ((SurfaceType)surfaceTypeProp.floatValue == SurfaceType.Transparent))
			    {
				    DrawPopupProperty(ShaderGUIStyle.blendModeLabel, blendModeProp, materialEditor,
					    Enum.GetNames(typeof(BlendMode)));
			    }

			    DrawPopupProperty(ShaderGUIStyle.renderFaceLabel, renderFaceProp, materialEditor, Enum.GetNames(typeof(RenderFace)));
                
			    DrawPopupProperty(ShaderGUIStyle.depthWriteLabel, depthWriteProp, materialEditor, Enum.GetNames(typeof(ZWriteControl)));
			    if(depthTestProp != null)
				    DrawPopupProperty(ShaderGUIStyle.depthTestLabel, depthTestProp, materialEditor, Enum.GetNames(typeof(CompareFunction)));
                
			    // Unity won't add _Cutoff by default, we should manually add this property to the blackboard
			    DrawToggleProperty(ShaderGUIStyle.alphaClipLabel, alphaClipProp);
			    if(alphaClipProp != null && alphaCutoffProp != null && alphaClipProp.floatValue == 1)
				    materialEditor.ShaderProperty(alphaCutoffProp, ShaderGUIStyle.alphaCutoffLabel, 1);
                    
			    DrawToggleProperty(ShaderGUIStyle.castShadowLabel, castShadowsProp);
			    DrawToggleProperty(ShaderGUIStyle.receiveShadowLabel, receiveShadowsProp);
		    }
	    }

	    // Copied From Unity BaseShaderGUI
	    private static void SetupMaterialBlendMode(Material material, out int autoRenderQueue)
	    {
		    bool alphaClip = false;
		    if (material.HasProperty(Properties.AlphaClip))
			    alphaClip = material.GetFloat(Properties.AlphaClip) >= 0.5f;
		    
		    if (alphaClip)
		    {
			    material.EnableKeyword(ShaderKeywordStrings._ALPHATEST_ON);    
		    }
		    else
		    {
			    material.DisableKeyword(ShaderKeywordStrings._ALPHATEST_ON);
		    }

		    int renderQueue = material.shader.renderQueue;
		    material.SetOverrideTag("RenderType", "");
		    if (material.HasProperty(Properties.SurfaceType))
		    {
			    SurfaceType surfaceType =
				    (SurfaceType)material.GetFloat(Properties.SurfaceType);
			    bool zwrite = false;
			    
			    if (surfaceType == SurfaceType.Transparent)
			    {
				    material.EnableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
			    }
			    else
			    {
				    material.DisableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
			    }
				
			    if (surfaceType == SurfaceType.Opaque)
                {
                    if (alphaClip)
                    {
                        renderQueue = (int)RenderQueue.AlphaTest;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                    }
                    else
                    {
                        renderQueue = (int)RenderQueue.Geometry;
                        material.SetOverrideTag("RenderType", "Opaque");
                    }

                    SetMaterialSrcDstBlendProperties(material, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
                    zwrite = true;
                    material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                    material.DisableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
                }
                else // SurfaceType Transparent
                {
                    BlendMode blendMode = (BlendMode)material.GetFloat(Properties.BlendMode);

                    material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                    material.DisableKeyword(ShaderKeywordStrings._ALPHAMODULATE_ON);

                    // Specific Transparent Mode Settings
                    switch (blendMode)
                    {
                        case BlendMode.Alpha:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.SrcAlpha,
                                UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            break;
                        case BlendMode.Premultiply:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.One,
                                UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.EnableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                            break;
                        case BlendMode.Additive:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.SrcAlpha,
                                UnityEngine.Rendering.BlendMode.One);
                            break;
                        case BlendMode.Multiply:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.DstColor,
                                UnityEngine.Rendering.BlendMode.Zero);
                            material.EnableKeyword(ShaderKeywordStrings._ALPHAMODULATE_ON);
                            break;
                    }

                    // General Transparent Material Settings
                    material.SetOverrideTag("RenderType", "Transparent");
                    zwrite = false;
                    material.EnableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
                    renderQueue = (int)RenderQueue.Transparent;
                }
			    
			    // check for override enum
			    if (material.HasProperty(Properties.ZWriteControl))
			    {
				    var zwriteControl = (ZWriteControl)material.GetFloat(Properties.ZWriteControl);
				    if (zwriteControl == ZWriteControl.ForceEnabled)
					    zwrite = true;
				    else if (zwriteControl == ZWriteControl.ForceDisabled)
					    zwrite = false;
			    }
			    SetMaterialZWriteProperty(material, zwrite);
			    material.SetShaderPassEnabled("DepthOnly", zwrite);
		    }
		    else
		    {
			    // no surface type property -- must be hard-coded by the shadergraph,
			    // so ensure the pass is enabled at the material level
			    material.SetShaderPassEnabled("DepthOnly", true);
		    }
		    
		    if (material.HasProperty(Properties.QueueOffset))
			    renderQueue += (int)material.GetFloat(Properties.QueueOffset);

		    autoRenderQueue = renderQueue;
	    }

	    private static void UpdateSurfaceOptions(Material material, bool autoRenderQueue)
	    {
		    
		    SetupMaterialBlendMode(material, out int renderQueue);

		    if (autoRenderQueue && (renderQueue != material.renderQueue))
			   material.renderQueue = renderQueue;
		    
		    // Cast Shadows
		    bool castShadows = true;

		    if (material.HasProperty(Properties.CastShadows))
		    {
			    castShadows = material.GetFloat(Properties.CastShadows) != 0.0f;
		    }
		    else
		    {
			    if (isGraph)
			    {
				    castShadows = true;
			    }
			    else
			    {
				    castShadows = IsOpaque(material);
			    }
		    }
		    
		    material.SetShaderPassEnabled("ShadowCaster", castShadows);
		    
		    // Receive Shadows
		    if (material.HasProperty(Properties.ReceiveShadows))
		    {
			    if (material.GetFloat(Properties.ReceiveShadows) == 0.0f)
			    {
				    material.EnableKeyword(ShaderKeywordStrings._RECEIVE_SHADOWS_OFF);
			    }
			    else
			    {
				    material.DisableKeyword(ShaderKeywordStrings._RECEIVE_SHADOWS_OFF);
			    }
		    }
		    
		    // Setup double sided GI based on Cull state
		    if (material.HasProperty(Properties.RenderFace))
			    material.doubleSidedGI = (RenderFace)material.GetFloat(Properties.RenderFace) != RenderFace.Front;
	    }

	    private static void SetMaterialSrcDstBlendProperties(Material material,
													    UnityEngine.Rendering.BlendMode srcBlend,
													    UnityEngine.Rendering.BlendMode dstBlend)
	    {
		    if (material.HasProperty(Properties.SrcBlend))
			    material.SetFloat(Properties.SrcBlend, (float)srcBlend);

		    if (material.HasProperty(Properties.DstBlend))
			    material.SetFloat(Properties.DstBlend, (float)dstBlend);
	    }

	    private static void SetMaterialZWriteProperty(Material material, bool zwriteEnabled)
	    {
		    if (material.HasProperty(Properties.ZWrite))
			    material.SetFloat(Properties.ZWrite, zwriteEnabled ? 1.0f : 0.0f);
	    }

	    protected static bool GetAutoQueueControlSetting(Material material)
	    {
			
		    bool autoQueueControl = false;
		    if (material.HasProperty(Properties.QueueControl))
		    {
			    float queueControl = material.GetFloat(Properties.QueueControl);
			    if (queueControl < 0.0f)
			    {
				    UpdateRenderQueueControl(material);
			    }

			    // ReSharper disable once CompareOfFloatsByEqualityOperator
			    autoQueueControl = material.GetFloat(Properties.QueueControl) ==
			                       (float)QueueControl.Auto;
		    }

		    return autoQueueControl;
	    }

	    // My version of Unity's ValidateMaterial() to ignore Unity version
	    private void MaterialValidation(Material material)
	    {
		    if (material == null)
			    throw new ArgumentNullException("material");

		    UpdateMaterial(material, MaterialUpdateType.ModifiedMaterial);
	    }

	    private static void UpdateMaterial(Material material, MaterialUpdateType updateType)
	    {
		    if (updateType == MaterialUpdateType.CreatedNewMaterial)
			    material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

		    bool automaticRenderQueue = GetAutoQueueControlSetting(material);
		    UpdateSurfaceOptions(material, automaticRenderQueue);
		    SetupSpecularWorkflowKeyword(material, out bool isSpecularWorkflow);
	    }

	    private static void SetupSpecularWorkflowKeyword(Material material, out bool isSpecularWorkflow)
	    {
		    isSpecularWorkflow = false;     // default is metallic workflow
		    if (material.HasProperty(Properties.WorkflowMode))
			    isSpecularWorkflow = ((WorkflowMode)material.GetFloat(Properties.WorkflowMode)) == WorkflowMode.Specular;
		    
		    if (isSpecularWorkflow)
		    {
			    material.EnableKeyword("_SPECULAR_SETUP");
		    }
		    else
		    {
			    material.DisableKeyword("_SPECULAR_SETUP");
		    }
	    }

	    protected void DrawQueueOffsetField(MaterialEditor materialEditor)
	    {
		    if (queueOffsetProp != null)
			    IntSliderShaderProperty(materialEditor, queueOffsetProp, -50, 50, ShaderGUIStyle.queueOffsetLabel);
	    }

	    private static void UpdateRenderQueueControl(Material material)
	    {
		    if (isGraph)
		    {
			    // Automatic behavior - surface type override
			    material.SetFloat(Properties.QueueControl, (float)QueueControl.Auto); 
		    }
		    else
		    {
			    // User has selected explicit render queue
			    material.SetFloat(Properties.QueueControl, (float)QueueControl.UserOverride); 
		    }
	    }
	    
	    private static bool IsOpaque(Material material)
	    {
		    bool opaque = true;
		    if (material.HasProperty(Properties.SurfaceType))
			    opaque = ((SurfaceType)material.GetFloat(Properties.SurfaceType) == SurfaceType.Opaque);
		    return opaque;
	    }
	    
	    protected void DrawFolder(MaterialEditor materialEditor, string label, Action draw)
	    {
		    EditorGUILayout.BeginVertical(ShaderGUIStyle.Folder);
		    {
			    GUI.backgroundColor = ShaderGUIStyle.labelColor;
			    string toggleName = material.GetHashCode() + "_" + label;

			    bool toggle = true;

			    if (!toggles.TryGetValue(toggleName, out var storedToggle))
			    {
				    toggles.Add(toggleName, toggle);
			    }
			    else
			    {
				    toggle = storedToggle;
			    }
			    
			    toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, label, ShaderGUIStyle.FolderHeader);

			    EditorGUILayout.EndFoldoutHeaderGroup();
				
			    var materials = materialEditor.targets.OfType<Material>().ToArray();
			    foreach (var targetMaterial in materials)
			    {
				    string curToggleName = targetMaterial.GetHashCode() + "_" + label;
				    toggles[curToggleName] = toggle;
			    }
			    // toggles[toggleName] = toggle;

			    GUI.backgroundColor = ShaderGUIStyle.folderColor;

			    if(toggle)
			    {
				    EditorGUILayout.BeginVertical(ShaderGUIStyle.NestedFolder);
				    {
					    draw();
				    }
				    EditorGUILayout.EndVertical();
			    }
		    }
		    EditorGUILayout.EndVertical();
	    }
	    
	    protected static void DrawToggleProperty(GUIContent label, MaterialProperty property)
	    {
		    if (property == null)
			    return;

		    EditorGUI.BeginChangeCheck();

		    bool newValue = EditorGUILayout.Toggle(label, property.floatValue == 1);
            
		    if (EditorGUI.EndChangeCheck())
			    property.floatValue = newValue ? 1.0f : 0.0f;
	    }
        
	    protected static void DrawPopupProperty(GUIContent label, MaterialProperty property, MaterialEditor materialEditor, string[] options)
	    {
		    if (property != null)
			    PopupShaderProperty(materialEditor, property, label, options);
	    }
	    
	    protected static bool IsAllowMaterialOverride(MaterialProperty[] properties)
	    {
		    bool allowed = false;
		    foreach (var property in properties)
		    {
			    string name = property.name;

			    if (surfaceOptionsProperties.Contains(name))
				    allowed = true;
		    }

		    return allowed;
	    }
	    
	    private static void IntSliderShaderProperty(MaterialEditor materialEditor, MaterialProperty prop, int min, int max, GUIContent label)
	    {
#if UNITY_2022_1_OR_NEWER
		    MaterialEditor.BeginProperty(prop);
#endif

		    EditorGUI.BeginChangeCheck();
		    EditorGUI.showMixedValue = prop.hasMixedValue;
		    int newValue = EditorGUI.IntSlider(GetRect(prop), label, (int)prop.floatValue, min, max);
		    EditorGUI.showMixedValue = false;
		    if (EditorGUI.EndChangeCheck())
		    {
			    materialEditor.RegisterPropertyChangeUndo(label.text);
			    prop.floatValue = newValue;
		    }

#if UNITY_2022_1_OR_NEWER
		    MaterialEditor.EndProperty();
#endif
	    }
	    
	    private static Rect GetRect(MaterialProperty prop)
	    {
		    return EditorGUILayout.GetControlRect(true, MaterialEditor.GetDefaultPropertyHeight(prop), EditorStyles.layerMaskField);
	    }
	    
	    private static int PopupShaderProperty(MaterialEditor materialEditor, MaterialProperty prop, GUIContent label, string[] displayedOptions)
	    {
#if UNITY_2022_1_OR_NEWER
		    MaterialEditor.BeginProperty(prop);
#endif
		    int val = (int)prop.floatValue;

		    EditorGUI.BeginChangeCheck();
		    EditorGUI.showMixedValue = prop.hasMixedValue;
		    int newValue = EditorGUILayout.Popup(label, val, displayedOptions);
		    EditorGUI.showMixedValue = false;
		    if (EditorGUI.EndChangeCheck() && (newValue != val || prop.hasMixedValue))
		    {
			    materialEditor.RegisterPropertyChangeUndo(label.text);
			    prop.floatValue = val = newValue;
		    }

#if UNITY_2022_1_OR_NEWER
		    MaterialEditor.EndProperty();
#endif

		    return val;
	    }
	}
}

