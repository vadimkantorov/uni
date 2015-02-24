var ops = new Array();
ops["EncodeEscape"] = function(data)
{
	var res = "";
	var prev = '\0', count = 0;
	data += '\0';
	for(var i = 0; i < data.length; i++)
	{
		if(data.charAt(i) == prev)
			count++;
		else
		{
			if(count >= 4)
				res += '*' + String.fromCharCode(count) + prev;
			else
			{
				if(prev != '*')
				{
					for(var j = 0; j < count; j++)
						res += prev;
				}
				else
					res += '*' + String.fromCharCode(count) + '*';
			}
			count = 1;
		}
		prev = data.charAt(i);
	}
	return res;
}

ops["DecodeEscape"] = function(encoded)
{
	var res = "";
	for(var i = 0; i < encoded.length; i++)
	{
		if(encoded.charAt(i) == '*')
		{
			for(var j = 0; j < encoded.charCodeAt(i+1); j++)
				res += encoded.charAt(i+2);
			i += 2;
		}
		else
			res += encoded.charAt(i);
	}
	return res;
}

ops["EncodeJump"] = function(data)
{
	var lastCommit = -1, res = "";
	for(var i = 0; i < data.length; i++)
	{
		var commit = false;
		if(i+1 < data.length)
		{
			if(data.charAt(i+1) != data.charAt(i))
				commit = true;
		}
		else
			commit = true;
		
		if(commit)
		{
			var count = i - lastCommit;
			if(count > 3)
			{
				res += String.fromCharCode((count << 1) + 1);
				res += data.charAt(i);
			}
			else
			{
				res += String.fromCharCode(count << 1);
				res += data.substring(lastCommit+1, i+1);
			}
			lastCommit = i;
		}
	}
	return res;
}

ops["DecodeJump"] = function(encoded)
{
	var res = "";
	for(var i = 0; i < encoded.length; i++)
	{
		var code = encoded.charCodeAt(i);
		if(code & 1)
		{
			code >>= 1;
			for(var j = 0; j < code; j++)
				res += encoded.charAt(i+1);
			i++;
		}
		else
		{
			code >>= 1;
			for(var j = 1; j <= code; j++)
				res += encoded.charAt(i+j);
			i += code;
		}
	}
	return res;
}

function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create,-1);
}

function Main()
{
	var op = WScript.Arguments(0);
	var input = WScript.Arguments(1);
	var output = WScript.Arguments(2);
	
	OpenFile(output,true,true).
		Write(
			ops[op](OpenFile(input,false,false).ReadAll()
		)
	);
}

Main();