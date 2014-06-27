//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2013
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
using System.Configuration;
using System.Data;
using DotNetNuke;
using DotNetNuke.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Services.Localization;

namespace R7.Documents
{
	/// <summary>
	/// Holds the information about a single document
	/// </summary>
	[TableName ("Documents")]
	[PrimaryKey ("ItemId", AutoIncrement = true)]
	[Scope ("ModuleId")]
	public class DocumentInfo
	{
		/// <summary>
		/// Empty default cstor
		/// </summary>
		public DocumentInfo ()
		{
		}

		#region Private Members

		private int _clicks;

		#endregion


		#region DB Properties

		public int ItemId { get; set; }

		public int ModuleId { get; set; }

		public int CreatedByUserId { get; set; }

		public int ModifiedByUserId { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime ModifiedDate { get; set; }

		public string Url { get; set; }

		public string Title { get; set; }

		public string Category { get; set; }

		public int OwnedByUserId { get; set; }

		public int SortOrderIndex { get; set; }

		public string Description { get; set; }

		public bool ForceDownload { get; set; }

		#endregion

		#region External properties

		[ReadOnlyColumn]
		public bool TrackClicks { get; set; }

		[ReadOnlyColumn]
		public string ContentType { get; set; }

		[ReadOnlyColumn]
		public bool NewWindow { get; set; }

		[ReadOnlyColumn]
		public int Size { get; set; }

		[ReadOnlyColumn]
		public string CreatedByUser { get; set; }

		[ReadOnlyColumn]
		public string OwnedByUser { get; set; }

		[ReadOnlyColumn]
		public string ModifiedByUser { get; set; }

		#endregion
		
		#region Custom properties
		
		/// <summary>
		/// Gets the size of the format.
		/// </summary>
		/// <value>The size of the format.</value>
		[IgnoreColumn]
		public string FormatSize
		{
			get
			{
				try
				{
					if (!Null.IsNull (Size))
					{
						if (Size > (1024 * 1024))
						{
							return String.Format ("{0:#,##0.00} MB", Size / 1024 / 1024);
						}
						else
						{
							return String.Format ("{0:#,##0.00} KB", Size / 1024);
						}
					}
					else
					{
						return Localization.GetString ("Unknown");
					}
				}
				catch
				{
					return Localization.GetString ("Unknown");
				}
			}
		}

		/// <summary>
		/// Gets or sets the clicks.
		/// </summary>
		/// <value>The clicks.</value>
		[IgnoreColumn]
		public int Clicks
		{
			get { return (_clicks < 0) ? 0 : _clicks; }
			set { _clicks = value; }
		}

		#endregion
	}
}
