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
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;

namespace R7.Documents
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// The EditDocs Class provides the UI for managing the Documents
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
	///   [ag]  11 March 2007 Migrated to VS2005
	/// </history>
	/// -----------------------------------------------------------------------------
	public partial class EditDocuments : DocumentsPortalModuleBase
	{
		#region "Private Members"


		private int mintItemId;
		#endregion

		#region "Event Handlers"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Page_Load runs when the control is loaded
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
		///                       and localisation
		/// </history>
		/// -----------------------------------------------------------------------------
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			try {
				// Determine ItemId of Document to Update
				if ((Request.QueryString["ItemId"] != null)) {
					ItemID = Int32.Parse(Request.QueryString["ItemId"]);
				} else {
					ItemID = Convert.ToInt32(Null.NullInteger);
				}

				// Load module instance settings
				if (!IsPostBack) {
					// Configure categories entry as a list or textbox, based on user settings
					if (DocumentsSettings.UseCategoriesList) {
						// Configure category entry as a list
						lstCategory.Visible = true;
						txtCategory.Visible = false;

						// Populate categories list
						var _with1 = new DotNetNuke.Common.Lists.ListController();
						lstCategory.DataSource = _with1.GetListEntryInfoCollection(DocumentsSettings.CategoriesListName);
						lstCategory.DataTextField = "Text";
						lstCategory.DataValueField = "Value";

						lstCategory.DataBind();
						lstCategory.Items.Insert(0, new System.Web.UI.WebControls.ListItem(Localization.GetString("None_Specified"), "-1"));
					} else {
						// Configure category entry as a free-text entry
						lstCategory.Visible = false;
						txtCategory.Visible = true;
					}

					// Add the "are you sure" message to the delete button click event
					cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("DeleteItem") + "');");

					// If the page is being requested the first time, determine if an
					// document itemId value is specified, and if so populate page
					// contents with the document details

					if (!Null.IsNull(ItemID)) {
						// Read document information
						DocumentsController objDocuments = new DocumentsController();
						DocumentInfo objDocument = objDocuments.GetDocument(ItemID, ModuleId);

						// Read values from documentInfo object on to page
						if ((objDocument != null)) {
							txtName.Text = objDocument.Title;
							txtDescription.Text = objDocument.Description;
							chkForceDownload.Checked = objDocument.ForceDownload;

							if (objDocument.Url != string.Empty) {
								ctlUrl.Url = objDocument.Url;
							}

							// Test to see if the document has been removed/deleted
							if (CheckFileExists(objDocument.Url) == false) {
								ctlUrl.UrlType = "N";
							}

							CheckFileSecurity(objDocument.Url);

							txtSortIndex.Text = objDocument.SortOrderIndex.ToString();

							if (objDocument.OwnedByUser == string.Empty) {
								lblOwner.Text = Localization.GetString("None_Specified");
							} else {
								lblOwner.Text = objDocument.OwnedByUser;
							}

							if (txtCategory.Visible) {
								txtCategory.Text = objDocument.Category;
							} else {
								//Look for the category by name
								ListItem found = lstCategory.Items.FindByText(objDocument.Category);
								if (found != null) {
									lstCategory.SelectedValue = found.Value;
								} else {
									//Legacy support, do a fall-back
									found = lstCategory.Items.FindByValue(objDocument.Category);
									if (found != null) {
										lstCategory.SelectedValue = found.Value;
									}
								}
							}

							// The audit control methods are mis-named.  The property called 
							// "CreatedByUser" actually means "last modified user", and the property
							// called "CreatedDate" actually means "ModifiedDate"
							ctlAudit.CreatedByUser = objDocument.ModifiedByUser;
							ctlAudit.CreatedDate = objDocument.ModifiedDate.ToString();

							ctlTracking.URL = objDocument.Url;
							ctlTracking.ModuleID = ModuleId;

						// security violation attempt to access item not related to this Module
						} else {
							Response.Redirect(Globals.NavigateURL(), true);
						}
					} else {
						try {
							lstOwner.SelectedValue = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
							lblOwner.Text = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().DisplayName;
						} catch (Exception exc) {
							// suppress error (defensive code only, would only happen if the owner
							// user has been deleted)
						}

						cmdDelete.Visible = false;
						ctlAudit.Visible = false;
						ctlTracking.Visible = false;

						// Set default folder
						ctlUrl.Url = DocumentsSettings.DefaultFolder + "a";

					}
				}
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}

		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Compare file's folder security with module security settings and display 
		/// a warning message if they do not match.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[ag]	11 March 2007	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private bool CheckFileSecurity(string Url)
		{
			int intFileId = 0;
			DotNetNuke.Services.FileSystem.FileController objFiles = new DotNetNuke.Services.FileSystem.FileController();
			DotNetNuke.Services.FileSystem.FileInfo objFile = new DotNetNuke.Services.FileSystem.FileInfo();
		
			switch (Globals.GetURLType(Url)) {
				case TabType.File:
					if (Url.ToLower().StartsWith("fileid=") == false) {
						// to handle legacy scenarios before the introduction of the FileServerHandler
						Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
					}

					intFileId = int.Parse(UrlUtils.GetParameterValue(Url));

					objFile = objFiles.GetFileById(intFileId, PortalId);
					if ((objFile != null)) {
						// Get file's folder security
						return CheckRolesMatch(this.ModuleConfiguration.AuthorizedViewRoles, FileSystemUtils.GetRoles(objFile.Folder, PortalId, "READ"));
					}
					break;
			}
			return true;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Tests whether the roles that the module allows read access to are a subset of
		/// the file's read-access roles.  If not, display a warning.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[ag]	11 March 2007	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private bool CheckRolesMatch(string ModuleRoles, string FileRoles)
		{
			Hashtable objFileRoles = new Hashtable();
			bool blnNotMatching = false;
			string strRolesForMessage = "";

			foreach (string strFileRole in FileRoles.Split(';')) {
				objFileRoles.Add(strFileRole, strFileRole);
				if (strFileRole == DotNetNuke.Common.Globals.glbRoleAllUsersName) {
					// If read access to the file is available for "all users", the file can
					// always be accessed
					return true;
				}
			}

			foreach (string strModuleRole in ModuleRoles.Split(';')) {
				if (!objFileRoles.ContainsKey(strModuleRole)) {
					// A view role exists for the module that is not available for the file
					blnNotMatching = true;
					if (strRolesForMessage != string.Empty) {
						strRolesForMessage = strRolesForMessage + ", ";
					}
					strRolesForMessage = strRolesForMessage + strModuleRole;
				}
			}

			if (blnNotMatching) {
				// Warn user that roles do not match
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, DotNetNuke.Services.Localization.Localization.GetString("msgFileSecurityWarning.Text", this.LocalResourceFile).Replace("[$ROLELIST]", (strRolesForMessage.IndexOf(",") >= 0 ? "s" : "").ToString() + "'" + strRolesForMessage + "'"), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
				return false;
			} else {
				return true;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Tests whether the file exists.  If it does not, add a warning message.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[ag]	11 March 2007	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private bool CheckFileExists(string Url)
		{
			int intFileId = 0;
			DotNetNuke.Services.FileSystem.FileController objFiles = new DotNetNuke.Services.FileSystem.FileController();
			DotNetNuke.Services.FileSystem.FileInfo objFile = new DotNetNuke.Services.FileSystem.FileInfo();
			bool blnAddWarning = false;

			if (Url == string.Empty) {
				// File not selected
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, DotNetNuke.Services.Localization.Localization.GetString("msgNoFileSelected.Text", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
				return false;
			} else {
				switch (Globals.GetURLType(Url)) {
					case TabType.File:
						if (Url.ToLower().StartsWith("fileid=") == false) {
							// to handle legacy scenarios before the introduction of the FileServerHandler
							Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
						}

						intFileId = int.Parse(UrlUtils.GetParameterValue(Url));

						objFile = objFiles.GetFileById(intFileId, PortalId);

						blnAddWarning = false;
						if (objFile == null) {
							blnAddWarning = true;
						} else {
							switch ((FolderController.StorageLocationTypes)objFile.StorageLocation) {
								case FolderController.StorageLocationTypes.InsecureFileSystem:
									blnAddWarning = !File.Exists(objFile.PhysicalPath);
									break;
								case FolderController.StorageLocationTypes.SecureFileSystem:
									blnAddWarning = !File.Exists(objFile.PhysicalPath + Globals.glbProtectedExtension);
									break;
								case FolderController.StorageLocationTypes.DatabaseSecure:
									blnAddWarning = false;
									// Database-stored files cannot be deleted seperately
									break;
							}
						}

						if (blnAddWarning) {
							// Display a "file not found" warning
							DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, DotNetNuke.Services.Localization.Localization.GetString("msgFileDeleted.Text", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
							return false;
						}

						break;
					case TabType.Url:
						break;
					// Cannot validate "Url" link types 
				}
			}
			return true;
		}



		/// -----------------------------------------------------------------------------
		/// <summary>
		/// cmdCancel_Click runs when the cancel button is clicked
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
		///                       and localisation
		/// </history>
		/// -----------------------------------------------------------------------------
		private void cmdCancel_Click(object sender, EventArgs e)
		{
			try {
				// Redirect back to the portal home page
				Response.Redirect(Globals.NavigateURL(), true);
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// cmdDelete_Click runs when the delete button is clicked
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
		///                       and localisation
		/// </history>
		/// -----------------------------------------------------------------------------
		private void cmdDelete_Click(object sender, EventArgs e)
		{
			try {

				if (!Null.IsNull(ItemID)) {
					DocumentsController objdocuments = new DocumentsController();
					objdocuments.DeleteDocument(this.ModuleId, ItemID);
				}

				SynchronizeModule();
				DataCache.RemoveCache(this.CacheKey + ";anon-doclist");

				// Redirect back to the portal home page
				Response.Redirect(Globals.NavigateURL(), true);
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
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
		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			Update(false);
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
		private void cmdUpdateOverride_Click(object sender, System.EventArgs e)
		{
			Update(true);
		}

		private void Update(bool Override)
		{
			try {
				// Only Update if Input Data is Valid

				if (Page.IsValid == true) {
					if (!Override) {
						// Test file exists, security
						if (!CheckFileExists(ctlUrl.Url) || !CheckFileSecurity(ctlUrl.Url)) {
							this.cmdUpdateOverride.Visible = true;
							this.cmdUpdate.Visible = false;

							// '' Display page-level warning instructing users to click update again if they want to ignore the warning
							DotNetNuke.UI.Skins.Skin.AddPageMessage(this.Page,Localization.GetString("msgFileWarningHeading.Text", this.LocalResourceFile), DotNetNuke.Services.Localization.Localization.GetString("msgFileWarning.Text", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
							return;
						}
					}

					DocumentInfo objDocument = null;
					DocumentsController objDocuments = new DocumentsController();

					// Get existing document record
					objDocument = objDocuments.GetDocument(ItemID, ModuleId);

					if (objDocument == null) {
						// New record
						objDocument = new DocumentInfo();
						objDocument.ItemId = ItemID;
						objDocument.ModuleId = ModuleId;

						objDocument.CreatedByUserId = UserInfo.UserID;

						// Default ownerid value for new documents is current user, may be changed
						// by the value of the dropdown list (below)
						objDocument.OwnedByUserId = UserId;
					}

					objDocument.Title = txtName.Text;
					objDocument.Url = ctlUrl.Url;
					objDocument.Description = txtDescription.Text;
					objDocument.ForceDownload = chkForceDownload.Checked;

					if (lstOwner.Visible) {
						if (lstOwner.SelectedValue != string.Empty) {
							objDocument.OwnedByUserId = Convert.ToInt32(lstOwner.SelectedValue);
						} else {
							objDocument.OwnedByUserId = -1;
						}
					} else {
						// User never clicked "change", leave ownedbyuserid as is
					}

					if (txtCategory.Visible) {
						objDocument.Category = txtCategory.Text;
					} else {
						if (lstCategory.SelectedItem.Text == Localization.GetString("None_Specified")) {
							objDocument.Category = "";
						} else {
							objDocument.Category = lstCategory.SelectedItem.Value;
						}
					}

					if (txtSortIndex.Text == string.Empty) {
						objDocument.SortOrderIndex = 0;
					} else {
						objDocument.SortOrderIndex = Convert.ToInt32(txtSortIndex.Text);
					}

					// Create an instance of the Document DB component

					if (Null.IsNull(ItemID)) {
						objDocuments.AddDocument(objDocument);
					} else {
						objDocuments.UpdateDocument(objDocument);
					}

					// url tracking
					UrlController objUrls = new UrlController();
					objUrls.UpdateUrl(PortalId, ctlUrl.Url, ctlUrl.UrlType, ctlUrl.Log, ctlUrl.Track, ModuleId, ctlUrl.NewWindow);

					SynchronizeModule();
					DataCache.RemoveCache(this.CacheKey + ";anon-doclist");

					// Redirect back to the portal home page
					Response.Redirect(Globals.NavigateURL(), true);

				}
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		#endregion

		#region "Private methods"
		private int ItemID {
			get { return mintItemId; }
			set { mintItemId = value; }
		}

		#endregion

		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]

		private void InitializeComponent()
		{
		}

		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent();
		}

		#endregion

		private void lnkChange_Click(System.Object sender, System.EventArgs e)
		{
			lblOwner.Visible = false;
			lnkChange.Visible = false;
			lstOwner.Visible = true;

			PopulateOwnerList();

			try {
				DocumentInfo objDocument = null;
				DocumentsController objDocuments = new DocumentsController();

				// Get existing document record
				objDocument = objDocuments.GetDocument(ItemID, ModuleId);

				try {
					if (objDocument == null) {
						lstOwner.SelectedValue = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
					} else {
						lstOwner.SelectedValue = objDocument.OwnedByUserId.ToString();
					}
				} catch (Exception exc) {
					// suppress error selecting owner user
				}

			} catch (Exception exc) {
				// suppress error if the user no longer exists
			}
		}

		private void PopulateOwnerList()
		{
			// populate owner list
			//'With New DotNetNuke.Entities.Users.UserController
			lstOwner.DataSource = DotNetNuke.Entities.Users.UserController.GetUsers(PortalId, false);

			lstOwner.DataTextField = "FullName";
			lstOwner.DataValueField = "UserId";

			lstOwner.DataBind();

			// .GetUsers doesn't return super-users, but they can own documents
			// so add them to the list
			DotNetNuke.Entities.Users.UserInfo objSuperUser = default(DotNetNuke.Entities.Users.UserInfo);
			foreach (UserInfo objsu in DotNetNuke.Entities.Users.UserController.GetUsers(Null.NullInteger, false)) {
				lstOwner.Items.Insert(0, new System.Web.UI.WebControls.ListItem(objsu.DisplayName, objsu.UserID.ToString()));
			}

			lstOwner.Items.Insert(0, new System.Web.UI.WebControls.ListItem(Localization.GetString("None_Specified"), "-1"));
			//' End With
		}
		public EditDocuments()
		{
			Init += Page_Init;
			Load += Page_Load;
		}

	}

}
