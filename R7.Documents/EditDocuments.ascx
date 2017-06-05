<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditDocuments.ascx.cs" Inherits="R7.Documents.EditDocuments" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/DnnUrlControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents/admin.css" Priority="200" />
<div class="dnnForm dnnEditDocs dnnClear" id="dnnEditDocs">
	<div id="document-tabs" class="dnnForm dnnClear">
        <ul class="dnnAdminTabNav dnnClear">
            <li><a href="#document-common-tab"><%= LocalizeString ("Common.Tab") %></a></li>
            <li><a href="#document-owner-tab"><%= LocalizeString ("Owner.Tab") %></a></li>
            <li><a href="#document-audit-tab"><%= LocalizeString ("Audit.Tab") %></a></li>
        </ul>
    	<div id="document-common-tab">
        	<fieldset>
        	    <div class="dnnFormItem">
        	        <dnn:Label id="plName" runat="server" ControlName="txtName" CssClass="dnnFormRequired" />
        	        <asp:TextBox id="txtName" runat="server" MaxLength="255" CssClass="dnnFormRequired" />
        	        <asp:RequiredFieldValidator id="valName" runat="server" CssClass="dnnFormMessage dnnFormError"
					    resourcekey="Name.ErrorMessage" Display="Dynamic"
					    ErrorMessage="You Must Enter A Title For The Document"
					    ControlToValidate="txtName" />
        	    </div>
        	    <div class="dnnFormItem">
        	        <dnn:Label id="plDescription" runat="server" ControlName="txtDescription" />
        	        <asp:TextBox id="txtDescription" runat="server" TextMode="MultiLine" Rows="3" />
        	    </div>
        	    <div class="dnnFormItem">
        	        <dnn:Label id="plCategory" runat="server" ControlName="txtCategory" />
        	        <asp:TextBox id="txtCategory" runat="server" maxlength="50" />
        	        <asp:DropDownList id="lstCategory" runat="server" />
        	    </div>
        		<div class="dnnFormItem">
        	        <dnn:Label id="labelCreatedDate" runat="server" ControlName="textCreatedDate" />
        			<dnn:DnnDateTimePicker id="pickerCreatedDate" runat="server" />
        	    </div>
        	 	<div class="dnnFormItem">
        	        <dnn:Label id="labelLastModifiedDate" runat="server" ControlName="textLastModifiedDate" />
        			<dnn:DnnDateTimePicker id="pickerLastModifiedDate" runat="server" />
        	    </div>
        		<div class="dnnFormItem">
                    <dnn:Label id="labelStartDate" runat="server" ControlName="datetimeStartDate" />
                    <dnn:DnnDateTimePicker id="datetimeStartDate" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelEndDate" runat="server" ControlName="datetimeEndDate" />
                    <dnn:DnnDateTimePicker id="datetimeEndDate" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plSortIndex" runat="server" ControlName="txtSortIndex" />
                    <asp:TextBox id="txtSortIndex" runat="server" CssClass="dnnFormRequired" />
                    <asp:RangeValidator id="valSortIndex" runat="server" CssClass="dnnFormMessage dnnFormError"
					    ErrorMessage="Please enter a valid integer value." Display="Dynamic"
					    ControlToValidate="txtSortIndex" Type="Integer"
					    MaximumValue="2147483647" MinimumValue="-2147483648" />
                </div>
        		<div class="dnnFormItem">
        	        <dnn:Label id="plUrl" runat="server" ControlName="ctlURL" />
        	       	<div class="dnnLeft" style="width:440px">
        	            <dnn:Url id="ctlUrl" runat="server" UrlType="F"
    						ShowTabs="true" IncludeActiveTab="true"
    						ShowNone="True"  ShowNewWindow="True"
    						ShowSecure="True" ShowDatabase="True" />
        	        </div>
        	    </div>
        		<div class="dnnFormItem">
        	        <dnn:Label id="plForceDownload" runat="server" ControlName="chkForceDownload" />
        	        <asp:CheckBox id="chkForceDownload" runat="server" />
        	    </div>
                <div class="dnnFormItem">
                    <dnn:Label id="labelLinkAttributes" runat="server" ControlName="textLinkAttributes" />
                    <asp:TextBox runat="server" id="textLinkAttributes" MaxLength="255" />
                </div>
    		</fieldset>
    	</div>
    	<div id="document-owner-tab">
            <fieldset>
    			<div class="dnnFormItem">
                    <dnn:Label id="plOwner" runat="server" ControlName="lstOwner" />
					<asp:Label id="lblOwner" runat="server" />
                    <asp:DropDownList id="lstOwner" runat="server" Visible="False" 
                        DataTextField="DisplayName"
                        DataValueField="UserID" />
                </div>
				<div class="dnnFormItem">
					<div class="dnnLabel"></div>
                    <asp:LinkButton id="lnkChange" runat="server" CssClass="dnnSecondaryAction" resourcekey="lnkChangeOwner" causesvalidation="False" text="Change Owner" OnClick="lnkChange_Click" />
                </div>
            </fieldset>
        </div>
       	<div id="document-audit-tab">
    		<fieldset>
				<div class="dnnFormItem">
                    <dnn:Label id="labelAudit" runat="server" ControlName="ctlAudit" /> 
                    <dnn:Audit id="ctlAudit" runat="server" />
                </div>
    			<div class="dnnFormItem">
                    <dnn:Label id="labelUrlTracking" runat="server" ControlName="ctlUrlTracking" />
                    <dnn:Tracking id="ctlUrlTracking" runat="server" />
    			</div>		
        	</fieldset>
    	</div>		
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" Text="Update" OnClick="cmdUpdate_Click" /></li>
            <li><asp:LinkButton id="cmdUpdateOverride" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdateOverride" Text="Update Anyway" Visible="False" OnClick="cmdUpdateOverride_Click" /></li>
            <li><asp:HyperLink id="linkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Text="Cancel" /></li>
            <li><asp:LinkButton id="cmdDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDelete" CausesValidation="False" Text="Delete" OnClick="cmdDelete_Click" /></li>
        </ul>
        <asp:Panel id="panelUpdate" runat="server" CssClass="dnnFormItem">
            <dnn:Label id="labelDontUpdateLastModifiedDate" runat="server" ControlName="checkDontUpdateLastModifiedDate" />
            <asp:CheckBox id="checkDontUpdateLastModifiedDate" runat="server"  />
        </asp:Panel>
        <asp:Panel id="panelDelete" runat="server" CssClass="dnnFormItem">
            <dnn:Label id="labelDeleteWithResource" runat="server" ControlName="checkDeleteWithResource" />
            <asp:CheckBox id="checkDeleteWithResource" runat="server"  />
        </asp:Panel>
	</div>	
</div>
<input id="hiddenSelectedTab" type="hidden" value="<%= (int) SelectedTab %>" />
<script type="text/javascript">
(function($, Sys) {
    function setupModule() {
        var selectedTab = document.getElementById("hiddenSelectedTab").value;
        $("#document-tabs").dnnTabs({selected: selectedTab});
    };
    $(document).ready(function() {
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
            setupModule();
        });
    });
} (jQuery, window.Sys));
</script>