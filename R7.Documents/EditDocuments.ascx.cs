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
using R7.Documents.Data;
using R7.Documents.Models;
using R7.DotNetNuke.Extensions.ModuleExtensions;
using R7.DotNetNuke.Extensions.Modules;
using R7.DotNetNuke.Extensions.Utilities;

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
        #region Private fields

        int itemId;

        #endregion

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
			
            // set URL for cancel button
            linkCancel.NavigateUrl = UrlHelper.GetCancelUrl (UrlHelper.IsInPopup (Request));

            cmdDelete.Attributes.Add ("onClick", 
                "javascript:return confirm('" + LocalizeString ("Delete.Text")  + "');");
            
            buttonDeleteWithResource.Attributes.Add ("onClick", 
                "javascript:return confirm('" + LocalizeString ("DeleteWithResource.Text") + "');");

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

                // determine ItemId of Document to Update
                itemId = TypeUtils.ParseToNullable<int> (Request.QueryString ["ItemId"]) ?? Null.NullInteger;

                // Load module instance settings
                if (!IsPostBack) {
                    
                    // if the page is being requested the first time, determine if an
                    // document itemId value is specified, and if so populate page
                    // contents with the document details

                    if (!Null.IsNull (itemId)) {
                        // read document information
                        var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);

                        // read values from documentInfo object on to page
                        if (document != null) {
                            txtName.Text = document.Title;
                            txtDescription.Text = document.Description;
                            chkForceDownload.Checked = document.ForceDownload;
                            datetimeStartDate.SelectedDate = document.StartDate;
                            datetimeEndDate.SelectedDate = document.EndDate;
                            textLinkAttributes.Text = document.LinkAttributes;

                            pickerCreatedDate.SelectedDate = document.CreatedDate;
                            pickerLastModifiedDate.SelectedDate = document.ModifiedDate;
							
                            if (!string.IsNullOrEmpty (document.Url)) {
                                ctlUrl.Url = document.Url;
                            }

                            // test to see if the document has been removed/deleted
                            if (!CheckFileExists (document.Url)) {
                                ctlUrl.UrlType = "N";
                            }
                            else {
                                CheckFileSecurity (document.Url);
                            }

                            txtSortIndex.Text = document.SortOrderIndex.ToString ();

                            if (string.IsNullOrEmpty (document.OwnedByUser)) {
                                lblOwner.Text = Localization.GetString ("None_Specified");
                            }
                            else {
                                lblOwner.Text = document.OwnedByUser;
                            }

                            if (txtCategory.Visible) {
                                txtCategory.Text = document.Category;
                            }
                            else {
                                // look for the category by name
                                var found = lstCategory.Items.FindByText (document.Category);
                                if (found != null) {
                                    lstCategory.SelectedValue = found.Value;
                                }
                                else {
                                    // legacy support, do a fall-back
                                    found = lstCategory.Items.FindByValue (document.Category);
                                    if (found != null) {
                                        lstCategory.SelectedValue = found.Value;
                                    }
                                }
                            }

                            // FIXME: Audit data not preserved between postbacks
                            ctlAudit.CreatedByUser = document.CreatedByUser;
                            ctlAudit.CreatedDate = document.CreatedDate.ToString ();
                            ctlAudit.LastModifiedByUser = document.ModifiedByUser;
                            ctlAudit.LastModifiedDate = document.ModifiedDate.ToString ();

                            ctlUrlTracking.URL = document.Url;
                            ctlUrlTracking.ModuleID = ModuleId;
                        }
                        else {
                            AddLog ("Security violation: Attempt to access document not related to the module.", EventLogController.EventLogType.ADMIN_ALERT);
                            Response.Redirect (Globals.NavigateURL (), true);
                        }
                    }
                    else {
                        try {
                            lstOwner.SelectedValue = UserId.ToString ();
                            lblOwner.Text = UserInfo.DisplayName;

                            // HACK: Calculate sortindex for new documents
                            var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
                            if (documents != null && documents.Any ()) {
                                var maxSortIndex = documents.Max (d => d.SortOrderIndex);

                                // TODO: Move to portal settings
                                txtSortIndex.Text = (maxSortIndex + 10).ToString ();
                            }
                        }
                        catch {
                            // TODO: Log error
                            // suppress error (defensive code only, would only happen if the owner user has been deleted)
                        }

                        cmdDelete.Visible = false;
                        buttonDeleteWithResource.Visible = false;

                        // set default folder
                        if (Settings.DefaultFolder != null) {
                            var folder = FolderManager.Instance.GetFolder (Settings.DefaultFolder.Value);
                            if (folder != null) {
                                var file = FolderManager.Instance.GetFiles (folder).FirstOrDefault ();
                                if (file != null) {
                                    ctlUrl.Url = "FileId=" + file.FileId;
                                }
                                else {
                                    this.Message ("CurrentFolder.Warning", MessageType.Warning, true);

                                    // FIXME: select folder => postback => root folder is always selected.
                                    // Setting link type to none provide a way to mask this behavior
                                    // as user can select folder only after manually changing link type (i.e. after postback).

                                    ctlUrl.UrlType = "N";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc) {
                // module failed to load
                Exceptions.ProcessModuleLoadException (this, exc);
            }

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
        /// 	[ag]	11 March 2007	Created
        /// </history>
        bool CheckFileSecurity (string url)
        {
            var fileId = 0;
					
            switch (Globals.GetURLType (url)) {
                case TabType.File:
                    if (!url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase)) {
                        // to handle legacy scenarios before the introduction of the FileServerHandler
                        url = "FileID=" + FileManager.Instance.GetFile (PortalId, url).FileId;
                        // Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
                    }

                    fileId = int.Parse (UrlUtils.GetParameterValue (url));
                    var file = FileManager.Instance.GetFile (fileId);
					
                    if (file != null) {
                        // module view roles
                        var moduleViewRoles = new StringBuilder ();
                        foreach (ModulePermissionInfo modulePerm in ModuleConfiguration.ModulePermissions) {
                            if (modulePerm.PermissionKey == "VIEW" && modulePerm.AllowAccess) {
                                if (moduleViewRoles.Length > 0)
                                    moduleViewRoles.Append (';');
	
                                moduleViewRoles.Append (modulePerm.RoleName);
                            }
                        }
	
                        // folder view roles
                        var folder = FolderManager.Instance.GetFolder (file.FolderId);
                        var folderViewRoles = new StringBuilder ();
                        foreach (FolderPermissionInfo folderPerm in folder.FolderPermissions) {
                            if (folderPerm.PermissionKey == "READ" && folderPerm.AllowAccess) {
                                if (folderViewRoles.Length > 0)
                                    folderViewRoles.Append (';');
	
                                folderViewRoles.Append (folderPerm.RoleName);
                            }
                        }
					
                        return CheckRolesMatch (
							// this.ModuleConfiguration.AuthorizedViewRoles, 
                            moduleViewRoles.ToString (),
							// FileSystemUtils.GetRoles(objFile.Folder, PortalId, "READ")
                            folderViewRoles.ToString ()
                        );
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Tests whether the roles that the module allows read access to are a subset of
        /// the file's read-access roles.  If not, display a warning.
        /// </summary>
        /// <history>
        /// 	[ag]	11 March 2007	Created
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
        /// Tests whether the file exists.  If it does not, add a warning message.
        /// </summary>
        /// <history>
        /// 	[ag]	11 March 2007	Created
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
            else {
                switch (Globals.GetURLType (url)) {
                    case TabType.File:
                        if (!url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase)) {
                            // to handle legacy scenarios before the introduction of the FileServerHandler
                            url = "FileID=" + FileManager.Instance.GetFile (PortalId, url).FileId;
                            // Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
                        }

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
                    case TabType.Url:
                        // cannot validate "Url" link types
                        break;
                }
            }
            return true;
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
				
                // redirect back to the module page
                Response.Redirect (Globals.NavigateURL (), true);
				
            }
            catch (Exception exc) {
                // module failed to load
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

        void Update (bool Override)
        {
            try {
                // only Update if Input Data is Valid
                if (Page.IsValid == true) {
                    if (!Override) {
                        // test file exists, security
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
                        // new record
                        document = new DocumentInfo ();
                        document.ItemId = itemId;
                        document.ModuleId = ModuleId;
						
                        document.CreatedByUserId = UserInfo.UserID;
						
                        // default ownerid value for new documents is current user, may be changed
                        // by the value of the dropdown list (below)
                        document.OwnedByUserId = UserId;
                    }

                    document.StartDate = datetimeStartDate.SelectedDate;
                    document.EndDate = datetimeEndDate.SelectedDate;
                    document.ModifiedByUserId = UserInfo.UserID;

                    document.Title = txtName.Text;
                    document.Description = txtDescription.Text;
                    document.ForceDownload = chkForceDownload.Checked;

                    var oldUrl = document.Url;
                    document.Url = ctlUrl.Url;
                    document.LinkAttributes = textLinkAttributes.Text;
					
                    if (lstOwner.Visible) {
                        if (lstOwner.SelectedValue != string.Empty) {
                            document.OwnedByUserId = Convert.ToInt32 (lstOwner.SelectedValue);
                        }
                        else {
                            document.OwnedByUserId = Null.NullInteger;
                        }
                    }
                    else {
                        // user never clicked "change", leave ownedbyuserid as is
                    }

                    if (txtCategory.Visible) {
                        document.Category = txtCategory.Text;
                    }
                    else {
                        if (lstCategory.SelectedItem.Text == Localization.GetString ("None_Specified")) {
                            document.Category = "";
                        }
                        else {
                            document.Category = lstCategory.SelectedItem.Value;
                        }
                    }

                    // getting sort index
                    int sortIndex;
                    document.SortOrderIndex = int.TryParse (txtSortIndex.Text, out sortIndex) ? sortIndex : 0;

                    #region Update date & time

                    var now = DateTime.Now;

                    if (pickerCreatedDate.SelectedDate == null) {
                        pickerCreatedDate.SelectedDate = now;
                    }

                    if (pickerLastModifiedDate.SelectedDate == null || oldUrl != ctlUrl.Url) {
                        pickerLastModifiedDate.SelectedDate = now;
                    }

                    document.CreatedDate = pickerCreatedDate.SelectedDate.Value;
                    document.ModifiedDate = pickerLastModifiedDate.SelectedDate.Value;

                    #endregion

                    if (Null.IsNull (itemId)) {
                        DocumentsDataProvider.Instance.Add (document);
                    }
                    else {
                        DocumentsDataProvider.Instance.Update (document);
                        if (document.Url != oldUrl) {
                            // delete old URL tracking data
                            DocumentsDataProvider.Instance.DeleteDocumentUrl (oldUrl, PortalId, ModuleId);
                        }
                    }

                    // add or update URL tracking
                    var ctrlUrl = new UrlController ();
                    ctrlUrl.UpdateUrl (PortalId, ctlUrl.Url, ctlUrl.UrlType, ctlUrl.Log, ctlUrl.Track, ModuleId, ctlUrl.NewWindow);

                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
					
                    // redirect back to the module page
                    Response.Redirect (Globals.NavigateURL (), true);
                }
            }
            catch (Exception exc) {
                // module failed to load
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        #endregion

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
                catch {
                    // TODO: Log error
                    // suppress error selecting owner user
                }

            }
            catch {
                // TODO: Log error
                // suppress error if the user no longer exists
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
