function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create);
}

var output = OpenFile("output.txt",true,true);

function Test(message, fun)
{	
	output.WriteLine(message);
	var begin = new Date().getTime();
	fun();
	var end = new Date().getTime();
	output.WriteLine("Время работы: " + (end-begin)/1000.0);
	output.WriteLine("");
}

function StrEqual(str1, str2, i, j, len)
{
	if(i + len - 1 >= str1.length || j + len - 1 >= str2.length)
		return false;
	for(var s = 0; s < len; s++)
		if(str1.charAt(i + s) != str2.charAt(j + s))
			return false;
	return true;
}

var hashMod = 2067954989;
function Mod(a)
{
	/*while(a < 0)
		a += hashMod;
	return a % hashMod;*/
	
	return (a%hashMod + hashMod)%hashMod;
}

function ComputePrefixFunction(str)
{
	var prf = new Array();
	prf[1] = 0;
	var k = 0;
	
	for(var i = 2; i <= str.length; i++)
	{
		while(k > 0 && str.charAt(i-1) != str.charAt(k))
			k = prf[k];
		if(str.charAt(i-1) == str.charAt(k))
			k++;
		
		prf[i] = k;
	}
	
	return prf;
}

function ComputeAlphabet(text)
{
	var alphabet = new Array(), tmp = new Array();
	for(var i = 0; i < text.length; i++)
	{
		var ch = text.charAt(i);
		if(!tmp[ch])
		{
			alphabet.push(ch);
			tmp[ch] = true;
		}
	}
	
	alphabet.sort();
	return alphabet;
}

function MatchFound(index)
{
	output.WriteLine(index);
}

String.prototype.reverse = function()
{
	var res = "";
	for(var i = this.length-1; i >= 0; i--)
		res += this.charAt(i);
	return res;
}

/////////////////////////////////////////////////

function SimpleHash(ch)
{
	return ch.charCodeAt(0);
}

function SquareHash(ch)
{
	return SimpleHash(ch)*SimpleHash(ch);
}

function HashSumSearch(text, pattern, hashFunc)
{
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
	
	var p = 0, t = 0, collisions = 0;
	for(var i = 0; i < m; i++)
	{
		p = Mod(p + hashFunc(pattern.charAt(i)));
		t = Mod(t + hashFunc(text.charAt(i)));
	}
	
	for(var s = 0; s <= n - m; s++)
	{
		if(p == t)
		{
			if(StrEqual(pattern, text, 0, s, m))
				MatchFound(s+1);
			else
				collisions++;
		}
		if(s < n - m)
			t = Mod(t - hashFunc(text.charAt(s)) + hashFunc(text.charAt(s+m)));
	}
	
	output.WriteLine("Коллизий при поиске: " + collisions);
}

/////////////////////////////////////////////////

function Rabin_Karp_Binary(text, pattern)
{	
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
		
	var h = 1 << (m-1);
	var p = 0, t = 0, collisions = 0;

	for(var i = 0; i < m; i++)
	{
		p = (p << 1) + pattern.charCodeAt(i);
		t = (t << 1) + text.charCodeAt(i);
	}
	
	for(var s = 0; s <= n - m; s++)
	{
		if(p == t)
		{
			if(StrEqual(pattern, text, 0, s, m))
				MatchFound(s+1);
			else
				collisions++;
		}
		if(s < n - m)
			t = ((t - text.charCodeAt(s)*h) << 1) + text.charCodeAt(s + m);
	}
	
	output.WriteLine("Коллизий при поиске: " + collisions);
}

function Rabin_Karp(text, pattern)
{	
	var PowMod = function(base, power, mod)
	{
		var res = 1%mod;
		
		for(var i = 0; i < power; i++)
			res = (res*base)%mod;
		
		return res;
	}
	
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
		
	var d = ComputeAlphabet(text).length;
	var h = PowMod(d, m-1, hashMod);
	
	var p = 0, t = 0, collisions = 0;

	for(var i = 0; i < m; i++)
	{
		p = Mod(p*d + pattern.charCodeAt(i));
		t = Mod(t*d + text.charCodeAt(i));
	}
	
	for(var s = 0; s <= n - m; s++)
	{
		if(p == t)
		{
			if(StrEqual(pattern, text, 0, s, m))
				MatchFound(s+1);
			else
				collisions++;
		}
		if(s < n - m)
			t = Mod(d*(t - Mod(text.charCodeAt(s)*h)) + text.charCodeAt(s + m));
	}
	
	output.WriteLine("Коллизий при поиске: " + collisions);
}

function Rabin_Karp(text, pattern)
{	
	var PowMod = function(base, power, mod)
	{
		var res = 1%mod;
		
		for(var i = 0; i < power; i++)
			res = (res*base)%mod;
		
		return res;
	}
	
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
		
	var d = ComputeAlphabet(text).length;
	var h = PowMod(d, m-1, hashMod);
	
	var p = 0, t = 0, collisions = 0;

	for(var i = 0; i < m; i++)
	{
		p = Mod(p*d + pattern.charCodeAt(i));
		t = Mod(t*d + text.charCodeAt(i));
	}
	
	for(var s = 0; s <= n - m; s++)
	{
		if(p == t)
		{
			if(StrEqual(pattern, text, 0, s, m))
				MatchFound(s+1);
			else
				collisions++;
		}
		if(s < n - m)
			t = Mod(d*(t - Mod(text.charCodeAt(s)*h)) + text.charCodeAt(s + m));
	}
	
	output.WriteLine("Коллизий при поиске: " + collisions);
}


