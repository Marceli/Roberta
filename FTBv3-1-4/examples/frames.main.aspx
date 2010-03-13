<%@ Page Language="C#" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
	BrowserInfo.Text = Page.Request.UserAgent;

	string versionInfo = System.Reflection.Assembly.GetAssembly(typeof(FreeTextBoxControls.FreeTextBox)).FullName;
	int start = versionInfo.IndexOf("Version=") + 8;
	int end = versionInfo.IndexOf(",", start);
	versionInfo = versionInfo.Substring(start, end - start);
	VersionInfo.Text = versionInfo;
}

</script>
<html>
<head>
	<title>FreeTextBox 3.0</title>
<script>

</script>
</head>
<body>

    <form runat="server">
    
    	<p>
    		Browser: <asp:Literal id="BrowserInfo" runat="server" /><br>
    		Version: <asp:Literal id="VersionInfo" runat="server" /><br>
    	</p>    

	</form>
</body>
</html>
