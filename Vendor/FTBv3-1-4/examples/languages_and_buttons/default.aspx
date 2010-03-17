<%@ Page Language="C#" ValidateRequest=false %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {
	if (!IsPostBack) {

		ToolbarStyle.DataSource = Enum.GetNames(typeof(ToolbarStyleConfiguration));
		ToolbarStyle.DataBind();
		
		FreeTextBoxControls.Support.ResourceManager rm = new FreeTextBoxControls.Support.ResourceManager();
		NameValueCollection languages = rm.GetSupportedLanguages();

		foreach (string key in languages) {
			Language.Items.Add(new ListItem(key, languages[key]));
		}

		FreeTextBox1.Text = "<p>some <b>Bold</b> and <u>underlined</u> and <font color=\"#008000\">colored</font> text<p><ul><li>bulleted list 1</li></ul>";
	}
}

protected void SaveButton_Click(Object Src, EventArgs E) {

	Output.Text = FreeTextBox1.Text;
}
void ConfigureButton_Click(Object Src, EventArgs E) {
	FreeTextBox1.ToolbarStyleConfiguration = (ToolbarStyleConfiguration) Enum.Parse(typeof(ToolbarStyleConfiguration),ToolbarStyle.SelectedValue);
	FreeTextBox1.Language = Language.SelectedValue;
}
</script>
<html>
<head>
	<title>Languages and Buttons</title>
<script>

</script>
</head>
<body>

    <form id="Form1" runat="server">
    	
    	<h2>Languages and Buttons</h2>
    	
    	<div>    	    		
			<asp:dropdownlist ID="ToolbarStyle" Runat="Server" />
			<asp:dropdownlist ID="Language" Runat="Server" />
			<asp:Button ID="ConfigureButton" Text="Configure" OnClick="ConfigureButton_Click" runat="Server" />

			<br />
			<br />
		
			<FTB:FreeTextBox id="FreeTextBox1" runat="Server" />
				
			<asp:Button id="SaveButton" Text="Save" onclick="SaveButton_Click" runat="server" />
		</div>
		
		<div>
			<asp:Literal id="Output" runat="server" />
		</div>
	</form>
</body>
</html>
