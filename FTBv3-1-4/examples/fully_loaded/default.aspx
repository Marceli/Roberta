<%@ Page Language="C#" ValidateRequest=false Trace="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<script runat="server">

protected void Page_Load(Object Src, EventArgs E) {

	if (!IsPostBack) {		
		FreeTextBox1.Text = "<p><a href=\"http://www.freetextbox.com\" target=\"_blank\">FreeTextBox</a></p><p>some <b>Bold</b> and <u>underlined</u> and <font color=\"#008000\">colored</font> text<p><ul><li>bulleted list 1</li></ul><form><table border=1><tr><td>Name</td><td><input type=text></td></tr><tr><td>Secure</td><td><input type=checkbox></td></tr><tr><td>Category</td><td><select><option>Shopping</option><option>Clothes</option></select></td></tr></table></form>";
	}

}

protected void SaveButton_Click(Object Src, EventArgs E) {

	Output.Text = FreeTextBox1.Text;
}


</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head>
	<title>All Features</title>
<script>

</script>
</head>
<body>

    <form id="Form1" runat="server">
    	
    	<h2>All buttons and drop downs enabled</h2>

		<div>
    	
    	    	    		
		<FTB:FreeTextBox id="FreeTextBox1" OnSaveClick="SaveButton_Click" 
			toolbarlayout="ParagraphMenu,FontFacesMenu,FontSizesMenu,FontForeColorsMenu,FontForeColorPicker,FontBackColorsMenu,FontBackColorPicker|Bold,Italic,Underline,Strikethrough,Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage|Cut,Copy,Paste,Delete;Undo,Redo,Print,Save|SymbolsMenu,StylesMenu,InsertHtmlMenu|InsertRule,InsertDate,InsertTime|InsertTable,EditTable;InsertTableRowAfter,InsertTableRowBefore,DeleteTableRow;InsertTableColumnAfter,InsertTableColumnBefore,DeleteTableColumn|InsertForm,InsertTextBox,InsertTextArea,InsertRadioButton,InsertCheckBox,InsertDropDownList,InsertButton|InsertDiv,EditStyle,InsertImageFromGallery,Preview,SelectAll,WordClean,NetSpell"
			runat="Server"
			DesignModeCss="designmode.css"		 
			/>
	
			<asp:Button id="SaveButton" Text="Save" onclick="SaveButton_Click" runat="server" />		
		</div>
		
		<div>		
			<asp:Literal id="Output" runat="server" />			
		</div>
		

	</form>
	
</body>
</html>
