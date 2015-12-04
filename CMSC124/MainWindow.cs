	using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Gtk;
using CMSC124;

public partial class MainWindow: Gtk.Window
{
	List<Regex> regexList;
	List<string> literalsList = new List<string>();
	List<Lexeme> lexemeList;
	List<Variable> variableList;
	Gtk.ListStore lexStore;
	Gtk.ListStore idenStore;
	FileFilter filter;
	Boolean kthxbyeFlag = false;
	Boolean haiFlag = false;
	string stringLiteral = "\"([^\"]|\"\")*\"$";
	string floatLiteral = "^[\\-]?[0-9]*\\.[0-9]+$";
	string integerLiteral = "^[\\-]?[0-9]+$";
	string booleanLiteral = "^(WIN|FAIL)$";
	string literal = @"""([^""]|"""")*""$|^[\-]?[0-9]*\.[0-9]+$|^[\-]?[0-9]+$|^(WIN|FAIL)$";  
	string varName = "[a-zA-Z]([a-zA-Z0-9+_])*";
	object IT;
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		boxInit ();
		filterInit ();
		regexInit ();
		columnInit ();
	}
	protected void boxInit(){
		Gdk.Color col = new Gdk.Color ();
		Gdk.Color.Parse ("red", ref col);
		console.ModifyBase(StateType.Normal, new Gdk.Color(0,0,0));
		console.ModifyText(StateType.Normal, new Gdk.Color(0,255,0));
	}
	protected void columnInit(){
		statements.AppendColumn ("LEXEME",  new Gtk.CellRendererText(), "text", 0);
		statements.AppendColumn ("TAG",  new Gtk.CellRendererText(), "text", 1);
		lexStore = new Gtk.ListStore (typeof (string), typeof (string));
		statements.Model = lexStore;
		variables.AppendColumn ("NAME",  new Gtk.CellRendererText(), "text", 0);
		variables.AppendColumn ("VALUE",  new Gtk.CellRendererText(), "text", 1);
		variables.AppendColumn ("DATA TYPE",  new Gtk.CellRendererText(), "text", 2);
		idenStore = new Gtk.ListStore (typeof (string), typeof (string), typeof (string));
		variables.Model = idenStore;
	}
	protected void regexInit(){
		regexList = new List<Regex> ();	
		regexList.Add (new Regex (@"^\s*(HAI)(\sBTW\s+)?"));
		regexList.Add (new Regex (@"^\s*(I)\s+(HAS)\s+(A)\s+"+ varName + @"(\sITZ\s(A)?.+)?" + @"(\sBTW\s+.*)?"));
		regexList.Add (new Regex (@"^\s*(VISIBLE)\s+(\sBTW\s+)?"));
		regexList.Add (new Regex (@"^\s*" + varName + @"\s+(R)\s+.+"));
		regexList.Add (new Regex (@"^\s*(KTHXBYE)(\sBTW\s+)?"));

		//regexList.Add (new Regex(@"^(VISIBLE)\s\""?.*\""?(\sBTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(BOTH SAEM).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(DIFFRINT).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(BIGGR OF).*(AN).*(\sBTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(SMALLR OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(SUM OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(DIFF OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(PRODUKT OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(QUOSHUNT OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(MOD OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(BOTH OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(EITHER OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(WON OF).*(AN).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(ALL OF)\s.+(\s(AN)\s.+)+(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(ANY OF)\s.+(\s(AN)\s.+)+(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(NOT)\s.*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(YA RLY)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(NO WAI)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(OIC)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(WTF\?)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"\s*(OMG)\s*.*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"\s*(OMGWTF)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(GTFO)(\s+BTW\s+)?"));
//		regexList.Add (new Regex(@"(BTW)"));
		regexList.Add (new Regex(@"^\s*(OBTW).*(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(TLDR)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(SMOOSH)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(MKAY)(\s+BTW\s+)?"));
		regexList.Add (new Regex(@"^\s*(GIMMEH)\s.*(\s+BTW\s+)?"));

		literalsList.Add (integerLiteral);
		literalsList.Add (floatLiteral);
		literalsList.Add (stringLiteral);	
		literalsList.Add (booleanLiteral);

	}
	protected void OnExecClicked (object sender, EventArgs e)
	{	
		haiFlag = false;
		kthxbyeFlag = false;
		variableList = new List<Variable>();
		lexemeList = new List<Lexeme> ();
		console.Buffer.Text = "";
		statements.Model = lexStore = new Gtk.ListStore (typeof (string), typeof (string));
		variables.Model = idenStore = new Gtk.ListStore (typeof (string), typeof (string), typeof (string));
		string[] lines = input.Buffer.Text.Split('\n');
		int linecounter = 1;
		foreach (string line in lines){
			Boolean somethingMatched = false;
			foreach(Regex r in regexList){
				Match m = r.Match (line);
				if (m.Success) {
					console.Buffer.Text += "REGEX MATCH!\n";
					somethingMatched = true;
					evaluateTokens (line, linecounter);
				}
				
			}//inner
			if(!somethingMatched){
				console.Buffer.Text += "Error on Line " + linecounter + " : Unknown Statement!\n"; 
			}
		linecounter++;		
		}//outerloop
		foreach (Lexeme lex in lexemeList){
			lexStore.AppendValues (lex.lex, lex.tag);
		}
		foreach (Variable var in variableList){
			idenStore.AppendValues (var.variableName, var.variableValue, var.variableType);
		}
	}
	protected void evaluateTokens(string line, int linecounter)
	{	
		line += " ";
		string token = ""; // current token
		string prevToken = "";
		string variableToken = "";
		Boolean stringLiteral = false;
		foreach(char c in line)
		{	
			if (c == ' ' && !stringLiteral) {
				// RESERVED WORDS
				token = token.Trim();
				if (token == "HAI") {
					if (!haiFlag) {
						lexemeList.Add (new Lexeme (token, "Program Start"));
						haiFlag = true;
						prevToken = token;
						token = "";
					} else {
						console.Buffer.Text += "Error on Line " + linecounter + " : HAI again\n";  
					}
				} 
				if(!haiFlag){
						console.Buffer.Text += "Error on Line " + linecounter + " : HAI plsss.\n";
						break;
				}
				else if (token == "I" || token == "I HAS" || token == "YA" || token == "NO" || token == "SUM" || token == "DIFF" || token == "PRODUKT" || token == "QUOSHUNT" || token == "MOD" || token == "BOTH" || token == "EITHER" || token == "BIGGR" || token == "SMALLR" || token == "WON" || token == "ALL" || token == "ANY")
					token += c;
				else if (token == "I HAS A") {
					lexemeList.Add (new Lexeme (token, "Variable Declaration"));
					prevToken = token;
					token = "";
				}
				else if (token == "R") {
					lexemeList.Add (new Lexeme (token, "Variable Assignment"));
					prevToken = token;
					token = "";
				}
				else if (token == "A" && prevToken == "ITZ") {
					lexemeList.Add (new Lexeme (token, "Variable Data Type Initializer"));
					prevToken = token;
					token = "";
				}
				else if (token == "ITZ") {
					lexemeList.Add (new Lexeme (token, "Variable Initializer"));
					prevToken = token;
					token = "";
				}
				else if (token == "BTW") {
					lexemeList.Add (new Lexeme (token, "Comment"));
					prevToken = token;
					token = "";
				}
				else if (token == "VISIBLE") {
					lexemeList.Add (new Lexeme (token, "Output"));

					prevToken = token;
					token = "";
				}
				else if (token == "GIMMEH") {
					lexemeList.Add (new Lexeme (token, "Input"));

					prevToken = token;
					token = "";
				}
				else if (token == "YA RLY") {
					lexemeList.Add (new Lexeme (token, "If"));

					prevToken = token;
					token = "";
				}
				else if (token == "NO WAI") {
					lexemeList.Add (new Lexeme (token, "Else"));

					prevToken = token;
					token = "";
				}
				else if (token == "WTF?") {
					lexemeList.Add (new Lexeme (token, "Switch"));
					prevToken = token;
					token = "";
				}
				else if (token == "OMG") {
					lexemeList.Add (new Lexeme (token, "Case"));
					prevToken = token;
					token = "";
				}
				else if (token == "OMGWTF") {
					lexemeList.Add (new Lexeme (token, "Default"));

					prevToken = token;
					token = "";
				}
				else if (token == "GTFO") {
					lexemeList.Add (new Lexeme (token, "Break"));

					prevToken = token;
					token = "";
				}
				else if (token == "SUM OF") {
					lexemeList.Add (new Lexeme (token, "Add"));

					prevToken = token;
					token = "";
				}
				else if (token == "DIFF OF") {
					lexemeList.Add (new Lexeme (token, "Subtract"));

					prevToken = token;
					token = "";
				}
				else if (token == "PRODUKT OF") {
					lexemeList.Add (new Lexeme (token, "Multiply"));

					prevToken = token;
					token = "";
				}
				else if (token == "QUOSHUNT OF") {
					lexemeList.Add (new Lexeme (token, "Divide"));

					prevToken = token;
					token = "";
				}
				else if (token == "MOD OF") {
					lexemeList.Add (new Lexeme (token, "Modulo"));

					prevToken = token;
					token = "";
				}
				else if (token == "BOTH OF") {
					lexemeList.Add (new Lexeme (token, "And"));

					prevToken = token;
					token = "";
				}
				else if (token == "BIGGR OF") {
					lexemeList.Add (new Lexeme (token, "Greater-Than"));

					prevToken = token;
					token = "";
				}
				else if (token == "SMALLR OF") {
					lexemeList.Add (new Lexeme (token, "Less-Than"));

					prevToken = token;
					token = "";
				}
				else if (token == "BOTH SAEM") {
					lexemeList.Add (new Lexeme (token, "Equality"));

					prevToken = token;
					token = "";
				}
				else if (token == "DIFFRINT") {
					lexemeList.Add (new Lexeme (token, "Unequality"));

					prevToken = token;
					token = "";
				}
				else if (token == "ALL OF") {
					lexemeList.Add (new Lexeme (token, "Compound And"));

					prevToken = token;
					token = "";
				}
				else if (token == "ANY OF") {
					lexemeList.Add (new Lexeme (token, "Compound Or"));

					prevToken = token;
					token = "";
				}
				else if (token == "EITHER OF") {
					lexemeList.Add (new Lexeme (token, "Or"));

					prevToken = token;
					token = "";
				}
				else if (token == "WON OF") {
					lexemeList.Add (new Lexeme (token, "Xor"));

					prevToken = token;
					token = "";
				}
				else if (token == "SMOOSH") {
					lexemeList.Add (new Lexeme (token, "Concatinate"));

					prevToken = token;
					token = "";
				}
				else if (token == "KTHXBYE") {
					if (!kthxbyeFlag) {
						lexemeList.Add (new Lexeme (token, "Program End"));
						kthxbyeFlag = true;
						prevToken = token;
						token = "";
					} else {
						console.Buffer.Text += "Error on Line " + linecounter + " : KTHXBYE again\n";  
					}					
				}
				//VARIABLES LITERALS

				else {
					
					Boolean MATCHED = false;
					//LITERALS
					foreach(string literal in literalsList)
					{
						if (Regex.IsMatch (token, literal)) 
						{
							string valType = checkValueType(token);
							MATCHED = true;
							if(prevToken == "ITZ" || prevToken == "R"){
								foreach(Variable v in variableList)
								{
									if(v.variableName == variableToken)
									{
										v.variableValue = token;
										v.variableType = valType;
									}	
								}
							} 
							lexemeList.Add((new Lexeme(token, valType)));
							prevToken = token;
							token = "";
						}
					}

					//VARIABLES
					if(Regex.IsMatch(token, varName) && !MATCHED)
					{
						if(prevToken == "I HAS A")
						{
							variableList.Add (new Variable (token, "NOOB", "Untyped variable"));
						}
						else if(prevToken == "ITZ" || prevToken == "R")
						{
							Variable assignee = null, assigner = null;
							foreach(Variable v in variableList)
							{
								
								if (v.variableName == token) 
								{
									assigner = v;	
								}
								if(v.variableName == variableToken)
								{
									assignee = v;
								}
								if(assignee!= null && assigner != null)
								{
									assignee.variableValue = assigner.variableValue;
									assignee.variableType = assigner.variableType;
								}	
							}	
						}
						else if(prevToken == "BTW")
						{
							Console.Write(true);
							continue;
						}
						lexemeList.Add (new Lexeme(token, "Variable Identifier"));
						prevToken = token;
						variableToken = token;
						token = "";
					}	
				}

			} else {
				if (c == '\"') {
					stringLiteral = stringLiteral ? false : true;
					token += c;
					continue;
				}
				token += c;

			}	
		} //end of loop	
	}	
	protected void checkDataType(Variable v){
	
	}	
	protected string checkValueType(string token){
		if (Regex.IsMatch (token, stringLiteral))
			return "YARN";
		else if (Regex.IsMatch (token, integerLiteral))
			return "NUMBR";
		else if (Regex.IsMatch (token, floatLiteral))
			return "NUMBAR";
		else if (Regex.IsMatch (token, booleanLiteral))
			return "TROOF";
		else
			return "Untyped";
	}
	protected void filterInit(){
		filter = new FileFilter (); 
		filter.Name = "LOL CODE Files";
		filter.AddPattern ("*.lol");
		filechooserbutton.AddFilter (filter);
	}
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	protected void OnFilechooserbuttonSelectionChanged (object sender, EventArgs e)
	{
		String filename = filechooserbutton.Filename;
		input.Buffer.Text = File.ReadAllText (filename);
	}	
}	