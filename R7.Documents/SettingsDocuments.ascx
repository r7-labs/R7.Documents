<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SettingsDocuments.ascx.cs" Inherits="R7.Documents.SettingsDocuments" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents/admin.css" Priority="200" />
<div class="dnnForm dnnDocSettings" id="dnnDocSettings">
	<fieldset>
	    <div class="dnnFormItem">
	        <dnn:Label id="plUseCategoriesList" runat="server" ControlName="chkUseCategoriesList" />
	        <asp:CheckBox id="chkUseCategoriesList" runat="server" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="plCategoriesListName" runat="server" Controlname="cboCategoriesList" />
	        <div style="float:left">
	        	<asp:DropDownList ID="cboCategoriesList" runat="server" Style="width:340px;margin-right:0.5em" />
	       		<asp:Hyperlink ID="lnkEditLists" runat="server" CssClass="dnnSecondaryAction" />
	        	<asp:Label ID="lstNoListsAvailable" runat="server" CssClass="dnnFormMessage dnnFormInfo" Style="max-width:400px" Visible="false" />
	        	<asp:Label ID="lblCannotEditLists" runat="server" CssClass="dnnFormMessage dnnFormError" Style="max-width:400px" Visible="false" />
	    	</div>
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="plDefaultFolder" runat="server" ControlName="folderDefaultFolder" />
	        <dnn:DnnFolderDropDownList id="folderDefaultFolder" runat="server" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:label id="plShowTitleLink" runat="server" controlname="chkShowTitleLink" />
	        <asp:CheckBox id="chkShowTitleLink" runat="server" />
	    </div>
        <div class="dnnFormItem">
            <dnn:label id="labelGridStyle" runat="server" controlname="comboGridStyle" />
            <asp:DropDownList id="comboGridStyle" runat="server" DataTextField="Name" DataValueField="Name" />
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="plDisplayColumns" runat="server" controlname="grdColumns" />
	        <asp:DataGrid id="grdDisplayColumns" runat="server" AutoGenerateColumns="False" GridLines="None" Width="350px" CssClass="dnnGrid"
                OnItemCreated="grdDisplayColumns_ItemCreated" OnItemCommand="grdDisplayColumns_ItemCommand">
	            <HeaderStyle CssClass="dnnGridHeader" VerticalAlign="Top" />
	            <ItemStyle CssClass="dnnGridItem" HorizontalAlign="Left" />
	            <AlternatingItemStyle CssClass="dnnGridAltItem" />
	            <EditItemStyle CssClass="dnnFormInput" />
	            <SelectedItemstyle CssClass="dnnFormError" />
	            <FooterStyle CssClass="dnnGridFooter" />
	            <PagerStyle CssClass="dnnGridPager" />
	            <Columns>
	                <asp:BoundColumn DataField="LocalizedColumnName" HeaderText="Name" />
	                <asp:TemplateColumn HeaderText="Visible">
	                    <HeaderStyle Width="60px"></HeaderStyle>
	                    <ItemTemplate>
	                        <asp:CheckBox id="chkVisible" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "Visible") %>' />
	                    </ItemTemplate>
	                </asp:TemplateColumn>
	                <asp:TemplateColumn>
	                    <HeaderStyle Width="60px"></HeaderStyle>
	                    <ItemTemplate>
	                        <asp:ImageButton id=imgUp runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ColumnName") %>' CommandName="DisplayOrderUp" AlternateText='<%# GetLocalizedText("cmdUp.Text")%>' />
	                        <asp:ImageButton id=imgDown runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ColumnName") %>' CommandName="DisplayOrderDown" AlternateText='<%# GetLocalizedText("cmdDown.Text")%>' />
	                    </ItemTemplate>
	                </asp:TemplateColumn>
	            </Columns>
	      </asp:DataGrid>
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="plSorting" runat="server" ControlName="comboSortFields" />
	        <asp:DropDownList id="comboSortFields" runat="server" CssClass="comboSortFields" />
		</div>
	    <div class="dnnFormItem">
	        <label class="dnnLabel"></label>
	        <asp:DropDownList id="comboSortOrderDirection" runat="server" CssClass="comboSortOrderDirection" />
	        <asp:LinkButton id="lnkAddSortColumn" runat="server" resourcekey="cmdAdd" CssClass="dnnSecondaryAction" 
                OnClick="lnkAddSortColumn_Click" />
	    </div>
	    <div class="dnnFormItem">
	        <label class="dnnLabel"></label>
	        <asp:DataGrid id="grdSortColumns" runat="server" GridLines="None" AutoGenerateColumns="False" ShowHeader="False" Width="400px" CssClass="dnnGrid"
                OnItemCreated="grdSortColumns_ItemCreated" OnDeleteCommand="grdSortColumns_DeleteCommand">
	            <HeaderStyle CssClass="dnnGridHeader" VerticalAlign="Top" />
	            <ItemStyle CssClass="dnnGridItem" HorizontalAlign="Left" />
	            <AlternatingItemStyle CssClass="dnnGridAltItem" />
	            <EditItemStyle CssClass="dnnFormInput" />
	            <SelectedItemStyle CssClass="dnnFormError" />
	            <FooterStyle CssClass="dnnGridFooter" />
	            <PagerStyle CssClass="dnnGridPager" />
	            <Columns>
	                <asp:BoundColumn DataField="LocalizedColumnName" HeaderText="Name" />
	                <asp:BoundColumn DataField="Direction" HeaderText="DirectionString" />
	            	<asp:TemplateColumn>
						<ItemTemplate>
							<asp:ImageButton id="buttonDeleteSortOrder" runat="server" CommandName="Delete" />
						</ItemTemplate>
	                </asp:TemplateColumn>
	            </Columns>
	      </asp:DataGrid>
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="plAllowUserSort" runat="server" ControlName="chkAllowUserSort" />
	        <asp:CheckBox id="chkAllowUserSort" runat="server" />
	    </div>
	</fieldset>
</div>