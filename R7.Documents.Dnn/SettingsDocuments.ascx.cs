﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.Documents.Models;
using R7.Documents.ViewModels;
using R7.University.Components;

namespace R7.Documents
{
    public partial class SettingsDocuments : ModuleSettingsBase<DocumentsSettings>
    {
        const string VIEWSTATE_SORTCOLUMNSETTINGS = "SortColumnSettings";

        const string VIEWSTATE_DISPLAYCOLUMNSETTINGS = "DisplayColumnSettings";

        ViewModelContext _dnn;
        protected ViewModelContext Dnn => _dnn ?? (_dnn = new ViewModelContext (this));

        #region Event handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            rblSortOrderDirection.AddItem (LocalizeString ("SortOrderAscending.Text"), "ASC");
            rblSortOrderDirection.AddItem (LocalizeString ("SortOrderDescending.Text"), "DESC");
            rblSortOrderDirection.SelectedIndex = 0;

            // bind grid styles
            ddlGridStyle.DataSource = DocumentsConfig.Instance.GridStyles;
            ddlGridStyle.DataBind ();

            valFileFilter.ErrorMessage = LocalizeString ("FileFilter.Invalid");
            valFilenameToTitleRules.ErrorMessage = LocalizeString ("FilenameToTitleRules.Invalid");

