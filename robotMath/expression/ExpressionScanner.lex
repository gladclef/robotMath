/*
 * Scans for identifier and numbers.
 */

%namespace Scanner
%visibility public
%scannertype Scanner    //names the Scannerclass to "Scanner"
%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%tokentype Tokens

%option verbose summary caseinsensitive out:Build/ExpressionScanner.cs

Identifier [a-z_][a-z0-9_]*
floatVal [0-9]*(.,)[0-9]+
intVal [0-9]+

%%

{Identifier}   {yylval = yytext; return (int)Tokens.ID;}
{floatVal}     {return (int)Tokens.floatVal}
{intVal}       {return (int)Tokens.intVal}

%%
public static void Main(string[] argp)
{
	if (argp.Length == 0)
	{
		Console.WriteLine("Usage: strings filename(s), (wildcards ok)");
	}
	DirectoryInfo dirInfo = new DirectoryInfo(".");
	for (int i = 0; i < argp.Length; i++)
	{
		string name = argp[i];
		FileInfo[] fInfo = dirInfo.GetFiles(name);
		foreach (FileInfo info in fInfo)
		{
			try {
				int tok;
				FileStream file = new FileStream(info.Name, FileMode.Open); 
				Scanner scnr = new Scanner(file);
				Console.WriteLine("File: " + info.Name);
				do
				{
					tok = scnr.yylex();
					Console.WriteLine("tok: " + tok);
				} while (tok > (int)Tokens.EOF);
			} catch (IOException) {
				Console.WriteLine("File " + name + " not found");
			}
		}
	}
}