<%@ Page Language="C#" ValidateRequest=false Trace="false" %>
<%@ Register TagPrefix="FTB" Src="subcontrol.ascx" TagName="SubControl" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
}

</script>
<html>
<head>
	<title>Next Control</title>
</head>
<body>

    <form id="Form1" runat="server">
    	
    	<h2>Nested FreeTextBox</h2>
		
		<FTB:SubControl runat="Server" />

	</form>

</body>
</html>
