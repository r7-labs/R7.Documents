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
using DotNetNuke;
using DotNetNuke.Framework;

namespace R7.Documents
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// The DataProvider Class Is an abstract class that provides the DataLayer
	/// for the Documents Module.
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// 	[cnurse]	9/22/2004	Moved Documents to a separate Project
	/// </history>
	/// -----------------------------------------------------------------------------
	public abstract class DataProvider
	{

		#region "Shared/Static Methods"

		// singleton reference to the instantiated object 

		private static DataProvider objProvider = null;
		// constructor
		static DataProvider()
		{
			CreateProvider();
		}

		// dynamically create provider
		private static void CreateProvider()
		{
			objProvider = (DataProvider)Reflection.CreateObject("data", "R7.Documents", "");
		}

		// return the provider
		public static new DataProvider Instance()
		{
			return objProvider;
		}

		#endregion

		#region "Abstract methods"

		public abstract int AddDocument(int ModuleId, string Title, string URL, int UserId, int OwnedByUserID, string Category, int SortOrderIndex, string Description, bool ForceDownload);
		public abstract void DeleteDocument(int ModuleId, int ItemID);
		public abstract IDataReader GetDocument(int ItemId, int ModuleId);
		public abstract IDataReader GetDocuments(int ModuleId, int PortalId);
		public abstract void UpdateDocument(int ModuleId, int ItemId, string Title, string URL, int UserId, int OwnedByUserID, string Category, int SortOrderIndex, string Description, bool ForceDownload);

		public abstract int AddDocumentsSettings(int ModuleId, bool ShowTitleLink, string SortOrder, string DisplayColumns, bool UseCategoriesList, string DefaultFolder, string CategoriesListName, bool AllowUserSort);
		public abstract void DeleteDocumentsSettings(int ModuleID);
		public abstract IDataReader GetDocumentsSettings(int ModuleId);
		public abstract void UpdateDocumentsSettings(int ModuleId, bool ShowTitleLink, string SortOrder, string DisplayColumns, bool UseCategoriesList, string DefaultFolder, string CategoriesListName, bool AllowUserSort);

		#endregion

	}

}
