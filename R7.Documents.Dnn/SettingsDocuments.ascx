<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SettingsDocuments.ascx.cs" Inherits="R7.Documents.SettingsDocuments" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents.Dnn/admin.css" Priority="200" />
<div class="dnnForm dnnDocSettings" id="dnnDocSettings">
	<fieldset>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblUseCategoriesList" runat="server" ControlName="chkUseCategoriesList" />
	        <asp:CheckBox id="chkUseCategoriesList" runat="server" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblCategoriesListName" runat="server" Controlname="ddlCategoriesList" />
	        <div style="float:left">
	        	<asp:DropDownList ID="ddlCategoriesList" runat="server" Style="width:340px;margin-right:0.5em" />
	       		<asp:Hyperlink ID="lnkEditLists" runat="server" CssClass="dnnSecondaryAction" />
	        	<asp:Label ID="lblNoListsAvailable" runat="server" CssClass="dnnFormMessage dnnFormInfo" Style="max-width:400px" Visible="false" />
	        	<asp:Label ID="lblCannotEditLists" runat="server" CssClass="dnnFormMessage dnnFormError" Style="max-width:400px" Visible="false" />
	    	</div>
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblDefaultFolder" runat="server" ControlName="ddlDefaultFolder" />
	        <dnn:DnnFolderDropDownList id="ddlDefaultFolder" runat="server" />
	    </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblFileFilter" runat="server" ControlName="txtFileFilter" />
            <asp:TextBox id="txtFileFilter" runat="server" />
			<asp:CustomValidator id="valFileFilter" runat="server" ControlToValidate="txtFileFilter"
								 ValidateEmptyText="false" OnServerValidate="valFileFilter_ServerValidate"
								 Display="Dynamic" CssClass="dnnFormMessage dnnFormError" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblFilenameToTitleRules" runat="server" ControlName="txtFilenameToTitleRules" />
            <asp:TextBox id="txtFilenameToTitleRules" runat="server" TextMode="MultiLine" />
			<asp:CustomValidator id="valFilenameToTitleRules" runat="server" ControlToValidate="txtFilenameToTitleRules"
								 ValidateEmptyText="false" OnServerValidate="valFilenameToTitleRules_ServerValidate"
								 Display="Dynamic" CssClass="dnnFormMessage dnnFormError" />
		</div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblShowTitleAsLink" runat="server" controlname="chkShowTitleAsLink" />
	        <asp:CheckBox id="chkShowTitleAsLink" runat="server" />
	    </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblGridStyle" runat="server" controlname="ddlGridStyle" />
            <asp:DropDownList id="ddlGridStyle" runat="server" DataTextField="Name" DataValueField="Name" />
        </div>
		<div class="dnnFormItem">
            <dnn:Label id="lblDateTimeFormat" runat="server" ControlName="txtDateTimeFormat" />
            <asp:TextBox id="txtDateTimeFormat" runat="server" />
        </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblDisplayColumns" runat="server" controlname="grdColumns" />
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
	                        <asp:ImageButton id="imgUp" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ColumnName") %>' CommandName="DisplayOrderUp" AlternateText='<%# LocalizeString("cmdUp.Text")%>' />
	                        <asp:ImageButton id="imgDown" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ColumnName") %>' CommandName="DisplayOrderDown" AlternateText='<%# LocalizeString("cmdDown.Text")%>' />
	                    </ItemTemplate>
	                </asp:TemplateColumn>
	            </Columns>
	      </asp:DataGrid>
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblSorting" runat="server" ControlName="ddlSortFields" />
	        <asp:DropDownList id="ddlSortFields" runat="server" CssClass="ddlSortFields" />
		</div>
	    <div class="dnnFormItem">
	        <label class="dnnLabel"></label>
	        <asp:RadioButtonList id="rblSortOrderDirection" runat="server" CssClass="rblSortOrderDirection" RepeatDirection="Horizontal" />
	    </div>
	    <div class="dnnFormItem">
			<label class="dnnLabel"></label>
			<asp:LinkButton id="lnkAddSortColumn" runat="server" resourcekey="lnkAddSortColumn" CssClass="dnnSecondaryAction" OnClick="lnkAddSortColumn_Click" />
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
	                <asp:BoundColumn DataField="LocalizedDirection" HeaderText="DirectionString" />
	            	<asp:TemplateColumn>
						<ItemTemplate>
							<asp:ImageButton id="btnDeleteSortOrder" runat="server" CommandName="Delete" />
						</ItemTemplate>
	                </asp:TemplateColumn>
	            </Columns>
	      </asp:DataGrid>
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblAllowUserSort" runat="server" ControlName="chkAllowUserSort" />
	        <asp:CheckBox id="chkAllowUserSort" runat="server" />
	    </div>
		 <div class="dnnFormItem">
            <dnn:Label id="lblAutoHide" runat="server" ControlName="chkAutoHide" />
            <asp:CheckBox id="chkAutoHide" runat="server" />
        </div>
	</fieldset>
</div>
