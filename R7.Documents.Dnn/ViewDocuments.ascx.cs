﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.HtmlControls;
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
using R7.Dnn.Extensions.Collections;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.Documents.Components;
using R7.Documents.Data;
using R7.Documents.Commands;
using R7.Documents.Models;
using R7.Documents.ViewModels;
using R7.University.Components;

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

            var gridStyle = DocumentsConfig.Instance.GridStyles.First (gs => gs.Name == Settings.GridStyle);
            var gridStyleApplicator = new GridStyleApplicator ();
            gridStyleApplicator.Apply (grdDocuments, gridStyle);

            AddActionHandler (new ActionEventHandler (DocumentsActionEventHandler));
        }

        void DocumentsActionEventHandler (object sender, ActionEventArgs e)
        {
            try {
                var documentIds = hiddenSelectedDocuments.Value.Substring (1, hiddenSelectedDocuments.Value.Length - 2)
                                                         .Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select (d => int.Parse (d));

                if (documentIds.IsNullOrEmpty ()) {
                    return;
                }

                var bulkCommands = new DocumentBulkCommands ();
                switch (e.Action.CommandName) {
                    case "DuplicateDocuments.Action":
                        bulkCommands.Duplicate (documentIds, ModuleId, LocalizeString ("CopySuffix.Text"));
                        break;
                    case "DeleteDocuments.Action":
                        bulkCommands.Delete (documentIds, PortalId, ModuleId);
                        break;
                    case "DeleteDocumentsWithFiles.Action":
                        bulkCommands.DeleteWithFile (documentIds, PortalId, ModuleId);
                        break;
                    case "PublishDocuments.Action":
                        bulkCommands.Publish (documentIds, ModuleId);
                        break;
                    case "UnPublishDocuments.Action":
                        bulkCommands.UnPublish (documentIds, ModuleId);
                        break;
                }

                ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
                Response.Redirect (Globals.NavigateURL (), true);

            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
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
                    BindColumns ();
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
            var objCustomSortDirecton = Models.SortDirection.Ascending;
            var strSortDirectionString = "ASC";

            // Set the sort column name
            objCustomSortColumn.ColumnName = e.SortExpression;

            // Determine if we need to reverse the sort.  This is needed if an existing sort on the same column existed that was desc
            if (ViewState ["CurrentSortOrder"] != null && ViewState ["CurrentSortOrder"].ToString () != string.Empty) {
                var existingSort = ViewState ["CurrentSortOrder"].ToString ();
                if (existingSort.StartsWith (e.SortExpression, StringComparison.InvariantCulture)
                    && existingSort.EndsWith ("ASC", StringComparison.InvariantCulture)) {
                    objCustomSortDirecton = Models.SortDirection.Descending;
                    strSortDirectionString = "DESC";
                }
            }

            // Set the sort
            objCustomSortColumn.Direction = objCustomSortDirecton;
            objCustomSortList.Add (objCustomSortColumn);

            var docComparer = new DocumentViewModelComparer (objCustomSortList);
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
                var docComparer = new DocumentViewModelComparer (Settings.GetSortColumnList ());
                Documents.Sort (docComparer.Compare);

                grdDocuments.DataSource = Documents;
                grdDocuments.DataBind ();
            }

            Localization.LocalizeGridView (ref grdDocuments, LocalResourceFile);
        }

        HtmlInputCheckBox CreateSelectUnselectAllDocumentsCheckBox ()
        {
            var allCheckBox = new HtmlInputCheckBox ();
            allCheckBox.Checked = false;
            allCheckBox.Attributes.Add ("title", LocalizeString ("SelectUnselectAllDocuments.Text"));
            allCheckBox.Attributes.Add ("onchange", "r7d_selectDeselectAll(this)");

            return allCheckBox;
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
                e.Row.Cells [0].Visible = IsEditable && !Settings.FolderMode;

                switch (e.Row.RowType) {
                    case DataControlRowType.Header:
                        e.Row.TableSection = TableRowSection.TableHeader;
                        e.Row.Cells [0].CssClass = "EditHeader";
                        e.Row.Cells [0].Controls.Add (CreateSelectUnselectAllDocumentsCheckBox ());
                        break;

                    case DataControlRowType.DataRow:
                    	// if ShowTitleLink is true, the title column is generated dynamically
						// as a template, which we can't data-bind, so we need to set the text
						// value here
                        var document = Documents [e.Row.RowIndex];

						// set CSS class for edit column cells
                        e.Row.Cells [0].CssClass = "EditCell";

                        e.Row.CssClass = GetRowCssClass (document, e.Row.RowIndex);

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

        string GetRowCssClass (IDocument document, int rowIndex)
        {
            var rowCssClass = (rowIndex % 2 == 0)
                ? grdDocuments.RowStyle.CssClass
                : grdDocuments.AlternatingRowStyle.CssClass;

            if (document.IsFeatured) {
                rowCssClass += " r7docs-featured";
            }

            if (!document.IsPublished (HttpContext.Current.Timestamp)) {
                rowCssClass += " _nonpublished";
            }

            return rowCssClass;
        }

        #endregion

        #region IActionable implementation

        public ModuleActionCollection ModuleActions
        {
            get {
                var actions = new ModuleActionCollection ();

                actions.Add (
                    GetNextActionID (),
                    LocalizeString (ModuleActionType.AddContent),
                    ModuleActionType.AddContent,
                    "",
                    IconController.IconURL ("Add"),
                    EditUrl (),
                    false,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("ImportDocuments.Action"),
                    "ImportDocuments.Action",
                    "",
                    IconController.IconURL ("Rt"),
                    EditUrl ("ImportDocuments"),
                    false,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("ChangeFolder.Action"),
                    "ChangeFolder.Action",
                    "",
                    IconController.IconURL ("FileMove", "16X16", "Gray"),
                    EditUrl ("ChangeFolder"),
                    false,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("DuplicateDocuments.Action"),
                    "DuplicateDocuments.Action",
                    "",
                    IconController.IconURL ("FileCopy", "16X16", "Gray"),
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("PublishDocuments.Action"),
                    "PublishDocuments.Action",
                    "",
                    IconController.IconURL ("Cog", "16X16", "Gray"),
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("UnPublishDocuments.Action"),
                    "UnPublishDocuments.Action",
                    "",
                    IconController.IconURL ("Cog", "16X16", "Gray"),
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("DeleteDocuments.Action"),
                    "DeleteDocuments.Action",
                    "",
                    IconController.IconURL ("Delete"),
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
                    false);

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("DeleteDocumentsWithFiles.Action"),
                    "DeleteDocumentsWithFiles.Action",
                    "",
                    IconController.IconURL ("Delete", "16X16", "Standard_2"),
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    !Settings.FolderMode,
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

        void BindColumns ()
        {
            var dateTimeFormat = Settings.GetDateTimeFormat ();

            // add columns dynamically
            foreach (var column in Settings.GetDisplayColumnList (LocalResourceFile)) {
                if (!column.Visible) {
                    continue;
                }

                switch (column.ColumnName) {
                    case DocumentsDisplayColumnInfo.COLUMN_CATEGORY:
                    case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION:
                    case DocumentsDisplayColumnInfo.COLUMN_CLICKS:
                        AddDocumentColumn (LocalizeString (column.ColumnName + ".Column"), column.ColumnName,
                            column.ColumnName);
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY:
                    case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY:
                        AddDocumentColumn (LocalizeString (column.ColumnName + ".Column"), column.ColumnName,
                            column.ColumnName + "User");
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE:
                    case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE:
                    case DocumentsDisplayColumnInfo.COLUMN_PUBLISHEDONDATE:
                        AddDocumentColumn (LocalizeString (column.ColumnName + ".Column"), column.ColumnName,
                            column.ColumnName, true, dateTimeFormat);
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK:
                        AddDownloadLink ("DownloadLink.Column", "DownloadLink", "DownloadLink", "ctlDownloadLink");
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY:
                        AddDocumentColumn (LocalizeString ("Owner.Column"), "Owner",
                            nameof (DocumentViewModel.OwnedByUser));
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_SIZE:
                        AddDocumentColumn (LocalizeString ("Size.Column"), "Size",
                            nameof (DocumentViewModel.FormatSize));
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_ICON:
                        AddDocumentColumn (LocalizeString ("Icon.Column"), "Icon",
                            nameof (DocumentViewModel.FormatIcon), false);
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_TITLE:
                        if (Settings.ShowTitleLink) {
                            AddDownloadLink (LocalizeString ("Title.Column"),
                                "Title", nameof (DocumentViewModel.Title), "ctlTitle");
                        }
                        else {
                            AddDocumentColumn (LocalizeString ("Title.Column"),
                                "Title", nameof (DocumentViewModel.Title));
                        }
                        break;

                    case DocumentsDisplayColumnInfo.COLUMN_SIGNATURE:
                        AddDocumentColumn (LocalizeString ("Signature.Column"), "signature-link",
                            nameof (DocumentViewModel.SignatureLink), false);
                        break;
                }
            }
        }

        List<DocumentViewModel> LoadDocuments ()
        {
            IEnumerable<DocumentViewModel> documents;
            if (!Settings.FolderMode) {
                var cacheKey = ModuleSynchronizer.GetDataCacheKey (ModuleId, TabModuleId);
                documents = DataCache.GetCachedData<IEnumerable<DocumentViewModel>> (
                    new CacheItemArgs (cacheKey, DocumentsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                    c => LoadDocuments_Internal ()
                );
            }
            else {
                documents = LoadDocuments_Internal ();
            }

            var isAdmin = UserInfo.IsSuperUser || UserInfo.IsInRole ("Administrators");
            // remove unpublished and inaccessible documents from the list
            // TODO: Add test to check filtering logic
            var filteredDocuments = documents
                .Where (d =>
                        (IsEditable || d.IsPublished (HttpContext.Current.Timestamp)) &&
                        (isAdmin || CanView (d.Url)))
                .ToList ();

            // sort documents
            var docComparer = new DocumentViewModelComparer (Settings.GetSortColumnList ());
            filteredDocuments.Sort (docComparer.Compare);

            return filteredDocuments;
        }

        IEnumerable<DocumentViewModel> LoadDocuments_Internal ()
        {
            var viewModelContext = new ViewModelContext<DocumentsSettings> (this, Settings);
            return GetDocuments ().Select (d => new DocumentViewModel (d, viewModelContext));
        }

        IEnumerable<DocumentInfo> GetDocuments ()
        {
            if (Settings.FolderMode) {
                return DocumentsDataProvider.Instance.CreateDocumentsFromFolder (Settings.DefaultFolder.Value,
                                                                                 PortalId,
                                                                                 ModuleId,
                                                                                 Settings.FileFilter,
                                                                                 Settings.FilenameToTitleRulesParsed);
            }

            return DocumentsDataProvider.Instance.GetDocuments (ModuleId, PortalId);
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

                default:
                    return true;
            }

            return false;
        }

        void AddDocumentColumn (string title, string cssClass, string dataField, bool htmlEncode = true, string format = null)
        {
            var objBoundColumn = new BoundField ();

            objBoundColumn.HtmlEncode = htmlEncode;

            objBoundColumn.DataField = dataField;

            if (format != null) {
                objBoundColumn.DataFormatString = "{0:" + format + "}";
            }

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
                name, LocalizeString ("DownloadLink.Text"), ListItemType.Item
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
