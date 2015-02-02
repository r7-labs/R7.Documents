//
// Copyright (c) 2014-2015 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using System.Web;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Modules;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;

namespace R7.Documents
{
	/// <summary>
	/// Message severities
	/// </summary>
	public enum MessageSeverity
	{
		Info,
		Warning,
		Error
	}

	public class Utils
	{
		public static string GetUserDisplayName (int userId)
		{
			var portalId = PortalController.GetCurrentPortalSettings ().PortalId;
			var user = UserController.GetUserById (portalId, userId);

			// TODO: "System" user name needs localization
			return (user != null) ? user.DisplayName : "System";
		}

		/// <summary>
		/// Determines if the specified file is an images.
		/// </summary>
		/// <returns></returns>
		/// <param name="fileName">File name.</param>
		public static bool IsImage (string fileName)
		{
			if (!string.IsNullOrWhiteSpace (fileName))
				return Globals.glbImageFileTypes.Contains (
					Path.GetExtension (fileName).Substring (1).ToLowerInvariant ());
			else
				return false;
		}

		/// <summary>
		/// Formats the URL by DNN rules.
		/// </summary>
		/// <returns>Formatted URL.</returns>
		/// <param name="module">A module reference.</param>
		/// <param name="link">A link value. May be TabID, FileID=something or in other valid forms.</param>
		/// <param name="trackClicks">If set to <c>true</c> then track clicks.</param>
		public static string FormatURL (IModuleControl module, string link, bool trackClicks)
		{
			return DotNetNuke.Common.Globals.LinkClick 
				(link, module.ModuleContext.TabId, module.ModuleContext.ModuleId, trackClicks);
		}

		/// <summary>
		/// Formats the Edit control URL by DNN rules (popups supported).
		/// </summary>
		/// <returns>Formatted Edit control URL.</returns>
		/// <param name="module">A module reference.</param>
		/// <param name="controlKey">Edit control key.</param>
		/// <param name="args">Additional parameters.</param>
		public static string EditUrl (IModuleControl module, string controlKey, params string[] args)
		{
			var argList = new List<string> (args); 
			argList.Add ("mid");
			argList.Add (module.ModuleContext.ModuleId.ToString ());

			return module.ModuleContext.NavigateUrl (module.ModuleContext.TabId, controlKey, false, argList.ToArray ());
		}

		/// <summary>
		/// Finds the item index by it's value in ListControl-type list.
		/// </summary>
		/// <returns>Item index.</returns>
		/// <param name="list">List control.</param>
		/// <param name="value">A value.</param>
		/// <param name="defaultIndex">Default index (in case item not found).</param>
		public static int FindIndexByValue (ListControl list, object value, int defaultIndex = 0)
		{ 
			if (value != null)
			{
				var index = 0;
				var strvalue = value.ToString ();
				foreach (ListItem item in list.Items)
				{
					if (item.Value == strvalue)
						return index;
					index++;
				}
			}

			return defaultIndex; 
		}

		/// <summary>
		/// Sets the selected index of ListControl-type list.
		/// </summary>
		/// <param name="list">List control.</param>
		/// <param name="value">A value.</param>
		/// <param name="defaultIndex">Default index (in case item not found).</param>
		public static void SelectByValue (ListControl list, object value, int defaultIndex = 0)
		{
			list.SelectedIndex = FindIndexByValue (list, value, defaultIndex);
		}

		/// <summary>
		/// Display a message at the top of the specified module.
		/// </summary>
		/// <param name="module">Module reference.</param>
		/// <param name="severity">Message severity level.</param>
		/// <param name="message">Message text.</param>
		public static void Message (IModuleControl module, MessageSeverity severity, string message, bool localize = false)
		{
			var label = new Label ();
			label.CssClass = "dnnFormMessage dnnForm" + severity;
			label.Text = localize ? Localization.GetString (message, module.LocalResourceFile) : message;

			module.Control.Controls.AddAt (0, label);
		}


		public static bool IsNull<T> (Nullable<T> n) where T: struct
		{
			// NOTE: n.HasValue is equvalent to n != null
			if (n.HasValue && !Null.IsNull (n.Value))
				return false;

			return true;
		}

		public static Nullable<T> ToNullable<T> (T n) where T: struct
		{
			return Null.IsNull (n) ? null : (Nullable<T>)n;
		}

		/// <summary>
		/// Parses specified string value to a nullable int, 
		/// also with convertion of Null.NullInteger to null 
		/// </summary>
		/// <returns>The nullable int.</returns>
		/// <param name="value">String value to parse.</param>
		public static int? ParseToNullableInt (string value)
		{
			// TODO: Make another variant of ParseToNullableInt() without using DNN Null object

			int n;

			if (int.TryParse (value, out n))
				return Null.IsNull (n) ? null : (int?)n;
			else
				return null;
		}

		/*
		public static Nullable<T> ParseToNullable<T>(string value) where T: struct
		{
			T n;

			if (Convert.ChangeType(value, typeof(T))
				return Null.IsNull (n)? null : (Nullable<T>) n;
			else
				return null;
		}*/

		/// <summary>
		/// Formats the list of arguments, excluding empty
		/// </summary>
		/// <returns>Formatted list.</returns>
		/// <param name="separator">Separator.</param>
		/// <param name="args">Arguments.</param>
		public static string FormatList (string separator, params object[] args)
		{
			var sb = new StringBuilder (args.Length);

			var i = 0;
			foreach (var a in args)
			{
				if (!string.IsNullOrWhiteSpace (a.ToString ()))
				{
					if (i++ > 0)
						sb.Append (separator);

					sb.Append (a);
				}
			}

			return sb.ToString ();
		}

		public static string FirstCharToUpper (string s)
		{
			if (!string.IsNullOrWhiteSpace (s))
			if (s.Length == 1)
				return s.ToUpper ();
			else
				return s.ToUpper () [0].ToString () + s.Substring (1);
			else
				return s;
		}

		public static string FirstCharToUpperInvariant (string s)
		{
			if (!string.IsNullOrWhiteSpace (s))
			if (s.Length == 1)
				return s.ToUpperInvariant ();
			else
				return s.ToUpperInvariant () [0].ToString () + s.Substring (1);
			else
				return s;
		}

		public static void SynchronizeModule (IModuleControl module)
		{
			ModuleController.SynchronizeModule (module.ModuleContext.ModuleId);

			// NOTE: update module cache (temporary fix before 7.2.0)?
			// more info: https://github.com/dnnsoftware/Dnn.Platform/pull/21
			var moduleController = new ModuleController ();
			moduleController.ClearCache (module.ModuleContext.TabId);

		}
	}
	// class
}
// namespace

