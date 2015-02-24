function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create);
}

Array.prototype.Last = function()
{
	return this[this.length-1];
}

Array.prototype.Empty = function()
{
	return this.length == 0;
}

function IsDigit(c)
{
	return '0' <= c && c <= '9';
}

function IsOperator(c)
{
	return c == '+' || c == '*' || c == '/' || c == '-';
}

function LessEqual(u,v)
{
	var prs = {
		'(' : 0,
		')' : 1,
		'-' : 2,
		'+' : 2,
		'*' : 3,
		'/' : 3
		};
		
	return prs[u] <= prs[v];
}

function RPN(s)
{
	var res = "", stack = new Array();
	for(var i = 0; i < s.length; i++)
	{
		var c = s.charAt(i);
		
		if(IsDigit(c))
			res += c;
		else if(IsOperator(c))
		{
			if(!stack.Empty())
			{
				while(LessEqual(c, stack.Last()))
					res += stack.pop();
			}
			stack.push(c);
		}
		else if(c == '(')
			stack.push(c);
		else if(c == ')')
		{
			while(stack.Last() != '(')
				res += stack.pop();
			stack.pop();
		}
	}
	while(!stack.Empty())
		res += stack.pop();
	return res;
}

function Tree(label, left, right)
{
	this.label = label;
	this.left = left;
	this.right = right;
}

Tree.prototype.SerializeToFS = function(folder)
{
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var separator = "\\";
	var folderPath = folder.Path + separator + this.label;
	if(fso.FolderExists(folderPath))
	    folderPath += "_";
	
	var f = fso.CreateFolder(folderPath);
	
	if(this.left != null)
		this.left.SerializeToFS(f);
	
	if(this.right != null)
		this.right.SerializeToFS(f);
}

function GenerateTree(rpn)
{
	var dic = {
		'-' : "Вычитание",
		'+' : "Сложение",
		'*' : "Умножение",
		'/' : "Деление"
		};
		
	var stack = new Array();
	for(var i = 0; i < rpn.length; i++)
	{
		var c = rpn.charAt(i);
		if(IsDigit(c))
			stack.push(new Tree(c));
		else
		{
			var right = stack.pop();
			var left = stack.pop();
			stack.push(new Tree(dic[c],left,right));
		}
	}
	return stack.pop();
}

function GetVisualizingFolder()
{
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var visFolderName = "Vis";
	
	if(fso.FolderExists(visFolderName))
		fso.DeleteFolder(visFolderName);
	
	return fso.CreateFolder(visFolderName);
}

function Main()
{
	var s = OpenFile("input.txt",false,false).ReadLine();
	var rpn = RPN(s);
	OpenFile("output.txt",true,true).WriteLine(rpn);
	GenerateTree(rpn).SerializeToFS(GetVisualizingFolder());
}

Main();