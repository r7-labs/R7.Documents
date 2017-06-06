//
// Copyright (c) 2014-2017 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.Documents.Components;
using R7.Documents.Data;
using R7.Documents.Models;
using R7.DotNetNuke.Extensions.ControlExtensions;
using R7.DotNetNuke.Extensions.Utilities;

namespace R7.Documents
{
    public partial class ImportDocuments : PortalModuleBase
    {
        #region Event Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            var mctrl = new ModuleController ();
            var docModules = new List<ModuleInfo> ();

            // get all document modules (R7.Documents and DNN Documents)
            foreach (var module in mctrl.GetTabModules (TabId).Values) {
                var mdef = module.ModuleDefinition.DefinitionName;
                if (module.ModuleID != ModuleId && !module.IsDeleted && (mdef == ModuleDefinitions.R7_DOCUMENTS || mdef == ModuleDefinitions.DNN_DOCUMENTS)) {
                    docModules.Add (module);
                }
            }

            // fill modules combo
            comboModule.AddItem (LocalizeString ("NotSelected.Text"), Null.NullInteger.ToString ());
            foreach (var docModule in docModules) {
                comboModule.AddItem (docModule.ModuleTitle, docModule.ModuleID.ToString ());
            }

            // set Cancel button link
            linkCancel.NavigateUrl = UrlHelper.GetCancelUrl (UrlHelper.IsInPopup (Request));
        }

        protected void buttonImport_Click (object sender, EventArgs e)
        {
            try {
                var mctrl = new ModuleController ();
                var module = mctrl.GetModule (int.Parse (comboModule.SelectedValue), TabId);
                var mdef = module.ModuleDefinition.DefinitionName;

                foreach (ListItem item in listDocuments.Items) {
                    if (item.Selected) {
                        DocumentInfo document = null;

                        if (mdef == ModuleDefinitions.R7_DOCUMENTS) {
                            document = DocumentsDataProvider.Instance.GetDocument (
                                int.Parse (item.Value),
                                module.ModuleID);
                        }
                        else if (mdef == ModuleDefinitions.DNN_DOCUMENTS) {
                            document = DocumentsDataProvider.Instance.GetDNNDocument (
                                int.Parse (item.Value),
                                module.ModuleID);
                        }

                        if (document != null) {
                            var ctrlUrl = new UrlController ();

                            // get original document tracking data
                            var url = ctrlUrl.GetUrlTracking (PortalId, document.Url, document.ModuleId);

                            document.ItemId = Null.NullInteger;
                            document.ModuleId = ModuleId;

                            // add new document
                            DocumentsDataProvider.Instance.Add (document);

                            // add new url tracking data
                            ctrlUrl.UpdateUrl (PortalId, document.Url, url.UrlType, 
                                url.LogActivity, url.TrackClicks, ModuleId, url.NewWindow);

                            // WTF: using url.Clicks, url.LastClick, url.CreatedDate not working
                        }
                    }
                }

                ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
                Response.Redirect (Globals.NavigateURL (), true);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        protected void comboModule_SelectedIndexChanged (object sender, EventArgs e)
        {
            try {
                var mctrl = new ModuleController ();
                var module = mctrl.GetModule (int.Parse (((ListControl) sender).SelectedValue), TabId);

                if (module != null) {
                    IEnumerable<DocumentInfo> documents = null;

                    var mdef = module.ModuleDefinition.DefinitionName;

                    if (mdef == ModuleDefinitions.R7_DOCUMENTS) {
                        documents = DocumentsDataProvider.Instance.GetDocuments (module.ModuleID, module.PortalID);
                    }
                    else if (mdef == ModuleDefinitions.DNN_DOCUMENTS) {
                        documents = DocumentsDataProvider.Instance.GetDNNDocuments (module.ModuleID, module.PortalID);
                    }
				
                    if (documents != null && documents.Any ()) {
                        panelDocuments.Visible = true;
                        buttonImport.Visible = true;

                        listDocuments.DataSource = documents;
                        listDocuments.DataBind ();

                        foreach (ListItem item in listDocuments.Items) {
                            item.Selected = true;
                        }
                    }
                    else {
                        panelDocuments.Visible = false;
                        buttonImport.Visible = false;

                        listDocuments.Items.Clear ();
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion
    }
}
