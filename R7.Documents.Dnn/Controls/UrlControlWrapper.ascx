<%@ Control Language="C#" CodeBehind="UrlControlWrapper.ascx.cs" Inherits="R7.Documents.Controls.UrlControlWrapper" %>
<%@ Register TagPrefix="dnn" TagName="Url" Src="~/controls/DnnUrlControl.ascx" %>

<div style="float:left">
	<div>None? <asp:CheckBox id="chkNone" runat="server" /></div>
	<div>URL: <asp:TextBox id="txtUrl" runat="server" /></div>

	<dnn:Url id="ctlUrl" runat="server"
	         UrlType="F"
	         ShowUrls="false"
	         ShowTabs="true" IncludeActiveTab="true"
    		 ShowNone="false"  ShowNewWindow="True"
    		 ShowSecure="True" ShowDatabase="True" />
</div>
