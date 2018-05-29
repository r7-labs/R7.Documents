//
// Copyright (c) 2014-2018 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search.Entities;
using R7.Dnn.Extensions.Utilities;
using R7.Documents.Data;
using R7.Documents.Models;

namespace R7.Documents
{
    public class DocumentsController : ModuleSearchBase, IPortable
    {
        #region ModuleSearchBase implementaion

        public override IList<SearchDocument> GetModifiedSearchDocuments (ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            var searchDocs = new List<SearchDocument> ();
			
            var portalAlias = PortalAliasController.Instance.GetPortalAliasesByPortalId (moduleInfo.PortalID).First (pa => pa.IsPrimary);
            var portalSettings = new PortalSettings (moduleInfo.TabID, portalAlias);

            var documents = DocumentsDataProvider.Instance.GetDocuments (moduleInfo.ModuleID, moduleInfo.PortalID);

            var now = DateTime.Now;
            foreach (var document in documents ?? Enumerable.Empty<DocumentInfo> ()) {
                if (document.ModifiedDate.ToUniversalTime () > beginDateUtc.ToUniversalTime ()) {
                    var sd = new SearchDocument {
                        PortalId = moduleInfo.PortalID,
                        AuthorUserId = document.ModifiedByUserId,
                        Title = document.Title,
                        Description = document.Description,
                        ModifiedTimeUtc = document.ModifiedDate.ToUniversalTime (),
                        UniqueKey = string.Format ("Documents_Document_{0}", document.ItemId),
                        Url = Globals.NavigateURL (moduleInfo.TabID, portalSettings, "", "mid", moduleInfo.ModuleID.ToString ()),
                        IsActive = document.IsPublished (now)
                    };
					
                    searchDocs.Add (sd);
                }
            }

            return searchDocs;
        }

        #endregion

        #region IPortable implementation

        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be exported</param>
        /// <history>
        ///		[cnurse]	    17 Nov 2004	documented
        ///		[aglenwright]	18 Feb 2006	Added new fields: Createddate, description, 
        ///                             modifiedbyuser, modifieddate, OwnedbyUser, SortorderIndex
        ///                             Added DocumentsSettings
        ///   [togrean]     10 Jul 2007 Fixed issues with importing documet settings since new fileds 
        ///                             were added: AllowSorting, default folder, list name
        ///   [togrean]     13 Jul 2007 Added support for exporting documents Url tracking options  
        /// </history>
        public string ExportModule (int moduleId)
        {
            var mCtrl = new ModuleController ();
            var module = mCtrl.GetModule (moduleId, Null.NullInteger);
		
            var strXml = new StringBuilder ("<documents>");
		
            try {
                var documents = DocumentsDataProvider.Instance.GetDocuments (moduleId, module.PortalID);

                var now = DateTime.Now;
                if (documents.Any ()) {
                    foreach (var document in documents) {
                        strXml.Append ("<document>");
                        strXml.AppendFormat ("<title>{0}</title>", XmlUtils.XMLEncode (document.Title));
                        strXml.AppendFormat ("<url>{0}</url>", XmlUtils.XMLEncode (document.Url));
                        strXml.AppendFormat ("<category>{0}</category>", XmlUtils.XMLEncode (document.Category));
                        strXml.AppendFormat (
                            "<description>{0}</description>",
                            XmlUtils.XMLEncode (document.Description));
                        strXml.AppendFormat (
                            "<forcedownload>{0}</forcedownload>",
                            XmlUtils.XMLEncode (document.ForceDownload.ToString ()));
                        strXml.AppendFormat ("<startDate>{0}</startDate>", XmlUtils.XMLEncode (document.StartDate.ToString ()));
                        strXml.AppendFormat ("<endDate>{0}</endDate>", XmlUtils.XMLEncode (document.EndDate.ToString ()));
                        strXml.AppendFormat (
                            "<ownedbyuserid>{0}</ownedbyuserid>",
                            XmlUtils.XMLEncode (document.OwnedByUserId.ToString ()));
                        strXml.AppendFormat (
                            "<sortorderindex>{0}</sortorderindex>",
                            XmlUtils.XMLEncode (document.SortOrderIndex.ToString ()));
                        strXml.AppendFormat (
                            "<linkattributes>{0}</linkattributes>",
                            XmlUtils.XMLEncode (document.LinkAttributes));

                        // Export Url Tracking options too
                        var urlCtrl = new UrlController ();
                        var urlTrackingInfo = urlCtrl.GetUrlTracking (module.PortalID, document.Url, moduleId);

                        if ((urlTrackingInfo != null)) {
                            strXml.AppendFormat (
                                "<logactivity>{0}</logactivity>",
                                XmlUtils.XMLEncode (urlTrackingInfo.LogActivity.ToString ()));
                            strXml.AppendFormat (
                                "<trackclicks>{0}</trackclicks>",
                                XmlUtils.XMLEncode (urlTrackingInfo.TrackClicks.ToString ()));
                            strXml.AppendFormat ("<newwindow>{0}</newwindow>", XmlUtils.XMLEncode (urlTrackingInfo.NewWindow.ToString ()));
                        }
                        strXml.Append ("</document>");
                    }
                }

                ExportSettings (module, strXml);
            }
            catch (Exception ex) {
                Exceptions.LogException (ex);
            }
            finally {
                // make sure XML is valid
                strXml.Append ("</documents>");
            }

            return strXml.ToString ();
        }

