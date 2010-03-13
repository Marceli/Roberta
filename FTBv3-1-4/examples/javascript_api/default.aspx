<%@ Page Language="C#" ValidateRequest=false %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
	if (!IsPostBack) {
		FreeTextBox1.Text = "<p>some <b>Bold</b> and <u>underlined</u> and <font color=\"#008000\">colored</font> text<p><ul><li>bulleted list 1</li></ul>";
	}
}

protected void SaveButton_Click(Object Src, EventArgs E) {
	Output.Text = FreeTextBox1.Text;
}

</script>
<html>
<head>
	<title>JavaScript Demo</title>
<script>

</script>
</head>
<body>

    <form id="Form1" runat="server">
    	
    	<h2>JavaScript API examples</h2>
    	
    	<table>
    		<tr>
    			<td>
					<FTB:FreeTextBox id="FreeTextBox1" width="400px" height="200px" runat="server" />
				</td>
				<td>
					<a href="#" onclick="FTB_API['<%= FreeTextBox1.ClientID %>'].SetHtml(document.getElementById('<%= TextBox1.ClientID %>').value);" style="display:block; border: solid 1px #ccc; padding: 3px; text-decoration:none;">
						<-- Copy to
						<br />
						FreeTextBox
					</a>
					<br />
					<a href="#" onclick="document.getElementById('<%= TextBox1.ClientID %>').value = FTB_API['<%= FreeTextBox1.ClientID %>'].GetHtml();" style="display:block; border: solid 1px #ccc; padding: 3px; text-decoration:none;">
						Copy from
						<br />
						FreeTextBox -->
					</a>
					<br />
					
				</td>
				<td>
					<asp:TextBox id="TextBox1" name="TextBox1" TextMode="MultiLine" Columns="30" rows="15" runat="server" />
				</td>
			</tr>
		</table>
		
		<h4>More JavaScript</h4>
		<a href="#" onclick="FTB_API['<%= FreeTextBox1.ClientID %>'].ExecuteCommand('bold');" style="border: solid 1px #ccc; padding: 3px; text-decoration:none;">
			Bold
		</a>
		<a href="#" onclick="FTB_API['<%= FreeTextBox1.ClientID %>'].ExecuteCommand('italic');" style="border: solid 1px #ccc; padding: 3px; text-decoration:none;">
			Italic
		</a>	
		
		<br />
		
		<input type="text" id="LinkUrl" value="www.freetextbox.com" />
		<a href="#" onclick="FTB_API['<%= FreeTextBox1.ClientID %>'].ExecuteCommand('createlink',null,document.getElementById('LinkUrl').value);" style="border: solid 1px #ccc; padding: 3px; text-decoration:none;">
			Create Link
		</a>		
		
			
		<hr />
		
		<asp:Button id="SaveButton" Text=" PostBack " onclick="SaveButton_Click" runat="server" />
				
		<div>
			<asp:Literal id="Output" runat="server" />
		</div>
	</form>
</body>
</html>
