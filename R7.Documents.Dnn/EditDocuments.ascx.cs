//
// Copyright (c) 2002-2011 by DotNetNuke Corporation
// Copyright (c) 2014-2019 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.UI.WebControls;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.Users;
using R7.Documents.Data;
using R7.Documents.Models;
using R7.Dnn.Extensions.Urls;
using R7.Dnn.Extensions.Text;
using R7.Dnn.Extensions.FileSystem;
using R7.University.Components;

namespace R7.Documents
{
    /// <summary>
    /// Provides the UI for editing document
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
    ///   [ag]  11 March 2007 Migrated to VS2005
    /// </history>
    public partial class EditDocuments: PortalModuleBase<DocumentsSettings>
    {
        protected int? ItemId {
            get { return (int?) ViewState ["ItemId"] ?? null; }
            set { ViewState ["ItemId"] = value; }
        }

        protected bool IsFirstLoad {
            get { return (bool?) ViewState ["IsFirstLoad"] ?? true; }
            set { ViewState ["IsFirstLoad"] = value; }
        }

        protected enum EditDocumentTab
        {
            Common,
            Advanced,
            Audit
        }

        protected EditDocumentTab SelectedTab {
        	get {
        		// get postback initiator control
        		var eventTarget = Request.Form ["__EVENTTARGET"];

        		if (!string.IsNullOrEmpty (eventTarget)) {
        			// check if postback initiator is on Owner tab
        			if (eventTarget.Contains ("$" + btnChangeOwner.ID)) {
                        ViewState ["SelectedTab"] = EditDocumentTab.Advanced;
        				return EditDocumentTab.Advanced;
        			}
        		}

        		// otherwise, get current tab from viewstate
        		var obj = ViewState ["SelectedTab"];
        		if (obj != null) {
        			return (EditDocumentTab) obj;
        		}

        		return EditDocumentTab.Common;
        	}
        	set { ViewState ["SelectedTab"] = value; }
        }

        #region Event Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // set URLs for cancel links
            lnkCancel.NavigateUrl = UrlHelper.GetCancelUrl (UrlUtils.InPopUp ());
            lnkClose.NavigateUrl = Globals.NavigateURL ();

            btnDelete.Attributes.Add ("onClick",
                "javascript:return confirm('" + LocalizeString ("Delete.Text")  + "');");

            btnDeleteWithAsset.Attributes.Add ("onClick",
                "javascript:return confirm('" + LocalizeString ("DeleteWithAsset.Text") + "');");

            // Configure categories entry as a list or textbox, based on user settings
            if (Settings.UseCategoriesList) {
                // Configure category entry as a list
                lstCategory.Visible = true;
                txtCategory.Visible = false;

                // Populate categories list
                var listController = new ListController ();
                lstCategory.DataSource = listController.GetListEntryInfoItems (Settings.CategoriesListName);
                lstCategory.DataTextField = "Text";
                lstCategory.DataValueField = "Value";

                lstCategory.DataBind ();
                lstCategory.Items.Insert (0, new ListItem (Localization.GetString ("None_Specified"),
                    Null.NullInteger.ToString ()));
            }
            else {
                // Configure category entry as a free-text entry
                lstCategory.Visible = false;
                txtCategory.Visible = true;
            }
        }

        /// <summary>
        /// OnLoad runs when the control is loaded
        /// </summary>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            try {
                if (ItemId == null) {
                    // determine ItemId of document to update
                    ItemId = ParseHelper.ParseToNullable<int> (Request.QueryString ["ItemId"]);
                }

