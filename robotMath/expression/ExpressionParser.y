/*
 *  This specification is for a version of the RealCalc example.
 *  This version creates a abstract syntax tree from the input,
 *  and demonstrates how to use a reference type as the semantic
 *  value type.
 *
 *  The parser class is declared %partial, so that the bulk of
 *  the code can be placed in the separate file RealTreeHelper.cs
 *
 *  Process with > gppg /nolines RealTree.y
 */

%using Scanner
%output=Build/ExpressionParser.cs
%namespace Parser
%partial
//%sharetokens
//%start list

%parsertype Parser      //names the Parserclass to "Parser"
%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%tokentype Tokens       //names the Tokensenumeration to "Tokens"

%token ID //the received Tokens from GPLEX
%token float, int

/*
 * The accessibility of the Parser object must not exceed that
 * of the inherited ShiftReduceParser<,>. Thus if you want to include 
 * the *source* of ShiftReduceParser from ShiftReduceParserCode.cs, 
 * then you must either set the compilation flag EXPORT_GPPG or  
 * override the default, public visibility with %visibility internal.
 * If you reference the pre-compiled QUT.ShiftReduceParser.dll then 
 * ShiftReduceParser<> is public and either visibility will work.
 */

%visibility public

%YYSTYPE ExpressionTree.Node

%left '+' '-'
%left '*' '/' '%'
%left '^'
%left UMINUS

%%

expr
	: Identifier '(' expr ')'   { $$ = MakeIdentifier($1, $3); }
	| '(' expr ')'		        { $$ = $2; }
	|  expr '^' expr            { $$ = MakeBinary(NodeTag.exp, $1, $3); }
    |  expr '*' expr	        { $$ = MakeBinary(NodeTag.mul, $1, $3); }
	|  expr '/' expr	        { $$ = MakeBinary(NodeTag.div, $1, $3); }
	|  expr '%' expr	        { $$ = MakeBinary(NodeTag.rem, $1, $3); }
	|  expr '+' expr	        { $$ = MakeBinary(NodeTag.plus, $1, $3); }
	|  expr '-' expr	        { $$ = MakeBinary(NodeTag.minus, $1, $3); }
	|  int                      // $$ is automatically lexer.yylval
	|  float                    // $$ is automatically lexer.yylval
	|  '-' expr %prec UMINUS {
				$$ = MakeUnary(NodeTag.negate, $2);
			}
	;

%%
/*
 * All the code is in the helper file ExpressionTreeHelper.cs
 */ 

