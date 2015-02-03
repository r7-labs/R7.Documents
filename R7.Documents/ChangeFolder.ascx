<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChangeFolder.ascx.cs" Inherits="R7.Documents.ChangeFolder" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents/admin.css" Priority="200" />
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
	        <dnn:Label id="labelUpdateDefaultFolder" runat="server" ControlName="checkUpdateDefaultFolder" Suffix="?" />
			<asp:CheckBox id="checkUpdateDefaultFolder" runat="server" Checked="true" />
	    </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelPublishUpdated" runat="server" ControlName="checkPublishUpdated" Suffix="?" />
            <asp:CheckBox id="checkPublishUpdated" runat="server" Checked="true" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelOldFilesAction" runat="server" ControlName="radioOldFilesAction" Suffix=":" />
            <asp:RadioButtonList id="radioOldFilesAction" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="OldFilesActionKeep.Text" Value="0" Selected="true" />    
                <asp:ListItem resourcekey="OldFilesActionDelete.Text" Value="1" />
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
	        <dnn:Label id="labelSkippedAction" runat="server" ControlName="radioSkippedAction" Suffix=":" />
            <asp:RadioButtonList id="radioSkippedAction" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="SkippedActionDoNothing.Text" Value="0" />    
                <asp:ListItem resourcekey="SkippedActionUnpublish.Text" Value="1" Selected="true" />
                <asp:ListItem resourcekey="SkippedActionDelete.Text" Value="2" />
                <asp:ListItem resourcekey="SkippedActionDeleteWithResources.Text" Value="3" />
            </asp:RadioButtonList>
        </div>
		<ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" Text="Update" /></li>
	        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Text="Cancel" /></li>
	    </ul>
	</fieldset>
</div>
