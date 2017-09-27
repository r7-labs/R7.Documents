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
using R7.Dnn.Extensions.ControlExtensions;
using R7.Dnn.Extensions.Utilities;

namespace R7.Documents
{
    public partial class ImportDocuments : PortalModuleBase
    {
        #region Event Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // get all document modules (R7.Documents and DNN Documents)
            var docModules = new List<ModuleInfo> ();
            foreach (var module in ModuleController.Instance.GetTabModules (TabId).Values) {
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
                var fromModule = ModuleController.Instance.GetModule (int.Parse (comboModule.SelectedValue), TabId, false);
                foreach (ListItem item in listDocuments.Items) {
                    if (item.Selected) {
                        var document = GetDocument (int.Parse (item.Value), fromModule);
                        if (document != null) {
                            // get original document tracking data
                            var ctrlUrl = new UrlController ();
                            var urlTracking = ctrlUrl.GetUrlTracking (PortalId, document.Url, document.ModuleId);

                            // import document
                            document.ItemId = Null.NullInteger;
                            document.ModuleId = ModuleId;
                            DocumentsDataProvider.Instance.Add (document);

                            // add new url tracking data
                            if (urlTracking != null) {
                                // WTF: using url.Clicks, url.LastClick, url.CreatedDate overload not working?
                                ctrlUrl.UpdateUrl (PortalId, document.Url, urlTracking.UrlType,
                                                   urlTracking.LogActivity, urlTracking.TrackClicks,
                                                   ModuleId, urlTracking.NewWindow);
                            }
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

        DocumentInfo GetDocument (int documentId, ModuleInfo module)
        {
            if (module.ModuleDefinition.DefinitionName == ModuleDefinitions.DNN_DOCUMENTS) {
                return DocumentsDataProvider.Instance.GetDNNDocument (documentId, module.ModuleID);
            }

            return DocumentsDataProvider.Instance.GetDocument (documentId, module.ModuleID);
        }

        IEnumerable<DocumentInfo> GetDocuments (ModuleInfo module)
        {
            if (module.ModuleDefinition.DefinitionName == ModuleDefinitions.DNN_DOCUMENTS) {
                return DocumentsDataProvider.Instance.GetDNNDocuments (module.ModuleID, module.PortalID);
            }

            return DocumentsDataProvider.Instance.GetDocuments (module.ModuleID, module.PortalID);
        }

        protected void comboModule_SelectedIndexChanged (object sender, EventArgs e)
        {
            try {
                var module = ModuleController.Instance.GetModule (int.Parse (((ListControl) sender).SelectedValue), TabId, false);
                if (module != null) {
                    var  documents = GetDocuments (module);
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
