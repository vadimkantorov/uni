with(Heap = new Function())
{	
	prototype.heap = new Array();
	prototype.heap[0] = 0;
	prototype.heap.swap = function(a,b)
	{
		var tmp = this[b];
		this[b] = this[a]
		this[a] = tmp;
	}
		
	prototype.Less = function(a,b)
	{
		if(a && b)
			return a.Less(b);
		return false;
	}
	
	prototype.Left = function(i)
	{
		return 2*i;
	}
	
	prototype.Right = function(i)
	{
		return 2*i+1;
	}
	
	prototype.Parent = function(i)
	{
		return i/2;
	}
	
	prototype.ExtractMin = function()
	{
		var res = this.heap[1];
		this.heap[1] = this.heap[this.heap.length-1];
		this.heap.pop();
		this.Heapify(1);
		return res;
	}
	
	prototype.Insert = function(value)
	{
		i = this.heap.length;
		this.heap.push(value);
		while(i > 1 && this.Less(this.heap[i], this.heap[this.Parent(i)]))
		{
			this.heap.swap(i, this.Parent(i));
			i = this.Parent(i);
		}
	}
	
	prototype.Heapify = function(i)
	{
		var l = this.Left(i);
		var r = this.Right(i);
		var smallest = i;
		
		if(this.Less(this.heap[l], this.heap[i]))
			smallest = l;
		if(this.Less(this.heap[r], this.heap[smallest]))
			smallest = r;
		
		if(smallest != i)
		{
			this.heap.swap(i, smallest);
			this.Heapify(smallest);
		}
	}
	
	prototype.Empty = function()
	{
		return this.heap.length == 1;
	}
}

with(Tree = function(left, right, weight, label)
{
	this.Left = left;
	this.Right = right;
	this.Weight = weight;
	this.Label = label;
	}){
	
	prototype.Less = function(b)
	{
		if(this.Weight < b.Weight)
			return true;
		return false;
	}
	
	prototype.IsLeaf = function()
	{
		return this.Left == null && this.Right == null;
	}
}

function ComputeEncodingTable(huffmanTree, table, code)
{
	if(table == null && code == null)
	{
		table = new Array();
		code = "";
	}
	
	if(huffmanTree.IsLeaf())
		table[huffmanTree.Label] = code;
	else
	{
		if(huffmanTree.Left != null)
			ComputeEncodingTable(huffmanTree.Left, table, code + '0');
		if(huffmanTree.Right != null)
			ComputeEncodingTable(huffmanTree.Right, table, code + '1');
	}
	
	return table;
}

function OpenFile(filePath, create, writing)
{
	var ForWriting = 2, ForReading = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	return fso.OpenTextFile(filePath, writing?ForWriting:ForReading, create);
}

function BuildHuffmanTree(freq)
{
	var heap = new Heap(Tree.Less);
	for(var key in freq)
	{
		var ver = new Tree(null, null, freq[key], key);
		heap.Insert(ver);
	}
	
	while(true)
	{
		var l = heap.ExtractMin();
		var r = heap.ExtractMin();
		var tree = new Tree(l, r, l.Weight + r.Weight);
		if(heap.Empty())
			return tree;
		heap.Insert(tree);
	}
}

function ReadFrequencyTable(file)
{
	var freq = new Array();
	var re = /(.) (\d+)/;
	
	while(!file.AtEndOfStream)
	{
		var line = file.ReadLine();
		re.exec(line);
		freq[RegExp.$1] = parseInt(RegExp.$2);
	}
	return freq;
}

function WriteFrequencyTable(file, freq)
{
	for(var key in freq)
		file.WriteLine(key + ' ' + freq[key]);
}

function SerializeBinaryString(file, binaryStr)
{
	for(var i = 0; i < binaryStr.length; i++)
		file.Write(binaryStr.charAt(i));
}

function DeserializeBinaryString(file)
{
	return file.ReadAll();
}

function Encode(inputFile, outputFile, freqFile)
{
	var freq = new Array();
	var cont = inputFile.ReadAll();
	
	for(var i = 0; i < cont.length; i++)
	{
		var c = cont.charAt(i);
		if(freq[c] == null)
			freq[c] = 1;
		else
			freq[c]++;
	}
	
	WriteFrequencyTable(freqFile, freq);
	var tree = BuildHuffmanTree(freq);
	var table = ComputeEncodingTable(tree);
	
	var res = "";
	for(var i = 0; i < cont.length; i++)
		res += table[cont.charAt(i)];
	
	SerializeBinaryString(outputFile, res);
}

function Decode(inputFile, outputFile, freqFile)
{
	var freq = ReadFrequencyTable(freqFile);
	var tree = BuildHuffmanTree(freq);
	var str = DeserializeBinaryString(inputFile) + '\0';
	
	var cur = tree;
	for(var i = 0; i < str.length; i++)
	{
		if(cur.IsLeaf())
		{
			outputFile.Write(cur.Label);
			cur = tree;
		}
		var c = str.charAt(i);
		if(c == '0')
			cur = cur.Left;
		else if(c == '1')
			cur = cur.Right;
	}
}

function Main()
{
	if(WScript.Arguments(0) == "Encode")
		Encode(
			OpenFile("text.txt",false,false),
			OpenFile("encoded.txt",true,true),
			OpenFile("freq.txt",true,true)
			);
	else
		Decode(
			OpenFile("encoded.txt",false,false),
			OpenFile("decoded.txt",true,true),
			OpenFile("freq.txt",false,false)
			);
}

Main();