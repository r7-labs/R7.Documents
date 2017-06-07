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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using R7.Documents.Data;
using R7.Documents.Models;
using R7.DotNetNuke.Extensions.ModuleExtensions;
using R7.DotNetNuke.Extensions.Modules;

namespace R7.Documents
{
    /// <summary>
    /// Provides the UI for displaying the documents
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
    /// </history>
    public partial class ViewDocuments : PortalModuleBase<DocumentsSettings>, IActionable
    {
        const int NOT_READ = -2;

        List<DocumentInfo> documentList;
		
        int mintTitleColumnIndex = NOT_READ;
		
        int mintDownloadLinkColumnIndex = NOT_READ;

        bool readComplete;

        #region Event Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            grdDocuments.AllowSorting = Settings.AllowUserSort;

            var gridStyle = GridStyle.Styles [Settings.GridStyle];
            gridStyle.ApplyToGrid (grdDocuments);
        }

        /// <summary>
        /// OnLoad runs when the control is loaded
        /// </summary>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            try {
                if (!readComplete) {
                    documentList = LoadData ();
                }

                var now = HttpContext.Current.Timestamp;
                if (IsEditable && documentList.Count == 0) {
                    this.Message ("NothingToDisplay.Text", MessageType.Info, true);
                }
                else if (!IsEditable && documentList.Count (d => d.IsPublished (now)) == 0) {
                    ContainerControl.Visible = false;
                }
                else {
                    LoadColumns ();
                    grdDocuments.DataSource = documentList;
                    grdDocuments.DataBind ();
                }
            }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        /// <summary>
        /// Process user-initiated sort operation
        /// </summary>
        /// <history>
        /// 	[msellers]	5/17/2007	 Added
        /// </history>
        protected void grdDocuments_Sorting (object sender, GridViewSortEventArgs e)
        {
            var objCustomSortList = new ArrayList ();
            var objCustomSortColumn = new DocumentsSortColumnInfo ();
            var objCustomSortDirecton = DocumentsSortColumnInfo.SortDirection.Ascending;
            var strSortDirectionString = "ASC";

            // Set the sort column name
            objCustomSortColumn.ColumnName = e.SortExpression;

            // Determine if we need to reverse the sort.  This is needed if an existing sort on the same column existed that was desc
            if (ViewState ["CurrentSortOrder"] != null && ViewState ["CurrentSortOrder"].ToString () != string.Empty) {
                var existingSort = ViewState ["CurrentSortOrder"].ToString ();
                if (existingSort.StartsWith (e.SortExpression, StringComparison.InvariantCulture) 
                    && existingSort.EndsWith ("ASC", StringComparison.InvariantCulture)) {
                    objCustomSortDirecton = DocumentsSortColumnInfo.SortDirection.Descending;
                    strSortDirectionString = "DESC";
                }
            }

            // Set the sort
            objCustomSortColumn.Direction = objCustomSortDirecton;
            objCustomSortList.Add (objCustomSortColumn);

            var docComparer = new DocumentComparer (objCustomSortList);
            documentList.Sort (docComparer.Compare);
            grdDocuments.DataSource = documentList;
            grdDocuments.DataBind ();

            // Save the sort to viewstate
            ViewState ["CurrentSortOrder"] = e.SortExpression + " " + strSortDirectionString;

            // Mark as a user selected sort
            readComplete = true;
        }

        /// <summary>
        /// If the datagrid was not sorted and bound via the "_Sort" method it will be bound at this time using
        /// default values
        /// </summary>
        protected override void OnPreRender (EventArgs e)
        {
            base.OnPreRender (e);
			
            // only bind if not a user selected sort
            if (!readComplete) {
                
                documentList = LoadData ();

                // use DocumentComparer to do sort based on the default sort order (mobjSettings.SortOrder)
                var docComparer = new DocumentComparer (Settings.GetSortColumnList (LocalResourceFile));
                documentList.Sort (docComparer.Compare);

                grdDocuments.DataSource = documentList;
                grdDocuments.DataBind ();
            }

            Localization.LocalizeGridView (ref grdDocuments, LocalResourceFile);
        }

        DocumentInfoFormatter _documentFormatter;
        DocumentInfoFormatter DocumentFormatter {
            get { return _documentFormatter ?? (_documentFormatter = new DocumentInfoFormatter ()); }
        }

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
        protected void grdDocuments_RowCreated (object sender, GridViewRowEventArgs e)
        {
            try {
                e.Row.Cells [0].Visible = IsEditable;  

                switch (e.Row.RowType) {
                    case DataControlRowType.Header:
						// set CSS class for edit column header
                        e.Row.Cells [0].CssClass = "EditHeader";

                        // TODO: Doesn't UseAccessibleHeader=true does same thing?
						// setting "scope" to "col" indicates to for text-to-speech
						// or braille readers that this row containes headings
                        for (var count = 1; count <= e.Row.Cells.Count - 1; count++) {
                            e.Row.Cells [count].Attributes.Add ("scope", "col");
                        }
                        break;

                    case DataControlRowType.DataRow:
                    	// if ShowTitleLink is true, the title column is generated dynamically
						// as a template, which we can't data-bind, so we need to set the text
						// value here
                        var document = documentList [e.Row.RowIndex];
                        
						// set CSS class for edit column cells
                        e.Row.Cells [0].CssClass = "EditCell";

						// decorate unpublished items
                        if (!document.IsPublished (HttpContext.Current.Timestamp)) {
                            e.Row.CssClass = ((e.Row.RowIndex % 2 == 0) ? grdDocuments.RowStyle.CssClass
                                : grdDocuments.AlternatingRowStyle.CssClass) + " _nonpublished";
                        }

                        if (Settings.ShowTitleLink) {
                            if (mintTitleColumnIndex == NOT_READ) {
                                mintTitleColumnIndex = DocumentsSettings.FindGridColumn (
                                    DocumentsDisplayColumnInfo.COLUMN_TITLE,
                                    Settings.GetDisplayColumnList (LocalResourceFile), true);
                            }

                            if (mintTitleColumnIndex >= 0) {
                                // dynamically set the title link URL
                                var linkTitle = (HyperLink) e.Row.Controls [mintTitleColumnIndex + 1].FindControl ("ctlTitle");
                                linkTitle.Text = document.Title;
								
                                // set link title to display document description
                                linkTitle.ToolTip = document.Description;

                                SetupHyperLink (linkTitle, document);
                                
                                // set HTML attributes for the link
                                foreach (var htmlAttr in DocumentFormatter.GetLinkAttributesCollection (document)) {
                                    linkTitle.Attributes.Add (htmlAttr.Item1, htmlAttr.Item2);
                                }
                            }
                        }

						// if there's a "download" link, set the NavigateUrl 
                        if (mintDownloadLinkColumnIndex == NOT_READ) {
                            mintDownloadLinkColumnIndex = DocumentsSettings.FindGridColumn (
                                DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK,
                                Settings.GetDisplayColumnList (LocalResourceFile), true);
                        }
                        if (mintDownloadLinkColumnIndex >= 0) {
                            var linkDownload = (HyperLink) e.Row.Controls [mintDownloadLinkColumnIndex].FindControl ("ctlDownloadLink");
                            SetupHyperLink (linkDownload, document);

                            // display clicks in the tooltip
                            if (document.Clicks >= 0) {
                                linkDownload.ToolTip = string.Format (LocalizeString ("Clicks.Format"), document.Clicks);
                            }
                        }
                        break;
                }
            }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        #endregion

        void SetupHyperLink (HyperLink link, DocumentInfo document)
        {
            link.NavigateUrl = Globals.LinkClick (document.Url, TabId, ModuleId,
                                                  document.TrackClicks, document.ForceDownload);
            if (document.NewWindow) {
                link.Target = "_blank";
            }
        }

        #region IActionable implementation

        public ModuleActionCollection ModuleActions
        {
            get {
                var actions = new ModuleActionCollection ();
				
                actions.Add (
                    GetNextActionID (), 
                    Localization.GetString (ModuleActionType.AddContent, LocalResourceFile), 
                    ModuleActionType.AddContent,
                    "",
                    IconController.IconURL ("Add"),
                    EditUrl (),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);
                
                actions.Add (
                    GetNextActionID (), 
                    Localization.GetString ("ImportDocuments.Action", LocalResourceFile),
                    "ImportDocuments.Action",
                    "",
                    IconController.IconURL ("Rt"),
                    EditUrl ("ImportDocuments"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);

                actions.Add (
                    GetNextActionID (), 
                    Localization.GetString ("ChangeFolder.Action", LocalResourceFile),
                    "ChangeFolder.Action",
                    "",
                    IconController.IconURL ("FileMove", "16X16", "Gray"),
                    EditUrl ("ChangeFolder"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);
			
                return actions;
            }
        }

        #endregion

        #region Private Methods

        void LoadColumns ()
        {
            DocumentsDisplayColumnInfo objDisplayColumn = null;

            // add columns dynamically
            foreach (var objDisplayColumn_loopVariable in Settings.GetDisplayColumnList (LocalResourceFile)) {
                objDisplayColumn = objDisplayColumn_loopVariable;
                var dateTimeFormat = Settings.GetDateTimeFormat ();

                if (objDisplayColumn.Visible) {
                    switch (objDisplayColumn.ColumnName) {
                        case DocumentsDisplayColumnInfo.COLUMN_CATEGORY:
                            AddDocumentColumn (Localization.GetString ("Category.Column", LocalResourceFile), "Category", "Category");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY:
                            AddDocumentColumn (Localization.GetString ("CreatedBy.Column", LocalResourceFile), "CreatedBy", "CreatedByUser", "");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE:
                            AddDocumentColumn (
                                Localization.GetString ("CreatedDate.Column", LocalResourceFile),
                                "CreatedDate",
                                "CreatedDate",
                                dateTimeFormat);
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_PUBLISHEDONDATE:
                            AddDocumentColumn (
								Localization.GetString ("PublishedOnDate.Column", LocalResourceFile),
                                "PublishedOnDate",
                                "PublishedOnDate",
                                dateTimeFormat);
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION:
                            AddDocumentColumn (Localization.GetString ("Description.Column", LocalResourceFile), "Description", "Description");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK:
                            AddDownloadLink ("DownloadLink.Column", "DownloadLink", "DownloadLink", "ctlDownloadLink");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY:
                            AddDocumentColumn (Localization.GetString ("ModifiedBy.Column", LocalResourceFile), "ModifiedBy", "ModifiedByUser");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE:
                            AddDocumentColumn (
                                Localization.GetString ("ModifiedDate.Column", LocalResourceFile),
                                "ModifiedDate",
                                "ModifiedDate",
                                dateTimeFormat);
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY:
                            AddDocumentColumn (Localization.GetString ("Owner.Column", LocalResourceFile), "Owner", "OwnedByUser");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_SIZE:
                            AddDocumentColumn (Localization.GetString ("Size.Column", LocalResourceFile), "Size", "FormatSize");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_CLICKS:
                            AddDocumentColumn (Localization.GetString ("Clicks.Column", LocalResourceFile), "Clicks", "Clicks");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_ICON:
                            AddDocumentColumn (Localization.GetString ("Icon.Column", LocalResourceFile), "Icon", "FormatIcon");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_TITLE:
                            if (Settings.ShowTitleLink) {
                                AddDownloadLink (Localization.GetString ("Title.Column", LocalResourceFile), "Title", "Title", "ctlTitle");
                            }
                            else {
                                AddDocumentColumn (Localization.GetString ("Title.Column", LocalResourceFile), "Title", "Title");
                            }
                            break;
                    }
                }
            }
        }

        List<DocumentInfo> LoadData ()
        {
            List<DocumentInfo> documents = null;

            // only read from the cache if the users is not logged in
            if (!Request.IsAuthenticated) {
                var cacheKey = ModuleSynchronizer.GetDataCacheKey (ModuleId, TabModuleId);
                documents = DataCache.GetCachedData<List<DocumentInfo>> (
                    new CacheItemArgs (cacheKey, 1200, CacheItemPriority.Normal),
                    c => LoadData_Internal (false, false)
                );
            }
            else {
                documents = LoadData_Internal (IsEditable, UserInfo.IsSuperUser || UserInfo.IsInRole ("Administrators"));
            }

            // sort documents
            var docComparer = new DocumentComparer (Settings.GetSortColumnList (LocalResourceFile));
            documents.Sort (docComparer.Compare);

            // TODO: Move outside method or implement as 'out' argument
            readComplete = true;

            return documents;
        }

        List<DocumentInfo> LoadData_Internal (bool isEditable, bool userIsAdmin)
        {
            var documents = DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId).ToList ();

            DocumentInfo objDocument = null;

            for (var intCount = documents.Count - 1; intCount >= 0; intCount--) {
                objDocument = documents [intCount];

                // no need to check security for superusers and admins
                if (!userIsAdmin) {
                    if (objDocument.Url.IndexOf ("fileid=", StringComparison.InvariantCultureIgnoreCase) >= 0) {
                        // document is a file, check security
                        var objFile = FileManager.Instance.GetFile (int.Parse (objDocument.Url.Split ('=') [1]));

                        if (objFile != null) {
                            var folder = FolderManager.Instance.GetFolder (objFile.FolderId);
                            if (folder != null && !FolderPermissionController.CanViewFolder ((FolderInfo)folder)) {
                                documents.Remove (objDocument);
                                continue;
                            }
                        }
                    }
                }

                // remove unpublished documents from the list
                if (!isEditable && !objDocument.IsPublished (HttpContext.Current.Timestamp)) {
                    documents.Remove (objDocument);
                    continue;
                }

                objDocument.OnLocalize += OnLocalize;
            }

            return documents;
        }

        string OnLocalize (string text)
        {
            return Localization.GetString (text, LocalResourceFile);
        }

        /// <summary>
        /// Dynamically adds a column to the datagrid
        /// </summary>
        /// <param name="title">The name of the property to read data from</param>
        /// <param name="cssClass"></param>
        /// <param name="dataField">The name of the property to read data from</param>
        void AddDocumentColumn (string title, string cssClass, string dataField)
        {
            AddDocumentColumn (title, cssClass, dataField, "");
        }

        /// <summary>
        /// Dynamically adds a column to the datagrid
        /// </summary>
        /// <param name="title">The name of the property to read data from</param>
        /// <param name="cssClass"></param>
        /// <param name="dataField">The name of the property to read data from</param>
        /// <param name="format">Format string for value</param>
        void AddDocumentColumn (string title, string cssClass, string dataField, string format)
        {
            var objBoundColumn = new BoundField ();

            // don't HTML encode icons markup
            if (dataField == "FormatIcon") {
                objBoundColumn.HtmlEncode = false;
            }

            objBoundColumn.DataField = dataField;
            objBoundColumn.DataFormatString = "{0:" + format + "}";
            objBoundColumn.HeaderText = title;
		
            // added 5/17/2007 by Mitchel Sellers
            if (Settings.AllowUserSort) {
                objBoundColumn.SortExpression = dataField;
            }

            objBoundColumn.HeaderStyle.CssClass = cssClass + "Header";
            objBoundColumn.ItemStyle.CssClass = cssClass + "Cell";

            grdDocuments.Columns.Add (objBoundColumn);
        }

        /// <summary>
        /// Dynamically adds a DownloadColumnTemplate column to the datagrid.  Used to
        /// add the download link and title (if "title as link" is set) columns.
        /// </summary>
        /// <param name="title">The name of the property to read data from</param>
        /// <param name="cssClass"></param>
        /// <param name="dataField"></param>
        /// <param name="name">The name of the property to read data from</param>
        void AddDownloadLink (string title, string cssClass, string dataField, string name)
        {
            var objTemplateColumn = new TemplateField ();

            objTemplateColumn.ItemTemplate = new DownloadColumnTemplate (
                name, Localization.GetString ("DownloadLink.Text", LocalResourceFile), ListItemType.Item
            );

            objTemplateColumn.HeaderText = (name == "ctlDownloadLink") ? string.Empty : title;
            objTemplateColumn.HeaderStyle.CssClass = cssClass + "Header";
            objTemplateColumn.ItemStyle.CssClass = cssClass + "Cell";
			
            // added 5/17/2007 by Mitchel Sellers
            // add the sort expression, however ensure that it is NOT added for download
            if (Settings.AllowUserSort && !name.Equals ("ctlDownloadLink")) {
                objTemplateColumn.SortExpression = dataField;
            }
			
            grdDocuments.Columns.Add (objTemplateColumn);
        }

        #endregion
    }
}
