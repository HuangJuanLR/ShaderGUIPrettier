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
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SGP;

namespace SGP
{
    public enum SubType
    {
        TexturePrefix,
        
        DefaultMax,
    }

    public enum InlineType
    {
        Vector2,
        Vector3,
        Vector4,
        Remapping,
        ScaleOffset,
        Separator,
        Space,
        
        DefaultMax,
    }

    public enum FolderType
    {
        Folder,
        FeatureFolder,
        ConditionFolder,
        ConditionBlock,
        Text,
        
        DefaultMax,
    }
    
    public class ShaderGraphGUIPrettier : ShaderGUIPrettierBase
    {
        // private Material material;
        private IUniversalDrawer containerDrawer;
        
        // private static Dictionary<string, bool> toggles = new Dictionary<string, bool>();
        
        private static Dictionary<MaterialProperty, List<MaterialProperty>> subProps = new Dictionary<MaterialProperty, List<MaterialProperty>>();

        private static List<MaterialProperty> tempSubPropList = new List<MaterialProperty>();

        private static Dictionary<SubType, string> subTypes = new Dictionary<SubType, string>()
        {
            {SubType.TexturePrefix, "&"}, // & stands for texture and another property
            
            {SubType.DefaultMax, "DefaultMax"},
        };

        // InlineType and FolderType can share the same symbol
        // Cause we use [] to check whether this is a folder
        private static Dictionary<InlineType, string> inlineTypes = new Dictionary<InlineType, string>()
        {
            { InlineType.Vector2, "@2"},
            { InlineType.Vector3, "@3"},
            { InlineType.Vector4, "@4"},
            { InlineType.Remapping , "~"}, // ~ looks like a from/to
            { InlineType.ScaleOffset , "#"}, // # scale offset take 4 float field, look like a #
            { InlineType.Separator , "^"},
            { InlineType.Space , "%"},
            
            {InlineType.DefaultMax, "DefaultMax"},
        };

        private static Dictionary<FolderType, string> folderTypes = new Dictionary<FolderType, string>()
        {
            { FolderType.Folder , "$"}, // $ is a Houdini like symbol
            { FolderType.FeatureFolder , "+"}, // + stands for adding feature
            { FolderType.ConditionFolder , "?"}, // ? stands for comparison
            { FolderType.ConditionBlock, "!"},
            { FolderType.Text , "*"}, // * for annotation

            { FolderType.DefaultMax , "DefaultMax"},
        };
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            isGraph = true;

            hasSurfaceOptions = IsAllowMaterialOverride(properties);

            if (hasSurfaceOptions)
            {
                DrawFolder(materialEditor, "Surface Options", () =>
                {
                    DrawSurfaceOptions(material, materialEditor, properties);
                });
            }
            
            if (Event.current.type == EventType.Layout) containerDrawer = BuildDrawer(material, properties);
            
            containerDrawer.Draw(materialEditor, material, name => FindProperty(name, properties));
            
            DrawFolder(materialEditor, "Advanced Options", () =>
            {
                DrawAdvancedOptions(material, materialEditor, properties);
            });
        }
        