                if (!IsPostBack) {
                    LoadDocument ();
                    IsFirstLoad = false;
                }
            }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        void LoadDocument ()
        {
            if (ItemId != null) {
                LoadExistingDocument (ItemId.Value);
            }
            else {
                LoadNewDocument ();
            }
        }

        /// <summary>
        /// btnDelete_Click runs when the delete button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void btnDelete_Click (object sender, EventArgs e)
        {
            try {
                // TODO: Duplicate calls
                var document = DocumentsDataProvider.Instance.GetDocument (ItemId.Value, ModuleId);
                if (document != null) {
                    DocumentsDataProvider.Instance.DeleteDocument (ItemId.Value, sender == btnDeleteWithAsset, PortalId, ModuleId);
                    this.Message (string.Format (LocalizeString ("DocumentDeleted.Format"), document.Title), MessageType.Warning);

                    mvEditDocument.ActiveViewIndex = 1;
                    btnEdit.Visible = false;
                    ItemId = null;

                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
                }
		    }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        protected void btnAddMore_Click (object sender, EventArgs e)
        {
            mvEditDocument.ActiveViewIndex = 0;

            // document was added before, so we need to reload page
            lnkCancel.NavigateUrl = Globals.NavigateURL ();

            ItemId = null;
            LoadDocument ();
        }

        protected void btnEdit_Click (object sender, EventArgs e)
        {
            mvEditDocument.ActiveViewIndex = 0;

            // document was added before, so we need to reload page
            lnkCancel.NavigateUrl = Globals.NavigateURL ();

            LoadDocument ();
        }

        protected void lnkChangeOwner_Click (object sender, EventArgs e)
        {
            txtOwner.Visible = false;
            btnChangeOwner.Visible = false;
            lstOwner.Visible = true;

            PopulateOwnerList ();

            try {
                // get existing document record
                var document = DocumentsDataProvider.Instance.GetDocument ((ItemId != null) ? ItemId.Value : -1, ModuleId);

                try {
                    if (document == null) {
                        lstOwner.SelectedValue = UserId.ToString ();
                    }
                    else {
                        lstOwner.SelectedValue = document.OwnedByUserId.ToString ();
                    }
                }
                catch (Exception ex) {
                    // defensive code only, would only happen if the owner user has been deleted
                    Exceptions.LogException (ex);
                }
            }
            catch (Exception ex) {
                // would happen if the user no longer exists
                Exceptions.LogException (ex);
            }
        }

        #endregion

        void LoadNewDocument ()
        {
            try {
                lstOwner.SelectedValue = UserId.ToString ();
                txtOwner.Text = UserInfo.DisplayName;
            } catch (Exception ex) {
                // defensive code only, would only happen if the owner user has been deleted
                Exceptions.LogException (ex);
            }

            btnAdd.Visible = true;
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            btnDeleteWithAsset.Visible = false;

            txtSortIndex.Text = ((CalculateSortIndex () ?? 0) + 10).ToString ();

            if (IsFirstLoad) {
                SelectFolder (ctlUrl, Settings.DefaultFolder ?? FolderHistory.GetLastFolderId (Request, PortalId));
            }

            ctlUrl.NewWindow = DocumentsConfig.Instance.NewWindow;
        }

        void SelectFolder (DnnUrlControl ctlUrl, int? folderId)
        {
            if (folderId == null) {
                return;
            }

            var folderIsSelected = ctlUrl.SelectFolder (folderId.Value, true);
            if (folderIsSelected) {
                return;
            }

            var folder = FolderManager.Instance.GetFolder (folderId.Value);
            if (folder != null) {
                this.Message (string.Format (LocalizeString ("CannotSelectFolder.Text"), folder.FolderName), MessageType.Warning, false);
            }
        }

        int? CalculateSortIndex ()
        {
            // HACK: Calculate sortindex for new documents
            var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
            if (documents != null && documents.Any ()) {
                return documents.Max (d => d.SortOrderIndex);
            }

            return null;
        }

        void LoadExistingDocument (int documentId)
        {
            var document = DocumentsDataProvider.Instance.GetDocument (documentId, ModuleId);
            if (document != null) {
                txtTitle.Text = document.Title;
                txtDescription.Text = document.Description;
                chkForceDownload.Checked = document.ForceDownload;
                dtStartDate.SelectedDate = document.StartDate;
                dtEndDate.SelectedDate = document.EndDate;
                txtLinkAttributes.Text = document.LinkAttributes;
                dtCreatedDate.SelectedDate = document.CreatedDate;
                dtLastModifiedDate.SelectedDate = document.ModifiedDate;
                txtSortIndex.Text = document.SortOrderIndex.ToString ();

                if (!string.IsNullOrEmpty (document.Url)) {
                    ctlUrl.Url = document.Url;
                }

                // test to see if the document has been removed/deleted
                if (CheckFileExists (document.Url)) {
                    CheckFileSecurity (document.Url);
                }

                txtOwner.Text = UserHelper.GetUserDisplayName (PortalId, document.OwnedByUserId) ?? LocalizeString ("None_Specified");

                if (txtCategory.Visible) {
                    txtCategory.Text = document.Category;
                } else {
                    // look for the category by name
                    var found = lstCategory.Items.FindByText (document.Category);
                    if (found != null) {
                        lstCategory.SelectedValue = found.Value;
                    } else {
                        // legacy support, do a fall-back
                        found = lstCategory.Items.FindByValue (document.Category);
                        if (found != null) {
                            lstCategory.SelectedValue = found.Value;
                        }
                    }
                }

                // FIXME: Audit data not preserved between postbacks
                ctlAudit.CreatedByUser = UserHelper.GetUserDisplayName (PortalId, document.CreatedByUserId) ?? LocalizeString ("None_Specified");
                ctlAudit.CreatedDate = document.CreatedDate.ToString ();
                ctlAudit.LastModifiedByUser = UserHelper.GetUserDisplayName (PortalId, document.ModifiedByUserId) ?? LocalizeString ("None_Specified");
                ctlAudit.LastModifiedDate = document.ModifiedDate.ToString ();

                ctlUrlTracking.URL = document.Url;
                ctlUrlTracking.ModuleID = ModuleId;
            }
            else {
                AddLog ("Security violation: Attempt to access document not related to the module.", EventLogController.EventLogType.ADMIN_ALERT);
                Response.Redirect (Globals.NavigateURL (), true);
            }

            btnAdd.Visible = false;
            btnUpdate.Visible = true;
            btnDelete.Visible = true;
            btnDeleteWithAsset.Visible = true;
        }

        void AddLog (string message, EventLogController.EventLogType logType)
        {
            var log = new LogInfo ();
            log.AddProperty ("Module", ModuleConfiguration.ModuleDefinition.DefinitionName);
            log.AddProperty ("PortalId", PortalId.ToString ());
            log.AddProperty ("UserId", UserId.ToString ());
            log.AddProperty ("UserEmail", UserInfo.Email);
            log.AddProperty ("RawUrl", Request.RawUrl);
            log.AddProperty ("Message", message);
            log.LogTypeKey = logType.ToString ();
            EventLogController.Instance.AddLog (log);
        }

        /// <summary>
        /// Compare file's folder security with module security settings and display
        /// a warning message if they do not match.
        /// </summary>
        /// <history>
        ///     [ag]    11 March 2007   Created
        /// </history>
        bool CheckFileSecurity (string url)
        {
            // TODO: Add support for other link types

            switch (Globals.GetURLType (url)) {
            case TabType.File:
                url = FixLegacyFileUrl (url);
                var fileId = int.Parse (UrlUtils.GetParameterValue (url));
                var file = FileManager.Instance.GetFile (fileId);
                if (file != null) {
                    return CheckRolesMatch (
                        // TODO review old code: this.ModuleConfiguration.AuthorizedViewRoles,
                        GetModuleViewRoles (),
                        // TODO review old code: FileSystemUtils.GetRoles(objFile.Folder, PortalId, "READ")
                        GetFolderViewRoles (file.FolderId)
                    );
                }
                break;
            }
            return true;
        }

        string GetModuleViewRoles ()
        {
            var moduleViewRoles = new StringBuilder ();
            foreach (ModulePermissionInfo modulePerm in ModuleConfiguration.ModulePermissions) {
                if (modulePerm.PermissionKey == "VIEW" && modulePerm.AllowAccess) {
                    if (moduleViewRoles.Length > 0)
                        moduleViewRoles.Append (';');

                    moduleViewRoles.Append (modulePerm.RoleName);
                }
            }

            return moduleViewRoles.ToString ();
        }

        string GetFolderViewRoles (int folderId)
        {
            var folder = FolderManager.Instance.GetFolder (folderId);
            var folderViewRoles = new StringBuilder ();
            foreach (FolderPermissionInfo folderPerm in folder.FolderPermissions) {
                if (folderPerm.PermissionKey == "READ" && folderPerm.AllowAccess) {
                    if (folderViewRoles.Length > 0) {
                        folderViewRoles.Append (';');
                    }
                    folderViewRoles.Append (folderPerm.RoleName);
                }
            }

            return folderViewRoles.ToString ();
        }

        /// <summary>
        /// Tests whether the roles that the module allows read access to are a subset of
        /// the file's read-access roles.  If not, display a warning.
        /// </summary>
        /// <history>
        ///     [ag]    11 March 2007   Created
        /// </history>
        bool CheckRolesMatch (string moduleRoles, string fileRolesString)
        {
            var fileRoles = new Hashtable ();
            var notMatching = false;
            var strRolesForMessage = "";

            foreach (var fileRoleString in fileRolesString.Split (';')) {
                fileRoles.Add (fileRoleString, fileRoleString);
                if (fileRoleString == Globals.glbRoleAllUsersName) {
                    // if read access to the file is available for "all users", the file can always be accessed
                    return true;
                }
            }

            foreach (var moduleRoleString in moduleRoles.Split(';')) {
                if (!fileRoles.ContainsKey (moduleRoleString)) {
                    // a view role exists for the module that is not available for the file
                    notMatching = true;
                    if (strRolesForMessage != string.Empty) {
                        strRolesForMessage = strRolesForMessage + ", ";
                    }

                    strRolesForMessage = strRolesForMessage + moduleRoleString;
                }
            }

            return notMatching;
        }

        /// <summary>
        /// Handle legacy scenarios before the introduction of the FileServerHandler
        /// </summary>
        /// <returns>The legacy file URL.</returns>
        /// <param name="url">Fixed file URL.</param>
        string FixLegacyFileUrl (string url)
        {
            if (!url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase)) {
                return "FileID=" + FileManager.Instance.GetFile (PortalId, url).FileId;
            }
            return url;
        }

        /// <summary>
        /// Tests whether the file exists.  If it does not, add a warning message.
        /// </summary>
        /// <history>
        ///     [ag]    11 March 2007   Created
        /// </history>
        bool CheckFileExists (string url)
        {
            // TODO: Add support for other link types

            var fileId = 0;
            var blnAddWarning = false;

            if (url == string.Empty) {
                // file not selected
                return false;
            }

            switch (Globals.GetURLType (url)) {
            case TabType.File:
                url = FixLegacyFileUrl (url);
                fileId = int.Parse (UrlUtils.GetParameterValue (url));
                var objFile = FileManager.Instance.GetFile (fileId);

                blnAddWarning = false;
                if (objFile == null) {
                    blnAddWarning = true;
                }
                else {
                    switch ((FolderController.StorageLocationTypes) objFile.StorageLocation) {
                    case FolderController.StorageLocationTypes.InsecureFileSystem:
                        blnAddWarning = !File.Exists (objFile.PhysicalPath);
                        break;
                    case FolderController.StorageLocationTypes.SecureFileSystem:
                        blnAddWarning = !File.Exists (objFile.PhysicalPath + Globals.glbProtectedExtension);
                        break;
                    case FolderController.StorageLocationTypes.DatabaseSecure:
                        blnAddWarning = false;
                        // Database-stored files cannot be deleted seperately
                        break;
                    }
                }

                if (blnAddWarning) {
                    // file was deleted
                    return false;
                }
                break;
            }

            return true;
        }

        protected void btnAdd_Click (object sender, EventArgs e)
        {
            var document = new DocumentInfo {
                ItemId = 0,
                ModuleId = ModuleId,
                CreatedByUserId = UserId,
                OwnedByUserId = UserId
            };

            Update (document, true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// btnUpdate_Click runs when the update button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    9/22/2004   Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void btnUpdate_Click (object sender, EventArgs e)
        {
            var document = DocumentsDataProvider.Instance.GetDocument (ItemId.Value, ModuleId);
            Update (document, false);
        }

        void Update (DocumentInfo document, bool isNew)
        {
            try {
                if (Page.IsValid) {
                    var oldDocument = document.Clone ();

                    document.Title = txtTitle.Text;
                    document.Description = txtDescription.Text;
                    document.ForceDownload = chkForceDownload.Checked;
                    document.Url = ctlUrl.Url;
                    document.LinkAttributes = txtLinkAttributes.Text;
                    document.ModifiedByUserId = UserInfo.UserID;

                    UpdateDateTime (document, oldDocument);
                    UpdateOwner (document);
                    UpdateCategory (document);

                    int sortIndex;
                    document.SortOrderIndex = int.TryParse (txtSortIndex.Text, out sortIndex) ? sortIndex : 10;

                    if (isNew) {
                        DocumentsDataProvider.Instance.Add (document);
                    } else {
                        DocumentsDataProvider.Instance.Update (document);
                        if (document.Url != oldDocument.Url) {
                            // delete old URL tracking data
                            DocumentsDataProvider.Instance.DeleteDocumentUrl (oldDocument.Url, PortalId, ModuleId);
                        }
                    }

                    // add or update URL tracking
                    var ctrlUrl = new UrlController ();
                    ctrlUrl.UpdateUrl (PortalId, ctlUrl.Url, ctlUrl.UrlType, ctlUrl.Log, ctlUrl.Track, ModuleId, ctlUrl.NewWindow);

                    this.Message (string.Format (LocalizeString (isNew ? "DocumentAdded.Format" : "DocumentUpdated.Format"), document.Title), MessageType.Success);

                    if (!CheckFileExists (ctlUrl.Url) || !CheckFileSecurity (ctlUrl.Url)) {
                        // display warning
                        this.Message ("DocumentLinkWarningHeading.Text", "DocumentLinkWarning.Text", MessageType.Warning, true);
                    }

                    FolderHistory.RememberFolderByFileUrl (Request, Response, document.Url, PortalId);
                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);

                    // WTF: should do this after UpdateUrl, or DnnUrlControl loose its flags
                    mvEditDocument.ActiveViewIndex = 1;
                    btnEdit.Visible = true;
                    ItemId = document.ItemId;
                }
            } catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        void UpdateOwner (DocumentInfo document)
        {
            if (lstOwner.Visible) {
                if (lstOwner.SelectedValue != string.Empty) {
                    document.OwnedByUserId = Convert.ToInt32 (lstOwner.SelectedValue);
                } else {
                    document.OwnedByUserId = Null.NullInteger;
                }
            }
        }

        void UpdateCategory (DocumentInfo document)
        {
            if (txtCategory.Visible) {
                document.Category = txtCategory.Text;
            } else {
                if (lstCategory.SelectedItem.Text == Localization.GetString ("None_Specified")) {
                    document.Category = "";
                } else {
                    document.Category = lstCategory.SelectedItem.Value;
                }
            }
        }

        void UpdateDateTime (DocumentInfo document, DocumentInfo oldDocument)
        {
            var now = DateTime.Now;

            document.StartDate = dtStartDate.SelectedDate;
            document.EndDate = dtEndDate.SelectedDate;

            if (dtCreatedDate.SelectedDate == null) {
                document.CreatedDate = now;
            } else {
                document.CreatedDate = dtCreatedDate.SelectedDate.Value;
            }

            if (dtLastModifiedDate.SelectedDate == null || oldDocument.Url != ctlUrl.Url) {
                document.ModifiedDate = now;
            } else {
                document.ModifiedDate = dtLastModifiedDate.SelectedDate.Value;
            }
        }

        void PopulateOwnerList ()
        {
            // populate owner list, sort by display name
            lstOwner.DataSource = UserController.GetUsers (PortalId)
                .Cast<UserInfo> ()
                .OrderBy (u => u.DisplayName);

            lstOwner.DataBind ();

            // add default item
            lstOwner.Items.Insert (0, new ListItem (
                "<" + LocalizeString ("None_Specified") + ">", Null.NullInteger.ToString ())
            );

            // add superusers *before* default item and with user names in brackets
            foreach (UserInfo superUsers in UserController.GetUsers (false, true, Null.NullInteger)) {
                lstOwner.Items.Insert (0, new ListItem (
                    superUsers.DisplayName + " (" + superUsers.Username + ")", superUsers.UserID.ToString ())
                );
            }
        }
    }
}
