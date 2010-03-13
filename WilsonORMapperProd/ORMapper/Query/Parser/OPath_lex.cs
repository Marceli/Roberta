namespace Wilson.ORMapper.Query
{
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
/* test */


internal class OPathBaseLexer
{
private const int YY_BUFFER_SIZE = 512;
private const int YY_F = -1;
private const int YY_NO_STATE = -1;
private const int YY_NOT_ACCEPT = 0;
private const int YY_START = 1;
private const int YY_END = 2;
private const int YY_NO_ANCHOR = 4;
delegate Yytoken AcceptMethod();
AcceptMethod[] accept_dispatch;
private const int YY_BOL = 128;
private const int YY_EOF = 129;

private StringBuilder buff = null;
private int buffStartChar;
private System.IO.TextReader yy_reader;
private int yy_buffer_index;
private int yy_buffer_read;
private int yy_buffer_start;
private int yy_buffer_end;
private char[] yy_buffer;
private int yychar;
private bool yy_at_bol;
private int yy_lexical_state;

internal OPathBaseLexer(System.IO.TextReader reader) : this()
  {
  if (null == reader)
    {
    throw new System.ApplicationException("Error: Bad input stream initializer.");
    }
  yy_reader = reader;
  }

internal OPathBaseLexer(System.IO.FileStream instream) : this()
  {
  if (null == instream)
    {
    throw new System.ApplicationException("Error: Bad input stream initializer.");
    }
  yy_reader = new System.IO.StreamReader(instream);
  }

private OPathBaseLexer()
  {
  yy_buffer = new char[YY_BUFFER_SIZE];
  yy_buffer_read = 0;
  yy_buffer_index = 0;
  yy_buffer_start = 0;
  yy_buffer_end = 0;
  yychar = 0;
  yy_at_bol = true;
  yy_lexical_state = YYINITIAL;
accept_dispatch = new AcceptMethod[] 
 {
  null,
  null,
  new AcceptMethod(this.Accept_2),
  new AcceptMethod(this.Accept_3),
  new AcceptMethod(this.Accept_4),
  new AcceptMethod(this.Accept_5),
  new AcceptMethod(this.Accept_6),
  new AcceptMethod(this.Accept_7),
  new AcceptMethod(this.Accept_8),
  new AcceptMethod(this.Accept_9),
  new AcceptMethod(this.Accept_10),
  new AcceptMethod(this.Accept_11),
  new AcceptMethod(this.Accept_12),
  new AcceptMethod(this.Accept_13),
  new AcceptMethod(this.Accept_14),
  new AcceptMethod(this.Accept_15),
  new AcceptMethod(this.Accept_16),
  new AcceptMethod(this.Accept_17),
  new AcceptMethod(this.Accept_18),
  new AcceptMethod(this.Accept_19),
  new AcceptMethod(this.Accept_20),
  new AcceptMethod(this.Accept_21),
  new AcceptMethod(this.Accept_22),
  new AcceptMethod(this.Accept_23),
  new AcceptMethod(this.Accept_24),
  new AcceptMethod(this.Accept_25),
  new AcceptMethod(this.Accept_26),
  new AcceptMethod(this.Accept_27),
  new AcceptMethod(this.Accept_28),
  new AcceptMethod(this.Accept_29),
  new AcceptMethod(this.Accept_30),
  new AcceptMethod(this.Accept_31),
  new AcceptMethod(this.Accept_32),
  new AcceptMethod(this.Accept_33),
  new AcceptMethod(this.Accept_34),
  new AcceptMethod(this.Accept_35),
  new AcceptMethod(this.Accept_36),
  new AcceptMethod(this.Accept_37),
  new AcceptMethod(this.Accept_38),
  new AcceptMethod(this.Accept_39),
  new AcceptMethod(this.Accept_40),
  null,
  new AcceptMethod(this.Accept_42),
  null,
  new AcceptMethod(this.Accept_44),
  null,
  new AcceptMethod(this.Accept_46),
  null,
  };
  }

Yytoken Accept_2()
    { // begin accept action #2
{ return null; }
    } // end accept action #2

Yytoken Accept_3()
    { // begin accept action #3
{ return new Yytoken(TokenType.LPAREN, yytext(), yychar); }
    } // end accept action #3

Yytoken Accept_4()
    { // begin accept action #4
{ return new Yytoken(TokenType.RPAREN, yytext(), yychar); }
    } // end accept action #4

Yytoken Accept_5()
    { // begin accept action #5
{ return new Yytoken(TokenType.LBRACE, yytext(), yychar); }
    } // end accept action #5

Yytoken Accept_6()
    { // begin accept action #6
{ return new Yytoken(TokenType.RBRACE, yytext(), yychar); }
    } // end accept action #6

Yytoken Accept_7()
    { // begin accept action #7
{
	string msg = string.Format("Syntax error in expression at position {0}. Character '{1}' is not allowed or was not expected.", yychar, yytext());
	throw new OPathException(msg);
}
    } // end accept action #7

Yytoken Accept_8()
    { // begin accept action #8
{ return new Yytoken(TokenType.OP_EQ, yytext(), yychar); }
    } // end accept action #8

Yytoken Accept_9()
    { // begin accept action #9
{ return new Yytoken(TokenType.NOT, yytext(), yychar); }
    } // end accept action #9

Yytoken Accept_10()
    { // begin accept action #10
{ return new Yytoken(TokenType.OP_LT, yytext(), yychar); }
    } // end accept action #10

Yytoken Accept_11()
    { // begin accept action #11
{ return new Yytoken(TokenType.OP_GT, yytext(), yychar); }
    } // end accept action #11

Yytoken Accept_12()
    { // begin accept action #12
{ return new Yytoken(TokenType.PARENT, yytext(), yychar); }
    } // end accept action #12

Yytoken Accept_13()
    { // begin accept action #13
{ return new Yytoken(TokenType.PERIOD, yytext(), yychar); }
    } // end accept action #13

Yytoken Accept_14()
    { // begin accept action #14
{ return new Yytoken(TokenType.COMMA, yytext(), yychar); }
    } // end accept action #14

Yytoken Accept_15()
    { // begin accept action #15
{ return new Yytoken(TokenType.PLUS, yytext(), yychar); }
    } // end accept action #15

Yytoken Accept_16()
    { // begin accept action #16
{ return new Yytoken(TokenType.MINUS, yytext(), yychar); }
    } // end accept action #16

Yytoken Accept_17()
    { // begin accept action #17
{ return new Yytoken(TokenType.MULTIPLY, yytext(), yychar); }
    } // end accept action #17

Yytoken Accept_18()
    { // begin accept action #18
{ return new Yytoken(TokenType.DIVIDE, yytext(), yychar); }
    } // end accept action #18

Yytoken Accept_19()
    { // begin accept action #19
{ return new Yytoken(TokenType.MODULO, yytext(), yychar); }
    } // end accept action #19

Yytoken Accept_20()
    { // begin accept action #20
{ return new Yytoken(TokenType.PARAM, yytext(), yychar); }
    } // end accept action #20

Yytoken Accept_21()
    { // begin accept action #21
{
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
    } // end accept action #21

Yytoken Accept_22()
    { // begin accept action #22
{
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
    } // end accept action #22

Yytoken Accept_23()
    { // begin accept action #23
{
	buff = new StringBuilder();
	buffStartChar = yychar;
	yybegin(QUOTE);
	return null;
}
    } // end accept action #23

Yytoken Accept_24()
    { // begin accept action #24
{
	buff = new StringBuilder();
	buffStartChar = yychar;
	yybegin(DATE);
	return null;
}
    } // end accept action #24

Yytoken Accept_25()
    { // begin accept action #25
{
	buff = new StringBuilder();
	buffStartChar = yychar;
	yybegin(GUID);
	return null;
}
    } // end accept action #25

Yytoken Accept_26()
    { // begin accept action #26
{ return new Yytoken(TokenType.AND, yytext(), yychar); }
    } // end accept action #26

Yytoken Accept_27()
    { // begin accept action #27
{ return new Yytoken(TokenType.OR, yytext(), yychar); }
    } // end accept action #27

Yytoken Accept_28()
    { // begin accept action #28
{ return new Yytoken(TokenType.OP_EQ, yytext(), yychar); }
    } // end accept action #28

Yytoken Accept_29()
    { // begin accept action #29
{ return new Yytoken(TokenType.OP_NE, yytext(), yychar); }
    } // end accept action #29

Yytoken Accept_30()
    { // begin accept action #30
{ return new Yytoken(TokenType.OP_LE, yytext(), yychar); }
    } // end accept action #30

Yytoken Accept_31()
    { // begin accept action #31
{ return new Yytoken(TokenType.OP_NE, yytext(), yychar); }
    } // end accept action #31

Yytoken Accept_32()
    { // begin accept action #32
{ return new Yytoken(TokenType.OP_GE, yytext(), yychar); }
    } // end accept action #32

Yytoken Accept_33()
    { // begin accept action #33
{
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
    } // end accept action #33

Yytoken Accept_34()
    { // begin accept action #34
{
	buff.Append(yytext());
	return null;
}
    } // end accept action #34

Yytoken Accept_35()
    { // begin accept action #35
{
	string text = buff.ToString();
	buff = null;
	yybegin(YYINITIAL);
	return new Yytoken(TokenType.CONST, text, buffStartChar, text);
}
    } // end accept action #35

Yytoken Accept_36()
    { // begin accept action #36
{
	buff.Append("'");
	return null;
}
    } // end accept action #36

Yytoken Accept_37()
    { // begin accept action #37
{
	buff.Append(yytext());
	return null;
}
    } // end accept action #37

Yytoken Accept_38()
    { // begin accept action #38
{
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
    } // end accept action #38

Yytoken Accept_39()
    { // begin accept action #39
{
	buff.Append(yytext());
	return null;
}
    } // end accept action #39

Yytoken Accept_40()
    { // begin accept action #40
{
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
    } // end accept action #40

Yytoken Accept_42()
    { // begin accept action #42
{
	string msg = string.Format("Syntax error in expression at position {0}. Character '{1}' is not allowed or was not expected.", yychar, yytext());
	throw new OPathException(msg);
}
    } // end accept action #42

Yytoken Accept_44()
    { // begin accept action #44
{
	string msg = string.Format("Syntax error in expression at position {0}. Character '{1}' is not allowed or was not expected.", yychar, yytext());
	throw new OPathException(msg);
}
    } // end accept action #44

Yytoken Accept_46()
    { // begin accept action #46
{
	string msg = string.Format("Syntax error in expression at position {0}. Character '{1}' is not allowed or was not expected.", yychar, yytext());
	throw new OPathException(msg);
}
    } // end accept action #46

private bool yy_eof_done = false;
private void yy_do_eof ()
  {
  if (!yy_eof_done)
    {
    
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

    }
  yy_eof_done = true;
  }

private const int YYINITIAL = 0;
private const int GUID = 3;
private const int QUOTE = 1;
private const int DATE = 2;
private static int[] yy_state_dtrans = new int[] 
  {   0,
  43,
  45,
  47
  };
private void yybegin (int state)
  {
  yy_lexical_state = state;
  }

private char yy_advance ()
  {
  int next_read;
  int i;
  int j;

  if (yy_buffer_index < yy_buffer_read)
    {
    return yy_buffer[yy_buffer_index++];
    }

  if (0 != yy_buffer_start)
    {
    i = yy_buffer_start;
    j = 0;
    while (i < yy_buffer_read)
      {
      yy_buffer[j] = yy_buffer[i];
      i++;
      j++;
      }
    yy_buffer_end = yy_buffer_end - yy_buffer_start;
    yy_buffer_start = 0;
    yy_buffer_read = j;
    yy_buffer_index = j;
    next_read = yy_reader.Read(yy_buffer,yy_buffer_read,
                  yy_buffer.Length - yy_buffer_read);
    if (next_read <= 0)
      {
      return (char) YY_EOF;
      }
    yy_buffer_read = yy_buffer_read + next_read;
    }
  while (yy_buffer_index >= yy_buffer_read)
    {
    if (yy_buffer_index >= yy_buffer.Length)
      {
      yy_buffer = yy_double(yy_buffer);
      }
    next_read = yy_reader.Read(yy_buffer,yy_buffer_read,
                  yy_buffer.Length - yy_buffer_read);
    if (next_read <= 0)
      {
      return (char) YY_EOF;
      }
    yy_buffer_read = yy_buffer_read + next_read;
    }
  return yy_buffer[yy_buffer_index++];
  }
private void yy_move_end ()
  {
  if (yy_buffer_end > yy_buffer_start && 
      '\n' == yy_buffer[yy_buffer_end-1])
    yy_buffer_end--;
  if (yy_buffer_end > yy_buffer_start &&
      '\r' == yy_buffer[yy_buffer_end-1])
    yy_buffer_end--;
  }
private bool yy_last_was_cr=false;
private void yy_mark_start ()
  {
  yychar = yychar + yy_buffer_index - yy_buffer_start;
  yy_buffer_start = yy_buffer_index;
  }
private void yy_mark_end ()
  {
  yy_buffer_end = yy_buffer_index;
  }
private void yy_to_mark ()
  {
  yy_buffer_index = yy_buffer_end;
  yy_at_bol = (yy_buffer_end > yy_buffer_start) &&
    (yy_buffer[yy_buffer_end-1] == '\r' ||
    yy_buffer[yy_buffer_end-1] == '\n');
  }
internal string yytext()
  {
  return (new string(yy_buffer,
                yy_buffer_start,
                yy_buffer_end - yy_buffer_start)
         );
  }
private int yylength ()
  {
  return yy_buffer_end - yy_buffer_start;
  }
private char[] yy_double (char[] buf)
  {
  int i;
  char[] newbuf;
  newbuf = new char[2*buf.Length];
  for (i = 0; i < buf.Length; i++)
    {
    newbuf[i] = buf[i];
    }
  return newbuf;
  }
private const int YY_E_INTERNAL = 0;
private const int YY_E_MATCH = 1;
private static string[] yy_error_string = new string[]
  {
  "Error: Internal error.\n",
  "Error: Unmatched input.\n"
  };
private void yy_error (int code,bool fatal)
  {
  System.Console.Write(yy_error_string[code]);
  if (fatal)
    {
    throw new System.ApplicationException("Fatal Error.\n");
    }
  }
private static int[] yy_acpt = new int[]
  {
  /* 0 */   YY_NOT_ACCEPT,
  /* 1 */   YY_NO_ANCHOR,
  /* 2 */   YY_NO_ANCHOR,
  /* 3 */   YY_NO_ANCHOR,
  /* 4 */   YY_NO_ANCHOR,
  /* 5 */   YY_NO_ANCHOR,
  /* 6 */   YY_NO_ANCHOR,
  /* 7 */   YY_NO_ANCHOR,
  /* 8 */   YY_NO_ANCHOR,
  /* 9 */   YY_NO_ANCHOR,
  /* 10 */   YY_NO_ANCHOR,
  /* 11 */   YY_NO_ANCHOR,
  /* 12 */   YY_NO_ANCHOR,
  /* 13 */   YY_NO_ANCHOR,
  /* 14 */   YY_NO_ANCHOR,
  /* 15 */   YY_NO_ANCHOR,
  /* 16 */   YY_NO_ANCHOR,
  /* 17 */   YY_NO_ANCHOR,
  /* 18 */   YY_NO_ANCHOR,
  /* 19 */   YY_NO_ANCHOR,
  /* 20 */   YY_NO_ANCHOR,
  /* 21 */   YY_NO_ANCHOR,
  /* 22 */   YY_NO_ANCHOR,
  /* 23 */   YY_NO_ANCHOR,
  /* 24 */   YY_NO_ANCHOR,
  /* 25 */   YY_NO_ANCHOR,
  /* 26 */   YY_NO_ANCHOR,
  /* 27 */   YY_NO_ANCHOR,
  /* 28 */   YY_NO_ANCHOR,
  /* 29 */   YY_NO_ANCHOR,
  /* 30 */   YY_NO_ANCHOR,
  /* 31 */   YY_NO_ANCHOR,
  /* 32 */   YY_NO_ANCHOR,
  /* 33 */   YY_NO_ANCHOR,
  /* 34 */   YY_NO_ANCHOR,
  /* 35 */   YY_NO_ANCHOR,
  /* 36 */   YY_NO_ANCHOR,
  /* 37 */   YY_NO_ANCHOR,
  /* 38 */   YY_NO_ANCHOR,
  /* 39 */   YY_NO_ANCHOR,
  /* 40 */   YY_NO_ANCHOR,
  /* 41 */   YY_NOT_ACCEPT,
  /* 42 */   YY_NO_ANCHOR,
  /* 43 */   YY_NOT_ACCEPT,
  /* 44 */   YY_NO_ANCHOR,
  /* 45 */   YY_NOT_ACCEPT,
  /* 46 */   YY_NO_ANCHOR,
  /* 47 */   YY_NOT_ACCEPT
  };
private static int[] yy_cmap = new int[]
  {
  26, 26, 26, 26, 26, 26, 26, 26,
  27, 27, 1, 26, 26, 1, 26, 26,
  26, 26, 26, 26, 26, 26, 26, 26,
  26, 26, 26, 26, 26, 26, 26, 26,
  27, 9, 26, 28, 26, 19, 6, 25,
  2, 3, 17, 15, 14, 16, 13, 18,
  23, 23, 23, 23, 23, 23, 23, 23,
  23, 23, 26, 26, 10, 8, 11, 20,
  21, 22, 22, 22, 22, 22, 22, 22,
  22, 22, 22, 22, 22, 22, 22, 22,
  22, 22, 22, 22, 22, 22, 22, 22,
  22, 22, 22, 4, 26, 5, 12, 24,
  26, 22, 22, 22, 22, 22, 22, 22,
  22, 22, 22, 22, 22, 22, 22, 22,
  22, 22, 22, 22, 22, 22, 22, 22,
  22, 22, 22, 29, 7, 30, 26, 26,
  0, 0 
  };
private static int[] yy_rmap = new int[]
  {
  0, 1, 2, 1, 1, 1, 1, 3,
  4, 5, 6, 7, 1, 1, 1, 1,
  1, 1, 1, 1, 1, 8, 9, 1,
  1, 1, 1, 1, 1, 1, 1, 1,
  1, 10, 1, 11, 1, 1, 1, 1,
  1, 10, 12, 13, 14, 15, 1, 16
  };
private static int[,] yy_nxt = new int[,]
  {
  { 1, 2, 3, 4, 5, 6, 7, 42,
   8, 9, 10, 11, 12, 13, 14, 15,
   16, 17, 18, 19, 20, 44, 21, 22,
   46, 23, 46, 2, 24, 25, 46 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, 2, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, 2, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, 26, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   28, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   29, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   30, -1, -1, 31, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   32, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, 21, 21,
   21, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, 41, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, 22,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, 33,
   -1, -1, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, 36, -1, -1, -1, -1, -1 },
  { -1, -1, -1, -1, -1, -1, -1, 27,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { 1, -1, 34, 34, 34, 34, 34, 34,
   34, 34, 34, 34, 34, 34, 34, 34,
   34, 34, 34, 34, 34, 34, 34, 34,
   34, 35, 34, 34, 34, 34, 34 },
  { -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, 21, -1,
   -1, -1, -1, -1, -1, -1, -1 },
  { 1, -1, 37, 37, 37, 37, 37, 37,
   37, 37, 37, 37, 37, 37, 37, 37,
   37, 37, 37, 37, 37, 37, 37, 37,
   37, 37, 37, 37, 38, 37, 37 },
  { 1, -1, 39, 39, 39, 39, 39, 39,
   39, 39, 39, 39, 39, 39, 39, 39,
   39, 39, 39, 39, 39, 39, 39, 39,
   39, 39, 39, 39, 39, 39, 40 }
  };
public Yytoken Lex()
  {
  char yy_lookahead;
  int yy_anchor = YY_NO_ANCHOR;
  int yy_state = yy_state_dtrans[yy_lexical_state];
  int yy_next_state = YY_NO_STATE;
  int yy_last_accept_state = YY_NO_STATE;
  bool yy_initial = true;
  int yy_this_accept;

  yy_mark_start();
  yy_this_accept = yy_acpt[yy_state];
  if (YY_NOT_ACCEPT != yy_this_accept)
    {
    yy_last_accept_state = yy_state;
    yy_mark_end();
    }
  while (true)
    {
    if (yy_initial && yy_at_bol)
      yy_lookahead = (char) YY_BOL;
    else
      {
      yy_lookahead = yy_advance();
      }
    yy_next_state = yy_nxt[yy_rmap[yy_state],yy_cmap[yy_lookahead]];
    if (YY_EOF == yy_lookahead && yy_initial)
      {
        yy_do_eof();
        return null;
      }
    if (YY_F != yy_next_state)
      {
      yy_state = yy_next_state;
      yy_initial = false;
      yy_this_accept = yy_acpt[yy_state];
      if (YY_NOT_ACCEPT != yy_this_accept)
        {
        yy_last_accept_state = yy_state;
        yy_mark_end();
        }
      }
    else
      {
      if (YY_NO_STATE == yy_last_accept_state)
        {
        throw new System.ApplicationException("Lexical Error: Unmatched Input.");
        }
      else
        {
        yy_anchor = yy_acpt[yy_last_accept_state];
        if (0 != (YY_END & yy_anchor))
          {
          yy_move_end();
          }
        yy_to_mark();
        if (yy_last_accept_state < 0)
          {
          if (yy_last_accept_state < 48)
            yy_error(YY_E_INTERNAL, false);
          }
        else
          {
          AcceptMethod m = accept_dispatch[yy_last_accept_state];
          if (m != null)
            {
            Yytoken tmp = m();
            if (tmp != null)
              return tmp;
            }
          }
        yy_initial = true;
        yy_state = yy_state_dtrans[yy_lexical_state];
        yy_next_state = YY_NO_STATE;
        yy_last_accept_state = YY_NO_STATE;
        yy_mark_start();
        yy_this_accept = yy_acpt[yy_state];
        if (YY_NOT_ACCEPT != yy_this_accept)
          {
          yy_last_accept_state = yy_state;
          yy_mark_end();
          }
        }
      }
    }
  }
}

}
