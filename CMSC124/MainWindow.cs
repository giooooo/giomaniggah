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
		regexList.Add (new Regex (@"^(HAI)(\sBTW\s+)?"));
		regexList.Add (new Regex (@"^(I)\s+(HAS)\s+(A)\s+"+ varName + @"(\sITZ\s.+)?" + @"(\sBTW\s+.*)?"));
		regexList.Add (new Regex (@"^(VISIBLE)\s+(\sBTW\s+)?"));
		regexList.Add (new Regex (@"^(GIMMEH)\s+(\sBTW\s+)?"));
		regexList.Add (new Regex (@"^\s*" + varName + @"\s+(R)\s+.+"));
		regexList.Add (new Regex (@"^(KTHXBYE)(\sBTW\s+)?"));

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
				if (token == "HAI") {
					if (!haiFlag) {
						lexemeList.Add (new Lexeme (token, "Program Start"));
						haiFlag = true;
						prevToken = token;
						token = "";
					} else {
						console.Buffer.Text += "Error on Line " + linecounter + " : HAI again\n";  
					}X
				} else if (token == "I" || token == "I HAS")
					token += c;
				else if (token == "I HAS A") {
					lexemeList.Add (new Lexeme (token, "Variable Declaration"));
					prevToken = token;
					token = "";
				}
				else if (token == "VISIBLE") {
					lexemeList.Add (new Lexeme (token, "Output"));
					prevToken = token;
					token = "";
				}
				else if (token == "R") {
					lexemeList.Add (new Lexeme (token, "Variable Assignment"));
					prevToken = token;
					token = "";
				}
				else if (token == "ITZ") {
					lexemeList.Add (new Lexeme (token, "Variable Initializer"));
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
							else if(prevToken == "VISIBLE")
							{
								if (valType == "YARN") {
									string[] temp = token.Split ('\"');
									console.Buffer.Text += temp [1] + "\n";
								} else {
									console.Buffer.Text += token + "\n";
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
						else if(prevToken == "VISIBLE")
						{
							Boolean isDeclared = false;
							foreach(Variable v in variableList)
							{
								if (v.variableName == token) 
								{
									if (v.variableType == "YARN") {
										string[] temp = v.variableValue.Split ('\"');
										console.Buffer.Text += temp [1] + "\n";										
									} 
									else if (v.variableType == "Untyped variable") {
										console.Buffer.Text += "\n";										
									} 
									else {
										console.Buffer.Text += v.variableValue + "\n";
									}
									isDeclared = true;
									break;
								}	
							}
							if(!isDeclared)
								console.Buffer.Text += "Error on Line " + linecounter + " : " + token + " not declared.\n"; 
						}
						else if(prevToken == "GIMMEH")
						{
							Boolean isDeclared = false;
							foreach(Variable v in variableList)
							{
								if (v.variableName == token) 
								{
									gimmeh box = new gimmeh ();
									box.Run ();
									v.variableValue = "\"" + box.input + "\"";
									v.variableType = "YARN";
									isDeclared = true;
									break;
								}	
							}
							if(!isDeclared)
								console.Buffer.Text += "Error on Line " + linecounter + " : " + token + " not declared.\n"; 
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
			return "Untyped variable";
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