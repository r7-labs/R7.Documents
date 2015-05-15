//
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using DotNetNuke.Collections;
using DotNetNuke.Data;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Services.FileSystem;


namespace R7.Documents
{
	public partial class DocumentsController : ControllerBase, IPortable
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
					System.Data.CommandType.StoredProcedure, "Documents_GetDocument", ItemId, ModuleId);
			}

			return document;
		}

		public IEnumerable<DocumentInfo> GetDocuments (int ModuleId, int PortalId)
		{
			IEnumerable<DocumentInfo> documents;

			using (var ctx = DataContext.Instance ())
			{
				documents = ctx.ExecuteQuery<DocumentInfo> (
					System.Data.CommandType.StoredProcedure, "Documents_GetDocuments", ModuleId, PortalId);
			}

			return documents;
		}

		/// <summary>
		/// Gets documents from DNN Documents module
		/// </summary>
		/// <returns>The DNN documents.</returns>
		/// <param name="ModuleId">Module identifier.</param>
		/// <param name="PortalId">Portal identifier.</param>
		public IEnumerable<DocumentInfo> GetDNNDocuments (int ModuleId, int PortalId)
		{
			IEnumerable<DocumentInfo> documents;

			using (var ctx = DataContext.Instance ())
			{
				documents = ctx.ExecuteQuery<DocumentInfo> (
					System.Data.CommandType.StoredProcedure, "GetDocuments", ModuleId, PortalId);
			}

			return documents;
		}

		public DocumentInfo GetDNNDocument (int ItemId, int ModuleId)
		{
			DocumentInfo document;

			using (var ctx = DataContext.Instance ())
			{
				document = ctx.ExecuteSingleOrDefault<DocumentInfo> (
					System.Data.CommandType.StoredProcedure, "GetDocument", ItemId, ModuleId);
			}

			return document;
		}

        /// <summary>
        /// Deletes the resource, accociated with the document (only files and URLs are supported).
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="portalId">Portal identifier.</param>
        public int DeleteDocumentResource (DocumentInfo document, int portalId)
        {
            // count resource references
            var count = GetObjects<DocumentInfo> ("WHERE [Url]=@0", document.Url).Count ();

            // delete if it's the only reference
            if (count == 1)
            {
                switch (Globals.GetURLType (document.Url))
                {
                    // delete file
                    case TabType.File:
                        var file = FileManager.Instance.GetFile (Utils.GetResourceId (document.Url));
                        if (file != null)
                            FileManager.Instance.DeleteFile (file);
                        break;

                    // delete URL
                    case TabType.Url:
                        new UrlController ().DeleteUrl (portalId, document.Url);
                        break;
                }
            }

            return count;
        }

		public void DeleteDocumentUrl (string oldUrl, int PortalId, int ModuleId)
		{
			// NOTE: we shouldn't delete URL itself as is can be used in other modules
			// DataProvider.Instance().DeleteUrl (PortalId, document.Url);
			
			// delete URL tracking data
			DataProvider.Instance ().DeleteUrlTracking (PortalId, oldUrl, ModuleId);
		}

		public void UpdateDocumentUrl (DocumentInfo document, string oldUrl, int PortalId, int ModuleId)
		{
			if (document.Url != oldUrl)
			{
				var ctrlUrl = new UrlController ();
	
				// get tracking data for the old URL
				var url = ctrlUrl.GetUrlTracking (PortalId, oldUrl, ModuleId);
				
				// delete old URL tracking data
				DataProvider.Instance ().DeleteUrlTracking (PortalId, oldUrl, ModuleId);
				
				// create new URL tracking data
				ctrlUrl.UpdateUrl (PortalId, document.Url, url.UrlType, url.LogActivity, url.TrackClicks, ModuleId, url.NewWindow);
			}
		}

		#region ModuleSearchBase implementaion

		public override IList<SearchDocument> GetModifiedSearchDocuments (ModuleInfo moduleInfo, DateTime beginDate)
		{
			var searchDocs = new List<SearchDocument> ();
			
            var documents = GetDocuments (moduleInfo.ModuleID, moduleInfo.PortalID);

            foreach (var document in documents ?? Enumerable.Empty<DocumentInfo> ())
			{
				if (document.ModifiedDate.ToUniversalTime () > beginDate.ToUniversalTime ())
				{
                    var documentText = Utils.FormatList (" ", document.Title, document.Description);

					var sd = new SearchDocument () {
						PortalId = moduleInfo.PortalID,
                        AuthorUserId = document.ModifiedByUserId,
						Title = document.Title,
						Description = HtmlUtils.Shorten (documentText, 255, "..."),
						Body = documentText,
						ModifiedTimeUtc = document.ModifiedDate.ToUniversalTime (),
						UniqueKey = string.Format ("Documents_Document_{0}", document.ItemId),
						IsActive = document.IsPublished,
                        Url = string.Format ("/Default.aspx?tabid={0}#{1}", moduleInfo.TabID, moduleInfo.ModuleID)

                        // FIXME: This one produce null reference exception
                        // Url = Globals.LinkClick (document.Url, moduleInfo.TabID, moduleInfo.ModuleID, document.TrackClicks, document.ForceDownload)
					};
					
					searchDocs.Add (sd);
				}
			}
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

		#region IPortable implementation

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
		public string ExportModule (int ModuleID)
		{

			ModuleController objModules = new ModuleController ();
			ModuleInfo objModule = objModules.GetModule (ModuleID, Null.NullInteger);
		
			StringBuilder strXML = new StringBuilder ("<documents>");
		
			try
			{
				var arrDocuments = GetDocuments (ModuleID, objModule.PortalID);
				
				if (arrDocuments.Any ())
				{
					foreach (DocumentInfo objDocument_loopVariable in arrDocuments)
					{
						var objDocument = objDocument_loopVariable;
						strXML.Append ("<document>");
						strXML.AppendFormat ("<title>{0}</title>", XmlUtils.XMLEncode (objDocument.Title));
						strXML.AppendFormat ("<url>{0}</url>", XmlUtils.XMLEncode (objDocument.Url));
						strXML.AppendFormat ("<category>{0}</category>", XmlUtils.XMLEncode (objDocument.Category));
						strXML.AppendFormat ("<description>{0}</description>", XmlUtils.XMLEncode (objDocument.Description));
						strXML.AppendFormat ("<forcedownload>{0}</forcedownload>", XmlUtils.XMLEncode ((objDocument.ForceDownload.ToString ())));
						strXML.AppendFormat ("<ispublished>{0}</ispublished>", XmlUtils.XMLEncode ((objDocument.IsPublished.ToString ())));
						strXML.AppendFormat ("<ownedbyuserid>{0}</ownedbyuserid>", XmlUtils.XMLEncode (objDocument.OwnedByUserId.ToString ()));
						strXML.AppendFormat ("<sortorderindex>{0}</sortorderindex>", XmlUtils.XMLEncode (objDocument.SortOrderIndex.ToString ()));
                        strXML.AppendFormat ("<linkattributes>{0}</linkattributes>", XmlUtils.XMLEncode (objDocument.LinkAttributes));

						// Export Url Tracking options too
						UrlController objUrlController = new UrlController ();
						UrlTrackingInfo objUrlTrackingInfo = objUrlController.GetUrlTracking (objModule.PortalID, objDocument.Url, ModuleID);

						if ((objUrlTrackingInfo != null))
						{
							strXML.AppendFormat ("<logactivity>{0}</logactivity>", XmlUtils.XMLEncode (objUrlTrackingInfo.LogActivity.ToString ()));
							strXML.AppendFormat ("<trackclicks>{0}</trackclicks>", XmlUtils.XMLEncode (objUrlTrackingInfo.TrackClicks.ToString ()));
							strXML.AppendFormat ("<newwindow>{0}</newwindow>", XmlUtils.XMLEncode (objUrlTrackingInfo.NewWindow.ToString ()));
						}
						strXML.Append ("</document>");
					}
				}

				var settings = new DocumentsSettings (objModule);
				strXML.Append ("<settings>");
				strXML.AppendFormat ("<allowusersort>{0}</allowusersort>", XmlUtils.XMLEncode (settings.AllowUserSort.ToString ()));
				strXML.AppendFormat ("<showtitlelink>{0}</showtitlelink>", XmlUtils.XMLEncode (settings.ShowTitleLink.ToString ()));
				strXML.AppendFormat ("<usecategorieslist>{0}</usecategorieslist>", XmlUtils.XMLEncode (settings.UseCategoriesList.ToString ()));
				strXML.AppendFormat ("<categorieslistname>{0}</categorieslistname>", XmlUtils.XMLEncode (settings.CategoriesListName));
				strXML.AppendFormat ("<defaultfolder>{0}</defaultfolder>", XmlUtils.XMLEncode (settings.DefaultFolder.ToString()));
				strXML.AppendFormat ("<displaycolumns>{0}</displaycolumns>", XmlUtils.XMLEncode (settings.DisplayColumns));
				strXML.AppendFormat ("<sortorder>{0}</sortorder>", XmlUtils.XMLEncode (settings.SortOrder));
				strXML.Append ("</settings>");
			}
			catch
			{
				// Catch errors but make sure XML is valid
			}
			finally
			{
				strXML.Append ("</documents>");
			}

			return strXML.ToString ();

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
		public void ImportModule (int ModuleID, string Content, string Version, int UserId)
		{
			ModuleController objModules = new ModuleController ();
			ModuleInfo objModule = objModules.GetModule (ModuleID, Null.NullInteger);
			
			// XmlNode xmlDocument = default(XmlNode);
			string strUrl = string.Empty;
			XmlNode xmlDocuments = Globals.GetContent (Content, "documents");
			XmlNodeList documentNodes = xmlDocuments.SelectNodes ("document");
			foreach (XmlNode xmlDocument in documentNodes)
			{
				DocumentInfo objDocument = new DocumentInfo ();
				objDocument.ModuleId = ModuleID;
				objDocument.Title = xmlDocument ["title"].InnerText;
				strUrl = xmlDocument ["url"].InnerText;

                objDocument.Url = strUrl.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase) ?
                    strUrl : Globals.ImportUrl (ModuleID, strUrl);

				objDocument.Category = xmlDocument ["category"].InnerText;
				objDocument.Description = XmlUtils.GetNodeValue (xmlDocument, "description");
				objDocument.OwnedByUserId = XmlUtils.GetNodeValueInt (xmlDocument, "ownedbyuserid");
				objDocument.SortOrderIndex = XmlUtils.GetNodeValueInt (xmlDocument, "sortorderindex");
                objDocument.LinkAttributes = XmlUtils.GetNodeValue (xmlDocument, "linkattributes");
                objDocument.ForceDownload = XmlUtils.GetNodeValueBoolean (xmlDocument, "forcedownload");
                objDocument.IsPublished = XmlUtils.GetNodeValueBoolean (xmlDocument, "ispublished");

				objDocument.CreatedByUserId = UserId;
				objDocument.ModifiedByUserId = UserId;
				
				var now = DateTime.Now;
				objDocument.CreatedDate = now;
				objDocument.ModifiedDate = now;

				Add<DocumentInfo> (objDocument);

				// Update Tracking options
                var urlType = objDocument.Url.StartsWith ("fileid=", StringComparison.InvariantCultureIgnoreCase)? "F" : "U";

				UrlController urlController = new UrlController ();
				// If nodes not found, all values will be false
				urlController.UpdateUrl (objModule.PortalID, objDocument.Url, urlType, XmlUtils.GetNodeValueBoolean (xmlDocument, "logactivity"), XmlUtils.GetNodeValueBoolean (xmlDocument, "trackclicks", true), ModuleID, XmlUtils.GetNodeValueBoolean (xmlDocument, "newwindow"));
			}

			XmlNode xmlDocumentsSettings = Globals.GetContent (Content, "documents/settings");
			if (xmlDocumentsSettings != null)
			{
				var settings = new DocumentsSettings (objModule);
			
				settings.AllowUserSort = XmlUtils.GetNodeValueBoolean (xmlDocumentsSettings, "allowusersort");
				settings.ShowTitleLink = XmlUtils.GetNodeValueBoolean (xmlDocumentsSettings, "showtitlelink");
				settings.UseCategoriesList = XmlUtils.GetNodeValueBoolean (xmlDocumentsSettings, "usecategorieslist");
				settings.CategoriesListName = XmlUtils.GetNodeValue (xmlDocumentsSettings, "categorieslistname");
				settings.DefaultFolder = Utils.ParseToNullableInt (XmlUtils.GetNodeValue (xmlDocumentsSettings, "defaultfolder"));
				settings.DisplayColumns = XmlUtils.GetNodeValue (xmlDocumentsSettings, "displaycolumns");
				settings.SortOrder = XmlUtils.GetNodeValue (xmlDocumentsSettings, "sortorder");

				// Need Utils.SynchronizeModule() call
			}

		}

		#endregion
	}
}

