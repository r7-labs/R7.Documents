<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ImportDocuments.ascx.cs" Inherits="R7.Documents.ImportDocuments" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
	<fieldset>
		<div class="dnnFormItem">
			<dnn:Label id="labelModule" runat="server" ControlName="comboModule" Suffix=":" />
			<dnn:DnnComboBox id="comboModule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="comboModules_SelectedIndexChanged" /> 
		</div>
		<asp:Panel id="panelDocuments" runat="server" CssClass="dnnFormItem" Visible="false">
			<dnn:Label id="labelDocuments" runat="server" ControlName="listDocuments" Suffix=":" />
			<asp:CheckBoxList id="listDocuments" runat="server" DataTextField="Title" DataValueField="ItemID" />
		</asp:Panel>	
		<ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="buttonImport" runat="server" CssClass="dnnPrimaryAction" resourcekey="buttonImport.Text" Visible="false" Text="Import" OnClick="buttonImport_Click" /></li>
	        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Text="Cancel" /></li>
	    </ul>
	</fieldset>
</div>
