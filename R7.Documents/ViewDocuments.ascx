<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewDocuments.ascx.cs" Inherits="R7.Documents.ViewDocuments" %>
<div class="ViewDocuments">
  <asp:GridView id="grdDocuments" runat="server" EnableViewState="false"
				AutoGenerateColumns="false" UseAccessibleHeader="true"
				ItemType="R7.Documents.ViewModels.DocumentViewModel" DataKeyField="ItemId"
				OnSorting="grdDocuments_Sorting" OnRowCreated="grdDocuments_RowCreated">
    <Columns>
      <asp:TemplateField>
        <ItemTemplate>
          <asp:HyperLink id="linkEdit" runat="server" Visible="<%# IsEditable && Item.ItemId > 0 %>"
						 NavigateUrl='<%# EditUrl ("ItemID", Item.ItemId.ToString ()) %>'>
            <asp:Image id="imageEdit" runat="server" IconKey="Edit" IconStyle="Gray" ToolTip='<%# Item.ToolTip %>' />
          </asp:HyperLink>
      	</ItemTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
</div>
