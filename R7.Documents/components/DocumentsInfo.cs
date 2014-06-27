//
// DocumentsInfo.cs
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
using System.Linq;

using DotNetNuke.Data;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace R7.Documents
{
	// More attributes for class:
	// Set caching for table: [Cacheable("Documents_DocumentsInfos", CacheItemPriority.Default, 20)]
	// Explicit mapping declaration: [DeclareColumns]
	
	// More attributes for class properties:
	// Custom column name: [ColumnName("DocumentsID")]
	// Explicit include column: [IncludeColumn]
	// Note: DAL 2 has no AutoJoin analogs from PetaPOCO at this time
	
	[TableName ("Documents_Documentss")]
	[PrimaryKey ("DocumentsID", AutoIncrement = true)]
	[Scope ("ModuleID")]
	public class DocumentsInfo
	{
		#region Fields

		private string createdByUserName = null;

		#endregion

		/// <summary>
		/// Empty default cstor
		/// </summary>
		public DocumentsInfo ()
		{
		}

		#region Properties

		public int DocumentsID { get; set; }

		public int ModuleID { get; set; }

		public string Content { get; set; }

		public int CreatedByUser { get; set; }

		[ReadOnlyColumn]
		public DateTime CreatedOnDate { get; set; }

		[IgnoreColumn]
		public string CreatedByUserName
		{
			get
			{
				if (createdByUserName == null)
				{
					var portalId = PortalController.GetCurrentPortalSettings ().PortalId;
					var user = UserController.GetUserById (portalId, CreatedByUser);
					createdByUserName = user.DisplayName;
				}

				return createdByUserName; 
			}
		}

		#endregion

		/* // Joins example
     	
     	// foreign key
     	public int AnotherID { get; set; }
     	
     	// private object reference
     	private AnotherInfo _another;
     	
     	// public object reference
     	public AnotherInfo Another 
     	{
     	   	// this getter method hide underlying access to database, 
     	   	// and perform simple caching by storing reference
     	   	// to retrived AnotherInfo object in a private field "_another"
     		get 
     		{
     			if (_other == null)
     			{
     				// load joined object to reference it
     				var ctrl = new DocumentsController();
     				_another = ctrl.Get<AnotherInfo>(AnotherID);
     			}
     			return _another;	
     		}
     		set 
     		{
     			_another = value;
     		}
     	}      
     	
     	/// <summary>
     	/// Nullifies all private fields with references to joined objects,
     	/// so next access to corresponding object properties 
     	/// results in reloading them from the database  
     	/// </summary>
     	public void ResetJoins ()
     	{
     		_another = null;
     	}
        
        // Now we have ability to use DocumentsInfo objects
        // to access members of joined AnotherInfo objects 
        
       	// Get DocumentsInfo object by it's primary key (ID):
       	// var ctrl = new DocumentsController();
     	// var item = ctrl.Get<DocumentsInfo>(itemId);
     	
     	// Now simply get data from another table:
     	// Console.WriteLine(item.Another.SomeProperty);
     	
        // True is, that it is not very effective way to retrieve multiple objects, 
        // but it is 1) simple and 2) object-oriented, so then PetaPOCO AutoJoin 
        // attribute will be included in DAL 2, existing business logic code 
        // can be upgraded with almost no efforts.
       
        */
	}
}

