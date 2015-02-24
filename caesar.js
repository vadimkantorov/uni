String.prototype.IsAlphaNumeric = function()
{
	for(var i = 0; i < this.length; i++)
	{
		var c = this.charAt(i);
		if(!('a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' || '0' <= c && c <= '9'))
			return false;
	}
	return true;
}

function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create);
}

function ComputeFrequencyTable(text)
{
	var freq = new Array(), keys = new Array(), realLen = 0;

	for(var i = 0; i < text.length; i++)
	{
		var c = text.charAt(i).toLowerCase();
		if(c.IsAlphaNumeric())
		{
			if(freq[c] == null)
			{
				freq[c] = 0;
				keys.push(c);
			}
			
			freq[c]++;
			realLen++;
		}
	}
	
	for(var key in freq)
		freq[key] /= realLen;
	
	keys.sort();
	return {Keys : keys, Table: freq};
}

function SaveFrequencyTable(freq)
{
	var output = OpenFile("freq.txt",true,true);
	for(var i = 0; i < freq.Keys.length; i++)
	{
		var key = freq.Keys[i];
		output.WriteLine(key + ' ' + freq.Table[key]);
	}
}
 
function LoadFrequencyTable(input)
{
	var input = OpenFile("freq.txt",false,false);
	var keys = new Array(), freq = new Array();
	var re = /(.) (.+)/;
	
	while(!input.AtEndOfStream)
	{
		re.exec(input.ReadLine());
		var key = RegExp.$1;
		keys.push(key);
		freq[key] = parseFloat(RegExp.$2);
	}
	
	return {Keys : keys, Table: freq};
}

function Alphabet(freq)
{
	this.charByCode = new Array();
	this.codeByChar = new Array();
	
	for(var i = 0; i < freq.Keys.length; i++)
	{
		var key = freq.Keys[i];
		this.charByCode.push(key);
		this.codeByChar[key] = this.charByCode.length-1;
	}
	
	this.Length = function()
	{
		return this.charByCode.length;
	}
	
	this.GetCharByCode = function(code)
	{
		return this.charByCode[code];
	}
	
	this.GetCodeByChar = function(c)
	{
		return this.codeByChar[c];
	}
}

function Shift(text, shift, alphabet)
{
	var res = "";
	for(var i = 0; i < text.length; i++)
	{
		var c = text.charAt(i);
		
		if(c.IsAlphaNumeric())
		{
			var code = alphabet.GetCodeByChar(c);
			res += alphabet.GetCharByCode((code + shift)%alphabet.Length());
		}
		else
			res += c;
	}
	return res;
}

function Decode(text, freq)
{
	var alphabet = new Alphabet(freq);
	var bestDecoding = "", bestEst = Number.MAX_VALUE;
	
	for(var shift = 0; shift < alphabet.Length(); shift++)
	{
		var shifted = Shift(text,shift,alphabet);
		var shiftedFreq = ComputeFrequencyTable(shifted);
		
		var est = 0;
		for(var key in shiftedFreq.Table)
			est += Math.abs((freq.Table[key] || 0) - shiftedFreq.Table[key]);
		
		if(est < bestEst)
		{
			bestEst = est;
			bestDecoding = shifted;
		}
	}
	
	return bestDecoding;
}

function Main()
{
	var input = OpenFile("input.txt",false,false);
	var output = OpenFile("output.txt",true,true);
	
	switch(input.ReadLine())
	{
		case "e":
			var shift = parseInt(input.ReadLine());
			var text = input.ReadAll();
			output.WriteLine(Shift(text,shift,new Alphabet(LoadFrequencyTable(input))));
			break;
		case "d":
			output.WriteLine(Decode(input.ReadAll(), LoadFrequencyTable()));
			break;
		case "g":
			var freq = ComputeFrequencyTable(input.ReadAll());
			SaveFrequencyTable(freq);
			break;
	}
}

Main();