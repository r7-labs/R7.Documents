//
// Copyright (c) 2002-2011 by DotNetNuke Corporation
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
using R7.Dnn.Extensions.ModuleExtensions;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.UrlHistory;
using R7.Dnn.Extensions.Utilities;
using R7.Documents.Data;
using R7.Documents.Models;
using R7.Documents.Helpers;

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
    public partial class EditDocuments : PortalModuleBase<DocumentsSettings>
    {
        int itemId;

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
        			if (eventTarget.Contains ("$" + lnkChange.ID)) {
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
            linkCancel.NavigateUrl = UrlHelper.GetCancelUrl (UrlUtils.InPopUp ());
            linkCancelAdd.NavigateUrl = Globals.NavigateURL ();

            cmdDelete.Attributes.Add ("onClick", 
                "javascript:return confirm('" + LocalizeString ("Delete.Text")  + "');");
            
            buttonDeleteWithResource.Attributes.Add ("onClick", 
                "javascript:return confirm('" + LocalizeString ("DeleteWithResource.Text") + "');");

            cmdUpdateOverride.Text = LocalizeString ("Proceed.Text");
            cmdUpdateOverride.ToolTip = LocalizeString ("Proceed.ToolTip");
            linkAddMore.Text = LocalizeString ("AddMoreDocuments.Text");
            linkAddMore.ToolTip = LocalizeString ("AddMoreDocuments.Text");

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

            BindUrlHistory ();
        }

        /// <summary>
        /// OnLoad runs when the control is loaded
        /// </summary>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            try {
                // determine ItemId of Document to Update
                itemId = TypeUtils.ParseToNullable<int> (Request.QueryString ["ItemId"]) ?? Null.NullInteger;

                if (!IsPostBack) {
                    if (!Null.IsNull (itemId)) {
                        LoadExistingDocument (itemId);
                    }
                    else {
                        LoadNewDocument ();
                    }
                }
            }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the delete button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click (object sender, EventArgs e)
        {
            try {
                if (!Null.IsNull (itemId)) {
                    var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);
                    if (document != null) {
                        DocumentsDataProvider.Instance.Delete (document);
                        DocumentsDataProvider.Instance.DeleteDocumentUrl (document.Url, PortalId, ModuleId);

                        if (sender == buttonDeleteWithResource) {
                            DocumentsDataProvider.Instance.DeleteDocumentResource (document, PortalId);
                        }
                    }
                }

                ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
			    Response.Redirect (Globals.NavigateURL (), true);
		    }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// cmdUpdate_Click runs when the update button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void cmdUpdate_Click (object sender, EventArgs e)
        {
            Update (false);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// cmdUpdate_Click runs when the update "override" button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[ag]	11 March 2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void cmdUpdateOverride_Click (object sender, EventArgs e)
        {
            Update (true);
        }

        protected void linkAddMore_Click (object sender, EventArgs e)
        {
            multiView.ActiveViewIndex = 0;
            CalculateSortIndex ();

            // document was added before, so we need to reload page
            linkCancel.NavigateUrl = Globals.NavigateURL ();
        }

        protected void lnkChange_Click (object sender, EventArgs e)
        {
            lblOwner.Visible = false;
            lnkChange.Visible = false;
            lstOwner.Visible = true;

            PopulateOwnerList ();

            try {
                // get existing document record
                var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);

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

        protected void linkSelectUrl_Click (object sender, EventArgs e)
        {
            ctlUrl.Url = comboUrlHistory.SelectedValue;
        }

        #endregion

        void LoadNewDocument ()
        {
            try {
                lstOwner.SelectedValue = UserId.ToString ();
                lblOwner.Text = UserInfo.DisplayName;
            } catch (Exception ex) {
                // defensive code only, would only happen if the owner user has been deleted
                Exceptions.LogException (ex);
            }

            cmdDelete.Visible = false;
            buttonDeleteWithResource.Visible = false;

            ctlUrl.NewWindow = true;

            CalculateSortIndex ();

            if (Settings.DefaultFolder != null) {
                var folderSelected = SelectFolder (ctlUrl, Settings.DefaultFolder.Value);
                if (!folderSelected) {
                    this.Message ("CurrentFolder.Warning", MessageType.Warning, true);
                }
            }

            cmdUpdate.Text = LocalizeString ("AddDocument.Text");
            cmdUpdate.ToolTip = LocalizeString ("AddDocument.ToolTip");
        }

        void CalculateSortIndex ()
        {
            // HACK: Calculate sortindex for new documents
            var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
            if (documents != null && documents.Any ()) {
                var maxSortIndex = documents.Max (d => d.SortOrderIndex);

                // TODO: Move to portal settings
                txtSortIndex.Text = (maxSortIndex + 10).ToString ();
            }
        }

        void LoadExistingDocument (int documentId)
        {
            var document = DocumentsDataProvider.Instance.GetDocument (documentId, ModuleId);
            if (document != null) {
                txtName.Text = document.Title;
                txtDescription.Text = document.Description;
                chkForceDownload.Checked = document.ForceDownload;
                datetimeStartDate.SelectedDate = document.StartDate;
                datetimeEndDate.SelectedDate = document.EndDate;
                textLinkAttributes.Text = document.LinkAttributes;
                pickerCreatedDate.SelectedDate = document.CreatedDate;
                pickerLastModifiedDate.SelectedDate = document.ModifiedDate;
                txtSortIndex.Text = document.SortOrderIndex.ToString ();

                if (!string.IsNullOrEmpty (document.Url)) {
                    ctlUrl.Url = document.Url;
                }

                // test to see if the document has been removed/deleted
                if (CheckFileExists (document.Url)) {
                    CheckFileSecurity (document.Url);
                }

                var ownerUserName = UserHelper.GetUserDisplayName (PortalId, document.OwnedByUserId);
                if (string.IsNullOrEmpty (ownerUserName)) {
                    lblOwner.Text = Localization.GetString ("None_Specified");
                } else {
                    lblOwner.Text = ownerUserName;
                }

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
                ctlAudit.CreatedByUser = UserHelper.GetUserDisplayName (PortalId, document.CreatedByUserId);
                ctlAudit.CreatedDate = document.CreatedDate.ToString ();
                ctlAudit.LastModifiedByUser = UserHelper.GetUserDisplayName (PortalId, document.ModifiedByUserId);
                ctlAudit.LastModifiedDate = document.ModifiedDate.ToString ();

                ctlUrlTracking.URL = document.Url;
                ctlUrlTracking.ModuleID = ModuleId;
            }
            else {
                AddLog ("Security violation: Attempt to access document not related to the module.", EventLogController.EventLogType.ADMIN_ALERT);
                Response.Redirect (Globals.NavigateURL (), true);
            }

            cmdUpdate.Text = LocalizeString ("UpdateDocument.Text");
            cmdUpdate.ToolTip = LocalizeString ("UpdateDocument.ToolTip");
        }

        // TODO: Implement as extension method, move to the base library
        bool SelectFolder (DnnUrlControl urlControl, int folderId)
        {
            var folder = FolderManager.Instance.GetFolder (folderId);
            if (folder != null) {
                var file = FolderManager.Instance.GetFiles (folder).FirstOrDefault ();
                if (file != null) {
                    urlControl.Url = "fileid=" + file.FileId;
                    return true;
                }
            }
            return false;
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
            switch (Globals.GetURLType (url)) {
            case TabType.File:
                url = FixLegacyFileUrl (url);
                var fileId = int.Parse (UrlUtils.GetParameterValue (url));
                var file = FileManager.Instance.GetFile (fileId);
                if (file != null) {
                    return CheckRolesMatch (
                        // TODO review old code: this.ModuleConfiguration.AuthorizedViewRoles,
                        GetModuleViewRoles (),
                        // TODO reviewold code: FileSystemUtils.GetRoles(objFile.Folder, PortalId, "READ")
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

            if (notMatching) {
                // if no roles selected for message, assume it "All users"
                if (string.IsNullOrWhiteSpace (strRolesForMessage)) {
                    strRolesForMessage = Globals.glbRoleAllUsersName;
                }

                // warn user that roles do not match
                this.Message (LocalizeString ("msgFileSecurityWarning.Text")
                              .Replace ("[$ROLELIST]", strRolesForMessage), MessageType.Warning);

                return false;
            }

            return true;
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
            var fileId = 0;
            var blnAddWarning = false;

            if (url == string.Empty) {
                // file not selected
                this.Message ("msgNoFileSelected.Text", MessageType.Warning, true);
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
                    // display a "file not found" warning
                    this.Message ("msgFileDeleted.Text", MessageType.Warning, true);
                    return false;
                }
                break;
            }

            return true;
        }

        void Update (bool ignoreWarnings)
        {
            try {
                if (Page.IsValid) {
                    if (!ignoreWarnings) {
                        if (!CheckFileExists (ctlUrl.Url) || !CheckFileSecurity (ctlUrl.Url)) {
                            cmdUpdateOverride.Visible = true;
                            cmdUpdate.Visible = false;
                            // display warning instructing users to click update again if they want to ignore the warning
                            this.Message ("msgFileWarningHeading.Text", "msgFileWarning.Text", MessageType.Warning, true);
                            return;
                        }
                    }

                    // get existing document record
                    var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);
                    if (document == null) {
                        document = new DocumentInfo {
                            ItemId = itemId,
                            ModuleId = ModuleId,
                            CreatedByUserId = UserInfo.UserID,
                            OwnedByUserId = UserId
                        };
                    }

                    var oldDocument = document.Clone ();

                    document.Title = txtName.Text;
                    document.Description = txtDescription.Text;
                    document.ForceDownload = chkForceDownload.Checked;
                    document.Url = ctlUrl.Url;
                    document.LinkAttributes = textLinkAttributes.Text;
                    document.ModifiedByUserId = UserInfo.UserID;

                    UpdateDateTime (document, oldDocument);
                    UpdateOwner (document);
                    UpdateCategory (document);

                    int sortIndex;
                    document.SortOrderIndex = int.TryParse (txtSortIndex.Text, out sortIndex) ? sortIndex : 0;

                    if (Null.IsNull (itemId)) {
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

                    var urlHistory = new UrlHistory (Session);
                    urlHistory.StoreUrl (document.Url);

                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);

                    if (Null.IsNull (itemId)) {
                        this.Message (string.Format (LocalizeString ("DocumentAdded.Format"), document.Title), MessageType.Success);
                        multiView.ActiveViewIndex = 1;
                        BindUrlHistory ();
                    } else {
                        Response.Redirect (Globals.NavigateURL (), true);
                    }
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

            document.StartDate = datetimeStartDate.SelectedDate;
            document.EndDate = datetimeEndDate.SelectedDate;

            if (pickerCreatedDate.SelectedDate == null) {
                document.CreatedDate = now;
            } else {
                document.CreatedDate = pickerCreatedDate.SelectedDate.Value;
            }

            if (pickerLastModifiedDate.SelectedDate == null || oldDocument.Url != ctlUrl.Url) {
                document.ModifiedDate = now;
            } else {
                document.ModifiedDate = pickerLastModifiedDate.SelectedDate.Value;
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

        void BindUrlHistory ()
        {
            var urlHistory = new UrlHistory (Session);
            var urls = urlHistory.GetBindableUrls ();
            if (urls.Count > 0) {
                comboUrlHistory.DataSource = urls;
                comboUrlHistory.DataBind ();
            } else {
                panelUrlHistory.Visible = false;
            }
        }
    }
}
