/*
//factorial:
setIO input.txt,output.txt
in n
let f = 1
let f = f*n
let n = n-1
jif n,2
out f

//gcd
setIO input.txt, output.txt
in a,b
let c = a%b
let a = b
let b = c
jif b,2
out a	
	
*/
	
function ReadAllLines(file)
{
	return file.ReadAll().split("\r\n");
}

function ReadOperands(line)
{
	return line.split(/[\s,=]/);
}

function GetVar(name, vars)
{
	if(vars[name] == null)
		return parseInt(name);
	
	return vars[name];
}

function EvaluateExpression(expr, vars)
{
	var re = new RegExp(/(\w+)\s*([*+-/%])\s*(\w+)/);
	re.exec(expr);
	var op = RegExp.$2, op1 = RegExp.$1, op2 = RegExp.$3;
	if(op != "")
	{
		
		switch(op)
		{
			case '*':
				return GetVar(op1, vars) * GetVar(op2, vars);
			case '+':
				return GetVar(op1, vars) + GetVar(op2, vars);
			case '-':
				return GetVar(op1, vars) - GetVar(op2, vars);
			case '/':
				return GetVar(op1, vars) / GetVar(op2, vars);
			case '%':
				return GetVar(op1, vars) % GetVar(op2, vars);
		}
	}
	else
		return GetVar(expr, vars);
}

function InterpretFile(inputFile)
{
	var vars = new Array();
	var inp = null, out = null;
	
	var lines = ReadAllLines(inputFile);
	for(var ip = 0; ip < lines.length; ip++)
	{
		var operands = ReadOperands(lines[ip]);
		var opName = operands[0];
		
		switch(opName)
		{
			case "setIO":
				inp = OpenFile(operands[1], false, false);
				if(operands.length > 2)
					out = OpenFile(operands[2], true, true);
				break;
			case "out":
				for(var i = 1; i < operands.length; i++)
				{
					if(out != null)
						out.WriteLine(vars[operands[i]]);
					else
						WScript.Echo(vars[operands[i]]);
				}
				break;
			case "in":
				for(var i = 1; i < operands.length; i++)
				{
					if(inp != null)
						vars[operands[i]] = parseInt(inp.ReadLine());
					else
						vars[operands[i]] = parseInt(WScript.StdIn.ReadLine());
				}
				break;
			case "jif":
				if(vars[operands[1]] != 0)
					ip = parseInt(operands[2]);
				break;
			case "let":
				vars[operands[1]] = EvaluateExpression(operands[2],vars);
				break;
		}
	}
}

function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create);
}

InterpretFile(OpenFile(WScript.Arguments(0), false, false));