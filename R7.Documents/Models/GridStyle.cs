//
// GridStyle.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2015-2018 Roman M. Yagodin 
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
using System.Web.UI.WebControls;

namespace R7.Documents.Models
{
    /// <summary>
    /// Grid style.
    /// </summary>
    public class GridStyle
    {
        public string Name { get; set; }

        public string GridCssClass { get; set; }

        public string HeaderCssClass { get; set; }

        public string FooterCssClass { get; set; }

        public string ItemCssClass { get; set; }

        public string AltItemCssClass { get; set; }

        public string GridLines { get; set; }

        public string Width { get; set; }

        // not used:
        // public string EditItemCssClass;
        // public string SelectedItemCssClass;
        // public string PagerCssClass;

        public void ApplyToGrid (GridView grid)
        {
            grid.CssClass = GridCssClass;
            grid.HeaderStyle.CssClass = HeaderCssClass;
            grid.FooterStyle.CssClass = FooterCssClass;
            grid.RowStyle.CssClass = ItemCssClass;
            grid.AlternatingRowStyle.CssClass = AltItemCssClass;
            grid.GridLines = (GridLines) Enum.Parse (typeof (GridLines), GridLines);
            grid.Width = Unit.Parse (Width);
        }

        public void ApplyToGrid (DataGrid grid)
        {
            grid.CssClass = GridCssClass;
            grid.HeaderStyle.CssClass = HeaderCssClass;
            grid.FooterStyle.CssClass = FooterCssClass;
            grid.ItemStyle.CssClass = ItemCssClass;
            grid.AlternatingItemStyle.CssClass = AltItemCssClass;
            grid.GridLines = (GridLines) Enum.Parse (typeof (GridLines), GridLines);
            grid.Width = Unit.Parse (Width);
        }
    }
}