            ddlDefaultFolder.UndefinedItem = new ListItem (LocalizeString ("NotSelected.Text"), "-1");
        }

        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            if (UserInfo.IsSuperUser) {
                lnkEditLists.Text = Localization.GetString ("lnkEditLists", LocalResourceFile);

                try {
                    var tabController = new TabController ();
                    lnkEditLists.NavigateUrl = tabController.GetTabByName ("Lists", Null.NullInteger).FullUrl;
                } catch {
                    // unable to locate "Lists" tab
                    lblCannotEditLists.Text = Localization.GetString ("UnableToFindLists", LocalResourceFile);
                    lblCannotEditLists.Visible = true;
                    lnkEditLists.Visible = false;
                }
            } else {
                // show error, then hide the "Edit" link
                lblCannotEditLists.Text = Localization.GetString ("NoListAccess", LocalResourceFile);
                lblCannotEditLists.Visible = true;
                lnkEditLists.Visible = false;
            }
        }

        protected void grdSortColumns_ItemCreated (object sender, DataGridItemEventArgs e)
        {
            switch (e.Item.ItemType) {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
            case ListItemType.SelectedItem:

                // Localize the delete button and set image
                var deleteButton = (ImageButton)e.Item.FindControl ("btnDeleteSortOrder");
                deleteButton.ToolTip = deleteButton.AlternateText = LocalizeString ("btnDeleteSortOrder.Text");
                deleteButton.ImageUrl = IconController.IconURL ("Delete");

                break;
            }
        }

        protected void grdDisplayColumns_ItemCreated (object sender, DataGridItemEventArgs e)
        {
            var objUpImage = default (ImageButton);
            var objDownImage = default (ImageButton);

            switch (e.Item.ItemType) {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
            case ListItemType.SelectedItem:

                // Center the "visible" checkbox in its cell
                e.Item.Cells [1].Style.Add ("text-align", "center");

                // imgUp
                objUpImage = (ImageButton)e.Item.Cells [2].FindControl ("imgUp");
                objUpImage.Visible = (e.Item.ItemIndex != 0);
                objUpImage.ImageUrl = IconController.IconURL ("Up", "16X16");

                // imgDown
                objDownImage = (ImageButton)e.Item.Cells [2].FindControl ("imgDown");
                objDownImage.ImageUrl = IconController.IconURL ("Dn", "16X16");
                if (objUpImage.Visible == false) {
                    objDownImage.Style.Add ("margin-left", "19px");
                }

                e.Item.CssClass = "Normal";
                break;

            case ListItemType.Header:
                e.Item.CssClass = "SubHead";
                break;
            }
        }

        protected void grdDisplayColumns_ItemCommand (object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName) {
            case "DisplayOrderDown":
                // swap e.CommandArgument and the one after it
                SwapColumn (e.CommandArgument.ToString (), ListSortDirection.Descending);
                break;
            case "DisplayOrderUp":
                // swap e.CommandArgument and the one before it
                SwapColumn (e.CommandArgument.ToString (), ListSortDirection.Ascending);
                break;
            }
        }

        protected void lnkAddSortColumn_Click (object sender, EventArgs e)
        {
            var objSortColumns = default (ArrayList);
            var objNewSortColumn = new DocumentsSortColumn ();

            objSortColumns = RetrieveSortColumnSettings ();
            objNewSortColumn.ColumnName = ddlSortFields.SelectedValue;
            if (rblSortOrderDirection.SelectedValue == "ASC") {
                objNewSortColumn.Direction = Models.SortDirection.Ascending;
            } else {
                objNewSortColumn.Direction = Models.SortDirection.Descending;
            }

            objSortColumns.Add (objNewSortColumn);
            BindSortSettings (objSortColumns);
        }

        protected void grdSortColumns_DeleteCommand (object source, DataGridCommandEventArgs e)
        {
            var objSortColumns = default (ArrayList);
            var objSortColumnToDelete = new DocumentsSortColumn ();

            objSortColumns = RetrieveSortColumnSettings ();

            foreach (DocumentsSortColumn objSortColumnToDelete_loopVariable in objSortColumns) {
                objSortColumnToDelete = objSortColumnToDelete_loopVariable;
                if (objSortColumnToDelete.ColumnName == grdSortColumns.DataKeys [e.Item.ItemIndex].ToString ()) {
                    objSortColumns.Remove (objSortColumnToDelete);
                    break;
                }
            }

            BindSortSettings (objSortColumns);
        }

        #endregion

        public override void LoadSettings ()
        {
            DocumentsDisplayColumn objColumn = null;

            try {
                if (!IsPostBack) {
                    LoadLists ();

                    chkShowTitleAsLink.Checked = Settings.ShowTitleLink;
                    chkUseCategoriesList.Checked = Settings.UseCategoriesList;
                    chkAllowUserSort.Checked = Settings.AllowUserSort;
                    ddlGridStyle.SelectByValue (Settings.GridStyle);
                    txtDateTimeFormat.Text = Settings.DateTimeFormat;
                    txtFileFilter.Text = Settings.FileFilter;
                    txtFilenameToTitleRules.Text = Settings.FilenameToTitleRules;
                    chkAutoHide.Checked = Settings.AutoHide;

                    try {
                        if (Settings.DefaultFolder != null) {
                            ddlDefaultFolder.SelectedFolder =
                                FolderManager.Instance.GetFolder (Settings.DefaultFolder.Value);
                        }
                    }
                    catch (Exception ex) {
                        // can be caused if the selected folder has been deleted
                        Exceptions.LogException (ex);
                    }

                    try {
                        ddlCategoriesList.SelectedValue = Settings.CategoriesListName;
                    }
                    catch (Exception ex) {
                        // can be caused if the selected list has been deleted
                        Exceptions.LogException (ex);
                    }

                    // read "saved" column sort orders in first
                    var objColumnSettings = Settings.GetDisplayColumnList (LocalResourceFile);

                    foreach (DocumentsDisplayColumn objColumnInfo_loopVariable in objColumnSettings) {
                        objColumn = objColumnInfo_loopVariable;
                        // set localized column names
                        objColumn.LocalizedColumnName = Localization.GetString (
                            objColumn.ColumnName + ".Column",
                            LocalResourceFile);
                    }

                    // add any missing columns to the end
                    foreach (string strColumnName_loopVariable in DocumentsDisplayColumn.AvailableDisplayColumns) {
                        var strColumnName = strColumnName_loopVariable;
                        if (DocumentsSettings.FindColumn (strColumnName, objColumnSettings, false) < 0) {
                            objColumn = new DocumentsDisplayColumn ();
                            objColumn.ColumnName = strColumnName;
                            objColumn.LocalizedColumnName = Localization.GetString (
                                objColumn.ColumnName + ".Column",
                                LocalResourceFile);
                            objColumn.DisplayOrder = objColumnSettings.Count + 1;
                            objColumn.Visible = false;

                            objColumnSettings.Add (objColumn);
                        }
                    }

                    // sort by DisplayOrder
                    BindColumnSettings (objColumnSettings);

                    // load sort columns
                    string strSortColumn = null;
                    foreach (string strSortColumn_loopVariable in DocumentsDisplayColumn.AvailableSortColumns) {
                        strSortColumn = strSortColumn_loopVariable;
                        ddlSortFields.AddItem (LocalizeString (strSortColumn + ".Column"), strSortColumn);
                    }

                    BindSortSettings (Settings.GetSortColumnList ());

                    // load grid style
                    ddlGridStyle.SelectByValue (Settings.GridStyle);
                }

            } catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        public void LoadLists ()
        {
            var listController = new ListController ();
            foreach (ListInfo objList in listController.GetListInfoCollection ()) {
                if (!objList.SystemList) {
                    // for some reason, the "DataType" is not marked as a system list, but we want to exclude that one too
                    if (objList.DisplayName != "DataType") {
                        ddlCategoriesList.Items.Add (new ListItem (objList.DisplayName, objList.DisplayName));
                    }
                }
            }

            if (ddlCategoriesList.Items.Count == 0) {
                lblNoListsAvailable.Text = Localization.GetString ("msgNoListsAvailable.Text", LocalResourceFile);
                lblNoListsAvailable.Visible = true;
            }
        }

        public override void UpdateSettings ()
        {
            try {
                if (Page.IsValid) {
                    FillSettings ();

                    SettingsRepository.SaveSettings (ModuleConfiguration, Settings);

                    ModuleSynchronizer.Synchronize (ModuleId, TabModuleId);
                }
            }
            catch (Exception exc) {
                Exceptions.ProcessModuleLoadException (this, exc);
            }
        }

        void BindSortSettings (ArrayList objSortColumns)
        {
            SaveSortColumnSettings (objSortColumns);
            grdSortColumns.DataSource = objSortColumns.Cast<IDocumentsSortColumn> ()
                .Select (sc => new DocumentSortColumnViewModel (sc, Dnn));
            grdSortColumns.DataKeyField = "ColumnName";

            Localization.LocalizeDataGrid (ref grdSortColumns, LocalResourceFile);
            grdSortColumns.DataBind ();
        }

        void BindColumnSettings (List<DocumentsDisplayColumn> objColumnSettings)
        {
            objColumnSettings.Sort ();
            SaveDisplayColumnSettings (objColumnSettings);
            grdDisplayColumns.DataSource = objColumnSettings;
            grdDisplayColumns.DataKeyField = "ColumnName";

            if (!IsPostBack) {
                Localization.LocalizeDataGrid (ref grdDisplayColumns, LocalResourceFile);
            }

            grdDisplayColumns.DataBind ();

            var imageDownload = (ImageButton)grdDisplayColumns.Items [grdDisplayColumns.Items.Count - 1].Cells [2].FindControl ("imgDown");
            // set down arrow invisible on the last item
            imageDownload.Visible = false;

        }

        void FillSettings ()
        {
            string strDisplayColumns = "";
            DocumentsDisplayColumn objColumn = null;
            int intIndex = 0;
            var objSortColumns = default (ArrayList);
            string strSortColumnList = "";
            DocumentsSortColumn objSortColumn = null;

            // ensure that if categories list is checked that we did have an available category
            if ((chkUseCategoriesList.Checked && !lblNoListsAvailable.Visible)) {
                // if so, set normally
                Settings.UseCategoriesList = chkUseCategoriesList.Checked;
                Settings.CategoriesListName = ddlCategoriesList.SelectedValue;
            } else {
                // otherwise default values
                Settings.UseCategoriesList = false;
                Settings.CategoriesListName = "";
            }

            Settings.ShowTitleLink = chkShowTitleAsLink.Checked;
            Settings.AllowUserSort = chkAllowUserSort.Checked;
            Settings.GridStyle = ddlGridStyle.SelectedItem.Value;
            Settings.FileFilter = txtFileFilter.Text.Trim ();
            Settings.FilenameToTitleRules = txtFilenameToTitleRules.Text;
            Settings.AutoHide = chkAutoHide.Checked;

            try {
                DateTime.Now.ToString (txtDateTimeFormat.Text);
                Settings.DateTimeFormat = txtDateTimeFormat.Text;
            }
            catch (Exception ex) {
                Exceptions.LogException (ex);
                Settings.DateTimeFormat = null;
            }

            if (ddlDefaultFolder.SelectedFolder != null) {
                Settings.DefaultFolder = ddlDefaultFolder.SelectedFolder.FolderID;
            } else {
                Settings.DefaultFolder = null;
            }

            var objColumnSettings = RetrieveDisplayColumnSettings ();
            intIndex = 0;
            foreach (DocumentsDisplayColumn objColumnInfo_loopVariable in objColumnSettings) {
                objColumn = objColumnInfo_loopVariable;
                // Figure out column visibility
                objColumn.Visible = ((CheckBox)grdDisplayColumns.Items [intIndex].Cells [1].FindControl ("chkVisible")).Checked;

                if (strDisplayColumns != string.Empty) {
                    strDisplayColumns = strDisplayColumns + ",";
                }
                strDisplayColumns = strDisplayColumns + objColumn.ColumnName + ";" + objColumn.Visible.ToString ();

                intIndex = intIndex + 1;
            }

            Settings.DisplayColumns = strDisplayColumns;

            objSortColumns = RetrieveSortColumnSettings ();
            foreach (DocumentsSortColumn objSortColumn_loopVariable in objSortColumns) {
                objSortColumn = objSortColumn_loopVariable;
                if (strSortColumnList != string.Empty) {
                    strSortColumnList = strSortColumnList + ",";
                }
                strSortColumnList = strSortColumnList + (objSortColumn.Direction == Models.SortDirection.Descending ? "-" : "") + objSortColumn.ColumnName;
            }
            Settings.SortOrder = strSortColumnList;
            Settings.GridStyle = ddlGridStyle.SelectedValue;
        }

        void SwapColumn (string columnName, ListSortDirection direction)
        {
            int intIndex = 0;
            int intDisplayOrderTemp = 0;

            // first, find the column we want
            var objColumnSettings = RetrieveDisplayColumnSettings ();
            intIndex = DocumentsSettings.FindColumn (columnName, objColumnSettings, false);

            // swap display orders
            if (intIndex >= 0) {
                switch (direction) {
                case ListSortDirection.Ascending:
                    // swap up
                    if (intIndex > 0) {
                        intDisplayOrderTemp = objColumnSettings [intIndex].DisplayOrder;
                        objColumnSettings [intIndex].DisplayOrder = objColumnSettings [intIndex - 1].DisplayOrder;
                        objColumnSettings [intIndex - 1].DisplayOrder = intDisplayOrderTemp;
                    }
                    break;
                case ListSortDirection.Descending:
                    // swap down
                    if (intIndex < objColumnSettings.Count) {
                        intDisplayOrderTemp = objColumnSettings[intIndex].DisplayOrder;
                            objColumnSettings[intIndex].DisplayOrder = objColumnSettings[intIndex + 1].DisplayOrder;
                            objColumnSettings[intIndex + 1].DisplayOrder = intDisplayOrderTemp;
                    }
                    break;
                }
            }

            // re-bind the newly sorted collection to the datagrid
            BindColumnSettings (objColumnSettings);
        }

        void SaveSortColumnSettings (ArrayList objSettings)
        {
            // custom viewstate implementation to avoid reflection
            DocumentsSortColumn objSortColumn = null;
            string strValues = "";

            foreach (DocumentsSortColumn objSortColumnInfo_loopVariable in objSettings) {
                objSortColumn = objSortColumnInfo_loopVariable;
                if (strValues != string.Empty) {
                    strValues = strValues + "#";
                }

                strValues = strValues + objSortColumn.ColumnName + "," + objSortColumn.Direction.ToString ();
            }
            ViewState [VIEWSTATE_SORTCOLUMNSETTINGS] = strValues;
        }

        ArrayList RetrieveSortColumnSettings ()
        {
            // custom viewstate implementation to avoid reflection
            var objSortColumnSettings = new ArrayList ();
            DocumentsSortColumn objSortColumn = null;

            string strValues = null;

            strValues = Convert.ToString (ViewState [VIEWSTATE_SORTCOLUMNSETTINGS]);
            if (!string.IsNullOrEmpty (strValues)) {
                foreach (string strSortColumnSetting in strValues.Split ('#')) {
                    objSortColumn = new DocumentsSortColumn ();
                    objSortColumn.ColumnName = strSortColumnSetting.Split (',') [0];
                    objSortColumn.Direction = (Models.SortDirection) Enum.Parse (
                        typeof (Models.SortDirection),
                        strSortColumnSetting.Split (',') [1]);

                    objSortColumnSettings.Add (objSortColumn);
                }
            }

            return objSortColumnSettings;
        }

        void SaveDisplayColumnSettings (List<DocumentsDisplayColumn> objSettings)
        {
            // custom viewstate implementation to avoid reflection
            DocumentsDisplayColumn objDisplayColumn = null;
            var strValues = "";

            foreach (DocumentsDisplayColumn objDisplayColumnInfo_loopVariable in objSettings) {
                objDisplayColumn = objDisplayColumnInfo_loopVariable;
                if (strValues != string.Empty) {
                    strValues = strValues + "#";
                }
                strValues = strValues + objDisplayColumn.ColumnName + "," + objDisplayColumn.LocalizedColumnName + "," + objDisplayColumn.DisplayOrder + "," + objDisplayColumn.Visible;
            }
            ViewState [VIEWSTATE_DISPLAYCOLUMNSETTINGS] = strValues;
        }

        List<DocumentsDisplayColumn> RetrieveDisplayColumnSettings ()
        {
            // custom viewstate implementation to avoid reflection
            var objDisplayColumnSettings = new List<DocumentsDisplayColumn> ();
            DocumentsDisplayColumn objDisplayColumn = null;

            string strValues = null;

            strValues = Convert.ToString (ViewState [VIEWSTATE_DISPLAYCOLUMNSETTINGS]);
            if (!string.IsNullOrEmpty (strValues)) {
                foreach (string strDisplayColumnSetting in strValues.Split ('#')) {
                    objDisplayColumn = new DocumentsDisplayColumn ();
                    objDisplayColumn.ColumnName = strDisplayColumnSetting.Split (',') [0];
                    objDisplayColumn.LocalizedColumnName = strDisplayColumnSetting.Split (',') [1];
                    objDisplayColumn.DisplayOrder = Convert.ToInt32 (strDisplayColumnSetting.Split (',') [2]);
                    objDisplayColumn.Visible = Convert.ToBoolean (strDisplayColumnSetting.Split (',') [3]);

                    objDisplayColumnSettings.Add (objDisplayColumn);
                }
            }

            return objDisplayColumnSettings;
        }

        protected void valFileFilter_ServerValidate (object sender, ServerValidateEventArgs e)
        {
            try {
                Regex.IsMatch ("Any", txtFileFilter.Text.Trim ());
            }
            catch {
                e.IsValid = false;
                return;
            }

            e.IsValid = true;
        }

        protected void valFilenameToTitleRules_ServerValidate (object sender, ServerValidateEventArgs e)
        {
            try {
                var rules = DocumentsSettings.ParseFilenameToTitleRules (txtFilenameToTitleRules.Text);
                foreach (var rule in rules) {
                    Regex.Replace ("Any", rule [0], rule [1]);
                }
            }
            catch {
                e.IsValid = false;
                return;
            }

            e.IsValid = true;
        }
    }
}
