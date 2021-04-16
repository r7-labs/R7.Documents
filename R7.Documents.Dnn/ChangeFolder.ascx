<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChangeFolder.ascx.cs" Inherits="R7.Documents.ChangeFolder" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents.Dnn/assets/css/admin.css" Priority="200" />
<div class="dnnForm dnnClear">
	<fieldset>
	    <div class="dnnFormItem">
			<asp:label id="labelInfo" runat="server" resourcekey="labelInfo.Text" CssClass="dnnFormMessage dnnFormWarning" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:Label id="lblFolder" runat="server" ControlName="ddlFolder" />
			<dnn:DnnFolderDropDownList id="ddlFolder" runat="server" AutoPostBack="true" />
	    </div>
		 <div class="dnnFormItem">
	        <dnn:Label id="lblUpdateDefaultFolder" runat="server" ControlName="chkUpdateDefaultFolder" />
			<asp:CheckBox id="chkUpdateDefaultFolder" runat="server" Checked="true" />
	    </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelPublishUpdated" runat="server" ControlName="chkPublishUpdated" />
            <asp:CheckBox id="chkPublishUpdated" runat="server" Checked="true" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblUnpublishSkipped" runat="server" ControlName="chkUnpublishSkipped" />
            <asp:CheckBox id="chkUnpublishSkipped" runat="server" Checked="true" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblDeleteOldFiles" runat="server" ControlName="chkDeleteOldFiles" />
            <asp:CheckBox id="chkDeleteOldFiles" runat="server" Checked="false" />
        </div>
		<ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="btnApply" runat="server" CssClass="dnnPrimaryAction" resourcekey="btnApply" OnClick="btnApply_Click" /></li>
			<li>&nbsp;</li>
	        <li><asp:HyperLink id="lnkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
	    </ul>
	</fieldset>
</div>