        private static IUniversalDrawer BuildDrawer(Material material, MaterialProperty[] properties)
        {
            var handlersQueue = new Stack<IUniversalDrawer>();
            IUniversalDrawer containerDrawer = new ContainerDrawer();
            var curContainer = containerDrawer;
            
            List<MaterialProperty> drawerProps = new List<MaterialProperty>();
            
            MaterialProperty curParentProp = new MaterialProperty();
            
            for (int i = 0; i < properties.Length; i++)
            {
                MaterialProperty curProp = properties[i];
                string displayName = curProp.displayName;
                
                // HideInInspector Properties in Shader Graph are for controlling Surface Options/Advanced Options
                if(curProp.flags.HasFlag(MaterialProperty.PropFlags.HideInInspector))
                {
                    continue;
                }

                // Store sub-props and corresponding parent
                // Do not add prefixed properties into the pool
                if (!HasSubPrefix(displayName))
                {
                    curParentProp = curProp;
                    tempSubPropList.Clear();
                }
                else
                {
                    if (i == 0)
                    {
                        Debug.LogError($"The first property {displayName} should not be a sub-prop");
                    }
                    else
                    {
                        tempSubPropList.Add(curProp);

                        if (!subProps.TryGetValue(curParentProp, out var tempProp))
                        {
                            List<MaterialProperty> propListForInit = new List<MaterialProperty>();
                            foreach (MaterialProperty p in tempSubPropList)
                            {
                                propListForInit.Add(p);
                            }
                            subProps.Add(curParentProp, propListForInit);
                        }
                        
                    }

                    continue;
                }
                
                drawerProps.Add(curProp);
                
            }
            
            // These are actual props need to be drawn
            try
            {
                foreach (MaterialProperty prop in drawerProps)
                {
                    string propName = prop.displayName;
                    ProcessAttributes(ref curContainer, handlersQueue, prop);

                    if (ProcessProperties(curContainer, handlersQueue, prop))
                    {
                        handlersQueue.Clear();
                    }
                
                    // Debug.Log(propName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + " in " +  $"{material.name}");

                return null;
            }
            
            return containerDrawer;
        }

        private static void ProcessAttributes(ref IUniversalDrawer container, Stack<IUniversalDrawer> queue, 
                        MaterialProperty property)
        {
            string attrib = "";
            string parms = "";

            string displayName = property.displayName;

            if (FindFolder(displayName, out attrib, out parms))
            {
                if (attrib.ToLower().Equals("close"))
                {
                    if (container.Container != null)
                    {
                        container = container.Container;
                    }
                }

                var drawer = CreateDrawers(attrib, parms, property);
                
                if (drawer != null)
                {
                    container.Add(drawer);
   
                    if (drawer.Containable)
                    {
                        container = drawer;
                    }
                }
            }
        }
        
        private static bool ProcessProperties(IUniversalDrawer container, Stack<IUniversalDrawer> queue,
            MaterialProperty prop)
        {
            string displayName = prop.displayName;
            string propName = prop.name;
            
            string attrib = "";
            string parms = "";
            if (!FindFolder(displayName, out attrib, out parms))
            {
                IUniversalDrawer drawer;
                if (queue.Count > 0)
                {
                    drawer = queue.Pop();
                }
                else
                {
                    drawer = CreatePropertyDrawer(prop);
                }
                
                drawer.Init(propName);
                container.Add(drawer);
                
                return true;
            }

            return false;
        }

        private static bool FindFolder(string data, out string attrib, out string parms)
        {
            attrib = "";
            parms = "";

            bool found = false;
            
            int startIndex = 0;
	    
            if ((startIndex = data.IndexOf("[")) != -1)
            {
                int endIndex = data.IndexOf("]", startIndex);
			
                if (endIndex != -1)
                {
                    found = true;
                    
                    string attribs = data.Substring(startIndex + 1, endIndex - startIndex - 1);
                    attrib = attribs;
                    // Find Parameters Inside ()
                    int startParmIndex = 0;
			    
                    if ((startParmIndex = attribs.IndexOf("(")) != -1)
                    {
                        int endParmIndex =  attribs.IndexOf(")", startParmIndex);
					
                        // Has Parameters
                        if (endParmIndex != -1)
                        {
                            parms = attribs.Substring(startParmIndex + 1, 
                                endParmIndex - startParmIndex - 1);
					    
                            //	attribs.Substring(0, startParmIndex - 0)
                            attrib = attribs.Substring(0, startParmIndex);
                        }
                    }
                }
            }

            return found;
        }
        
        private static IUniversalDrawer CreateDrawers(string attrib, string parms, MaterialProperty property)
        {
            IUniversalDrawer drawer = null;
            
            string propName = property.name;
            string displayName = property.displayName;
            string folderName = GetFolderName(displayName);

            FolderType curFolderType;
            if (TryFindFolderType(attrib, out curFolderType))
            {
                switch (curFolderType)
                {
                    case FolderType.Text:
                        drawer = new TextDrawer();
                        break;
                    case FolderType.Folder:
                        drawer = new FolderDrawer();
                        parms = folderName + "," + parms;
                        break;
                    case FolderType.FeatureFolder:
                        drawer = new FeatureFolderDrawer();
                        parms = folderName + "," + propName;
                        break;
                    case FolderType.ConditionFolder:
                        drawer = new ConditionFolderDrawer();
                        break;
                    case FolderType.ConditionBlock:
                        drawer = new ConditionBlockDrawer();
                        break;
                }
            }
            

            if (drawer != null)
            {
                drawer.Init(parms);
            }

            return drawer;
        }

        private static IUniversalDrawer CreatePropertyDrawer(MaterialProperty prop)
        {
            IUniversalDrawer drawer = null;

            var propType = prop.type;
            var propName = prop.name;
            var displayName = prop.displayName;

            SubType curSubType;
            if(TryFindSubType(prop, out curSubType))
            {
                List<MaterialProperty> curSubProps;
                switch (curSubType)
                {
                    case SubType.TexturePrefix:
                        if (propType != MaterialProperty.PropType.Texture) return new PropertiesDrawer();
                        
                        if (subProps.TryGetValue(prop, out curSubProps))
                        {
                            if (curSubProps.Count > 1)
                            {
                                throw new Exception($"Exceed the length of sub-props {propName} needs");
                            }
                            
                            drawer = new GraphTextureDrawer(curSubProps[0]);
                        }
                        else
                        {
                            drawer = new GraphTextureDrawer();
                        }
                        break;
                }
            }
            else
            {
                if (propType == MaterialProperty.PropType.Texture) return new GraphTextureDrawer();
            }

            InlineType curInlineType;
            if (TryFindInlineType(displayName, out curInlineType))
            {
                float height = 1.0f;
                string actualDisplayName = displayName.Replace(inlineTypes[curInlineType], "");
                switch (curInlineType)
                {
                    case InlineType.Remapping:
                        if (propType != MaterialProperty.PropType.Vector) return new PropertiesDrawer();

                        drawer = new GraphRemappingDrawer(actualDisplayName);
                        break;
                    case InlineType.Vector2:
                        if (propType != MaterialProperty.PropType.Vector) return new PropertiesDrawer();

                        drawer = new GraphVector2Drawer(actualDisplayName);
                        break;
                    case InlineType.Vector3:
                        if (propType != MaterialProperty.PropType.Vector) return new PropertiesDrawer();

                        drawer = new GraphVector3Drawer(actualDisplayName);
                        break;
                    case InlineType.Vector4:
                        if (propType != MaterialProperty.PropType.Vector) return new PropertiesDrawer();

                        drawer = new GraphVector4Drawer(actualDisplayName);
                        break;
                    case InlineType.ScaleOffset:
                        if (propType != MaterialProperty.PropType.Vector) return new PropertiesDrawer();

                        drawer = new GraphScaleOffsetDrawer();
                        break;
                    case InlineType.Separator:
                        if (TryFindHeight(displayName, out float separatorHeight))
                        {
                            height = separatorHeight;
                        }
                        drawer = new GraphSeparatorDrawer(height);
                        break;
                    case InlineType.Space:
                        if (TryFindHeight(displayName, out float spaceHeight))
                        {
                            height = spaceHeight;
                        }
                        else
                        {
                            height = 10.0f;
                        }
                        drawer = new GraphSpaceDrawer(height);
                        break;
                }
            }
            
            return drawer?? new PropertiesDrawer();
        }


        
        
        private static bool TryFindSubType(MaterialProperty prop, out SubType type)
        {
            type = SubType.DefaultMax;
            if (subProps.TryGetValue(prop, out var curSubProps))
            {
                foreach (SubType t in subTypes.Keys)
                {
                    if (curSubProps[0].displayName.Contains(subTypes[t]))
                    {
                        type = t;
                        return true;
                    }
                }
            }
            

            return false;
        }

        private static bool HasSubPrefix(string displayName)
        {
            foreach (string prefix in subTypes.Values)
            {
                if (displayName.Contains(prefix)) return true;
            }

            return false;
        }

        private static bool TryFindInlineType(string name, out InlineType type)
        {
            type = InlineType.DefaultMax;
            
            foreach (InlineType t in inlineTypes.Keys)
            {
                if (name.Contains(inlineTypes[t]))
                {
                    type = t;
                    return true;
                }
            }

            return false;
        }

        private static bool TryFindFolderType(string name, out FolderType type)
        {
            type = FolderType.DefaultMax;
            
            // Already calls FindFolder in ProcessAttribute()
            // No need to check if this is a folder again
            foreach (FolderType t in folderTypes.Keys)
            {
                if (name.Contains(folderTypes[t]))
                {
                    type = t;
                    return true;
                }
            }

            return false;
        }

        private static string GetFolderName(string displayName)
        {
            string[] name = displayName.Split(new[] { "]" }, StringSplitOptions.RemoveEmptyEntries);
            
            return name[name.Length - 1];
        }

        private static bool TryFindHeight(string displayName, out float height)
        {
            height = 1.0f;
            foreach (string prefix in inlineTypes.Values)
            {
                if (displayName.Contains(prefix))
                {
                    int startIndex = 0;
	    
                    if ((startIndex = displayName.IndexOf("(", StringComparison.Ordinal)) != -1)
                    {
                        int endIndex = displayName.IndexOf(")", startIndex, StringComparison.Ordinal);
			
                        if (endIndex != -1)
                        {
                             height = float.Parse(displayName.Substring(startIndex + 1, 
                                endIndex - startIndex - 1));

                             return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}

public class ShaderGraphGUIPrettier : SGP.ShaderGraphGUIPrettier
{
}