        void ExportSettings (ModuleInfo module, StringBuilder strXml)
        {
            var settings = new DocumentsSettingsRepository ().GetSettings (module);
            strXml.Append ("<settings>");
            strXml.AppendFormat ("<allowusersort>{0}</allowusersort>", XmlUtils.XMLEncode (settings.AllowUserSort.ToString ()));
            strXml.AppendFormat ("<showtitlelink>{0}</showtitlelink>", XmlUtils.XMLEncode (settings.ShowTitleLink.ToString ()));
            strXml.AppendFormat (
                "<usecategorieslist>{0}</usecategorieslist>",
                XmlUtils.XMLEncode (settings.UseCategoriesList.ToString ()));
            strXml.AppendFormat (
                "<categorieslistname>{0}</categorieslistname>",
                XmlUtils.XMLEncode (settings.CategoriesListName));
            strXml.AppendFormat ("<defaultfolder>{0}</defaultfolder>", XmlUtils.XMLEncode (settings.DefaultFolder.ToString ()));
            strXml.AppendFormat ("<filefilter>{0}</filefilter>", XmlUtils.XMLEncode (settings.FileFilter));
            strXml.AppendFormat ("<displaycolumns>{0}</displaycolumns>", XmlUtils.XMLEncode (settings.DisplayColumns));
            strXml.AppendFormat ("<sortorder>{0}</sortorder>", XmlUtils.XMLEncode (settings.SortOrder));
            strXml.AppendFormat ("<dateTimeFormat>{0}</dateTimeFormat>", XmlUtils.XMLEncode (settings.DateTimeFormat));
            strXml.Append ("</settings>");
        }

        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be imported</param>
        /// <history>
        ///		[cnurse]	    17 Nov 2004	documented
        ///		[aglenwright]	18 Feb 2006	Added new fields: Createddate, description, 
        ///                             modifiedbyuser, modifieddate, OwnedbyUser, SortorderIndex
        ///                             Added DocumentsSettings
        ///   [togrean]     10 Jul 2007 Fixed issues with importing documet settings since new fileds 
        ///                             were added: AllowSorting, default folder, list name    
        ///   [togrean]     13 Jul 2007 Added support for importing documents Url tracking options     
        /// </history>
        public void ImportModule (int moduleId, string content, string version, int userId)
        {
            var mCtrl = new ModuleController ();
            var module = mCtrl.GetModule (moduleId, Null.NullInteger);
			
            var xmlDocuments = Globals.GetContent (content, "documents");
            var documentNodes = xmlDocuments.SelectNodes ("document");
            var now = DateTime.Now;

            foreach (XmlNode documentNode in documentNodes) {
                var strUrl = documentNode ["url"].InnerText;
                var document = new DocumentInfo {
                    ModuleId = moduleId,
                    Title = documentNode ["title"].InnerText,
                    Category = documentNode ["category"].InnerText,
                    Description = XmlUtils.GetNodeValue (documentNode, "description"),
                    OwnedByUserId = XmlUtils.GetNodeValueInt (documentNode, "ownedbyuserid"),
                    SortOrderIndex = XmlUtils.GetNodeValueInt (documentNode, "sortorderindex"),
                    LinkAttributes = XmlUtils.GetNodeValue (documentNode, "linkattributes"),
                    ForceDownload = XmlUtils.GetNodeValueBoolean (documentNode, "forcedownload"),
                    StartDate = GetNodeValueDateNullable (documentNode, "startDate"),
                    EndDate = GetNodeValueDateNullable (documentNode, "endDate"),
                    CreatedByUserId = userId,
                    ModifiedByUserId = userId,
                    CreatedDate = now,
                    ModifiedDate = now,
                    Url = strUrl.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase) ?
                        strUrl : Globals.ImportUrl (moduleId, strUrl)
                };

                DocumentsDataProvider.Instance.Add (document);

                // Update Tracking options
                var urlType = document.Url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase) ? "F" : "U";

