<%@ Page Language="C#" ValidateRequest=false %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
	if (!IsPostBack) {

		ArrayList stuff = new ArrayList();
		stuff.Add(new ListItem("Hey", "<b>asdasdf</b>"));
		stuff.Add(new ListItem("Yo", "<i>italic</i>"));

		FtbRepeater1.DataSource = stuff;
		FtbRepeater1.DataBind();

	}
}

protected void SaveButton_Click(object sender, EventArgs E) {
	Output.Text = "";
	
	foreach (RepeaterItem repeaterItem in FtbRepeater1.Items) {
		
		FreeTextBox FreeTextBox1 = repeaterItem.FindControl("FreeTextBox1") as FreeTextBox;
		
		Output.Text += FreeTextBox1.Text + "<hr />";
		
	}

}


</script>
<html>
<head>
	<title>Nested in a Repeater</title>
<script>

</script>
</head>
<body>

    <form id="Form1" runat="server">
    	
    	<h2>Nested in a Repeater</h2>
    	
    	<div>       	
			<asp:Repeater ID="FtbRepeater1" runat="server">
				<ItemTemplate>

					<FTB:FreeTextBox id="FreeTextBox1" Text='<%# DataBinder.Eval(Container.DataItem, "value") %>' 
						runat="Server" />

				</ItemTemplate>    	
				<SeparatorTemplate>
					<hr />    	
				</SeparatorTemplate>
			</asp:Repeater>
    	
			<asp:Button id="SaveButton" Text="Save" onclick="SaveButton_Click" runat="server" />
		</div>
		
		<div>
			<asp:Literal id="Output" runat="server" />
		</div>
	</form>
</body>
</html>
