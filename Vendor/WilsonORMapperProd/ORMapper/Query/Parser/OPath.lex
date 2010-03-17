//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

internal class Yytoken
{
	private TokenType _type;
	private string _text;
	private int _charBegin;
	private object _value;

	public Yytoken(TokenType type, string text, int charBegin) : this(type, text, charBegin, null)
	{
	}
	
	public Yytoken(TokenType type, string text, int charBegin, object value)
	{
		_type = type;
		_text = text;
		_charBegin = charBegin;
		_value = value;
	}

	public TokenType Type
	{
		get { return _type; }
		set { _type = value; }
	}

	public string Text
	{
		get { return _text; }
	}

	public int Position
	{
		get { return _charBegin; }
	}
	
	public object Value
	{
		get { return _value; }
	}
	
	public override string ToString()
	{
		if( _type != TokenType.CONST )
		{
			return string.Format("{0}: {1}", _type, _text);
		}
		else
		{
			return string.Format("{0}: {1} ({2})", _type, _value, _value.GetType());
		}
	}
}

// FROM: http://longhorn.msdn.microsoft.com/lhsdk/winfs/wfconoperatorprecedence.aspx
//
// The following table summarizes the operators in precedence from highest to lowest:
//
//	Operation Type:		Syntax:
//	Primary				. , [] ()
//	Unary				! not
//	Multiplicative		* / %
//	Additive			+ -
//	Relational			< > <= >=
//	Equality			= != <>
//	Conditional	AND		and &&
//	Conditional	OR		or ||
//
// When an operand occurs between two operators with the same precedence, the associativity of the operators
// controls the order in which the operations are performed. All operators are left associative except the
// conditional operator (?:), meaning that operations are performed from left to right.
// For example, x + y + z is evaluated as (x + y) + z.

// NOTE: The enum values are used to define precedence order!
//		 The last two hex digits are ignored when comparing precedance of operators.
//       Higher on list = higher precedence (e.g., low values = high precedence).
internal enum TokenType
{
	PERIOD		= 0x100,

	NOT			= 0x200,
	NEGATE		= 0x201,  // note: created from minus tokens by parser; our lexer isn't smart enought to do it automatically.

	MULTIPLY	= 0x300,
	DIVIDE		= 0x301,
	MODULO		= 0x302,

	PLUS		= 0x400,
	MINUS		= 0x401,

	COMMA		= 0x502,
	
	OP_LT		= 0x600,
	OP_GT		= 0x601,
	OP_LE		= 0x602,
	OP_GE		= 0x603,
	
	OP_EQ		= 0x700,
	OP_NE		= 0x701,
	
	OP_LIKE		= 0x800,
	OP_IN		= 0x801,
	//OP_IIF,				// a look-ahead parser is required to support the '?:' syntax
	
	FN_ISNULL	= 0x900,
	FN_EXISTS	= 0x901,
	FN_LEN		= 0x902,
	FN_TRIM		= 0x903,
	FN_LEFT 	= 0x904,
	FN_RIGHT	= 0x905,
	FN_SUBSTR	= 0x906,
	FN_UPPER	= 0x907,
	FN_LOWER	= 0x908,

	AND			= 0xA00,
	
	OR			= 0xB00,
	
	LPAREN		= 0xC00,
	RPAREN		= 0xC01,
	
	LBRACE		= 0xD00,
	RBRACE		= 0xD01,

	IDENT		= 0xF00,
	CONST		= 0xF01,
	PARAM		= 0xF02,
	PARENT		= 0xF03,

	ASCEND		= 0xFF0,
	DESCEND		= 0xFF1,
};

//V2: Consider defining a custom return type using: %type <name>
%%

%{
private StringBuilder buff = null;
private int buffStartChar;
%}

%namespace Wilson.ORMapper.Query
%class OPathBaseLexer
%function Lex

%char
%state QUOTE DATE GUID

