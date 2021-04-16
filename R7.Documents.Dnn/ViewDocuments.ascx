<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewDocuments.ascx.cs" Inherits="R7.Documents.ViewDocuments" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents.Dnn/js/selectDocuments.js" ForceProvider="DnnFormBottomProvider" />
<div class="r7-docs">
  <asp:GridView id="grdDocuments" runat="server" EnableViewState="false"
				AutoGenerateColumns="false" UseAccessibleHeader="true"
				ItemType="R7.Documents.ViewModels.DocumentViewModel" DataKeyField="ItemId"
				OnSorting="grdDocuments_Sorting" OnRowCreated="grdDocuments_RowCreated">
    <Columns>
      <asp:TemplateField>
        <ItemTemplate>
          <asp:HyperLink runat="server" Visible="<%# IsEditable && Item.ItemId >= 0 %>"
						 NavigateUrl='<%# EditUrl ("ItemID", Item.ItemId.ToString ()) %>'>
            <asp:Image id="imageEdit" runat="server" IconKey="Edit" IconStyle="Gray" ToolTip='<%# Item.ToolTip %>' />
			</asp:HyperLink>
			<input type="checkbox" runat="server" title='<%# LocalizeString("SelectUnselectDocument.Text") %>'
				data-document-id="<%# Item.ItemId %>" onchange="r7d_selectDocument2(this)" />
        </ItemTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
  <asp:HiddenField id="hiddenSelectedDocuments" runat="server" Value="[]" />
</div>
