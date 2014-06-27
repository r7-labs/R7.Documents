//
// DocumentsController.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2014 
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
using System.Text;
using System.Xml;
using System.Linq;
using DotNetNuke.Collections;
using DotNetNuke.Data;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;
using DotNetNuke.Services.Search.Entities;


namespace R7.Documents
{
	public partial class DocumentsController : ControllerBase, ISearchable, IPortable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Documents.DocumentsController"/> class.
		/// </summary>
		public DocumentsController () : base ()
		{ 

		}

		public DocumentInfo GetDocument (int ItemId, int ModuleId)
		{
			DocumentInfo document;

			using (var ctx = DataContext.Instance ())
			{
				document = ctx.ExecuteSingleOrDefault<DocumentInfo> (
					System.Data.CommandType.StoredProcedure, "GetDocument", ItemId, ModuleId);
			}

			return document;
		}

		public IEnumerable<DocumentInfo> GetDocuments (int ModuleId, int PortalId)
		{
			IEnumerable<DocumentInfo> documents;

			using (var ctx = DataContext.Instance ())
			{
				documents = ctx.ExecuteQuery<DocumentInfo> (
					System.Data.CommandType.StoredProcedure, "GetDocuments", ModuleId, PortalId);
			}

			return documents;
		}

		#region ModuleSearchBase implementaion

		public override IList<SearchDocument> GetModifiedSearchDocuments (ModuleInfo modInfo, DateTime beginDate)
		{
			var searchDocs = new List<SearchDocument> ();

			// TODO: Realize GetModifiedSearchDocuments()

			/* var sd = new SearchDocument();
			searchDocs.Add(searchDoc);
			*/

			return searchDocs;
		}

		#endregion

		#region "Public Methods"


		/*
		public void AddDocument(DocumentInfo objDocument)
		{
			DataProvider.Instance().AddDocument(objDocument.ModuleId, objDocument.Title, objDocument.Url, objDocument.CreatedByUserId, objDocument.OwnedByUserId, objDocument.Category, objDocument.SortOrderIndex, objDocument.Description, objDocument.ForceDownload);

		}


		public void DeleteDocument(int ModuleId, int ItemID)
		{
			DataProvider.Instance().DeleteDocument(ModuleId, ItemID);

		}

		public DocumentInfo GetDocument(int ItemId, int ModuleId)
		{

			return (DocumentInfo)CBO.FillObject(DataProvider.Instance().GetDocument(ItemId, ModuleId), typeof(DocumentInfo));

		}

		public ArrayList GetDocuments(int ModuleId, int PortalId)
		{

			return CBO.FillCollection(DataProvider.Instance().GetDocuments(ModuleId, PortalId), typeof(DocumentInfo));

		}

		public void UpdateDocument(DocumentInfo objDocument)
		{
			DataProvider.Instance().UpdateDocument(objDocument.ModuleId, objDocument.ItemId, objDocument.Title, objDocument.Url, objDocument.CreatedByUserId, objDocument.OwnedByUserId, objDocument.Category, objDocument.SortOrderIndex, objDocument.Description, objDocument.ForceDownload);
		}
*/
		#endregion

