//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;
using System.IO;

namespace Wilson.ORMapper.Query
{
	internal class OPathLexer
	{
		private OPathBaseLexer _lexer;
		private Yytoken _currentToken;
		private Yytoken _lastToken;
		private Queue _nextTokenQueue;

		public OPathLexer(TextReader reader)
		{
			_lexer = new OPathBaseLexer(reader);
			_currentToken = null;
			_lastToken = null;
			_nextTokenQueue = new Queue();
		}

		public bool MoveToNext()
		{
			return (Lex() != null);
		}

		public Yytoken Lex()
		{
			_lastToken = _currentToken;
			if( _nextTokenQueue.Count > 0 )
			{
				_currentToken = (Yytoken)_nextTokenQueue.Dequeue();
			}
			else // empty queue
			{
				_currentToken = _lexer.Lex();
			}
			return _currentToken;
		}

		public Yytoken CurrentToken
		{
			get { return _currentToken; }
		}

		public Yytoken LastToken
		{
			get { return _lastToken; }
		}

		public Yytoken NextToken
		{
			get
			{
				if( _nextTokenQueue.Count == 0 )
				{
					_nextTokenQueue.Enqueue(_lexer.Lex());
				}
				return (Yytoken)_nextTokenQueue.Peek();
			}
		}
	}
}