                var urlCtrl = new UrlController ();
                // If nodes not found, all values will be false
                urlCtrl.UpdateUrl (
                    module.PortalID,
                    document.Url,
                    urlType,
                    XmlUtils.GetNodeValueBoolean (documentNode, "logactivity"),
                    XmlUtils.GetNodeValueBoolean (documentNode, "trackclicks", true),
                    moduleId,
                    XmlUtils.GetNodeValueBoolean (documentNode,  "newwindow"));
            }

            ImportSettings (module, content);

            ModuleSynchronizer.Synchronize (module.ModuleID, module.TabModuleID);
        }

        void ImportSettings (ModuleInfo module, string content)
        {
            var xmlSettings = Globals.GetContent (content, "documents/settings");
            if (xmlSettings != null) {
                var settingsRepository = new DocumentsSettingsRepository ();
                var settings = settingsRepository.GetSettings (module);

                settings.AllowUserSort = XmlUtils.GetNodeValueBoolean (xmlSettings, "allowusersort");
                settings.ShowTitleLink = XmlUtils.GetNodeValueBoolean (xmlSettings, "showtitlelink");
                settings.UseCategoriesList = XmlUtils.GetNodeValueBoolean (xmlSettings, "usecategorieslist");
                settings.CategoriesListName = XmlUtils.GetNodeValue (xmlSettings, "categorieslistname");
                settings.DefaultFolder = TypeUtils.ParseToNullable<int> (XmlUtils.GetNodeValue (xmlSettings, "defaultfolder"));
                settings.FileFilter = XmlUtils.GetNodeValue (xmlSettings, "filefilter");
                settings.DisplayColumns = XmlUtils.GetNodeValue (xmlSettings, "displaycolumns");
                settings.SortOrder = XmlUtils.GetNodeValue (xmlSettings, "sortorder");
                settings.DateTimeFormat = XmlUtils.GetNodeValue (xmlSettings, "dateTimeFormat");

                settingsRepository.SaveSettings (module, settings);
            }
        }

        DateTime? GetNodeValueDateNullable (XmlNode node, string nodeName)
        {
            var date = XmlUtils.GetNodeValueDate (node, nodeName, DateTime.MinValue);
            return (date != DateTime.MinValue) ? (DateTime?) date : null;
        }

        #endregion
    }
}

