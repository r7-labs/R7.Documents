//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;

namespace R7.Documents
{
	public partial class ImportDocuments : DocumentsPortalModuleBase
	{
		#region Fields

		#endregion

		#region Event Handlers

		protected override void OnInit (EventArgs e)
		{
			base.OnInit (e);

			cmdUpdate.Click += cmdUpdate_Click;
			linkCancel.NavigateUrl = Globals.NavigateURL ();
		}

		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			try
			{
				if (!IsPostBack)
				{
				}
				else
				{
				}
			}
			catch (Exception ex)
			{
				// module failed to load
				Exceptions.ProcessModuleLoadException (this, ex);
			}
		}

		private void cmdUpdate_Click (object sender, EventArgs e)
		{
			try
			{
				Utils.SynchronizeModule (this);
				DataCache.RemoveCache (this.DataCacheKey + ";anon-doclist");

				// redirect back to the portal home page
				Response.Redirect (Globals.NavigateURL (), true);
			}
			catch (Exception ex)
			{
				// module failed to load
				Exceptions.ProcessModuleLoadException (this, ex);
			}
		}

		#endregion
	}
}
