<%@ Control Language="C#" CodeBehind="UrlControlWrapper.ascx.cs" Inherits="R7.Documents.Controls.UrlControlWrapper" %>
<%@ Register TagPrefix="dnn" TagName="Url" Src="~/controls/DnnUrlControl.ascx" %>
<dnn:Url id="ctlUrl" runat="server"
         UrlType="F"
         ShowTabs="true" IncludeActiveTab="true"
    	 ShowNone="True"  ShowNewWindow="True"
    	 ShowSecure="True" ShowDatabase="True" />
