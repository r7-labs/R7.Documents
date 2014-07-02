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
using System.Linq;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using System.Collections;
using System.Collections.Generic;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using System.Web.UI.WebControls;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Utilities;

namespace R7.Documents
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// The Document Class provides the UI for displaying the Documents
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
	/// </history>
	/// -----------------------------------------------------------------------------
	public partial class ViewDocuments : DocumentsPortalModuleBase, IActionable
	{
		private const int NOT_READ = -2;
		
		// private ArrayList mobjDocumentList;
		private List<DocumentInfo> mobjDocumentList;
		private int mintTitleColumnIndex = NOT_READ;
		private int mintDownloadLinkColumnIndex = NOT_READ;

		private bool mblnReadComplete = false;

		#region "    Event Handlers    "

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Page_Load runs when the control is loaded
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
		/// </history>
		/// -----------------------------------------------------------------------------
		private void Page_Load (System.Object sender, System.EventArgs e)
		{
			try
			{
				//  mobjSettings = LoadSettings();
				grdDocuments.AllowSorting = DocumentsSettings.AllowUserSort;
				LoadColumns ();
				LoadData ();
				grdDocuments.DataSource = mobjDocumentList;
				grdDocuments.DataBind ();

				//Module failed to load
			}
			catch (Exception exc)
			{
				Exceptions.ProcessModuleLoadException (this, exc);
			}
		}

		/// <summary>
		/// Process user-initiated sort operation
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		/// <history>
		/// 	[msellers]	5/17/2007	 Added
		/// </history>
		public void grdDocuments_SortCommand (object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			ArrayList objCustomSortList = new ArrayList ();
			DocumentsSortColumnInfo objCustomSortColumn = new DocumentsSortColumnInfo ();
			DocumentsSortColumnInfo.SortDirection objCustomSortDirecton = DocumentsSortColumnInfo.SortDirection.Ascending;
			string strSortDirectionString = "ASC";


			// Set the sort column name
			objCustomSortColumn.ColumnName = e.SortExpression;

			// Determine if we need to reverse the sort.  This is needed if an existing sort on the same column existed that was desc
			if (ViewState ["CurrentSortOrder"] != null && ViewState ["CurrentSortOrder"].ToString () != string.Empty)
			{
				string existingSort = ViewState ["CurrentSortOrder"].ToString ();
				if (existingSort.StartsWith (e.SortExpression) && existingSort.EndsWith ("ASC"))
				{
					objCustomSortDirecton = DocumentsSortColumnInfo.SortDirection.Descending;
					strSortDirectionString = "DESC";
				}
			}

			// Set the sort
			objCustomSortColumn.Direction = objCustomSortDirecton;
			objCustomSortList.Add (objCustomSortColumn);

			var docComparer = new DocumentComparer (objCustomSortList);
			mobjDocumentList.Sort (docComparer.Compare);
			grdDocuments.DataSource = mobjDocumentList;
			grdDocuments.DataBind ();

			// Save the sort to viewstate
			ViewState ["CurrentSortOrder"] = e.SortExpression + " " + strSortDirectionString;

			// Mark as a user selected sort
			IsReadComplete = true;
		}

		/// <summary>
		/// If the datagrid was not sorted and bound via the "_Sort" method it will be bound at this time using
		/// default values
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		/// <history>
		///   Mitchel Sellers 6/4/2007  Added method
		/// </history>
		private void Page_PreRender (System.Object sender, System.EventArgs e)
		{
			// Only bind if not a user selected sort

			if (!IsReadComplete)
			{
				LoadData ();

				// Use DocumentComparer to do sort based on the default sort order (mobjSettings.SortOrder)
				var docComparer = new DocumentComparer (DocumentsSettings.GetSortColumnList (this.LocalResourceFile));
				mobjDocumentList.Sort (docComparer.Compare);

				//Bind the grid
				grdDocuments.DataSource = mobjDocumentList;
				grdDocuments.DataBind ();
			}

			// Localize the Data Grid
			// REVIEW: Original: Localization.LocalizeDataGrid(ref grdDocuments, this.LocalResourceFile);
			Localization.LocalizeDataGrid (ref grdDocuments, this.LocalResourceFile);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// grdDocuments_ItemCreated runs when an item in the grid is created
		/// </summary>
		/// <remarks>
		/// Set NavigateUrl for title, download links.  Also sets "scope" on 
		/// header rows so that text-to-speech readers can interpret the header row.
		/// </remarks>
		/// <history>
		/// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
		/// </history>
		/// -----------------------------------------------------------------------------
		private void grdDocuments_ItemCreated (object sender, DataGridItemEventArgs e)
		{
			int intCount = 0;
			DocumentInfo objDocument = null;

			try
			{
				switch (e.Item.ItemType)
				{
					case ListItemType.Header:
						// Setting "scope" to "col" indicates to for text-to-speech
						// or braille readers that this row containes headings
						for (intCount = 1; intCount <= e.Item.Cells.Count - 1; intCount++)
						{
							e.Item.Cells [intCount].Attributes.Add ("scope", "col");
						}


						break;
					case ListItemType.AlternatingItem:
					case ListItemType.Item:
					case ListItemType.SelectedItem:
						// If ShowTitleLink is true, the title column is generated dynamically
						// as a template, which we can't data-bind, so we need to set the text
						// value here
						objDocument = (DocumentInfo)mobjDocumentList [e.Item.ItemIndex];
						
						// decorate unpublished items
						if (!objDocument.IsPublished)
						{
							if (e.Item.ItemType == ListItemType.Item)
								e.Item.CssClass = grdDocuments.ItemStyle.CssClass + " _nonpublished";
							else if (e.Item.ItemType == ListItemType.AlternatingItem)
								e.Item.CssClass = grdDocuments.AlternatingItemStyle.CssClass + " _nonpublished";
							// no need to add class to
							// else  if (e.Item.ItemType == ListItemType.SelectedItem)
							// 	e.Item.CssClass = grdDocuments.SelectedItemStyle.CssClass + " _nonpublished";
						}

						if (DocumentsSettings.ShowTitleLink)
						{
							if (mintTitleColumnIndex == NOT_READ)
							{
								mintTitleColumnIndex = DocumentsSettings.FindGridColumn (DocumentsDisplayColumnInfo.COLUMN_TITLE, DocumentsSettings.DisplayColumnList, true);
							}

							if (mintTitleColumnIndex >= 0)
							{
								// Dynamically set the title link URL
								var _with1 = (HyperLink)e.Item.Controls [mintTitleColumnIndex + 1].FindControl ("ctlTitle");
								_with1.Text = objDocument.Title;
								// Note: The title link should display inline if possible, so set
								// ForceDownload=False
								_with1.NavigateUrl = DotNetNuke.Common.Globals.LinkClick (objDocument.Url, TabId, ModuleId, objDocument.TrackClicks, objDocument.ForceDownload);
								if (objDocument.NewWindow)
								{
									_with1.Target = "_blank";
								}
							}
						}

						// If there's a "download" link, set the NavigateUrl 
						if (mintDownloadLinkColumnIndex == NOT_READ)
						{
							mintDownloadLinkColumnIndex = DocumentsSettings.FindGridColumn (DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK, DocumentsSettings.DisplayColumnList, true);
						}
						if (mintDownloadLinkColumnIndex >= 0)
						{
							var _with2 = (HyperLink)e.Item.Controls [mintDownloadLinkColumnIndex].FindControl ("ctlDownloadLink");
							// Note: The title link should display open/save dialog if possible, 
							// so set ForceDownload=True
							_with2.NavigateUrl = DotNetNuke.Common.Globals.LinkClick (objDocument.Url, TabId, ModuleId, objDocument.TrackClicks, objDocument.ForceDownload);
							if (objDocument.NewWindow)
							{
								_with2.Target = "_blank";
							}
						}
						break;
				}

				//Module failed to load
			}
			catch (Exception exc)
			{
				Exceptions.ProcessModuleLoadException (this, exc);
			}
		}


		#endregion

		#region "    Optional Interfaces    "

		public ModuleActionCollection ModuleActions
		{
			get
			{
				ModuleActionCollection Actions = new ModuleActionCollection ();
				Actions.Add (GetNextActionID (), 
					Localization.GetString (ModuleActionType.AddContent, LocalResourceFile), 
					ModuleActionType.AddContent, "", "", EditUrl (), false, SecurityAccessLevel.Edit, true, false);
				return Actions;
			}
		}

		#endregion

		#region "    Web Form Designer Generated Code    "

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

			grdDocuments.SortCommand += grdDocuments_SortCommand;
			grdDocuments.ItemCreated += grdDocuments_ItemCreated;
		}

		#endregion

		#region "    Private Methods    "

		private void LoadColumns ()
		{
			DocumentsDisplayColumnInfo objDisplayColumn = null;

			// Add columns dynamically
			foreach (DocumentsDisplayColumnInfo objDisplayColumn_loopVariable in DocumentsSettings.DisplayColumnList)
			{
				objDisplayColumn = objDisplayColumn_loopVariable;

				if (objDisplayColumn.Visible)
				{
					switch (objDisplayColumn.ColumnName)
					{
						case DocumentsDisplayColumnInfo.COLUMN_CATEGORY:
							AddDocumentColumn (Localization.GetString ("Category", LocalResourceFile), "Category", "Category");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY:
							AddDocumentColumn (Localization.GetString ("CreatedBy", LocalResourceFile), "CreatedBy", "CreatedByUser", "");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE:
							AddDocumentColumn (Localization.GetString ("CreatedDate", LocalResourceFile), "CreatedDate", "CreatedDate", "{0:d}");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION:
							AddDocumentColumn (Localization.GetString ("Description", LocalResourceFile), "Description", "Description");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK:
							AddDownloadLink ("", "ctlDownloadLink");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY:
							AddDocumentColumn (Localization.GetString ("ModifiedBy", LocalResourceFile), "ModifiedBy", "ModifiedByUser");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE:
							AddDocumentColumn (Localization.GetString ("ModifiedDate", LocalResourceFile), "ModifiedDate", "ModifiedDate", "{0:d}");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY:
							AddDocumentColumn (Localization.GetString ("Owner", LocalResourceFile), "Owner", "OwnedByUser");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_SIZE:
							AddDocumentColumn (Localization.GetString ("Size", LocalResourceFile), "Size", "FormatSize");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_CLICKS:
							AddDocumentColumn (Localization.GetString ("Clicks", LocalResourceFile), "Clicks", "Clicks");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_ICON:
							AddDocumentColumn (Localization.GetString ("Icon", LocalResourceFile), "Icon", "FormatIcon");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_FILENAME:
							AddDocumentColumn (Localization.GetString ("FileName", LocalResourceFile), "FileName", "FileName");

							break;
						case DocumentsDisplayColumnInfo.COLUMN_TITLE:
							if (DocumentsSettings.ShowTitleLink)
							{
								AddDownloadLink (Localization.GetString ("Title", LocalResourceFile), "ctlTitle");
							}
							else
							{
								AddDocumentColumn (Localization.GetString ("Title", LocalResourceFile), "Title", "Title");
							}
							break;
					}
				}
			}
		}

		private void LoadData ()
		{
			string strCacheKey = null;
			
			if (IsReadComplete)
				return;

			// Only read from the cache if the users is not logged in
			strCacheKey = this.DataCacheKey + ";anon-doclist";
			if (!Request.IsAuthenticated)
			{
				mobjDocumentList = (List<DocumentInfo>)DataCache.GetCache (strCacheKey);
			}

			if (mobjDocumentList == null)
			{
				//	mobjDocumentList = (ArrayList) DocumentsController.GetObjects<DocumentInfo>(ModuleId); // PortalId!!!

				mobjDocumentList = DocumentsController.GetDocuments (ModuleId, PortalId).ToList ();

				// Check security on files
				int intCount = 0;
				DocumentInfo objDocument = null;

				for (intCount = mobjDocumentList.Count - 1; intCount >= 0; intCount += -1)
				{
					objDocument = (DocumentInfo)mobjDocumentList [intCount];
					if (objDocument.Url.ToLower ().IndexOf ("fileid=") >= 0)
					{
						// document is a file, check security
						var objFile = FileManager.Instance.GetFile (int.Parse (objDocument.Url.Split ('=') [1]));
					
						//if ((objFile != null) && !PortalSecurity.IsInRoles(FileSystemUtils.GetRoles(objFile.Folder, PortalSettings.PortalId, "READ"))) {
						if (objFile != null)
						{
							var folder = FolderManager.Instance.GetFolder (objFile.FolderId);
							if (!FolderPermissionController.CanViewFolder ((FolderInfo)folder))
							{
								// remove document from the list
								mobjDocumentList.Remove (objDocument);
								continue;
							}
						}
					}
					
					// remove unpublished documents from the list
					if (!objDocument.IsPublished && !IsEditable)
					{
						mobjDocumentList.Remove (objDocument);
						continue;
					}

					objDocument.OnLocalize += new LocalizeHandler (OnLocalize);
				}

				// Only write to the cache if the user is not logged in
				if (!Request.IsAuthenticated)
				{
					DataCache.SetCache (strCacheKey, mobjDocumentList, new TimeSpan (0, 5, 0));
				}
			}

			//Sort documents
			var docComparer = new DocumentComparer (DocumentsSettings.GetSortColumnList (this.LocalResourceFile));
			mobjDocumentList.Sort (docComparer.Compare);

			IsReadComplete = true;
		}

		private string OnLocalize (string text)
		{
			return Localization.GetString (text, this.LocalResourceFile);
		}

		private bool IsReadComplete
		{
			get { return mblnReadComplete; }
			set { mblnReadComplete = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Dynamically adds a column to the datagrid
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="Title">The name of the property to read data from</param>
		/// <param name="DataField">The name of the property to read data from</param>
		/// -----------------------------------------------------------------------------
		private void AddDocumentColumn (string Title, string CssClass, string DataField)
		{
			AddDocumentColumn (Title, CssClass, DataField, "");
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Dynamically adds a column to the datagrid
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="Title">The name of the property to read data from</param>
		/// <param name="DataField">The name of the property to read data from</param>
		/// <param name="Format">Format string for value</param>
		/// -----------------------------------------------------------------------------
		private void AddDocumentColumn (string Title, string CssClass, string DataField, string Format)
		{
			System.Web.UI.WebControls.BoundColumn objBoundColumn = default(System.Web.UI.WebControls.BoundColumn);

			objBoundColumn = new System.Web.UI.WebControls.BoundColumn ();

			objBoundColumn.DataField = DataField;
			objBoundColumn.DataFormatString = Format;
			objBoundColumn.HeaderText = Title;
			//Added 5/17/2007
			//By Mitchel Sellers
			if (DocumentsSettings.AllowUserSort)
			{
				objBoundColumn.SortExpression = DataField;
			}

			objBoundColumn.HeaderStyle.CssClass = CssClass + "Header";
			//"NormalBold"
			objBoundColumn.ItemStyle.CssClass = CssClass + "Cell";
			//"Normal"

			this.grdDocuments.Columns.Add (objBoundColumn);

		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Dynamically adds a DownloadColumnTemplate column to the datagrid.  Used to
		/// add the download link and title (if "title as link" is set) columns.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="Title">The name of the property to read data from</param>
		/// <param name="Name">The name of the property to read data from</param>
		/// -----------------------------------------------------------------------------
		private void AddDownloadLink (string Title, string Name)
		{
			System.Web.UI.WebControls.TemplateColumn objTemplateColumn = default(System.Web.UI.WebControls.TemplateColumn);
			string strCellPrefix = null;

			objTemplateColumn = new System.Web.UI.WebControls.TemplateColumn ();
			objTemplateColumn.ItemTemplate = new DownloadColumnTemplate (Name, Localization.GetString ("DownloadLink.Text", LocalResourceFile), ListItemType.Item);
			objTemplateColumn.HeaderText = Title;

			strCellPrefix = "Title";
			if (strCellPrefix == string.Empty && Name == "ctlDownloadLink")
			{
				strCellPrefix = "Download";
			}

			objTemplateColumn.HeaderStyle.CssClass = strCellPrefix + "Header";
			//"NormalBold"
			objTemplateColumn.ItemStyle.CssClass = strCellPrefix + "Cell";
			//"Normal"

			//Added 5/17/2007
			//By Mitchel Sellers
			// Add the sort expression, however ensure that it is NOT added for download
			if (DocumentsSettings.AllowUserSort && !Name.Equals ("ctlDownloadLink"))
			{
				objTemplateColumn.SortExpression = Title;
			}
			this.grdDocuments.Columns.Add (objTemplateColumn);
		}

		/*
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Load module settings from the database.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// -----------------------------------------------------------------------------
		private DocumentsSettingsInfo LoadSettings()
		{
			DocumentsSettingsInfo objDocumentsSettings = null;
			// Load module instance settings
			var _with3 = new DocumentsController();
			objDocumentsSettings = _with3.GetDocumentsSettings(ModuleId);

			// first time around, no existing documents settings will exist
			if (objDocumentsSettings == null) {
				objDocumentsSettings = new DocumentsSettingsInfo();
			}

			return objDocumentsSettings;
		}
*/
		public ViewDocuments ()
		{
			Init += Page_Init;
			PreRender += Page_PreRender;
			Load += Page_Load;
		}

		#endregion
	}
}