%eof{
	if( buff != null )
	{
		switch( yy_lexical_state )
		{
			case QUOTE: throw new OPathException("Quoted string found with no matching ending quote.");
			case DATE: throw new OPathException("Date literal found with no ending delimiter.");
			case GUID: throw new OPathException("Guid literal found with no ending delimiter.");
			default:
				throw new OPathException("Literal value found with no ending delimiter.");
		}
	}
	bool ignore = yy_last_was_cr; //NOTE: this is to surpress the compiler warning about 'yy_last_was_cr' not being used.
%eof}

ALPHA=[A-Za-z]
DIGIT=[0-9]
WHITE_SPACE_CHAR=[\ \r\n\t\b\012]

%%
<YYINITIAL> {WHITE_SPACE_CHAR}+  { return null; }

<YYINITIAL> "("  { return new Yytoken(TokenType.LPAREN, yytext(), yychar); }
<YYINITIAL> ")"  { return new Yytoken(TokenType.RPAREN, yytext(), yychar); }
<YYINITIAL> "["  { return new Yytoken(TokenType.LBRACE, yytext(), yychar); }
<YYINITIAL> "]"  { return new Yytoken(TokenType.RBRACE, yytext(), yychar); }
<YYINITIAL> "&&" { return new Yytoken(TokenType.AND, yytext(), yychar); }
<YYINITIAL> "||" { return new Yytoken(TokenType.OR, yytext(), yychar); }
<YYINITIAL> "==" { return new Yytoken(TokenType.OP_EQ, yytext(), yychar); }
<YYINITIAL> "="  { return new Yytoken(TokenType.OP_EQ, yytext(), yychar); }
<YYINITIAL> "!=" { return new Yytoken(TokenType.OP_NE, yytext(), yychar); }
<YYINITIAL> "<>" { return new Yytoken(TokenType.OP_NE, yytext(), yychar); }
<YYINITIAL> "<"  { return new Yytoken(TokenType.OP_LT, yytext(), yychar); }
<YYINITIAL> "<=" { return new Yytoken(TokenType.OP_LE, yytext(), yychar); }
<YYINITIAL> ">"  { return new Yytoken(TokenType.OP_GT, yytext(), yychar); }
<YYINITIAL> ">=" { return new Yytoken(TokenType.OP_GE, yytext(), yychar); }
<YYINITIAL> "!"  { return new Yytoken(TokenType.NOT, yytext(), yychar); }
<YYINITIAL> "^"  { return new Yytoken(TokenType.PARENT, yytext(), yychar); }
<YYINITIAL> "."  { return new Yytoken(TokenType.PERIOD, yytext(), yychar); }
<YYINITIAL> ","  { return new Yytoken(TokenType.COMMA, yytext(), yychar); }
<YYINITIAL> "+"  { return new Yytoken(TokenType.PLUS, yytext(), yychar); }
<YYINITIAL> "-"  { return new Yytoken(TokenType.MINUS, yytext(), yychar); }
<YYINITIAL> "*"  { return new Yytoken(TokenType.MULTIPLY, yytext(), yychar); }
<YYINITIAL> "/"  { return new Yytoken(TokenType.DIVIDE, yytext(), yychar); }
<YYINITIAL> "%"  { return new Yytoken(TokenType.MODULO, yytext(), yychar); }
<YYINITIAL> "?"  { return new Yytoken(TokenType.PARAM, yytext(), yychar); }

