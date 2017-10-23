/*
 *  This specification is for a version of the RealCalc example.
 *  This version creates a abstract syntax tree from the input,
 *  and demonstrates how to use a reference type as the semantic
 *  value type.
 *
 *  The parser class is declared %partial, so that the bulk of
 *  the code can be placed in the separate file RealTreeHelper.cs
 *
 *  Process with > "Gppg.exe /nolines /gplex ExpressionParser.y" from the same directory.
 */

%output=GeneratedExpressionParser.cs
%namespace robotMath.Expression
%partial
//%sharetokens
//%start list

%parsertype Parser      //names the Parserclass to "Parser"
%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%tokentype Tokens       //names the Tokensenumeration to "Tokens"

%token Identifier, sym, lParen, rParent //the received Tokens from GPLEX
%token floatVal, intVal

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

%YYSTYPE Object

%left '+' '-'
%left '*' '/' '%'
%left '^'
%left UMINUS

%%

expr
	: Identifier '(' expr ',' expr ')'   { $$ = MakeIdentifierOp(  GetText(),  (Node) $3,  (Node) $5); }
	| Identifier '(' expr ')'            { $$ = MakeIdentifierOp(  GetText(),  (Node) $3); }
	| '(' expr ')'                       { $$ = (Node) $2; }
	| expr op expr                       { $$ = MakeBinary(    (NodeTag) $2,  (Node) $1,  (Node) $3  ); }
	| intVal                             { $$ = MakeLeaf( Convert.ToUInt64(GetText()) ); }
	| floatVal                           { $$ = MakeLeaf( Convert.ToDouble(GetText()) ); }
	| Identifier                         { $$ = MakeLeaf(    (String) $1 ); }
	;

op
	: sym  { $$ = GetNodeTag( GetText() ); }
	;

%%
/*
 * All the code is in the helper file ExpressionTreeHelper.cs
 */ 

