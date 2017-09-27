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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Utilities;
using R7.Documents.Components;
using R7.Documents.Data;
using R7.Documents.Models;

namespace R7.Documents
{
    public partial class ChangeFolder : PortalModuleBase<DocumentsSettings>
    {
        UrlHistory _urlHistory;
        protected UrlHistory UrlHistory {
            get { return _urlHistory ?? (_urlHistory = new UrlHistory (Session)); }
        } 

        #region Event Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // set folder to module's default folder
            if (Settings.DefaultFolder != null)
                ddlFolder.SelectedFolder = FolderManager.Instance.GetFolder (Settings.DefaultFolder.Value);

            linkCancel.NavigateUrl = UrlHelper.GetCancelUrl (UrlUtils.InPopUp ());
        }

        protected void buttonApply_Click (object sender, EventArgs e)
        {
            try {
                var folder = ddlFolder.SelectedFolder;
				
                if (folder != null) {
                    var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
                    var files = FolderManager.Instance.GetFiles (folder);
                    foreach (var document in documents) {
                        // only for files
                        if (Globals.GetURLType (document.Url) == TabType.File) {
                            var docFileId = Utils.GetResourceId (document.Url);
                            var docFile = FileManager.Instance.GetFile (docFileId);
							
                            if (docFile != null) {
                                var updated = false; 
                                var oldDocument = document.Clone ();

                                var matchedFile = FindMatchedFile (docFile, files);
                                if (matchedFile != null) {
                                    document.Url = "FileID=" + matchedFile.FileId;
                                    document.CreatedDate = DateTime.Now;
                                    document.ModifiedDate = document.CreatedDate;
                                    document.CreatedByUserId = UserId;
                                    document.ModifiedByUserId = UserId;
                                    updated = true;
                                }

                                PostUpdateDocument (document, oldDocument, updated);
                            }
                        }
                    }

                    // update module's default folder setting
                    if (checkUpdateDefaultFolder.Checked) {
                        Settings.DefaultFolder = ddlFolder.SelectedFolder.FolderID;
                        SettingsRepository.SaveSettings (ModuleConfiguration, Settings);
                    }

                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
                }

                Response.Redirect (Globals.NavigateURL (), true);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion

        IFileInfo FindMatchedFile (IFileInfo docFile, IEnumerable<IFileInfo> files)
        {
            return files.FirstOrDefault (f => 0 == string.Compare (f.FileName, docFile.FileName, StringComparison.InvariantCultureIgnoreCase));
        }

        void PostUpdateDocument (DocumentInfo document, DocumentInfo oldDocument, bool updated)
        {
            if (updated) {
                if (checkPublishUpdated.Checked) {
                    document.Publish ();
                }

                // safe remove old resources, if needed - need to do this before update!
                if (checkDeleteOldFiles.Checked) {
                    if (oldDocument.Url != document.Url && !string.IsNullOrEmpty (oldDocument.Url)) {
                        DocumentsDataProvider.Instance.DeleteDocumentResource (
                            oldDocument,
                            PortalId);
                    }
                }

                // update URL history
                UrlHistory.StoreUrl (document.Url);

                // update document & URL tracking data
                DocumentsDataProvider.Instance.Update (document);
                DocumentsDataProvider.Instance.UpdateDocumentUrl (
                    document,
                    oldDocument.Url,
                    PortalId,
                    ModuleId
                );
            }
            else {
                if (checkUnpublishSkipped.Checked) {
                    // unpublish not updated documents & update them
                    document.UnPublish ();
                    DocumentsDataProvider.Instance.Update (document);
                }
            } 
        }
    }
}
