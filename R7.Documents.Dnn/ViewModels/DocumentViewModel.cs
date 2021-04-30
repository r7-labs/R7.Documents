using System;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using R7.Dnn.Extensions.Models;
using R7.Dnn.Extensions.Text;
using R7.Dnn.Extensions.Users;
using R7.Dnn.Extensions.ViewModels;
using R7.Documents.Models;

namespace R7.Documents.ViewModels
{
    public class DocumentViewModel : IDocumentViewModel
    {
        protected IDocument Model;

        protected ViewModelContext<DocumentsSettings> Dnn;

        public DocumentViewModel (IDocument document, ViewModelContext<DocumentsSettings> dnn)
        {
            Model = document;
            Dnn = dnn;
        }

        #region IDocumentViewModel implementation

        public string Category => Model.Category;

        public int Clicks => Model.Clicks;

        public int CreatedByUserId => Model.CreatedByUserId;

        public DateTime CreatedDate => Model.CreatedDate;

        public string Description => Model.Description;

        public DateTime? EndDate => Model.EndDate;

        public bool ForceDownload => Model.ForceDownload;

        public int ItemId => Model.ItemId;

        public string LinkAttributes => Model.LinkAttributes;

        public int ModifiedByUserId => Model.ModifiedByUserId;

        public DateTime ModifiedDate => Model.ModifiedDate;

        public int ModuleId => Model.ModuleId;

        public bool NewWindow => Model.NewWindow;

        public int OwnedByUserId => Model.OwnedByUserId;

        public int Size => Model.Size;

        public int SortOrderIndex => Model.SortOrderIndex;

        public DateTime? StartDate => Model.StartDate;

        public string Title => Model.Title;

        public bool TrackClicks => Model.TrackClicks;

        public string Url => Model.Url;

        public bool IsFeatured => Model.IsFeatured;

        public DateTime PublishedOnDate => ModelHelper.PublishedOnDate (StartDate, CreatedDate);

        string _createdByUser;
        public string CreatedByUser => _createdByUser ?? (_createdByUser = UserHelper.GetUserDisplayName (Dnn.Module.PortalId, Model.CreatedByUserId) ?? Dnn.LocalizeString ("None_Specified"));

        string _modifiedByUser;
        public string ModifiedByUser => _modifiedByUser ?? (_modifiedByUser = UserHelper.GetUserDisplayName (Dnn.Module.PortalId, Model.ModifiedByUserId) ?? Dnn.LocalizeString ("None_Specified"));

        string _ownedByUser;
        public string OwnedByUser => _ownedByUser ?? (_ownedByUser = UserHelper.GetUserDisplayName (Dnn.Module.PortalId, Model.OwnedByUserId) ?? Dnn.LocalizeString ("None_Specified"));

        #endregion

        string _formatSize;
        public string FormatSize {
            get {
                if (_formatSize == null) {
                    try {
                        if (Size > 0) {
                            if (Size > (1024 * 1024)) {
                                _formatSize = string.Format ("{0:#,##0.00} {1}", Size / 1024f / 1024f, Dnn.LocalizeString ("Megabytes.Text"));
                            } else {
                                _formatSize = string.Format ("{0:#,##0.00} {1}", Size / 1024f, Dnn.LocalizeString ("Kilobytes.Text"));
                            }
                        }
                    } catch (Exception ex) {
                        Exceptions.LogException (ex);
                    }
                    if (_formatSize == null) {
                        _formatSize = Dnn.LocalizeString ("Unknown.Text");
                    }
                }
                return _formatSize;
            }
        }

        string _formatIcon;
        public string FormatIcon {
            get {
                if (_formatIcon == null) {
                    _formatIcon = "";

                    switch (Globals.GetURLType (Url)) {
                    case TabType.File:

                        var fileId = ParseHelper.ParseToNullable<int> (Url.ToLowerInvariant ().Substring ("fileid=".Length));
                        if (fileId != null) {
                            var fileInfo = FileManager.Instance.GetFile (fileId.Value);
                            if (fileInfo != null && !string.IsNullOrWhiteSpace (fileInfo.Extension)) {
                                // optimistic way
                                _formatIcon = string.Format (
                                    "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                                    IconController.IconURL ("Ext" + fileInfo.Extension),
                                    fileInfo.Extension.ToUpperInvariant ());
                            }
                        }
                        break;

                    case TabType.Tab:
                        _formatIcon = string.Format ("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                                                     IconController.IconURL ("FileLink", "16X16", "Black"), Dnn.LocalizeString ("Page.Text"));
                        break;

                    default:
                        _formatIcon = string.Format ("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                                                     IconController.IconURL ("FileLink", "16X16", "Black"), "URL");
                        break;
                    }
                }

                return _formatIcon;
            }
        }

        string _toolTip;
        public string ToolTip {
            get {
                if (_toolTip == null) {
                    _toolTip = "";

                    switch (Globals.GetURLType (Url)) {
                        case TabType.File:
                            var fileId = ParseHelper.ParseToNullable<int> (Url.ToLowerInvariant ().Substring ("fileid=".Length));
                            if (fileId != null) {
                                var fileInfo = FileManager.Instance.GetFile (fileId.Value);
                                if (fileInfo != null) {
                                    _toolTip = fileInfo.RelativePath;
                                }
                            }
                            break;

                        case TabType.Tab:
                            var tabId = ParseHelper.ParseToNullable<int> (Url);
                            if (tabId != null) {
                                var tabInfo = TabController.Instance.GetTab (tabId.Value, Null.NullInteger);
                                if (tabInfo != null) {
                                    _toolTip = tabInfo.LocalizedTabName;
                                }
                            }
                            break;

                        default:
                            _toolTip = Url;
                            break;
                    }
                }

                return _toolTip;
            }
        }

        public HtmlString SignatureLink {
            get {
                var sigFile = GetSignatureFile (Url);
                if (sigFile == null) {
                    return new HtmlString (string.Empty);
                }

                return new HtmlString (
                    $"<a href=\"{Globals.LinkClick ("fileid=" + sigFile.FileId, Dnn.Module.TabId, Dnn.Module.ModuleId)}\""
                    + $" title=\"{Dnn.LocalizeString ("SignatureLink_Tooltip.Text")}\">"
                    + $"{Dnn.LocalizeString ("SignatureLink_Label.Text")}</a>");
            }
        }

        IFileInfo GetSignatureFile (string url)
        {
            if (Globals.GetURLType (Url) != TabType.File) {
                return null;
            }

            var fileId = ParseHelper.ParseToNullable<int> (Url.ToLowerInvariant ().Substring ("fileid=".Length));
            if (fileId == null) {
                return null;
            }

            var file = FileManager.Instance.GetFile (fileId.Value);
            if (file == null) {
                return null;
            }

            var folder = FolderManager.Instance.GetFolder (file.FolderId);
            var sigFile = FileManager.Instance.GetFile (folder, file.FileName + ".sig");
            if (sigFile == null) {
                sigFile = FileManager.Instance.GetFile (folder, file.FileName + ".p7s");
            }

            return sigFile;
        }
    }
}
