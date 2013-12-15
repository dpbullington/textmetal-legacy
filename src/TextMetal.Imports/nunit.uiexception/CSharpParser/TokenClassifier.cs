// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Generic;

namespace NUnit.UiException.CodeFormatters
{
	/// <summary>
	/// Used at an internal stage to convert LexToken into ClassifiedToken. This class provides
	/// a very basic semantic analysis to make text following in one the categories below:
	/// - regular code,
	/// - developper comments,
	/// - strings / character.
	/// The output of this class is used by CSharpCodeFormatter to achieve the basic syntax coloring.
	/// </summary>
	public class TokenClassifier
	{
		// the list below contains constant values defining states for the finite
		// smState machine that makes all the work of converting LexToken into ClassifiedToken.
		// for instance, Lexer can send inputs like:
		//
		//   [Text][Separator][CommentC_Open][Text][CommentC_Close]
		//
		// This LexToken sequence can for instance be converted that way by TokenClassifier.
		//
		//   - [Text][Separator]                     => [Code]
		//   - [CommentC_Open][Text][CommentC_Close] => [Comment]
		// 

		#region Constructors/Destructors

		/// <summary>
		/// Build a new instance of TokenClassifier.
		/// </summary>
		public TokenClassifier()
		{
			string[] words;

			this._sm = new StateMachine();

			this._tags = new Dictionary<int, ClassificationTag>();
			this._tags.Add(SMSTATE_CODE, ClassificationTag.Code);
			this._tags.Add(SMSTATE_CCOMMENT, ClassificationTag.Comment);
			this._tags.Add(SMSTATE_CPPCOMMENT, ClassificationTag.Comment);
			this._tags.Add(SMSTATE_CHAR, ClassificationTag.String);
			this._tags.Add(SMSTATE_STRING, ClassificationTag.String);

			// build the list of predefined keywords.
			// this is from the official msdn site. Curiously, some keywords
			// were ommited from the official documentation.
			//   For instance "get", "set", "region" and "endregion" were
			// not part of the official list. Maybe it's a mistake or a misunderstanding
			// whatever... I want them paint in blue as well!

			words = new string[]
					{
						"abstract", "event", "new", "struct", "as", "explicit", "null", "switch",
						"base", "extern", "object", "this", "bool", "false", "operator", "throw",
						"break", "finally", "out", "true", "byte", "fixed", "override", "try", "case",
						"float", "params", "typeof", "catch", "for", "private", "uint", "char",
						"foreach", "protected", "ulong", "checked", "goto", "public", "unchecked",
						"class", "if", "readonly", "unsafe", "const", "implicit", "ref", "ushort",
						"continue", "in", "return", "using", "decimal", "int", "sbyte", "virtual",
						"default", "interface", "sealed", "volatile", "delegate", "internal",
						"short", "void", "do", "is", "sizeof", "while", "double", "lock", "stackalloc",
						"else", "long", "static", "enum", "namespace", "string", "partial", "get", "set",
						"region", "endregion",
					};

			this._keywords = new Dictionary<string, bool>();
			foreach (string key in words)
				this._keywords.Add(key, true);

			this.Reset();

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// State code for the smState machine.
		/// State when reaching a C comment block.
		/// </summary>
		public const int SMSTATE_CCOMMENT = 1;

		/// <summary>
		/// State code for the smState machine.
		/// State when reaching a char surrounded by single quotes.
		/// </summary>
		public const int SMSTATE_CHAR = 3;

		/// <summary>
		/// State code for the smState machine.
		/// State when reaching a code block.
		/// </summary>
		public const int SMSTATE_CODE = 0;

		/// <summary>
		/// State code for the smState machine.
		/// State when reaching a C++ comment block.
		/// </summary>
		public const int SMSTATE_CPPCOMMENT = 2;

		/// <summary>
		/// State code for the smState machine.
		/// State when reaching a string surrounded by double quotes.
		/// </summary>
		public const int SMSTATE_STRING = 4;

		/// <summary>
		/// Indicate whether Lexer is in escaping mode.
		/// This flag is set to true when parsing "\\" and
		/// can influate on the following LexerTag value.
		/// </summary>
		private bool _escaping;

		/// <summary>
		/// Contains the list of C# keywords.
		/// </summary>
		private Dictionary<string, bool> _keywords;

		/// <summary>
		/// A finite smState machine where states are: SMSTATE values and
		/// transitions are LexToken.
		/// </summary>
		private StateMachine _sm;

		/// <summary>
		/// The current StateMachine's SMTATE code.
		/// </summary>
		private int _sm_output;

		/// <summary>
		/// Makes a link between SMSTATE code and ClassificationTag.
		/// </summary>
		private Dictionary<int, ClassificationTag> _tags;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Tells whether TokenClassifier is currently in escaping mode. When true,
		/// this flag causes TokenClassifier to override the final classification
		/// of a basic entity (such as: ") to be treated as normal text instead of
		/// being interpreted as a string delimiter.
		/// </summary>
		public bool Escaping
		{
			get
			{
				return (this._escaping);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Classify the given token and get its corresponding SMSTATE value.
		/// </summary>
		/// <param name="token"> The LexToken to be classified. </param>
		/// <returns> An SMSTATE value. </returns>
		protected int AcceptLexToken(LexToken token)
		{
			int smState;

			if (this._escaping)
				return (SMSTATE_STRING);

			smState = this.GetTokenSMSTATE(this._sm_output, token.Tag);
			this._sm_output = this.GetSMSTATE(this._sm_output, token.Tag);

			return (smState);
		}

		/// <summary>
		/// Classify the given LexToken into a ClassificationTag.
		/// </summary>
		/// <param name="token"> The token to be classified. </param>
		/// <returns> The smState value. </returns>
		public ClassificationTag Classify(LexToken token)
		{
			int classTag;

			UiExceptionHelper.CheckNotNull(token, "token");

			classTag = this.AcceptLexToken(token);

			if (classTag == SMSTATE_CODE &&
				this._keywords.ContainsKey(token.Text))
				return (ClassificationTag.Keyword);

			// Parsing a token whoose Text value is set to '\'
			// causes the classifier to set/reset is escaping mode.

			if (token.Text == "\\" &&
				this._sm_output == SMSTATE_STRING &&
				!this._escaping)
				this._escaping = true;
			else
				this._escaping = false;

			return (this._tags[classTag]);
		}

		/// <summary>
		/// Gets the SMSTATE under the "transition" going from "smState".
		/// </summary>
		/// <param name="smState"> The current smState. </param>
		/// <param name="transition"> The current LexerTag. </param>
		/// <returns> The new smState. </returns>
		protected int GetSMSTATE(int smState, LexerTag transition)
		{
			return (this._sm.GetSMSTATE(smState, transition));
		}

		/// <summary>
		/// Gets a token SMSTATE under the "transition" going from "smState".
		/// </summary>
		/// <param name="smState"> The current smState machine. </param>
		/// <param name="transition"> The LexerTag to be classified. </param>
		/// <returns> The LexerTag's classification. </returns>
		protected int GetTokenSMSTATE(int smState, LexerTag transition)
		{
			return (this._sm.GetTokenSMSTATE(smState, transition));
		}

		/// <summary>
		/// Reset the StateMachine to default value. (code block).
		/// </summary>
		public void Reset()
		{
			this._sm_output = SMSTATE_CODE;
			this._escaping = false;

			return;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// Defines a state (of a state machine) and its associated transitions.
		/// </summary>
		private class State
		{
			#region Constructors/Destructors

			public State(int initialState, TransitionData[] transitions)
			{
				int i;
				int j;

				UiExceptionHelper.CheckNotNull(transitions, "transitions");
				UiExceptionHelper.CheckTrue(
					transitions.Length == 8,
					"expecting transitions.Length to be 8",
					"transitions");

				for (i = 0; i < transitions.Length; ++i)
				{
					for (j = 0; j < transitions.Length; ++j)
					{
						if (j == i)
							continue;

						if (transitions[j].Transition == transitions[i].Transition)
						{
							UiExceptionHelper.CheckTrue(false,
								String.Format("transition '{0}' already present", transitions[j].Transition),
								"transitions");
						}
					}
				}

				this.InitialState = initialState;
				this.Transitions = transitions;

				return;
			}

			#endregion

			#region Fields/Constants

			public int InitialState;
			public TransitionData[] Transitions;

			#endregion

			#region Properties/Indexers/Events

			public TransitionData this[LexerTag transition]
			{
				get
				{
					foreach (TransitionData couple in this.Transitions)
					{
						if (couple.Transition == transition)
							return (couple);
					}
					return (null);
				}
			}

			#endregion
		}

		/// <summary>
		/// A finite state machine. Where states are SMSTATE codes and
		/// transitions are LexTokens.
		/// </summary>
		private class StateMachine
		{
			#region Constructors/Destructors

			public StateMachine()
			{
				this._states = new State[5];

				// defines transitions from SMSTATE_CODE
				this._states[0] = new State(
					SMSTATE_CODE,
					new TransitionData[]
					{
						new TransitionData(LexerTag.EndOfLine, SMSTATE_CODE),
						new TransitionData(LexerTag.Separator, SMSTATE_CODE),
						new TransitionData(LexerTag.Text, SMSTATE_CODE),
						new TransitionData(LexerTag.CommentC_Open, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.CommentC_Close, SMSTATE_CODE, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.CommentCpp, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.SingleQuote, SMSTATE_CHAR),
						new TransitionData(LexerTag.DoubleQuote, SMSTATE_STRING),
					});

				// defines transitions from SMSTATE_CCOMMENT
				this._states[1] = new State(
					SMSTATE_CCOMMENT,
					new TransitionData[]
					{
						new TransitionData(LexerTag.EndOfLine, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.Separator, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.Text, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.CommentC_Open, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.CommentC_Close, SMSTATE_CODE, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.CommentCpp, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.SingleQuote, SMSTATE_CCOMMENT),
						new TransitionData(LexerTag.DoubleQuote, SMSTATE_CCOMMENT),
					});

				// defines transitions from SMSTATE_CPPCOMMENT
				this._states[2] = new State(
					SMSTATE_CPPCOMMENT,
					new TransitionData[]
					{
						new TransitionData(LexerTag.EndOfLine, SMSTATE_CODE),
						new TransitionData(LexerTag.Separator, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.Text, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.CommentC_Open, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.CommentC_Close, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.CommentCpp, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.SingleQuote, SMSTATE_CPPCOMMENT),
						new TransitionData(LexerTag.DoubleQuote, SMSTATE_CPPCOMMENT),
					});

				// defines transition from SMSTATE_CHAR
				this._states[3] = new State(
					SMSTATE_CHAR,
					new TransitionData[]
					{
						new TransitionData(LexerTag.EndOfLine, SMSTATE_CHAR),
						new TransitionData(LexerTag.Separator, SMSTATE_CHAR),
						new TransitionData(LexerTag.Text, SMSTATE_CHAR),
						new TransitionData(LexerTag.CommentC_Open, SMSTATE_CHAR),
						new TransitionData(LexerTag.CommentC_Close, SMSTATE_CHAR),
						new TransitionData(LexerTag.CommentCpp, SMSTATE_CHAR),
						new TransitionData(LexerTag.SingleQuote, SMSTATE_CODE, SMSTATE_CHAR),
						new TransitionData(LexerTag.DoubleQuote, SMSTATE_CHAR),
					});

				// defines transition from SMSTATE_STRING
				this._states[4] = new State(
					SMSTATE_STRING,
					new TransitionData[]
					{
						new TransitionData(LexerTag.EndOfLine, SMSTATE_STRING),
						new TransitionData(LexerTag.Separator, SMSTATE_STRING),
						new TransitionData(LexerTag.Text, SMSTATE_STRING),
						new TransitionData(LexerTag.CommentC_Open, SMSTATE_STRING),
						new TransitionData(LexerTag.CommentC_Close, SMSTATE_STRING),
						new TransitionData(LexerTag.CommentCpp, SMSTATE_STRING),
						new TransitionData(LexerTag.SingleQuote, SMSTATE_STRING),
						new TransitionData(LexerTag.DoubleQuote, SMSTATE_CODE, SMSTATE_STRING),
					});

				return;
			}

			#endregion

			#region Fields/Constants

			private State[] _states;

			#endregion

			#region Methods/Operators

			/// <summary>
			/// Follow "transition" going from "smState" and returns reached SMSTATE.
			/// </summary>
			public int GetSMSTATE(int smState, LexerTag transition)
			{
				foreach (State st in this._states)
				{
					if (st.InitialState == smState)
						return (st[transition].SMSTATE);
				}
				return (SMSTATE_CODE);
			}

			/// <summary>
			/// Follow "transition" going from "smState" and returns reached TokenSMSTATE.
			/// </summary>
			public int GetTokenSMSTATE(int smState, LexerTag transition)
			{
				foreach (State st in this._states)
				{
					if (st.InitialState == smState)
						return (st[transition].TokenSMSTATE);
				}
				return (SMSTATE_CODE);
			}

			#endregion
		}

		/// <summary>
		/// Defines a transition (of a state machine).
		/// </summary>
		private class TransitionData
		{
			#region Constructors/Destructors

			public TransitionData(LexerTag transition, int smState)
			{
				this.Transition = transition;

				this.SMSTATE = smState;
				this.TokenSMSTATE = smState;

				return;
			}

			public TransitionData(LexerTag transition, int smState, int tokenSmState)
				:
					this(transition, smState)
			{
				this.TokenSMSTATE = tokenSmState;
			}

			#endregion

			#region Fields/Constants

			/// <summary>
			/// The SMSTATE code reached when following that transition.
			/// </summary>
			public int SMSTATE;

			/// <summary>
			/// The TokenSMSTATE reached when following that transition.
			/// </summary>
			public int TokenSMSTATE;

			/// <summary>
			/// The current transition.
			/// </summary>
			public LexerTag Transition;

			#endregion
		}

		#endregion
	}
}