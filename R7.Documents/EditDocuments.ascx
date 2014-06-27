<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditDocuments.ascx.cs" Inherits="R7.Documents.EditDocuments" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnEditDocs dnnClear" id="dnnEditDocs">
    <div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>
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
        <dnn:label id="plUrl" runat="server" controlname="ctlURL" suffix=":" />
        <div class="dnnRight">
            <portal:url id="ctlUrl" runat="server" showtabs="False" shownone="True" urltype="F" shownewwindow="True" ShowSecure="True" ShowDatabase="True" />
        </div>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plSortIndex" runat="server" controlname="txtSortIndex" suffix=":" />
        <asp:textbox id="txtSortIndex" runat="server" maxlength="3" CssClass="dnnFormRequired" />
        <asp:rangevalidator id="valSortIndex" runat="server" CssClass="dnnFormMessage dnnFormError" ErrorMessage="Please enter a value from 0-999." Display="Dynamic" ControlToValidate="txtSortIndex" Type="Integer" MaximumValue="999" MinimumValue="0" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="plForceDownload" runat="server" controlname="chkForceDownload" suffix="?" />
        <asp:CheckBox runat="server" ID="chkForceDownload" />
    </div>
    <div class="dnnFormItem">
        <asp:label id="lblAudit" runat="server" />
        <div class="dnnRight">
            <portal:tracking id="ctlTracking" runat="server" />
        </div>
    </div>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdUpdate" runat="server" cssclass="dnnPrimaryAction" resourcekey="cmdUpdate" text="Update" /></li>
        <li><asp:LinkButton ID="cmdUpdateOverride" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdUpdateOverride" Text="Update Anyway" Visible="False" /></li>
        <li><asp:linkbutton id="cmdCancel" runat="server" cssclass="dnnSecondaryAction" resourcekey="cmdCancel" causesvalidation="False" text="Cancel" /></li>
        <li><asp:linkbutton id="cmdDelete" runat="server" cssclass="dnnSecondaryAction" resourcekey="cmdDelete" causesvalidation="False" text="Delete" /></li>
    </ul>
    <div class="dnnssStat dnnClear"><portal:audit id="ctlAudit" runat="server" /></div>
</div>
