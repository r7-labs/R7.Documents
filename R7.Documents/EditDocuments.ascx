<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditDocuments.ascx.cs" Inherits="R7.Documents.EditDocuments" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="dnnForm dnnEditDocs dnnClear" id="dnnEditDocs">
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
        <asp:linkbutton id="lnkChange" runat="server" cssclass="dnnSecondaryAction" resourcekey="lnkChangeOwner" causesvalidation="False" text="Change Owner" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plSortIndex" runat="server" controlname="txtSortIndex" suffix=":" />
        <asp:textbox id="txtSortIndex" runat="server" CssClass="dnnFormRequired" />
        <asp:rangevalidator id="valSortIndex" runat="server" CssClass="dnnFormMessage dnnFormError" ErrorMessage="Please enter a valid integer value." Display="Dynamic" ControlToValidate="txtSortIndex" Type="Integer" MaximumValue="2147483647" MinimumValue="-2147483648" />
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
        <dnn:Label id="labelIsPublished" runat="server" controlname="checkIsPublished" suffix="?" />
        <asp:CheckBox runat="server" ID="checkIsPublished" />
    </div>
	<asp:Panel id="panelUrlTracking" runat="server" CssClass="dnnFormItem">
        <dnn:label id="labelUrlTracking" runat="server" controlname="ctlUrlTracking" suffix=":" />
        <div class="dnnLeft" style="padding:1.5em;margin-top:0.5em;background-color:#eee">
            <portal:tracking id="ctlUrlTracking" runat="server" />
        </div>
    </asp:Panel>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdUpdate" runat="server" cssclass="dnnPrimaryAction" resourcekey="cmdUpdate" text="Update" /></li>
        <li><asp:LinkButton ID="cmdUpdateOverride" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdateOverride" Text="Update Anyway" Visible="False" /></li>
        <li><asp:linkbutton id="cmdCancel" runat="server" cssclass="dnnSecondaryAction" resourcekey="cmdCancel" causesvalidation="False" text="Cancel" /></li>
        <li><asp:linkbutton id="cmdDelete" runat="server" cssclass="dnnSecondaryAction" resourcekey="cmdDelete" causesvalidation="False" text="Delete" /></li>
    </ul>
	<hr />
    <div class="dnnssStat dnnClear"><portal:audit id="ctlAudit" runat="server" /></div>
</div>