		#region "Optional Interfaces"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// GetSearchItems implements the ISearchable Interface
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
		/// <history>
		///		[cnurse]	    17 Nov 2004	documented
		///   [aglenwright] 18 Feb 2006 Altered to accomodate change to CreatedByUser
		///                             field (changed from string to integer)
		/// </history>
		/// -----------------------------------------------------------------------------
		public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo)
		{
			SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();
			// ArrayList Documents = GetDocuments(ModInfo.ModuleID, ModInfo.PortalID);
			var documents = GetObjects<DocumentInfo>(ModInfo.ModuleID); // PortalID!

			// TODO: Add new fields

			object objDocument = null;
			foreach (object objDocument_loopVariable in documents) {
				objDocument = objDocument_loopVariable;
				SearchItemInfo SearchItem = default(SearchItemInfo);
				var _with1 = (DocumentInfo)objDocument;
				int UserId = Null.NullInteger;
				//If IsNumeric(.CreatedByUser) Then
				//    UserId = Integer.Parse(.CreatedByUser)
				//End If
				UserId = _with1.CreatedByUserId;
				SearchItem = new SearchItemInfo(
					ModInfo.ModuleTitle + " - " + _with1.Title, 
					_with1.Title, 
					UserId, 
					_with1.CreatedDate, 
					ModInfo.ModuleID, 
					_with1.ItemId.ToString(), 
					_with1.Title + " " + _with1.Category + " " + _with1.Description, 
					"ItemId=" + _with1.ItemId);
				SearchItemCollection.Add(SearchItem);
			}

			return SearchItemCollection;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ExportModule implements the IPortable ExportModule Interface
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="ModuleID">The Id of the module to be exported</param>
		/// <history>
		///		[cnurse]	    17 Nov 2004	documented
		///		[aglenwright]	18 Feb 2006	Added new fields: Createddate, description, 
		///                             modifiedbyuser, modifieddate, OwnedbyUser, SortorderIndex
		///                             Added DocumentsSettings
		///   [togrean]     10 Jul 2007 Fixed issues with importing documet settings since new fileds 
		///                             were added: AllowSorting, default folder, list name
		///   [togrean]     13 Jul 2007 Added support for exporting documents Url tracking options  
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ExportModule(int ModuleID)
		{

			ModuleController objModules = new ModuleController();
			ModuleInfo objModule = objModules.GetModule(ModuleID, Null.NullInteger);
		
			StringBuilder strXML = new StringBuilder("");
		
			try {
				var arrDocuments = GetObjects<DocumentInfo>(ModuleID); // objModule.PortalID!!!
				//if (arrDocuments.Count != 0) {
				if (arrDocuments.Any()) 
				{
					foreach (DocumentInfo objDocument_loopVariable in arrDocuments) {
						var objDocument = objDocument_loopVariable;
						strXML.Append("<document>");
						strXML.AppendFormat("<title>{0}</title>", XmlUtils.XMLEncode(objDocument.Title));
						strXML.AppendFormat("<url>{0}</url>", XmlUtils.XMLEncode(objDocument.Url));
						strXML.AppendFormat("<category>{0}</category>", XmlUtils.XMLEncode(objDocument.Category));

						strXML.AppendFormat("<createddate>{0}</createddate>", XmlUtils.XMLEncode(objDocument.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss tt")));
						strXML.AppendFormat("<description>{0}</description>", XmlUtils.XMLEncode(objDocument.Description));
						strXML.AppendFormat("<createdbyuserid>{0}</createdbyuserid>", XmlUtils.XMLEncode(objDocument.CreatedByUserId.ToString()));
						strXML.AppendFormat("<forcedownload>{0}</forcedownload>", XmlUtils.XMLEncode((objDocument.ForceDownload.ToString())));

						strXML.AppendFormat("<ownedbyuserid>{0}</ownedbyuserid>", XmlUtils.XMLEncode(objDocument.OwnedByUserId.ToString()));
						strXML.AppendFormat("<modifiedbyuserid>{0}</modifiedbyuserid>", XmlUtils.XMLEncode(objDocument.ModifiedByUserId.ToString()));
						strXML.AppendFormat("<modifieddate>{0}</modifieddate>", XmlUtils.XMLEncode(objDocument.ModifiedDate.ToString("dd-MMM-yyyy hh:mm:ss tt")));
						strXML.AppendFormat("<sortorderindex>{0}</sortorderindex>", XmlUtils.XMLEncode(objDocument.SortOrderIndex.ToString()));

						// Export Url Tracking options too
						UrlController objUrlController = new UrlController();
						UrlTrackingInfo objUrlTrackingInfo = objUrlController.GetUrlTracking(objModule.PortalID, objDocument.Url, ModuleID);

						if ((objUrlTrackingInfo != null)) {
							strXML.AppendFormat("<logactivity>{0}</logactivity>", XmlUtils.XMLEncode(objUrlTrackingInfo.LogActivity.ToString()));
							strXML.AppendFormat("<trackclicks>{0}</trackclicks>", XmlUtils.XMLEncode(objUrlTrackingInfo.TrackClicks.ToString()));
							strXML.AppendFormat("<newwindow>{0}</newwindow>", XmlUtils.XMLEncode(objUrlTrackingInfo.NewWindow.ToString()));
						}
						strXML.Append("</document>");
					}

				}
			} catch {
				// Catch errors but make sure XML is valid
			} finally {
				strXML.Append("</documents>");
			}

			return strXML.ToString();

		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ImportModule implements the IPortable ImportModule Interface
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="ModuleID">The Id of the module to be imported</param>
		/// <history>
		///		[cnurse]	    17 Nov 2004	documented
		///		[aglenwright]	18 Feb 2006	Added new fields: Createddate, description, 
		///                             modifiedbyuser, modifieddate, OwnedbyUser, SortorderIndex
		///                             Added DocumentsSettings
		///   [togrean]     10 Jul 2007 Fixed issues with importing documet settings since new fileds 
		///                             were added: AllowSorting, default folder, list name    
		///   [togrean]     13 Jul 2007 Added support for importing documents Url tracking options     
		/// </history>
		/// -----------------------------------------------------------------------------
		public void ImportModule(int ModuleID, string Content, string Version, int UserId)
		{
			
/*
// NOTE: Settings now imported by DNN
			bool isNewSettings = false;
			XmlNode xmlDocumentsSettings = Globals.GetContent(Content, "documents/documentssettings");
			if (((xmlDocumentsSettings != null))) {
				// Need to check before adding settings - update may be required instead
				var objDocumentsSettings = new DocumentsSettings(objModule);

				
				if ((objDocumentsSettings == null)) {
					objDocumentsSettings = new DocumentsSettingsInfo();
					isNewSettings = true;
				}

				objDocumentsSettings.ModuleId = ModuleID;
				objDocumentsSettings.DisplayColumns = XmlUtils.GetNodeValue(xmlDocumentsSettings, "displaycolumns");
				objDocumentsSettings.ShowTitleLink = XmlUtils.GetNodeValueBoolean(xmlDocumentsSettings, "showtitlelink");
				objDocumentsSettings.SortOrder = XmlUtils.GetNodeValue(xmlDocumentsSettings, "sortorder");
				objDocumentsSettings.UseCategoriesList = XmlUtils.GetNodeValueBoolean(xmlDocumentsSettings, "usecategorieslist");
				objDocumentsSettings.AllowUserSort = XmlUtils.GetNodeValueBoolean(xmlDocumentsSettings, "allowusersort");
				objDocumentsSettings.DefaultFolder = XmlUtils.GetNodeValue(xmlDocumentsSettings, "defaultfolder");
				objDocumentsSettings.CategoriesListName = XmlUtils.GetNodeValue(xmlDocumentsSettings, "categorieslistname");
				
				if (isNewSettings) {
					AddDocumentsSettings(objDocumentsSettings);
				} else {
					UpdateDocumentsSettings(objDocumentsSettings);
				}

			}*/

			// XmlNode xmlDocument = default(XmlNode);
			string strUrl = string.Empty;
			XmlNode xmlDocuments = Globals.GetContent(Content, "documents");
			XmlNodeList documentNodes = xmlDocuments.SelectNodes("document");
			foreach (XmlNode xmlDocument in documentNodes) {
				DocumentInfo objDocument = new DocumentInfo();
				objDocument.ModuleId = ModuleID;
				objDocument.Title = xmlDocument["title"].InnerText;
				strUrl = xmlDocument["url"].InnerText;
				if ((strUrl.ToLower().StartsWith("fileid="))) {
					objDocument.Url = strUrl;
				} else {
					objDocument.Url = Globals.ImportUrl(ModuleID, strUrl);
				}

				objDocument.Category = xmlDocument["category"].InnerText;
				objDocument.CreatedDate = XmlUtils.GetNodeValueDate(xmlDocument, "createddate", DateTime.Now);
				objDocument.Description = XmlUtils.GetNodeValue(xmlDocument, "description");
				objDocument.CreatedByUserId = UserId;

				objDocument.OwnedByUserId = XmlUtils.GetNodeValueInt(xmlDocument, "ownedbyuserid");
				objDocument.ModifiedByUserId = XmlUtils.GetNodeValueInt(xmlDocument, "modifiedbyuserid");
				objDocument.ModifiedDate = XmlUtils.GetNodeValueDate(xmlDocument, "modifieddate", DateTime.Now);
				objDocument.SortOrderIndex = XmlUtils.GetNodeValueInt(xmlDocument, "sortorderindex");
				objDocument.ForceDownload = XmlUtils.GetNodeValueBoolean(xmlDocument, "forcedownload");

				Add<DocumentInfo>(objDocument);

				// Update Tracking options
				ModuleController objModules = new ModuleController();
				ModuleInfo objModule = objModules.GetModule(ModuleID, Null.NullInteger);
				string urlType = "U";
				if (objDocument.Url.StartsWith("FileID")) {
					urlType = "F";
				}
				UrlController urlController = new UrlController();
				// If nodes not found, all values will be false
				urlController.UpdateUrl(objModule.PortalID, objDocument.Url, urlType, XmlUtils.GetNodeValueBoolean(xmlDocument, "logactivity"), XmlUtils.GetNodeValueBoolean(xmlDocument, "trackclicks", true), ModuleID, XmlUtils.GetNodeValueBoolean(xmlDocument, "newwindow"));

			}
		}

		#endregion
	}
}