<YYINITIAL> \@?{ALPHA}({ALPHA}|{DIGIT}|_)*  {
	string text = yytext();
	if( text[0] != '@' )
	{
		switch( text.ToUpper() )
		{
			case "AND": return new Yytoken(TokenType.AND, text, yychar);
			case "OR": return new Yytoken(TokenType.OR, text, yychar);
			case "NOT": return new Yytoken(TokenType.NOT, text, yychar);
			case "TRUE": return new Yytoken(TokenType.CONST, text, yychar, true);
			case "FALSE":  return new Yytoken(TokenType.CONST, text, yychar, false);
			case "IN": return new Yytoken(TokenType.OP_IN, text, yychar);
			case "LIKE": return new Yytoken(TokenType.OP_LIKE, text, yychar);
			case "ISNULL": return new Yytoken(TokenType.FN_ISNULL, text, yychar);
			case "LEN": return new Yytoken(TokenType.FN_LEN, text, yychar);
			case "TRIM": return new Yytoken(TokenType.FN_TRIM, text, yychar);
			case "LEFT": return new Yytoken(TokenType.FN_LEFT, text, yychar);
			case "RIGHT": return new Yytoken(TokenType.FN_RIGHT, text, yychar);
			case "SUBSTRING": return new Yytoken(TokenType.FN_SUBSTR, text, yychar);
			case "UPPER": return new Yytoken(TokenType.FN_UPPER, text, yychar);
			case "LOWER": return new Yytoken(TokenType.FN_LOWER, text, yychar);
			case "ASC": return new Yytoken(TokenType.ASCEND, text, yychar);
			case "DESC":  return new Yytoken(TokenType.DESCEND, text, yychar);
			//case "ESCAPE": return new Yytoken(TokenType.OP_ESCAPE, text, yychar); //V2: add parser support
			//case "IIF": return new Yytoken(TokenType.OP_IIF, text, yychar); //V2: add parser support
			//case "EXISTS": return new Yytoken(TokenType.FN_EXISTS, text, yychar); //V2: consider adding parser support
		}
	}
	return new Yytoken(TokenType.IDENT, text, yychar);
}

<YYINITIAL> \'  {
	buff = new StringBuilder();
	buffStartChar = yychar;
	yybegin(QUOTE);
	return null;
}
<QUOTE> (\'\')  {
	buff.Append("'");
	return null;
}
<QUOTE> \'  {
	string text = buff.ToString();
	buff = null;
	yybegin(YYINITIAL);
	return new Yytoken(TokenType.CONST, text, buffStartChar, text);
}
<QUOTE> . {
	buff.Append(yytext());
	return null;
}

<YYINITIAL> {DIGIT}+  {
	string text = yytext();
	try
	{
		long value = long.Parse(text, CultureInfo.InvariantCulture);
		if( value < int.MinValue || value > int.MaxValue )
		{
			return new Yytoken(TokenType.CONST, text, yychar, value);
		}
		else
		{
			return new Yytoken(TokenType.CONST, text, yychar, (int)value);
		}
	}
	catch
	{
		throw new OPathException("Could not convert '" + text + "' to an integer value.");
	}
}

<YYINITIAL> {DIGIT}+\.{DIGIT}+  {
	string text = yytext();
	try
	{
		decimal value = decimal.Parse(text, CultureInfo.InvariantCulture);
		return new Yytoken(TokenType.CONST, text, yychar, value);
	}
	catch
	{
		throw new OPathException("Could not convert '" + text + "' to a decimal value.");
	}
}

<YYINITIAL> \#  {
	buff = new StringBuilder();
	buffStartChar = yychar;
	yybegin(DATE);
	return null;
}
<DATE> \#  {
	string text = buff.ToString();
	buff = null;
	yybegin(YYINITIAL);
	try
	{
		DateTime value = DateTime.Parse(text);
		return new Yytoken(TokenType.CONST, text, buffStartChar, value);
	}
	catch
	{
		throw new OPathException("Could not convert '#" + text + "#' a DateTime value.");
		//throw new OPathException("Could not convert '#" + text + "#' a DateTime or TimeSpan value.");
	}

}
<DATE> . {
	buff.Append(yytext());
	return null;
}

<YYINITIAL> \{  {
	buff = new StringBuilder();
	buffStartChar = yychar;
	yybegin(GUID);
	return null;
}
<GUID> \}  {
	string text = buff.ToString();
	buff = null;
	yybegin(YYINITIAL);
	try
	{
		Guid value = new Guid(text);
		return new Yytoken(TokenType.CONST, text, buffStartChar, value);
	}
	catch
	{
		throw new OPathException("Could not convert '{" + text + "}' to a Guid value.");
	}
}
<GUID> . {
	buff.Append(yytext());
	return null;
}

<YYINITIAL,QUOTE,DATE,GUID> .  {
	string msg = string.Format("Syntax error in expression at position {0}. Character '{1}' is not allowed or was not expected.", yychar, yytext());
	throw new OPathException(msg);
}
