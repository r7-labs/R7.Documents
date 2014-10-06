<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDocuments.ascx.cs" Inherits="R7.Documents.ViewDocuments" %>
<div class="ViewDocuments">
  <asp:datagrid id="grdDocuments" runat="server" datakeyfield="ItemID" enableviewstate="False" autogeneratecolumns="False" GridLines="None" CssClass="dnnGrid" Width="100%">
    <headerstyle cssclass="dnnGridHeader" verticalalign="Top" />
    <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
    <alternatingitemstyle cssclass="dnnGridAltItem" />
    <edititemstyle cssclass="dnnFormInput" />
    <selecteditemstyle cssclass="dnnFormError" />
    <footerstyle cssclass="dnnGridFooter" />
    <pagerstyle cssclass="dnnGridPager" />
    <Columns>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:HyperLink id="linkEdit" runat="server" Visible="<%# IsEditable %>" 
				NavigateUrl='<%# EditUrl("ItemID",DataBinder.Eval(Container.DataItem,"ItemID").ToString()) %>' >
            <asp:Image id="imageEdit" runat="server" ImageUrl="<%# EditImageUrl %>" 
				ToolTip='<%# DataBinder.Eval(Container.DataItem,"Info").ToString() %>' />
          </asp:HyperLink>
      	</ItemTemplate>
      </asp:TemplateColumn>
    </Columns>
  </asp:datagrid>
</div>
