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
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using R7.Documents.Data;
using R7.Documents.Models;
using R7.Documents.ViewModels;
using R7.DotNetNuke.Extensions.ModuleExtensions;
using R7.DotNetNuke.Extensions.Modules;
using R7.DotNetNuke.Extensions.ViewModels;

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

        int titleColumnIndex = NOT_READ;
		
        int downloadLinkColumnIndex = NOT_READ;

        List<DocumentViewModel> _documents;
        protected List<DocumentViewModel> Documents {
            get { return _documents ?? (_documents = LoadDocuments ()); }
        }

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
                if (Documents.Count == 0) {
                    if (IsEditable) {
                        this.Message ("NothingToDisplay.Text", MessageType.Info, true);
                    }
                    else {
                        ContainerControl.Visible = false;
                    }
                }
                else {
                    LoadColumns ();
                    grdDocuments.DataSource = Documents;
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
            Documents.Sort (docComparer.Compare);
            grdDocuments.DataSource = Documents;
            grdDocuments.DataBind ();

            // Save the sort to viewstate
            ViewState ["CurrentSortOrder"] = e.SortExpression + " " + strSortDirectionString;
        }

        /// <summary>
        /// If the datagrid was not sorted and bound via the "_Sort" method it will be bound at this time using
        /// default values
        /// </summary>
        protected override void OnPreRender (EventArgs e)
        {
            base.OnPreRender (e);
			
            // only bind if not a user selected sort
            if (_documents == null) {
                // use DocumentComparer to do sort based on the default sort order
                var docComparer = new DocumentComparer (Settings.GetSortColumnList (LocalResourceFile));
                Documents.Sort (docComparer.Compare);

                grdDocuments.DataSource = Documents;
                grdDocuments.DataBind ();
            }

            Localization.LocalizeGridView (ref grdDocuments, LocalResourceFile);
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
                        e.Row.TableSection = TableRowSection.TableHeader;
                        e.Row.Cells [0].CssClass = "EditHeader";
                        break;

                    case DataControlRowType.DataRow:
                    	// if ShowTitleLink is true, the title column is generated dynamically
						// as a template, which we can't data-bind, so we need to set the text
						// value here
                        var document = Documents [e.Row.RowIndex];
                        
						// set CSS class for edit column cells
                        e.Row.Cells [0].CssClass = "EditCell";

						// decorate unpublished items
                        if (!document.IsPublished (HttpContext.Current.Timestamp)) {
                            e.Row.CssClass = ((e.Row.RowIndex % 2 == 0) ? grdDocuments.RowStyle.CssClass
                                : grdDocuments.AlternatingRowStyle.CssClass) + " _nonpublished";
                        }

                        if (Settings.ShowTitleLink) {
                            if (titleColumnIndex == NOT_READ) {
                                titleColumnIndex = DocumentsSettings.FindGridColumn (
                                    DocumentsDisplayColumnInfo.COLUMN_TITLE,
                                    Settings.GetDisplayColumnList (LocalResourceFile), true);
                            }

                            if (titleColumnIndex >= 0) {
                                // dynamically set the title link URL
                                var linkTitle = (HyperLink) e.Row.Controls [titleColumnIndex + 1].FindControl ("ctlTitle");
                                linkTitle.Text = document.Title;
								
                                // set link title to display document description
                                linkTitle.ToolTip = document.Description;

                                SetupHyperLink (linkTitle, document);
                                SetHyperLinkAttributes (linkTitle, document);
                            }
                        }

						// if there's a "download" link, set the NavigateUrl 
                        if (downloadLinkColumnIndex == NOT_READ) {
                            downloadLinkColumnIndex = DocumentsSettings.FindGridColumn (
                                DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK,
                                Settings.GetDisplayColumnList (LocalResourceFile), true);
                        }
                        if (downloadLinkColumnIndex >= 0) {
                            var linkDownload = (HyperLink) e.Row.Controls [downloadLinkColumnIndex].FindControl ("ctlDownloadLink");
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

        void SetupHyperLink (HyperLink link, IDocument document)
        {
            link.NavigateUrl = Globals.LinkClick (document.Url, TabId, ModuleId,
                                                  document.TrackClicks, document.ForceDownload);
            if (document.NewWindow) {
                link.Target = "_blank";
            }
        }

        void SetHyperLinkAttributes (HyperLink link, IDocument document)
        {
            foreach (var htmlAttr in document.GetLinkAttributesCollection ()) {
                link.Attributes.Add (htmlAttr.Item1, htmlAttr.Item2);
            }
        }

        void LoadColumns ()
        {
            var dateTimeFormat = Settings.GetDateTimeFormat ();

            // add columns dynamically
            foreach (var column in Settings.GetDisplayColumnList (LocalResourceFile)) {
                if (column.Visible) {
                    switch (column.ColumnName) {
                        case DocumentsDisplayColumnInfo.COLUMN_CATEGORY:
                        case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION:
                        case DocumentsDisplayColumnInfo.COLUMN_CLICKS:
                            AddDocumentColumn (LocalizeString (column.ColumnName + ".Column"), column.ColumnName, column.ColumnName);
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY:
                        case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY:
                            AddDocumentColumn (LocalizeString (column.ColumnName + ".Column"), column.ColumnName, column.ColumnName + "User");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE:
                        case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE:
                        case DocumentsDisplayColumnInfo.COLUMN_PUBLISHEDONDATE:
                            AddDocumentColumn (LocalizeString (column.ColumnName + ".Column"), column.ColumnName, column.ColumnName, dateTimeFormat);
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK:
                            AddDownloadLink ("DownloadLink.Column", "DownloadLink", "DownloadLink", "ctlDownloadLink");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY:
                            AddDocumentColumn (LocalizeString ("Owner.Column"), "Owner", "OwnedByUser");
                            break;

                        case DocumentsDisplayColumnInfo.COLUMN_SIZE:
                            AddDocumentColumn (LocalizeString ("Size.Column"), "Size", "FormatSize");
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

        List<DocumentViewModel> LoadDocuments ()
        {
            var isAdmin = UserInfo.IsSuperUser || UserInfo.IsInRole ("Administrators");
            var cacheKey = ModuleSynchronizer.GetDataCacheKey (ModuleId, TabModuleId);
            var documents = DataCache.GetCachedData<IEnumerable<DocumentViewModel>> (
                new CacheItemArgs (cacheKey, 1200, CacheItemPriority.Normal),
                c => LoadDocuments_Internal ()
            );

            // remove unpublished and inaccessible documents from the list
            // TODO: Add test to check filtering logic
            var filteredDocuments = documents
                .Where (d =>
                        (IsEditable || d.IsPublished (HttpContext.Current.Timestamp)) &&
                        (isAdmin || CanView (d.Url)))
                .ToList ();

            // sort documents
            var docComparer = new DocumentComparer (Settings.GetSortColumnList (LocalResourceFile));
            filteredDocuments.Sort (docComparer.Compare);

            return filteredDocuments;
        }

        IEnumerable<DocumentViewModel> LoadDocuments_Internal ()
        {
            var viewModelContext = new ViewModelContext<DocumentsSettings> (this, Settings);
            return DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId)
                                        .Select (d => new DocumentViewModel (d, viewModelContext));
        }

        bool CanView (string url)
        {
            switch (Globals.GetURLType (url)) {
                case TabType.File:
                    if (url.IndexOf ("fileid=", StringComparison.InvariantCultureIgnoreCase) >= 0) {
                        var file = FileManager.Instance.GetFile (int.Parse (url.Split ('=') [1]));
                        if (file != null) {
                            var folder = FolderManager.Instance.GetFolder (file.FolderId);
                            if (folder != null && FolderPermissionController.CanViewFolder ((FolderInfo) folder)) {
                                return true;
                            }
                        }
                    }
                break;

            case TabType.Tab:
                var tab = TabController.Instance.GetTab (int.Parse (url), PortalId);
                if (tab != null && TabPermissionController.CanViewPage (tab)) {
                    return true;
                }
                break;
            }

            return false;
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
    }
}
