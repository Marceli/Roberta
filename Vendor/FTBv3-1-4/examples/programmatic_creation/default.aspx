<%@ Page Language="C#" ValidateRequest=false %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
	//if (!IsPostBack) {

		FreeTextBox FreeTextBox1 = new FreeTextBox();
		FreeTextBox1.ID = "FreeTextBox1";
		
		FreeTextBoxPlaceHolder.Controls.Add(FreeTextBox1);
	//}
}

protected void SaveButton_Click(Object Src, EventArgs E) {
	
	FreeTextBox FreeTextBox1 = FreeTextBoxPlaceHolder.FindControl("FreeTextBox1") as FreeTextBox;

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
    	
    	<h2>Programmatically added FreeTextBox</h2>
    	
    	<div>
			<asp:PlaceHolder id="FreeTextBoxPlaceHolder" runat="server" />
				
			<asp:Button id="SaveButton" Text="Save" onclick="SaveButton_Click" runat="server" />
		</div>
				
		<div>
		<asp:Literal id="Output" runat="server" />
		</div>
	</form>
</body>
</html>