/////////////////////////////////////////////////

function Bruteforce(text, pattern)
{
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
	
	for(var i = 0; i < text.length; i++)
		if(StrEqual(pattern, text, 0, i, m))
			MatchFound(i+1);
}

/////////////////////////////////////////////////

function DFA(text, pattern)
{
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
	
	var prf = ComputePrefixFunction(pattern);
	var alphabet = ComputeAlphabet(text);
	
	var automaton = new Array();
	for(var i = 0; i <= m; i++)
	{
		automaton[i] = new Array();
		for(var j = 0; j < alphabet.length; j++)
			automaton[i][alphabet[j]] = 0;
	}
		
	automaton[0][pattern.charAt(0)] = 1;
	
	for(var i = 1; i <= m; i++)
	{
		for(var j = 0; j < alphabet.length; j++)
		{
			if(i < m && pattern.charAt(i) ==  alphabet[j])
				automaton[i][alphabet[j]] = i+1;
			else
				automaton[i][alphabet[j]] = automaton[prf[i]][alphabet[j]];
		}
	}
	
	var state = 0;
	for(var i = 0; i < n; i++)
	{
		if(state == m)
			MatchFound(i-m+1);
		state = automaton[state][text.charAt(i)];
	}
	if(state == m)
		MatchFound(i-m+1);
}

/////////////////////////////////////////////////

function Boyer_Moore(text, pattern)
{
	var ComputeBadCharacterFunction = function(pattern, alphabet)
	{
		var res = new Array();
		for(var i = 0; i < alphabet.length; i++)
			res[alphabet[i]] = 0;
		
		for(var i = 0; i < pattern.length; i++)
			res[pattern.charAt(i)] = i+1;
		
		return res;
	}

	var ComputeGoodSuffixFunction = function(pattern)
	{
		var m = pattern.length;
		
		var prf = ComputePrefixFunction(pattern);
		var nrettap = pattern.reverse();
		var frp = ComputePrefixFunction(nrettap);
		
		var res = new Array();
		
		for(var i = 0; i <= m; i++)
			res[i] = m - prf[m];
			
		for(var l = 1; l <= m; l++)
		{
			var j = m - frp[l];
			if(res[j] > l - frp[l])
				res[j] = l - frp[l];
		}
		
		return res;
	}
	
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
	
	var bc = ComputeBadCharacterFunction(pattern, ComputeAlphabet(text));
	var gs = ComputeGoodSuffixFunction(pattern);
	
	var s = 0;
	while(s <= n - m)
	{
		var j = m;
		while(j > 0 && pattern.charAt(j-1) == text.charAt(s + j-1))
			j--;
		if(j == 0)
		{
			MatchFound(s+1);
			s += gs[0];
		}
		else
		{
			s += Math.max(gs[j],j-bc[text.charAt(s + j-1)]);
		}
	}
}

/////////////////////////////////////////////////

function Boyer_Moore_Horspool(text, pattern)
{
	var ComputeBadCharacterFunctionEx = function(pattern, alphabet)
	{
		var m = pattern.length;
		
		var res = new Array();
		for(var i = 0; i < alphabet.length; i++)
			res[alphabet[i]] = m;
		
		for(var i = 0; i < m-1; i++)
			res[pattern.charAt(i)] = m - (i+1);
		
		return res;
	}
	
	var m = pattern.length;
	var n = text.length;
	
	if(n < m)
		return;
		
	var bc = ComputeBadCharacterFunctionEx(pattern, ComputeAlphabet(text));
		
	var s = 0;
	while(s <= n - m)
	{
		var j = m;
		while(j > 0 && pattern.charAt(j-1) == text.charAt(s + j-1))
			j--;
		if(j == 0)
			MatchFound(s+1);
		s += bc[text.charAt(s+m-1)];
	}
}

function Main()
{
	var input = OpenFile("input.txt",false,false);
	
	var pattern = input.ReadLine();
	var text = input.ReadAll();
	
	/*Test("Грубая сила:",function() {Bruteforce(text,pattern)});
	Test("Автомат:", function() {DFA(text, pattern)});
	Test("Бойер-Мур-Хорспул", function() {Boyer_Moore_Horspool(text, pattern)});
	Test("Бойер-Мур", function() {Boyer_Moore(text, pattern)});
	output.WriteLine("\r\nХэши:");
	Test("по сумме:", function() {HashSumSearch(text, pattern, SimpleHash)});
	Test("по сумме квадратов:", function() {HashSumSearch(text, pattern, SquareHash)});*/
	Test("по Рабину-Карпу (база - размер алфавита):", function() {Rabin_Karp(text, pattern)});
	Test("по Рабину-Карпу (база - 2):", function() {Rabin_Karp_Binary(text, pattern)});
}

Main();
