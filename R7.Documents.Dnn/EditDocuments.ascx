<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditDocuments.ascx.cs" Inherits="R7.Documents.EditDocuments" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/DnnUrlControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.Documents/R7.Documents.Dnn/assets/css/admin.css" Priority="200" />
<div class="dnnForm dnnClear r7-docs-edit">
	<asp:MultiView id="mvEditDocument" runat="server" ActiveViewIndex="0">
	<asp:View runat="server">
	<div id="document-tabs" class="dnnForm dnnClear">
        <ul class="dnnAdminTabNav dnnClear">
            <li><a href="#document-common-tab"><%= LocalizeString ("Common.Tab") %></a></li>
            <li><a href="#document-advanced-tab"><%= LocalizeString ("Advanced.Tab") %></a></li>
		    <li><a href="#document-audit-tab"><%= LocalizeString ("Audit.Tab") %></a></li>
        </ul>
    	<div id="document-common-tab">
        	<fieldset>
        	    <div class="dnnFormItem">
        	        <dnn:Label id="lblTitle" runat="server" ControlName="txtTitle" CssClass="dnnFormRequired" />
        	        <asp:TextBox id="txtTitle" runat="server" MaxLength="255" CssClass="dnnFormRequired wide-textbox" />
        	        <asp:RequiredFieldValidator id="valName" runat="server" ControlToValidate="txtTitle"
                        resourcekey="Title.Required" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" />
        	    </div>
        		<div class="dnnFormItem">
        	        <dnn:Label id="lblUrl" runat="server" ControlName="ctlUrl" />
        	       	<div class="dnnLeft">
        	            <dnn:Url id="ctlUrl" runat="server" UrlType="F"
    						ShowTabs="true" IncludeActiveTab="true"
    						ShowNone="True"  ShowNewWindow="True"
    						ShowSecure="True" ShowDatabase="True" />
        	        </div>
				</div>
				<div class="dnnFormItem">
                    <dnn:Label id="lblSortIndex" runat="server" ControlName="txtSortIndex" />
                    <asp:TextBox id="txtSortIndex" runat="server" TextMode="Number">10</asp:TextBox>
                    <asp:RangeValidator id="valSortIndex" runat="server" ControlToValidate="txtSortIndex"
                        Type="Integer" MaximumValue="2147483647" MinimumValue="-2147483648"
						resourcekey="SortIndex.Invalid" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" />
                </div>
	            <div class="dnnFormItem control-group-start">
                    <dnn:Label id="lblStartDate" runat="server" ControlName="dtStartDate" />
                    <asp:TextBox id="dtStartDate" type="datetime-local" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="lblEndDate" runat="server" ControlName="dtEndDate" />
                    <asp:TextBox id="dtEndDate" type="datetime-local" runat="server" />
                </div>
            </fieldset>
    	</div>
    	<div id="document-advanced-tab">
            <fieldset>
				<div class="dnnFormItem">
                    <dnn:Label id="lblDescription" runat="server" ControlName="txtDescription" />
                    <asp:TextBox id="txtDescription" runat="server" TextMode="MultiLine" Rows="3" />
                </div>
				<div class="dnnFormItem">
                    <dnn:Label id="lblCategory" runat="server" ControlName="txtCategory" />
                    <asp:TextBox id="txtCategory" runat="server" maxlength="50" />
                    <asp:DropDownList id="lstCategory" runat="server" />
                </div>
				<div class="dnnFormItem control-group-start" >
                    <dnn:Label id="lblCreatedDate" runat="server" ControlName="dtCreatedDate" />
                    <asp:TextBox id="dtCreatedDate" type="datetime-local" runat="server" />
                </div>
                <div class="dnnFormItem control-group-end">
                    <dnn:Label id="lblLastModifiedDate" runat="server" ControlName="dtLastModifiedDate" />
                    <asp:TextBox id="dtLastModifiedDate" type="datetime-local" runat="server" />
                </div>
				<div class="dnnFormItem">
                    <dnn:Label id="lblLinkAttributes" runat="server" ControlName="txtLinkAttributes" />
                    <asp:TextBox runat="server" id="txtLinkAttributes" MaxLength="255" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="lblIsFeatured" runat="server" ControlName="chkIsFeatured" />
                    <asp:CheckBox id="chkIsFeatured" runat="server" />
                </div>
				<div class="dnnFormItem">
                    <dnn:Label id="lblForceDownload" runat="server" ControlName="chkForceDownload" />
                    <asp:CheckBox id="chkForceDownload" runat="server" />
                </div>
				<div class="dnnFormItem">
                    <dnn:Label id="lblOwner" runat="server" ControlName="lstOwner" />
					<asp:Label id="txtOwner" runat="server" />
                    <asp:DropDownList id="lstOwner" runat="server" Visible="false"
                        DataTextField="DisplayName"
                        DataValueField="UserID" />
                </div>
                <div class="dnnFormItem">
                    <div class="dnnLabel"></div>
                    <asp:LinkButton id="btnChangeOwner" runat="server" CssClass="dnnSecondaryAction"
									resourcekey="lnkChangeOwner" causesvalidation="false" OnClick="lnkChangeOwner_Click" />
                </div>
			</fieldset>
        </div>
       	<div id="document-audit-tab">
    		<fieldset>
				<div class="dnnFormItem">
                    <dnn:Label id="lblAudit" runat="server" ControlName="ctlAudit" />
                    <dnn:Audit id="ctlAudit" runat="server" />
                </div>
    			<div class="dnnFormItem">
                    <dnn:Label id="lblUrlTracking" runat="server" ControlName="ctlUrlTracking" />
                    <dnn:Tracking id="ctlUrlTracking" runat="server" />
    			</div>
        	</fieldset>
    	</div>
        <ul class="dnnActions dnnClear">
			<li><asp:LinkButton id="btnAdd" runat="server" CssClass="dnnPrimaryAction" resourcekey="btnAdd.Text" OnClick="btnAdd_Click" /></li>
            <li><asp:LinkButton id="btnUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="btnUpdate.Text" OnClick="btnUpdate_Click" /></li>
            <li>&nbsp;</li>
        	<li><asp:LinkButton id="btnDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="btnDelete" CausesValidation="False" OnClick="btnDelete_Click" /></li>
			<li><asp:LinkButton id="btnDeleteWithFile" runat="server" CssClass="dnnSecondaryAction" resourcekey="btnDeleteWithFile.Text" CausesValidation="False" OnClick="btnDelete_Click" /></li>
			<li>&nbsp;</li>
			<li><asp:HyperLink id="lnkCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
		</ul>
	</div>
    </asp:View>
	<asp:View runat="server">
		<ul class="dnnActions dnnClear">
            <li><asp:LinkButton id="btnAddMore" runat="server" CssClass="dnnPrimaryAction" resourcekey="btnAddMore.Text" OnClick="btnAddMore_Click" /></li>
			<li><asp:LinkButton id="btnEdit" runat="server" CssClass="dnnSecondaryAction" resourcekey="btnEdit.Text" OnClick="btnEdit_Click" /></li>
			<li>&nbsp;</li>
		    <li><asp:HyperLink id="lnkClose" runat="server" CssClass="dnnSecondaryAction" resourcekey="Close" /></li>
		</ul>
	</asp:View>
	</asp:MultiView>
</div>
<input id="hdnSelectedTab" type="hidden" value="<%= (int) SelectedTab %>" />
<script type="text/javascript">
(function($, Sys) {
    function setupModule() {
        var selectedTab = document.getElementById("hdnSelectedTab").value;
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
