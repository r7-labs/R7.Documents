using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using R7.Dnn.Extensions.FileSystem;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Urls;
using R7.Documents.Data;
using R7.Documents.Models;

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

            lnkCancel.NavigateUrl = UrlHelper.GetCancelUrl (UrlUtils.InPopUp ());
        }

        protected void btnApply_Click (object sender, EventArgs e)
        {
            try {
                var folder = ddlFolder.SelectedFolder;

                if (folder != null) {
                    var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
                    var files = FolderManager.Instance.GetFiles (folder);
                    foreach (var document in documents) {
                        // only for files
                        if (Globals.GetURLType (document.Url) == TabType.File) {
                            var docFileId = UrlHelper.GetResourceId (document.Url);
                            var docFile = FileManager.Instance.GetFile (docFileId.Value);

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
                    if (chkUpdateDefaultFolder.Checked) {
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
                if (chkPublishUpdated.Checked) {
                    document.Publish ();
                }

                // safe remove old resources, if needed - need to do this before update!
                if (chkDeleteOldFiles.Checked) {
                    if (oldDocument.Url != document.Url && !string.IsNullOrEmpty (oldDocument.Url)) {
                        DocumentsDataProvider.Instance.DeleteDocumentFile (
                            oldDocument,
                            PortalId);
                    }
                }

                FolderHistory.RememberFolderByFileUrl (Request, Response, document.Url, PortalId);

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
                if (chkUnpublishSkipped.Checked) {
                    // unpublish not updated documents & update them
                    document.UnPublish ();
                    DocumentsDataProvider.Instance.Update (document);
                }
            }
        }
    }
}
