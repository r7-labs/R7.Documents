//
// Copyright (c) 2002-2011 by DotNetNuke Corporation
// Copyright (c) 2014-2015 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.R7;
using DotNetNuke.Services.Messaging.Data;
using R7.Documents.Data;

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
		#region Private Members

		private int itemId;

		#endregion

		#region Event Handlers

		protected override void OnInit (EventArgs e)
		{
			base.OnInit (e);
			
            // set URL for cancel button
            linkCancel.NavigateUrl = Globals.NavigateURL ();

			// Add the "are you sure" message to the delete button click event
			cmdDelete.Attributes.Add ("onClick", 
                "javascript:return confirm('" + Localization.GetString ("DeleteItem") + "');");

            // Configure categories entry as a list or textbox, based on user settings
            if (Settings.UseCategoriesList)
            {
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
            else
            {
                // Configure category entry as a free-text entry
                lstCategory.Visible = false;
                txtCategory.Visible = true;
            }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// OnLoad runs when the control is loaded
		/// </summary>
		/// -----------------------------------------------------------------------------
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			try
			{
				// Determine ItemId of Document to Update
				if (Request.QueryString ["ItemId"] != null)
				{
					itemId = int.Parse (Request.QueryString ["ItemId"]);
				}
				else
				{
                    itemId = Null.NullInteger;
				}

				// Load module instance settings
				if (!IsPostBack)
				{
					// If the page is being requested the first time, determine if an
					// document itemId value is specified, and if so populate page
					// contents with the document details

					if (!Null.IsNull (itemId))
					{
						// Read document information
                        var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);

						// Read values from documentInfo object on to page
						if (document != null)
						{
							txtName.Text = document.Title;
							txtDescription.Text = document.Description;
							chkForceDownload.Checked = document.ForceDownload;
							checkIsPublished.Checked = document.IsPublished;
                            textLinkAttributes.Text = document.LinkAttributes;

							pickerCreatedDate.SelectedDate = document.CreatedDate;
							pickerLastModifiedDate.SelectedDate = document.ModifiedDate;
							
                            if (!string.IsNullOrEmpty (document.Url))
							{
								ctlUrl.Url = document.Url;
							}

							// Test to see if the document has been removed/deleted
							if (!CheckFileExists (document.Url))
							{
								ctlUrl.UrlType = "N";
							}

							CheckFileSecurity (document.Url);

							txtSortIndex.Text = document.SortOrderIndex.ToString ();

                            if (string.IsNullOrEmpty (document.OwnedByUser))
							{
								lblOwner.Text = Localization.GetString ("None_Specified");
							}
							else
							{
								lblOwner.Text = document.OwnedByUser;
							}

							if (txtCategory.Visible)
							{
								txtCategory.Text = document.Category;
							}
							else
							{
								//Look for the category by name
								var found = lstCategory.Items.FindByText (document.Category);
								if (found != null)
								{
									lstCategory.SelectedValue = found.Value;
								}
								else
								{
									//Legacy support, do a fall-back
									found = lstCategory.Items.FindByValue (document.Category);
									if (found != null)
									{
										lstCategory.SelectedValue = found.Value;
									}
								}
							}

							// The audit control methods are mis-named.  The property called 
							// "CreatedByUser" actually means "last modified user", and the property
							// called "CreatedDate" actually means "ModifiedDate"
							
							ctlAudit.CreatedByUser = document.CreatedByUser;
							ctlAudit.CreatedDate = document.CreatedDate.ToString ();
							ctlAudit.LastModifiedByUser = document.ModifiedByUser;
							ctlAudit.LastModifiedDate = document.ModifiedDate.ToString ();

							if (ctlUrl.UrlType == "N")
                            {
								panelUrlTracking.Visible = false;
                            }
							else
							{
								ctlUrlTracking.URL = document.Url;
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
                            lstOwner.SelectedValue = UserId.ToString ();
                            lblOwner.Text = UserInfo.DisplayName;

                            // HACK: Calculate sortindex for new documents
                            var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
                            if (documents != null && documents.Any ())
                            {
                                var maxSortIndex = documents.Max (d => d.SortOrderIndex);

                                // TODO: Move to portal settings
                                txtSortIndex.Text = (maxSortIndex + 10).ToString ();
                            }
						}
						catch
						{
							// suppress error (defensive code only, would only happen if the owner
							// user has been deleted)
						}

						cmdDelete.Visible = false;
                        panelDelete.Visible = false;
                        panelUpdate.Visible = false;
						ctlAudit.Visible = false;
						panelUrlTracking.Visible = false;

						// set document published by default
						checkIsPublished.Checked = true;

						// set default folder
						if (Settings.DefaultFolder != null)
                        {
                            var folder = FolderManager.Instance.GetFolder (Settings.DefaultFolder.Value);
                            if (folder != null)
                            {
                                var file = FolderManager.Instance.GetFiles (folder).FirstOrDefault ();
                                if (file != null)
                                {
                                    ctlUrl.Url = "FileId=" + file.FileId;
                                }
                                else
                                {
                                    this.Message ("CurrentFolder.Warning", MessageType.Warning, true);

                                    // BUG: select folder => postback => root folder is always selected.
                                    // Setting link type to none provide a way to mask this behavior
                                    // as user can select folder only after manually changing link type (i.e. after postback).

                                    ctlUrl.UrlType = "N";
                                }
                            }
                        }
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
            var fileId = 0;
					
			switch (Globals.GetURLType (Url))
			{
				case TabType.File:
                    if (!Url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase))
					{
						// to handle legacy scenarios before the introduction of the FileServerHandler
						Url = "FileID=" + FileManager.Instance.GetFile (PortalId, Url).FileId;
						// Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
					}

					fileId = int.Parse (UrlUtils.GetParameterValue (Url));
                    var file = FileManager.Instance.GetFile (fileId);
					
					if (file != null)
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
						var folder = FolderManager.Instance.GetFolder (file.FolderId);
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
		private bool CheckRolesMatch (string moduleRoles, string fileRolesString)
		{
            var fileRoles = new Hashtable ();
            var notMatching = false;
			var strRolesForMessage = "";

            foreach (var fileRoleString in fileRolesString.Split (';'))
			{
				fileRoles.Add (fileRoleString, fileRoleString);
				if (fileRoleString == Globals.glbRoleAllUsersName)
				{
					// If read access to the file is available for "all users", the file can
					// always be accessed
					return true;
				}
			}

            foreach (var moduleRoleString in moduleRoles.Split(';'))
			{
				if (!fileRoles.ContainsKey (moduleRoleString))
				{
					// A view role exists for the module that is not available for the file
					notMatching = true;
					if (strRolesForMessage != string.Empty)
					{
						strRolesForMessage = strRolesForMessage + ", ";
					}

					strRolesForMessage = strRolesForMessage + moduleRoleString;
				}
			}

			if (notMatching)
			{
				// if no roles selected for message, assume it "All users"
                if (string.IsNullOrWhiteSpace (strRolesForMessage))
                {
                    strRolesForMessage = Globals.glbRoleAllUsersName;
                }

				// Warn user that roles do not match
                this.Message (LocalizeString ("msgFileSecurityWarning.Text")
                    .Replace ("[$ROLELIST]", strRolesForMessage), MessageType.Warning);
                
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
            var fileId = 0;
			var blnAddWarning = false;

			if (Url == string.Empty)
			{
				// file not selected
                this.Message ("msgNoFileSelected.Text", MessageType.Warning, true);
				return false;
			}
			else
			{
				switch (Globals.GetURLType (Url))
				{
					case TabType.File:
                        if (!Url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase))
						{
							// to handle legacy scenarios before the introduction of the FileServerHandler
							Url = "FileID=" + FileManager.Instance.GetFile (PortalId, Url).FileId;
							// Url = "FileID=" + objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId);
						}

						fileId = int.Parse (UrlUtils.GetParameterValue (Url));

						var objFile = FileManager.Instance.GetFile (fileId);

						blnAddWarning = false;
						if (objFile == null)
						{
							blnAddWarning = true;
						}
						else
						{
							switch ((FolderController.StorageLocationTypes) objFile.StorageLocation)
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
                            this.Message ("msgFileDeleted.Text", MessageType.Warning, true);
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
		/// cmdDelete_Click runs when the delete button is clicked
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
		///                       and localisation
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void cmdDelete_Click (object sender, EventArgs e)
		{
			try
			{
				if (!Null.IsNull (itemId))
				{
                    var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);
					if (document != null)
					{
                        DocumentsDataProvider.Instance.Delete (document);
                        DocumentsDataProvider.Instance.DeleteDocumentUrl (document.Url, PortalId, ModuleId);

                        if (checkDeleteWithResource.Checked)
                            DocumentsDataProvider.Instance.DeleteDocumentResource (document, PortalId);
					}
				}

				Synchronize ();
				
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
		protected void cmdUpdateOverride_Click (object sender, System.EventArgs e)
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
                    var objDocument = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);

					if (objDocument == null)
					{
						// New record
						objDocument = new DocumentInfo ();
						objDocument.ItemId = itemId;
						objDocument.ModuleId = ModuleId;
						
						objDocument.CreatedByUserId = UserInfo.UserID;
						
						// Default ownerid value for new documents is current user, may be changed
						// by the value of the dropdown list (below)
						objDocument.OwnedByUserId = UserId;
					}
				
					objDocument.IsPublished = checkIsPublished.Checked;
					objDocument.ModifiedByUserId = UserInfo.UserID;

					objDocument.Title = txtName.Text;
					objDocument.Description = txtDescription.Text;
					objDocument.ForceDownload = chkForceDownload.Checked;

					var oldUrl = objDocument.Url;
					objDocument.Url = ctlUrl.Url;
                    objDocument.LinkAttributes = textLinkAttributes.Text;
					
					if (lstOwner.Visible)
					{
						if (lstOwner.SelectedValue != string.Empty)
						{
							objDocument.OwnedByUserId = Convert.ToInt32 (lstOwner.SelectedValue);
						}
						else
						{
                            objDocument.OwnedByUserId = Null.NullInteger;
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
						if (Null.IsNull (itemId) || objDocument.CreatedDate != pickerCreatedDate.SelectedDate.Value)
							objDocument.CreatedDate = pickerCreatedDate.SelectedDate.Value;
						// else leave CreatedDate as is
					}
					else
					{
						objDocument.CreatedDate = now;
					}

					if (pickerLastModifiedDate.SelectedDate != null)
					{
                        if (!checkDontUpdateLastModifiedDate.Checked)
                        {
                            if (Null.IsNull (itemId) || objDocument.ModifiedDate != pickerLastModifiedDate.SelectedDate.Value)
                            {
    							objDocument.ModifiedDate = pickerLastModifiedDate.SelectedDate.Value;
                            }
                            else
                            {
    							// update ModifiedDate
    							objDocument.ModifiedDate = now;
                            }
                        }
					}
					else
					{
						objDocument.ModifiedDate = now;
					}

					#endregion

					if (Null.IsNull (itemId))
					{
                        DocumentsDataProvider.Instance.Add (objDocument);
					}
					else
					{
                        DocumentsDataProvider.Instance.Update (objDocument);
						if (objDocument.Url != oldUrl)
						{
							// delete old URL tracking data
                            DocumentsDataProvider.Instance.DeleteDocumentUrl (oldUrl, PortalId, ModuleId);
						}
					}

					// add or update URL tracking
					var ctrlUrl = new UrlController ();
					ctrlUrl.UpdateUrl (PortalId, ctlUrl.Url, ctlUrl.UrlType, ctlUrl.Log, ctlUrl.Track, ModuleId, ctlUrl.NewWindow);

                    Synchronize ();
					
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

		

		protected void lnkChange_Click (System.Object sender, System.EventArgs e)
		{
			lblOwner.Visible = false;
			lnkChange.Visible = false;
			lstOwner.Visible = true;

			PopulateOwnerList ();

			try
			{
				// Get existing document record
                var document = DocumentsDataProvider.Instance.GetDocument (itemId, ModuleId);

				try
				{
					if (document == null)
					{
						lstOwner.SelectedValue = UserId.ToString ();
					}
					else
					{
						lstOwner.SelectedValue = document.OwnedByUserId.ToString ();
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
            foreach (UserInfo superUsers in UserController.GetUsers (false, true, Null.NullInteger))
            {
                lstOwner.Items.Insert (0, new ListItem (
                    superUsers.DisplayName + " (" + superUsers.Username + ")", superUsers.UserID.ToString ())
                );
            }
		}
	}
}
