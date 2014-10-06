<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChangeFolder.ascx.cs" Inherits="R7.Documents.ChangeFolder" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="dnnForm dnnClear">
	<fieldset>
	    <div class="dnnFormItem">
			<asp:label id="labelInfo" runat="server" resourcekey="labelInfo.Text" CssClass="dnnFormMessage dnnFormInfo" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="labelFolder" runat="server" ControlName="ddlFolder" Suffix=":" />
			<dnn:DnnFolderDropDownList id="ddlFolder" runat="server" AutoPostBack="true" />
	    </div>
		<div class="dnnFormItem">
	        <dnn:Label id="labelUnpublishSkipped" runat="server" ControlName="checkUnpublishSkipped" Suffix="?" />
			<asp:CheckBox id="checkUnpublishSkipped" runat="server" Checked="true" />
	    </div>
	    <ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" Text="Update" /></li>
	        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Text="Cancel" /></li>
	    </ul>
	</fieldset>
</div>
