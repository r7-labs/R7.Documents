<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingsDocuments.ascx.cs" Inherits="R7.Documents.SettingsDocuments" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnDocSettings" id="dnnDocSettings">
    <div class="dnnFormItem">
        <dnn:label id="plUseCategoriesList" runat="server" controlname="chkUseCategoriesList" suffix="" />
        <asp:CheckBox id="chkUseCategoriesList" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plCategoriesListName" runat="server" controlname="cboCategoriesList" suffix="" />
         <asp:DropDownList ID="cboCategoriesList" runat="server" />
         <asp:Label ID="lstNoListsAvailable" runat="server" Visible="False" />
         <asp:Hyperlink ID="lnkEditLists" runat="server" CssClass="dnnSecondaryAction" />
         <asp:Label ID="lblCannotEditLists" runat="server" Visible="false" CssClass="NormalRed" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plDefaultFolder" runat="server" controlname="cboDefaultFolder" suffix="" />
        <asp:DropDownList ID="cboDefaultFolder" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plShowTitleLink" runat="server" controlname="chkShowTitleLink" suffix="" />
        <asp:CheckBox id="chkShowTitleLink" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plDisplayColumns" runat="server" controlname="grdColumns" suffix=":" />
        <asp:DataGrid id="grdDisplayColumns" runat="server" AutoGenerateColumns="False" GridLines="None" Width="350px" CssClass="dnnGrid">
            <headerstyle cssclass="dnnGridHeader" verticalalign="Top" />
            <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
            <alternatingitemstyle cssclass="dnnGridAltItem" />
            <edititemstyle cssclass="dnnFormInput" />
            <selecteditemstyle cssclass="dnnFormError" />
            <footerstyle cssclass="dnnGridFooter" />
            <pagerstyle cssclass="dnnGridPager" />
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
        <dnn:label id="plSorting" runat="server" controlname="" suffix=":" />
        <asp:DropDownList id="lstSortFields" runat="server" Style="width:250px" />
	</div>
    <div class="dnnFormItem">
        <label class="dnnLabel"></label>
        <asp:DropDownList ID="cboSortOrderDirection" runat="server" Style="width:250px" />
        <asp:LinkButton id="lnkAddSortColumn" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdAdd" />
    </div>
    <div class="dnnFormItem">
        <label class="dnnLabel"></label>
        <asp:DataGrid id="grdSortColumns" runat="server" GridLines="None" AutoGenerateColumns="False" ShowHeader="False" Width="400px" CssClass="dnnGrid">
            <headerstyle cssclass="dnnGridHeader" verticalalign="Top" />
            <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
            <alternatingitemstyle cssclass="dnnGridAltItem" />
            <edititemstyle cssclass="dnnFormInput" />
            <selecteditemstyle cssclass="dnnFormError" />
            <footerstyle cssclass="dnnGridFooter" />
            <pagerstyle cssclass="dnnGridPager" />
            <Columns>
                <asp:BoundColumn DataField="LocalizedColumnName" HeaderText="Name" />
                <asp:BoundColumn DataField="Direction" HeaderText="DirectionString" />
                <asp:ButtonColumn Text="Delete" CommandName="Delete" />
            </Columns>
      </asp:DataGrid>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plAllowUserSort" runat="server" controlname="chkAllowUserSort" suffix="" />
        <asp:CheckBox id="chkAllowUserSort" runat="server" />
    </div>
</div>