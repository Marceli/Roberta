<%@ Page Language="C#" ValidateRequest=false %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
	if (!IsPostBack) {

		FreeTextBox1.FontFacesMenuList = new string[] { "Arial", "Times" };

		FreeTextBox1.Text = "<p>some <b>Bold</b> and <u>underlined</u> and <font color=\"#008000\">colored</font> text<p><ul><li>bulleted list 1</li></ul>";
	}
}

protected void SaveButton_Click(Object Src, EventArgs E) {

	Output.Text = FreeTextBox1.Text;
}

</script>
<html>
<head>
	<title>Custom DropDownList values</title>
<script>

</script>
</head>
<body>

    <form id="Form1" runat="server">
    	
    	<h2>Custom DropDownList values</h2>
    	
    	<div>		
			<FTB:FreeTextBox id="FreeTextBox1" runat="Server" />				
			<asp:Button id="SaveButton" Text="Save" onclick="SaveButton_Click" runat="server" />
		</div>
		
		<div>
			<asp:Literal id="Output" runat="server" />
		</div>
	</form>
</body>
</html>
