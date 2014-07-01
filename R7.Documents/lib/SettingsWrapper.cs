//
// SettingsWrapper.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2014 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.ComponentModel;
using System.Collections;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;

namespace R7.Documents
{
	/// <summary>
	/// Provides strong typed access to settings used by module
	/// </summary>
	public class SettingsWrapper
	{
		protected ModuleController ctrl;
		protected int ModuleId;
		protected int TabModuleId;

		/// <summary>
		/// Initializes a new instance of the <see cref="Documents.SettingsWrapper"/> class.
		/// </summary>
		/// <param name='module'>
		/// Module control.
		/// </param>
		public SettingsWrapper (IModuleControl module)
		{
			ctrl = new ModuleController (); 
			ModuleId = module.ModuleContext.ModuleId;
			TabModuleId = module.ModuleContext.TabModuleId;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Documents.SettingsWrapper"/> class.
		/// </summary>
		/// <param name='module'>
		/// Module info.
		/// </param>
		public SettingsWrapper (ModuleInfo module)
		{
			ctrl = new ModuleController ();
			ModuleId = module.ModuleID;
			TabModuleId = module.TabModuleID;
		}

		/// <summary>
		/// Reads module setting.
		/// </summary>
		/// <returns>
		/// The setting value.
		/// </returns>
		/// <param name='settingName'>
		/// Setting name.
		/// </param>
		/// <param name='defaultValue'>
		/// Default value for setting.
		/// </param>
		/// <param name='tabSpecific'>
		/// If set to <c>true</c>, read tab-specific setting.
		/// </param>
		/// <typeparam name='T'>
		/// Type of the setting
		/// </typeparam>
		protected T ReadSetting<T> (string settingName, T defaultValue, bool tabSpecific)
		{
			var settings = (tabSpecific) ? 
				ctrl.GetTabModuleSettings (TabModuleId) :
				ctrl.GetModuleSettings (ModuleId);

			T ret = default(T);

			if (settings.ContainsKey (settingName))
			{
				var tc = TypeDescriptor.GetConverter (typeof(T));
				try
				{
					ret = (T)tc.ConvertFrom (settings [settingName]);
				}
				catch
				{
					ret = defaultValue;
				}
			}
			else
				ret = defaultValue;

			return ret;
		}

		/// <summary>
		/// Writes module setting.
		/// </summary>
		/// <param name='settingName'>
		/// Setting name.
		/// </param>
		/// <param name='value'>
		/// Setting value.
		/// </param>
		/// <param name='tabSpecific'>
		/// If set to <c>true</c>, setting is for this module on current tab.
		/// If set to <c>false</c>, setting is for this module on all tabs.
		/// </param>
		protected void WriteSetting<T> (string settingName, T value, bool tabSpecific)
		{
			if (tabSpecific)
				ctrl.UpdateTabModuleSetting (TabModuleId, settingName, value.ToString ());
			else
				ctrl.UpdateModuleSetting (ModuleId, settingName, value.ToString ());
		}
	}
	// class
}
// namespace

