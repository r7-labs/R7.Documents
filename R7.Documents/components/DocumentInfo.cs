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
using DotNetNuke.Services.Localization;

namespace R7.Documents
{
	/// <summary>
	/// Holds the information about a single document
	/// </summary>
	public class DocumentInfo
	{

		#region "Private Members"

		private int _clicks;

		#endregion
	

		#region "Auto Implemented Properties"

		// public properties
		public int ItemId { get; set; }
		public int ModuleId { get; set; }
		public int CreatedByUserId { get; set; }
		public string CreatedByUser { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public string Url { get; set; }
		public string Title { get; set; }
		public string Category { get; set; }
		public int Size { get; set; }
		public string ContentType { get; set; }
		public bool TrackClicks { get; set; }
		public bool NewWindow { get; set; }
		public int OwnedByUserId { get; set; }
		public string OwnedByUser { get; set; }
		public int ModifiedByUserId { get; set; }
		public string ModifiedByUser { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int SortOrderIndex { get; set; }
		public string Description { get; set; }
		public bool ForceDownload { get; set; }
		#endregion





		#region "Custom Properties"

		/// <summary>
		/// Gets the size of the format.
		/// </summary>
		/// <value>The size of the format.</value>
		public string FormatSize {
			get {
				try {
					if (!DotNetNuke.Common.Utilities.Null.IsNull(Size)) {
						if (Size > (1024 * 1024)) {
							return String.Format("#,##0.00 MB", Size / (1024 * 1024));
						} else {
							return String.Format("#,##0.00 KB", Size / 1024);
						}
					} else {
						return Localization.GetString("Unknown");
					}
				} catch (Exception exc) {
					return Localization.GetString("Unknown");
				}
			}
		}

		/// <summary>
		/// Gets or sets the clicks.
		/// </summary>
		/// <value>The clicks.</value>
		public int Clicks {
			get {
				if (_clicks < 0) {
					return 0;
				} else {
					return _clicks;
				}

			}
			set { _clicks = value; }
		}
		#endregion


	}

}
