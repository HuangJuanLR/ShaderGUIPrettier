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
using UnityEngine;

namespace SGP
{
	public class PropertiesDrawer : IUniversalDrawer
	{
		private IUniversalDrawer container;

		protected int level;

		public int Level => level;

		public bool Containable => false;
		
		public IUniversalDrawer Container => container;
		
		protected string name;

		public string Name => name;

		protected string displayName;

		public void Init(string data)
		{
			name = data;
		}

		public void SetContainer(IUniversalDrawer container)
		{
			this.container = container;
		}

		public virtual void Draw(MaterialEditor materialEditor, Material material, Func<string, MaterialProperty> findProperty)
		{
			if (!material.HasProperty(name)) return;

			var property = findProperty(name);
			
			materialEditor.ShaderProperty(property, property.displayName);
		}
		
		public void Add(IUniversalDrawer drawer)
		{
			
		}
	}
}