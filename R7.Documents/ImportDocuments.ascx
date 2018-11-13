<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ImportDocuments.ascx.cs" Inherits="R7.Documents.ImportDocuments" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
	<fieldset>
		<div class="dnnFormItem">
			<dnn:Label id="lblModule" runat="server" ControlName="ddlModule" />
			<asp:DropDownList id="ddlModule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged" />
		</div>
		<asp:Panel id="plDocuments" runat="server" CssClass="dnnFormItem" Visible="false">
			<dnn:Label id="lblDocuments" runat="server" ControlName="lstDocuments" />
			<asp:CheckBoxList id="lstDocuments" runat="server" DataTextField="Title" DataValueField="ItemID" />
		</asp:Panel>
		<ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="btnImport" runat="server" CssClass="dnnPrimaryAction" resourcekey="btnImport.Text" Visible="false" OnClick="btnImport_Click" /></li>
	        <li><asp:HyperLink id="lnkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
	    </ul>
	</fieldset>
</div>
