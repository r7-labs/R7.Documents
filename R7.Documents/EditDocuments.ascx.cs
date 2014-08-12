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
		private void Page_Load (System.Object sender, System.EventArgs e)
		{
			try
			{
				// Determine ItemId of Document to Update
				if ((Request.QueryString ["ItemId"] != null))
				{
					ItemID = Int32.Parse (Request.QueryString ["ItemId"]);
				}
				else
				{
					ItemID = Convert.ToInt32 (Null.NullInteger);
				}

				// Load module instance settings
				if (!IsPostBack)
				{
					// Configure categories entry as a list or textbox, based on user settings
					if (DocumentsSettings.UseCategoriesList)
					{
						// Configure category entry as a list
						lstCategory.Visible = true;
						txtCategory.Visible = false;

						// Populate categories list
						var _with1 = new DotNetNuke.Common.Lists.ListController ();
						lstCategory.DataSource = _with1.GetListEntryInfoItems (DocumentsSettings.CategoriesListName);
						lstCategory.DataTextField = "Text";
						lstCategory.DataValueField = "Value";

						lstCategory.DataBind ();
						lstCategory.Items.Insert (0, new System.Web.UI.WebControls.ListItem (Localization.GetString ("None_Specified"), "-1"));
					}
					else
					{
						// Configure category entry as a free-text entry
						lstCategory.Visible = false;
						txtCategory.Visible = true;
					}

					// Add the "are you sure" message to the delete button click event
					cmdDelete.Attributes.Add ("onClick", "javascript:return confirm('" + Localization.GetString ("DeleteItem") + "');");

					// If the page is being requested the first time, determine if an
					// document itemId value is specified, and if so populate page
					// contents with the document details

					if (!Null.IsNull (ItemID))
					{
						// Read document information
						var objDocument = DocumentsController.GetDocument (ItemID, ModuleId);

						// Read values from documentInfo object on to page
						if ((objDocument != null))
						{
							txtName.Text = objDocument.Title;
							txtDescription.Text = objDocument.Description;
							chkForceDownload.Checked = objDocument.ForceDownload;
							checkIsPublished.Checked = objDocument.IsPublished;

							pickerCreatedDate.SelectedDate = objDocument.CreatedDate;
							pickerLastModifiedDate.SelectedDate = objDocument.ModifiedDate;
							
							if (objDocument.Url != string.Empty)
							{
								ctlUrl.Url = objDocument.Url;
							}

							// Test to see if the document has been removed/deleted
							if (CheckFileExists (objDocument.Url) == false)
							{
								ctlUrl.UrlType = "N";
							}

							CheckFileSecurity (objDocument.Url);

							txtSortIndex.Text = objDocument.SortOrderIndex.ToString ();

							if (objDocument.OwnedByUser == string.Empty)
							{
								lblOwner.Text = Localization.GetString ("None_Specified");
							}
							else
							{
								lblOwner.Text = objDocument.OwnedByUser;
							}

							if (txtCategory.Visible)
							{
								txtCategory.Text = objDocument.Category;
							}
							else
							{
								//Look for the category by name
								ListItem found = lstCategory.Items.FindByText (objDocument.Category);
								if (found != null)
								{
									lstCategory.SelectedValue = found.Value;
								}
								else
								{
									//Legacy support, do a fall-back
									found = lstCategory.Items.FindByValue (objDocument.Category);
									if (found != null)
									{
										lstCategory.SelectedValue = found.Value;
									}
								}
							}

							// The audit control methods are mis-named.  The property called 
							// "CreatedByUser" actually means "last modified user", and the property
							// called "CreatedDate" actually means "ModifiedDate"
							
							ctlAudit.CreatedByUser = objDocument.CreatedByUser;
							ctlAudit.CreatedDate = objDocument.CreatedDate.ToString ();
							ctlAudit.LastModifiedByUser = objDocument.ModifiedByUser;
							ctlAudit.LastModifiedDate = objDocument.ModifiedDate.ToString();

							if (ctlUrl.UrlType == "N")
								panelUrlTracking.Visible = false;
							else
							{
								ctlUrlTracking.URL = objDocument.Url;
								ctlUrlTracking.ModuleID = ModuleId;
							}
						}
						else
						{
							// security violation attempt to access item not related to this Module							
							Response.Redirect (Globals.NavigateURL (), true);
						}
					}
					else
					{
						try
						{
							lstOwner.SelectedValue = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo ().UserID.ToString ();
							lblOwner.Text = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo ().DisplayName;
						}
						catch
						{
							// suppress error (defensive code only, would only happen if the owner
							// user has been deleted)
						}

						cmdDelete.Visible = false;
						ctlAudit.Visible = false;
						panelUrlTracking.Visible = false;

						// set document published by default
						checkIsPublished.Checked = true;

						// Set default folder
						ctlUrl.Url = DocumentsSettings.DefaultFolder + "a";
					}
				}
			}
			catch (Exception exc)
			{
				//Module failed to load
				Exceptions.ProcessModuleLoadException (this, exc);
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
		private bool CheckFileSecurity (string Url)
		{
			int intFileId = 0;
					
			switch (Globals.GetURLType (Url))
			{
				case TabType.File:
					if (Url.ToLower ().StartsWith ("fileid=") == false)
					{
						// to handle legacy scenarios before the introduction of the FileServerHandler
						Url = "FileID=" + FileManager.Instance.GetFile (PortalId, Url).FileId;
						// Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
					}

					intFileId = int.Parse (UrlUtils.GetParameterValue (Url));
					var objFile = FileManager.Instance.GetFile (intFileId);
					
					if (objFile != null)
					{
						// module view roles
						var moduleViewRoles = new StringBuilder ();
						foreach (ModulePermissionInfo modulePerm in ModuleConfiguration.ModulePermissions)
						{
							if (modulePerm.PermissionKey == "VIEW" && modulePerm.AllowAccess)
							{
								if (moduleViewRoles.Length > 0)
									moduleViewRoles.Append (';');
	
								moduleViewRoles.Append (modulePerm.RoleName);
							}
						}
	
						// folder view roles
						var folder = FolderManager.Instance.GetFolder (objFile.FolderId);
						var folderViewRoles = new StringBuilder ();
						foreach (FolderPermissionInfo folderPerm in folder.FolderPermissions)
						{
							if (folderPerm.PermissionKey == "READ" && folderPerm.AllowAccess)
							{
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
		private bool CheckRolesMatch (string ModuleRoles, string FileRoles)
		{
			Hashtable objFileRoles = new Hashtable ();
			bool blnNotMatching = false;
			string strRolesForMessage = "";

			foreach (string strFileRole in FileRoles.Split(';'))
			{
				objFileRoles.Add (strFileRole, strFileRole);
				if (strFileRole == DotNetNuke.Common.Globals.glbRoleAllUsersName)
				{
					// If read access to the file is available for "all users", the file can
					// always be accessed
					return true;
				}
			}

			foreach (string strModuleRole in ModuleRoles.Split(';'))
			{
				if (!objFileRoles.ContainsKey (strModuleRole))
				{
					// A view role exists for the module that is not available for the file
					blnNotMatching = true;
					if (strRolesForMessage != string.Empty)
					{
						strRolesForMessage = strRolesForMessage + ", ";
					}
					strRolesForMessage = strRolesForMessage + strModuleRole;
				}
			}

			if (blnNotMatching)
			{
				// if no roles selected for message, assume it "All users"
				if (string.IsNullOrWhiteSpace (strRolesForMessage))
					strRolesForMessage = Globals.glbRoleAllUsersName;

				// Warn user that roles do not match
				DotNetNuke.UI.Skins.Skin.AddModuleMessage (
					this, 
					Localization.GetString ("msgFileSecurityWarning.Text", this.LocalResourceFile)
					.Replace ("[$ROLELIST]", strRolesForMessage), 
					DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
				return false;
			}

			return true;
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
		private bool CheckFileExists (string Url)
		{
			int intFileId = 0;
			bool blnAddWarning = false;

			if (Url == string.Empty)
			{
				// File not selected
				DotNetNuke.UI.Skins.Skin.AddModuleMessage (this, DotNetNuke.Services.Localization.Localization.GetString ("msgNoFileSelected.Text", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
				return false;
			}
			else
			{
				switch (Globals.GetURLType (Url))
				{
					case TabType.File:
						if (Url.ToLower ().StartsWith ("fileid=") == false)
						{
							// to handle legacy scenarios before the introduction of the FileServerHandler
							Url = "FileID=" + FileManager.Instance.GetFile (PortalId, Url).FileId;
							// Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
						}

						intFileId = int.Parse (UrlUtils.GetParameterValue (Url));

						var objFile = FileManager.Instance.GetFile (intFileId);

						blnAddWarning = false;
						if (objFile == null)
						{
							blnAddWarning = true;
						}
						else
						{
							switch ((FolderController.StorageLocationTypes)objFile.StorageLocation)
							{
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

						if (blnAddWarning)
						{
							// Display a "file not found" warning
							DotNetNuke.UI.Skins.Skin.AddModuleMessage (this, DotNetNuke.Services.Localization.Localization.GetString ("msgFileDeleted.Text", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
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
		private void cmdCancel_Click (object sender, EventArgs e)
		{
			try
			{
				// Redirect back to the portal home page
				Response.Redirect (Globals.NavigateURL (), true);
				//Module failed to load
			}
			catch (Exception exc)
			{
				Exceptions.ProcessModuleLoadException (this, exc);
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
		private void cmdDelete_Click (object sender, EventArgs e)
		{
			try
			{

				if (!Null.IsNull (ItemID))
				{
					DocumentsController.Delete<DocumentInfo> (ItemID); // ModuleID!
				}

				Utils.SynchronizeModule (this);
				DataCache.RemoveCache (this.DataCacheKey + ";anon-doclist");

				// Redirect back to the portal home page
				Response.Redirect (Globals.NavigateURL (), true);
				//Module failed to load
			}
			catch (Exception exc)
			{
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
		private void cmdUpdate_Click (object sender, EventArgs e)
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
		private void cmdUpdateOverride_Click (object sender, System.EventArgs e)
		{
			Update (true);
		}

		private void Update (bool Override)
		{
			try
			{
				// Only Update if Input Data is Valid

				if (Page.IsValid == true)
				{
					if (!Override)
					{
						// Test file exists, security
						if (!CheckFileExists (ctlUrl.Url) || !CheckFileSecurity (ctlUrl.Url))
						{
							this.cmdUpdateOverride.Visible = true;
							this.cmdUpdate.Visible = false;

							// '' Display page-level warning instructing users to click update again if they want to ignore the warning
							DotNetNuke.UI.Skins.Skin.AddPageMessage (this.Page, Localization.GetString ("msgFileWarningHeading.Text", this.LocalResourceFile), DotNetNuke.Services.Localization.Localization.GetString ("msgFileWarning.Text", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
							return;
						}
					}

					// Get existing document record
					var objDocument = DocumentsController.GetDocument (ItemID, ModuleId);

					if (objDocument == null)
					{
						// New record
						objDocument = new DocumentInfo ();
						objDocument.ItemId = ItemID;
						objDocument.ModuleId = ModuleId;
						
						objDocument.CreatedByUserId = UserInfo.UserID;
						
						// Default ownerid value for new documents is current user, may be changed
						// by the value of the dropdown list (below)
						objDocument.OwnedByUserId = UserId;
					}
				
					objDocument.IsPublished = checkIsPublished.Checked;
					objDocument.ModifiedByUserId = UserInfo.UserID;

					objDocument.Title = txtName.Text;
					objDocument.Url = ctlUrl.Url;
					objDocument.Description = txtDescription.Text;
					objDocument.ForceDownload = chkForceDownload.Checked;

					if (lstOwner.Visible)
					{
						if (lstOwner.SelectedValue != string.Empty)
						{
							objDocument.OwnedByUserId = Convert.ToInt32 (lstOwner.SelectedValue);
						}
						else
						{
							objDocument.OwnedByUserId = -1;
						}
					}
					else
					{
						// User never clicked "change", leave ownedbyuserid as is
					}

					if (txtCategory.Visible)
					{
						objDocument.Category = txtCategory.Text;
					}
					else
					{
						if (lstCategory.SelectedItem.Text == Localization.GetString ("None_Specified"))
						{
							objDocument.Category = "";
						}
						else
						{
							objDocument.Category = lstCategory.SelectedItem.Value;
						}
					}

					// getting sort index
					int sortIndex;
					objDocument.SortOrderIndex = int.TryParse (txtSortIndex.Text, out sortIndex) ? sortIndex : 0;

					#region Update date & time

					var now = DateTime.Now;
					
					if (pickerCreatedDate.SelectedDate != null)
					{
						if (Null.IsNull (ItemID) || objDocument.CreatedDate != pickerCreatedDate.SelectedDate.Value)
							objDocument.CreatedDate = pickerCreatedDate.SelectedDate.Value;
						// else leave CreatedDate as is
					}
					else
					{
						objDocument.CreatedDate = now;
					}

					if (pickerLastModifiedDate.SelectedDate != null)
					{
						if (Null.IsNull (ItemID) || objDocument.ModifiedDate != pickerLastModifiedDate.SelectedDate.Value)
							objDocument.ModifiedDate = pickerLastModifiedDate.SelectedDate.Value;
						else 
							// update ModifiedDate
							objDocument.ModifiedDate = now;
					}
					else
					{
						objDocument.ModifiedDate = now;
					}

					#endregion

					// Create an instance of the Document DB component
					if (Null.IsNull (ItemID))
					{
						DocumentsController.Add<DocumentInfo> (objDocument);
					}
					else
					{
						DocumentsController.Update<DocumentInfo> (objDocument);
					}

					// url tracking
					UrlController objUrls = new UrlController ();
					objUrls.UpdateUrl (PortalId, ctlUrl.Url, ctlUrl.UrlType, ctlUrl.Log, ctlUrl.Track, ModuleId, ctlUrl.NewWindow);

					Utils.SynchronizeModule (this);
					DataCache.RemoveCache (this.DataCacheKey + ";anon-doclist");

					// Redirect back to the portal home page
					Response.Redirect (Globals.NavigateURL (), true);

				}
				//Module failed to load
			}
			catch (Exception exc)
			{
				Exceptions.ProcessModuleLoadException (this, exc);
			}
		}

		#endregion

		#region "Private methods"

		private int ItemID
		{
			get { return mintItemId; }
			set { mintItemId = value; }
		}

		#endregion

		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough ()]

		private void InitializeComponent ()
		{
		}

		private void Page_Init (System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent ();
			
			cmdUpdate.Click += cmdUpdate_Click;
			cmdCancel.Click += cmdCancel_Click;
			cmdDelete.Click += cmdDelete_Click;
			cmdUpdateOverride.Click += cmdUpdateOverride_Click;
			lnkChange.Click += lnkChange_Click;
		}

		#endregion

		private void lnkChange_Click (System.Object sender, System.EventArgs e)
		{
			lblOwner.Visible = false;
			lnkChange.Visible = false;
			lstOwner.Visible = true;

			PopulateOwnerList ();

			try
			{
				// Get existing document record
				var objDocument = DocumentsController.GetDocument (ItemID, ModuleId);

				try
				{
					if (objDocument == null)
					{
						lstOwner.SelectedValue = UserId.ToString ();
					}
					else
					{
						lstOwner.SelectedValue = objDocument.OwnedByUserId.ToString ();
					}
				}
				catch
				{
					// suppress error selecting owner user
				}

			}
			catch
			{
				// suppress error if the user no longer exists
			}
		}

		private void PopulateOwnerList ()
		{
			// populate owner list
			//'With New DotNetNuke.Entities.Users.UserController
			lstOwner.DataSource = UserController.GetUsers (PortalId);

			lstOwner.DataTextField = "FullName";
			lstOwner.DataValueField = "UserId";

			lstOwner.DataBind ();

			// .GetUsers doesn't return super-users, but they can own documents
			// so add them to the list
			foreach (UserInfo objsu in DotNetNuke.Entities.Users.UserController.GetUsers(Null.NullInteger))
			{
				lstOwner.Items.Insert (0, new System.Web.UI.WebControls.ListItem (objsu.DisplayName, objsu.UserID.ToString ()));
			}

			lstOwner.Items.Insert (0, new System.Web.UI.WebControls.ListItem (Localization.GetString ("None_Specified"), "-1"));
			//' End With
		}

		public EditDocuments ()
		{
			Init += Page_Init;
			Load += Page_Load;
		}

	}

}
