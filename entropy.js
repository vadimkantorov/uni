function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create);
}

function log(base, val)
{
	return Math.log(val)/Math.log(base);
}

function Main()
{
	var text = "aaaaaa";//OpenFile("text.txt",false,false).ReadAll();
	var c = new Array(), n = 0;
	for(var i = 0; i < text.length; i++)
	{
		var ch = text.charAt(i);
		if(c[ch] != null)
			c[ch]++;
		else
		{
			c[ch] = 1;
			n++;
		}
	}
	
	var h = 0;
	if(n != 1)
	{
		for(var ch in c)
		{
			var p = c[ch]/text.length;
			h += p*log(n, p);
		}
	}
	else
		h = 1;
		
	WScript.Echo(-h);
}

Main();