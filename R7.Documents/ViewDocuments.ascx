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
          <asp:hyperlink id=editLink runat="server" visible="<%# IsEditable %>" navigateurl='<%# EditUrl("ItemID",DataBinder.Eval(Container.DataItem,"ItemID").ToString()) %>'>
            <asp:image id="editLinkImage" imageurl="/icons/Sigma/Edit_16X16_Standard.png" visible="<%# IsEditable %>" alternatetext="Edit" runat="server" resourcekey="Edit" />
          </asp:hyperlink>
        </ItemTemplate>
      </asp:TemplateColumn>
    </Columns>
  </asp:datagrid>
  </div>
