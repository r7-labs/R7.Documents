<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditDocuments.ascx.cs" Inherits="R7.Documents.EditDocuments" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents/admin.css" Priority="200" />
<div class="dnnForm dnnEditDocs dnnClear" id="dnnEditDocs">
	<fieldset>
	    <div class="dnnFormItem">
	        <dnn:label id="plName" runat="server" controlname="txtName" suffix=":" CssClass="dnnFormRequired" />
	        <asp:textbox id="txtName" runat="server" maxlength="150" CssClass="dnnFormRequired" />
	        <asp:requiredfieldvalidator id="valName" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="Name.ErrorMessage" display="Dynamic" errormessage="You Must Enter A Title For The Document" controltovalidate="txtName" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:label id="plDescription" runat="server" controlname="txtDescription" suffix=":" />
	        <asp:textbox id="txtDescription" runat="server" maxlength="255" TextMode="MultiLine" Rows="3" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:label id="plCategory" runat="server" controlname="txtCategory" suffix=":" />
	        <asp:textbox id="txtCategory" runat="server" maxlength="50" />
	        <asp:dropdownlist id="lstCategory" runat="server" />
	    </div>
	    <div class="dnnFormItem">
	        <dnn:label id="plOwner" runat="server" controlname="lstOwner" suffix=":" />
	        <asp:dropdownlist id="lstOwner" runat="server" Visible="False" />
	        <asp:label id="lblOwner" runat="server" />
	        <asp:linkbutton id="lnkChange" runat="server" cssclass="dnnSecondaryAction" resourcekey="lnkChangeOwner" causesvalidation="False" text="Change Owner" OnClick="lnkChange_Click" />
	    </div>
		<div class="dnnFormItem">
	        <dnn:label id="labelCreatedDate" runat="server" controlname="textCreatedDate" suffix=":" />
			<dnnweb:DnnDateTimePicker id="pickerCreatedDate" runat="server" />
	    </div>
	 	<div class="dnnFormItem">
	        <dnn:label id="labelLastModifiedDate" runat="server" controlname="textLastModifiedDate" suffix=":" />
			<dnnweb:DnnDateTimePicker id="pickerLastModifiedDate" runat="server" />
	    </div>
        <div class="dnnFormItem">
            <dnn:label id="plSortIndex" runat="server" controlname="txtSortIndex" suffix=":" />
            <asp:textbox id="txtSortIndex" runat="server" CssClass="dnnFormRequired" />
            <asp:rangevalidator id="valSortIndex" runat="server" CssClass="dnnFormMessage dnnFormError" ErrorMessage="Please enter a valid integer value." Display="Dynamic" ControlToValidate="txtSortIndex" Type="Integer" MaximumValue="2147483647" MinimumValue="-2147483648" />
        </div>
		<div class="dnnFormItem">
	        <dnn:label id="plUrl" runat="server" controlname="ctlURL" suffix=":" />
	       	<div class="dnnLeft" style="width:440px">
	            <portal:url id="ctlUrl" runat="server" showtabs="False" shownone="True" urltype="F" shownewwindow="True" ShowSecure="True" ShowDatabase="True" />
	        </div>
	    </div>
		<div class="dnnFormItem">
	        <dnn:Label id="plForceDownload" runat="server" controlname="chkForceDownload" suffix="?" />
	        <asp:CheckBox runat="server" ID="chkForceDownload" />
	    </div>
        <div class="dnnFormItem">
            <dnn:Label id="labelLinkAttributes" runat="server" controlname="textLinkAttributes" />
            <asp:TextBox runat="server" id="textLinkAttributes" MaxLength="255" />
        </div>
		<asp:Panel id="panelUrlTracking" runat="server" CssClass="dnnFormItem">
	        <dnn:label id="labelUrlTracking" runat="server" controlname="ctlUrlTracking" suffix=":" />
	        <div class="dnnLeft" style="padding:1.5em;margin-top:0.5em;background-color:#eee;width:43%">
	            <portal:tracking id="ctlUrlTracking" runat="server" />
	        </div>
	    </asp:Panel>
        <ul class="dnnActions dnnClear">
	        <li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" Text="Update" OnClick="cmdUpdate_Click" /></li>
	        <li><asp:LinkButton id="cmdUpdateOverride" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdateOverride" Text="Update Anyway" Visible="False" OnClick="cmdUpdateOverride_Click" /></li>
	        <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Text="Cancel" /></li>
	        <li><asp:LinkButton id="cmdDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDelete" CausesValidation="False" Text="Delete" OnClick="cmdDelete_Click" /></li>
	    </ul>
        <div class="dnnFormItem">
            <dnn:Label id="labelIsPublished" runat="server" controlname="checkIsPublished" suffix="?" />
            <asp:CheckBox runat="server" ID="checkIsPublished" />
        </div>
        <asp:Panel id="panelUpdate" runat="server" CssClass="dnnFormItem">
            <dnn:Label id="labelDontUpdateLastModifiedDate" runat="server" ControlName="checkDontUpdateLastModifiedDate" Suffix=":" />
            <asp:CheckBox id="checkDontUpdateLastModifiedDate" runat="server"  />
        </asp:Panel>
        <asp:Panel id="panelDelete" runat="server" CssClass="dnnFormItem">
            <dnn:Label id="labelDeleteWithResource" runat="server" ControlName="checkDeleteWithResource" Suffix="?" />
            <asp:CheckBox id="checkDeleteWithResource" runat="server"  />
        </asp:Panel>
		<hr />
	    <div class="dnnssStat dnnClear"><portal:audit id="ctlAudit" runat="server" /></div>
	</fieldset>
</div>
