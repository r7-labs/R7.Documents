<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ImportDocuments.ascx.cs" Inherits="R7.Documents.ImportDocuments" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="dnnForm dnnClear">
	<fieldset>
		<div class="dnnFormItem">
			<dnn:Label id="labelModule" runat="server" ControlName="comboModule" Suffix=":" />
			<dnn:DnnComboBox id="comboModule" runat="server" AutoPostBack="true" /> 
		</div>
		<asp:Panel id="panelDocuments" runat="server" CssClass="dnnFormItem" Visible="false">
			<dnn:Label id="labelDocuments" runat="server" ControlName="listDocuments" Suffix=":" />
			<asp:CheckBoxList id="listDocuments" runat="server" DataTextField="Title" DataValueField="ItemID" />
		</asp:Panel>	
		<ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="buttonImport" runat="server" CssClass="dnnPrimaryAction" resourcekey="buttonImport.Text" Visible="false" Text="Import" /></li>
	        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Text="Cancel" /></li>
	    </ul>
	</fieldset>
</div>
