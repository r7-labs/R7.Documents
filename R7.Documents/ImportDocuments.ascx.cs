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
using System.Linq;
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
using System.Collections.Generic;

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

			var mctrl = new ModuleController ();
			var docModules = new List<ModuleInfo>();

			// get all document modules (R7.Documents and DNN Documents)
			foreach (var module in mctrl.GetTabModules (TabId).Values)
			{
				var mdef = module.ModuleDefinition.DefinitionName.ToLowerInvariant();
				if (module.ModuleID != ModuleId && !module.IsDeleted && (mdef == "r7.documents" || mdef == "documents"))
					docModules.Add (module);
			}

			// fill modules combo
			comboModule.AddItem (LocalizeString ("NotSelected.Text"), Null.NullInteger.ToString ());
			foreach (var docModule in docModules)
				comboModule.AddItem (docModule.ModuleTitle, docModule.ModuleID.ToString());

			comboModule.SelectedIndexChanged += comboModules_SelectedIndexChanged;
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

		private void comboModules_SelectedIndexChanged (object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
		{
			try
			{
				var mctrl = new ModuleController ();
				var module = mctrl.GetModule (int.Parse (e.Value), TabId);

				if (module != null)
				{
					IEnumerable<DocumentInfo> documents = null;

					var mdef = module.ModuleDefinition.DefinitionName.ToLowerInvariant ();

					if (mdef == "r7.documents")
						documents = DocumentsController.GetDocuments (module.ModuleID, module.PortalID);
					else if (mdef == "documents")
						documents = DocumentsController.GetDNNDocuments (module.ModuleID, module.PortalID);
				
					if (documents != null && documents.Any ())
					{
						panelDocuments.Visible = true;
						listDocuments.DataSource = documents;
						listDocuments.DataBind ();

						foreach (ListItem item in listDocuments.Items)
							item.Selected = true;
					}
					else
					{
						panelDocuments.Visible = false;
						listDocuments.Items.Clear ();
					}
				}
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
