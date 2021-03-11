using System;
using System.Web.UI.WebControls;
using R7.Documents.Models;

namespace R7.Documents.Components
{
    public class GridStyleApplicator
    {
        public void Apply (GridView grid, GridStyle gridStyle)
        {
            grid.CssClass = gridStyle.GridCssClass;
            grid.HeaderStyle.CssClass = gridStyle.HeaderCssClass;
            grid.FooterStyle.CssClass = gridStyle.FooterCssClass;
            grid.RowStyle.CssClass = gridStyle.ItemCssClass;
            grid.AlternatingRowStyle.CssClass = gridStyle.AltItemCssClass;
            grid.GridLines = (GridLines) Enum.Parse (typeof (GridLines), gridStyle.GridLines);
            grid.Width = Unit.Parse (gridStyle.Width);
        }

        public void Apply (DataGrid grid, GridStyle gridStyle)
        {
            grid.CssClass = gridStyle.GridCssClass;
            grid.HeaderStyle.CssClass = gridStyle.HeaderCssClass;
            grid.FooterStyle.CssClass = gridStyle.FooterCssClass;
            grid.ItemStyle.CssClass = gridStyle.ItemCssClass;
            grid.AlternatingItemStyle.CssClass = gridStyle.AltItemCssClass;
            grid.GridLines = (GridLines) Enum.Parse (typeof (GridLines), gridStyle.GridLines);
            grid.Width = Unit.Parse (gridStyle.Width);
        }
    }
}
