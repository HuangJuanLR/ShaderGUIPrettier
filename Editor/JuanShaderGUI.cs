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
using System.IO;
using JuanShaderEditor;

namespace JuanShaderEditor
{
	public class JuanCustomShaderGUI : JuanBaseShaderGUI
	{
	    private IUniversalDrawer containerDrawer;
	    
	    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	    {
			base.OnGUI(materialEditor, properties);
			
			isGraph = false;
			
			hasSurfaceOptions = IsAllowMaterialOverride(properties);

			if (hasSurfaceOptions)
			{
				DrawFolder(materialEditor, "Surface Options", () =>
				{
					DrawSurfaceOptions(material, materialEditor, properties);
				});
			}
			
	        
	        if (Event.current.type == EventType.Layout) containerDrawer = BuildDrawer(material);

	        if (containerDrawer == null)
	        {
		        materialEditor.PropertiesDefaultGUI(properties);
		        return;
	        }
	        
	        // containerDrawer.Draw(materialEditor, material, FindProperty(containerDrawer.Name, properties));
	        containerDrawer.Draw(materialEditor, material, name => FindProperty(name, properties));

	        // Recreate Unity's Default Advanced Options Folder
	        DrawFolder(materialEditor, "Advanced Options", () => 
	        {
		        DrawAdvancedOptions(material, materialEditor, properties);
	        });

	    }
	    
	    public static IUniversalDrawer BuildDrawer(Material material)
	    {
		    var handlersQueue = new Stack<IUniversalDrawer>();

		    IUniversalDrawer containerDrawer = new ContainerDrawer();
		    var curContainer = containerDrawer;
		    
		    StreamReader streamReader = ReadShader(material);
		    
		    var lineNum = 1;
		    try
		    {
			    while (!streamReader.EndOfStream)
			    {
				    var line = streamReader.ReadLine();
				    
				    if (line.Trim() == "") continue;
					
				    // Avoid detecting commented line
				    if (FindComments(line) && !line.Contains("SubShader"))
				    {
					    continue;
				    }
					
				    // Process Special Drawers
				    ProcessAttributes(ref curContainer, handlersQueue, line);
				    
				    // Process Common Drawers
				    if (ProcessProperties(curContainer, handlersQueue, line))
				    {
					    handlersQueue.Clear();
				    }
				    
				    lineNum++;
				    
				    if (line.Contains("SubShader"))
				    {
					    break;
				    }
			    }
		    }
		    catch (Exception e)
		    {
			    Debug.LogError(e.Message + " in " +  $"{material.name}");
			    
			    streamReader.Close();

			    return null;
		    }

		    streamReader.Close();

		    return containerDrawer;
	    }
	    

	    private static void ProcessAttributes(ref IUniversalDrawer container, Stack<IUniversalDrawer> queue, string line)
	    {
		    string attrib = "";
		    string parms = "";

		    if (FindAttribWithParm(line, out attrib, out parms))
		    {
			    if (attrib.ToLower().Equals("close"))
			    {
				    if (container.Container != null)
				    {
					    container = container.Container;
				    }
			    }
			    
			    var drawer = CreateDrawers(attrib, parms);
			    
			    if (drawer != null)
			    {
				    if (attrib.ToLower().Equals("hideininspector"))
				    {
					    queue.Push(drawer);
				    }
				    else
				    {
					    container.Add(drawer);
	       
					    if (drawer.Containable)
					    {
						    container = drawer;
					    }
				    }
			    }
		    }
	    }
	    
	    private static bool ProcessProperties(IUniversalDrawer container, Stack<IUniversalDrawer> queue,
		    string line)
	    {
		    string property = "";
		    
		    if (FindProperties(line, out property))
		    {
			    IUniversalDrawer drawer;
			    if (queue.Count > 0)
			    {
				    drawer = queue.Pop();
			    }
			    else
			    {
				    drawer = new PropertiesDrawer();
			    }

			    drawer.Init(property);

			    container.Add(drawer);

			    return true;
		    }

		    return false;
	    }

	    private static bool FindComments(string line)
	    {
		    // This will return whether a line is commented
		    // If the comment appears at the latter part, this function returns false
		    return line.Trim().IndexOf("//") == 0;
	    }

	    private static bool FindAttribWithParm(string line, out string attrib, out string parms)
	    {
		    attrib = "";
		    parms = "";
		    
		    // Find Attributes Inside []
		    int startIndex = 0;
		    
		    if ((startIndex = line.IndexOf("[")) != -1)
		    {
			    int endIndex = line.IndexOf("]", startIndex);
				
			    if (endIndex != -1)
			    {
				    string attribs = line.Substring(startIndex + 1, endIndex - startIndex - 1);
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

		    return attrib != "";
	    }

	    private static bool FindProperties(string line, out string property)
	    {
		    property = "";

		    string attrib = "";
		    string parms = "";

		    int startIndex = 0;
		    if (FindAttribWithParm(line, out attrib, out parms))
		    {
			    if ((startIndex = line.IndexOf("]", startIndex)) != -1)
			    {
				    int endIndex = line.IndexOf("(", startIndex);

				    if (endIndex != -1)
				    {
					    property = line.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

				    }
			    }
		    }
		    else
		    {
			    if ((startIndex = line.IndexOf("_", startIndex)) != -1)
			    {
				    int endIndex = line.IndexOf("(", startIndex);

				    if (endIndex != -1)
				    {
					    property = line.Substring(startIndex, endIndex - startIndex).Trim();
				    }
			    }
		    }

		    return !property.Equals("");
	    }

	    private static StreamReader ReadShader(Material material)
	    {
		    string shaderPath = AssetDatabase.GetAssetPath(material.shader);
		    string[] relativePath = shaderPath.Split(new[] { "Assets" }, StringSplitOptions.None);
		    var fullPath = $"{Application.dataPath}" + relativePath[1];
		    return new StreamReader(fullPath);
	    }

	    private static IUniversalDrawer CreateDrawers(string attrib, string parms)
	    {
		    IUniversalDrawer drawer = null;
		    
		    int parmsLength = parms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
		    attrib = attrib.ToLower();
		    if (parmsLength > 0)
		    {
			    switch (attrib)
			    {
				    case "text":
					    drawer = new TextDrawer();
					    break;
				    case "folder":
					    drawer = new FolderDrawer();
					    break;
				    case "featurefolder":
					    drawer = new FeatureFolderDrawer();
					    break;
				    case "conditionfolder":
					    drawer = new ConditionFolderDrawer();
					    break;
				    case "conditionblock":
					    drawer = new ConditionBlockDrawer();
					    break;
			    }

			    if (drawer != null)
			    {
				    drawer.Init(parms);
			    }
		    }
		    else
		    {
			    switch (attrib)
			    {
				    case "hideininspector":
					    drawer = new HideInInspectorDrawer();
					    break;
			    }
		    }

		    return drawer;
	    }
	}
}

public class JuanShaderGUI : JuanShaderEditor.JuanCustomShaderGUI
{
	
}

