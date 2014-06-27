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
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace R7.Documents
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// The SqlDataProvider Class is an SQL Server implementation of the DataProvider Abstract
	/// class that provides the DataLayer for the Documents Module.
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
	/// </history>
	/// -----------------------------------------------------------------------------

	public class SqlDataProvider : DataProvider
	{
		#region "Private Members"

		private const string ProviderType = "data";
		private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
		private string _connectionString;
		private string _providerPath;
		private string _objectQualifier;

		private string _databaseOwner;
		#endregion

		#region "Constructors"

		public SqlDataProvider()
		{
			// Read the configuration specific information for this provider
			Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

			// This code handles getting the connection string from either the connectionString / appsetting section and uses the connectionstring section by default if it exists.  
			// Get Connection string from web.config
			_connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

			// If above funtion does not return anything then connectionString must be set in the dataprovider section.
			if (_connectionString == string.Empty) {
				// Use connection string specified in provider
				_connectionString = objProvider.Attributes["connectionString"];
			}

			_providerPath = objProvider.Attributes["providerPath"];

			_objectQualifier = objProvider.Attributes["objectQualifier"];
			if (_objectQualifier != string.Empty & _objectQualifier.EndsWith("_") == false) {
				_objectQualifier += "_";
			}

			_databaseOwner = objProvider.Attributes["databaseOwner"];
			if (!string.IsNullOrEmpty(_databaseOwner) & _databaseOwner.EndsWith(".") == false) {
				_databaseOwner += ".";
			}

		}

		#endregion

		#region "Properties"

		public string ConnectionString {
			get { return _connectionString; }
		}

		public string ProviderPath {
			get { return _providerPath; }
		}

		public string ObjectQualifier {
			get { return _objectQualifier; }
		}

		public string DatabaseOwner {
			get { return _databaseOwner; }
		}

		#endregion

		#region "Public Methods"

		private object GetNull(object Field)
		{
			return Null.GetNull(Field, DBNull.Value);
		}

		public override int AddDocument(int ModuleId, string Title, string URL, int UserId, int OwnedByUserID, string Category, int SortOrderIndex, string Description, bool ForceDownload)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddDocument", ModuleId, Title, URL, UserId, OwnedByUserID, Category, SortOrderIndex, Description,
			ForceDownload));
		}

		public override void DeleteDocument(int ModuleId, int ItemId)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDocument", ModuleId, ItemId);
		}

		public override IDataReader GetDocument(int ItemId, int ModuleId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDocument", ItemId, ModuleId);
		}

		public override IDataReader GetDocuments(int ModuleId, int PortalId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDocuments", ModuleId, PortalId);
		}

		public override void UpdateDocument(int moduleId, int ItemId, string Title, string URL, int UserId, int OwnedByUserID, string Category, int SortOrderIndex, string Description, bool ForceDownload)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDocument", moduleId, ItemId, Title, URL, UserId, Category, OwnedByUserID, SortOrderIndex,
			Description, ForceDownload);
		}


		public override int AddDocumentsSettings(int ModuleId, bool ShowTitleLink, string SortOrder, string DisplayColumns, bool UseCategoriesList, string DefaultFolder, string CategoriesListName, bool AllowUserSort)
		{
			return (int)SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddDocumentsSettings", ModuleId, ShowTitleLink, SortOrder, DisplayColumns, UseCategoriesList, DefaultFolder, CategoriesListName, AllowUserSort);
		}

		public override void DeleteDocumentsSettings(int ModuleID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDocumentsSettings", ModuleID);
		}

		public override System.Data.IDataReader GetDocumentsSettings(int ModuleId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDocumentsSettings", ModuleId);
		}

		public override void UpdateDocumentsSettings(int ModuleId, bool ShowTitleLink, string SortOrder, string DisplayColumns, bool UseCategoriesList, string DefaultFolder, string CategoriesListName, bool AllowUserSort)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDocumentsSettings", ModuleId, ShowTitleLink, SortOrder, DisplayColumns, UseCategoriesList, DefaultFolder, CategoriesListName, AllowUserSort);
		}

		#endregion

	}

}
