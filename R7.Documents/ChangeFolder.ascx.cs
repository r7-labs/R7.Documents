//
// Copyright (c) 2014-2016 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using R7.Documents.Data;
using R7.DotNetNuke.Extensions.Modules;

namespace R7.Documents
{
    public partial class ChangeFolder : PortalModuleBase<DocumentsSettings>
    {
        #region Event Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // set folder to module's default folder
            if (Settings.DefaultFolder != null)
                ddlFolder.SelectedFolder = FolderManager.Instance.GetFolder (Settings.DefaultFolder.Value);
			
            linkCancel.NavigateUrl = Globals.NavigateURL ();
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

                                foreach (var file in files) {
                                    // case-insensitive comparison
                                    if (0 == string.Compare (file.FileName, docFile.FileName, StringComparison.InvariantCultureIgnoreCase)) {
                                        document.Url = "FileID=" + file.FileId;
                                        document.CreatedDate = DateTime.Now;
                                        document.ModifiedDate = document.CreatedDate;
                                        document.CreatedByUserId = UserId;
                                        document.ModifiedByUserId = UserId;

                                        updated = true;
                                        break;
                                    }
                                } // foreach 

                                if (updated) {
                                    // publish updated documents
                                    document.IsPublished |= checkPublishUpdated.Checked;

                                    // safe remove old files, if needed.
                                    // need to do this before update!
                                    if (checkDeleteOldFiles.Checked) {
                                        if (oldDocument.Url != document.Url) {
                                            DocumentsDataProvider.Instance.DeleteDocumentResource (
                                                oldDocument,
                                                PortalId);
                                        }
                                    }

                                    // update document & URL tracking data
                                    DocumentsDataProvider.Instance.Update (document);
                                    DocumentsDataProvider.Instance.UpdateDocumentUrl (
                                        document,
                                        oldDocument.Url,
                                        PortalId,
                                        ModuleId);
                                }
                                else {
                                    if (checkUnpublishSkipped.Checked) {
                                        // unpublish not updated documents & update them
                                        document.IsPublished = false;
                                        DocumentsDataProvider.Instance.Update (document);
                                    }
                                } // if (updated)
                            }
                        }
                    } // foreach

                    // update module's default folder setting
                    if (checkUpdateDefaultFolder.Checked) {
                        Settings.DefaultFolder = ddlFolder.SelectedFolder.FolderID;
                    }

                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
                }

                // redirect back to the portal home page
                Response.Redirect (Globals.NavigateURL (), true);

            }
            catch (Exception ex) {
                // module failed to load
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion
    }
}
