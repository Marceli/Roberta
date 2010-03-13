<%@ Control Language="C#" %>
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
 
 	<div>    	    		
		<FTB:FreeTextBox id="FreeTextBox1" 
			OnSaveClick="SaveButton_Click" 
			runat="Server" />
		
		<asp:Button id="SaveButton" Text="Save" onclick="SaveButton_Click" runat="server" />
		</div>
		
		<div style="display:none;">
			<textarea id="Debug" style="width:500px; height: 250px;"></textarea>
		</div>
		
		<div>
		<asp:Literal id="Output" runat="server" />
	</div>
