function BigInteger(value)
{
	if(value == null)
		value = "0";
	
	this.DigitCount = 100;
	
	this.d = new Array(this.DigitCount);
	for(var i = 0; i < this.DigitCount; i++)
		this.d[i] = 0;
		
	this.sign = value.charAt(0) == '-' ? -1 : 1;
	
	for(this.size = 0; this.size < value.length; )
	{
		var cur = value.charAt(value.length-this.size-1);
		if(cur != '-')
			this.d[this.size++] = cur-'0';
		else
			break;
	}
	
	this.ToString = function()
	{
		var res = this.sign > 0 ? "" : "-";
		for(var i = this.size-1; i >= 0; i--)
			res += this.d[i];
		return res;
	}
	
	this.Add = function(x)
	{
		if(this.sign*x.sign > 0)				//a + b || -a + -b
		{
			var res = new BigInteger();
			res.sign = this.sign;
			res.size = Math.max(this.size, x.size)+2;
			for(var i = 0; i < res.size; i++)
				res.d[i] = this.d[i] + x.d[i];
			res.Normalize();
			return res;
		}
		else if(this.sign > 0 && x.sign < 0)	//a + -b
			return this.Subtract(x.Negate());
		else									//-a + b
			return x.Add(this);
	}
	
	this.Subtract = function(x)
	{
		if(this.sign > 0 && x.sign > 0)			//a - b
		{
			if(x.LessOrEqual(this))
			{
				var res = new BigInteger();
				res.size = Math.max(this.size, x.size)+2;
				for(var i = 0; i < res.size; i++)
					res.d[i] = this.d[i] - x.d[i];
				res.Normalize();
				return res;
			}
			else
				return x.Subtract(this).Negate();
		}
		else if(this.sign > 0 && x.sign < 0)	//a - -b
			return this.Add(x.Negate());
		else if(this.sign < 0 && x.sign > 0)
			return this.Negate().Add(x).Negate();
		else if(this.sign < 0 && x.sign < 0)
			return x.Negate().Add(this);
	}
	
	this.Multiply = function(x)
	{
		var res = new BigInteger();
		res.sign = this.sign * x.sign;
		res.size = this.size + x.size + 2;
		
		for(var i = 0; i < this.size; i++)
			for(var j = 0; j < x.size; j++)
				res.d[i+j] += this.d[i]*x.d[j];
		
		res.Normalize();
		return res;
	}
	
	this.Equal = function(x)
	{
		if(this.sign != x.sign || this.size != x.size)
			return false;
			
		for(var i = 0; i < this.size; i++)
			if(this.d[i] != x.d[i])
				return false; 
		
		return true;
	}
	
	this.Less = function(x)
	{
		if(this.sign != x.sign)
			return this.sign < x.sign;
		
		if(this.size != x.size)
			return this.size < x.size;
			
		for(var i = this.size-1; i >= 0; i--)
			if(this.d[i] != x.d[i])
				return this.sign*this.d[i] < x.sign*x.d[i];
		
		return false;
	}
	
	this.Greater = function(x)
	{
		return x.Less(this);
	}
	
	this.LessOrEqual = function(x)
	{
		if(this.Less(x) || this.Equal(x))
			return true;
	}
	
	this.GreaterOrEqual = function(x)
	{
		if(this.Greater(x) || this.Equal(x))
			return true;
	}
	
	this.LeftShift = function(x)
	{
		for(var i = BigInteger.$0.Clone(); i.Less(x); i.Inc())
		{
			this.d.unshift(0);
			this.size++;
		}
	}
	
	this.Abs = function()
	{
		var res = this.Clone();
		res.sign = 1;
		return res;
	}
	
	this.Inc = function()
	{
		this.d[0]++;
		this.Normalize();
	}
	
	this.Normalize = function()
	{
		var rem = 0;
		for(var i = 0; i < this.size; i++)
		{
			var tmp = ((this.d[i] + rem)/10 >= 0) ? Math.floor((this.d[i] + rem)/10) : Math.ceil((this.d[i] + rem)/10);
			this.d[i] = (this.d[i] + rem)%10;
			rem = tmp;
			if (this.d[i] < 0)
			{
				this.d[i] += 10;
				rem--;
			}
		}

		while(this.d[this.size-1] == 0)
			this.size--;

		if(this.size == 0)
			this.size = 1;
	}
	
	this.Negate = function()
	{
		var res = this.Clone();
		res.sign = -res.sign;
		return res;
	}
}

BigInteger.$0 = new BigInteger("0");
BigInteger.$1 = new BigInteger("1");
BigInteger.prototype.Clone = function()
{
	var res = new BigInteger();
	res.size = this.size;
	res.sign = this.sign;
	for(var i = 0; i < res.size; i++)
		res.d[i] = this.d[i];
	return res;
}

function Float(value)
{
	this.Clone = function()
	{
	    var res = new Float();
	    res.exp = this.exp.Clone();
	    res.mantissa = this.mantissa.Clone();
	    return res;
	}
	
	this.Multiply = function(d)
	{
		var res = new Float();
		res.mantissa = this.mantissa.Multiply(d.mantissa);
		res.exp = this.exp.Add(d.exp);
		return res;
	}
	
	this.Add = function(d)
	{
		var res = new Float();
		var expmin, expmax;
		if(this.exp.Less(d.exp))
		{
			expmin = this.Clone();
			expmax = d.Clone();
		}
		else
		{
			expmax = this.Clone();
			expmin = d.Clone();
		}
		
		var dexp = expmin.exp.Subtract(expmax.exp);
		expmax.exp = expmax.exp.Add(dexp);
		expmax.mantissa.LeftShift(dexp.Abs());
		
		res.exp = expmax.exp;
		res.mantissa = expmin.mantissa.Add(expmax.mantissa);
		
		return res;
	}
	
	this.ToString = function()
	{
		var res = this.mantissa.Less(BigInteger.$0) ? "-" : "";
		var ms = this.mantissa.Abs().ToString();
		if(this.exp.GreaterOrEqual(BigInteger.$0))
		{
			res += ms;
			if(this.exp.GreaterOrEqual(BigInteger.$1))
			{
				for(var i = BigInteger.$0.Clone(); i.Less(this.exp); i.Inc())
					res += '0';
			}
			res += ".0";
		}
		else
		{
			var tmp = this.exp.Add(new BigInteger(ms.length.toString()));
			if(tmp.Greater(BigInteger.$0) > 0)
			{
				for(var i = BigInteger.$0.Clone(); i.Less(tmp); i.Inc())
					res += ms.charAt(parseInt(i.ToString()));
				res += ".";
				
				var msl = new BigInteger(ms.length.toString());
				for(var i = tmp; i.Less(msl); i.Inc())
					res += ms.charAt(parseInt(i.ToString()));
			}
			else
			{
				tmp = tmp.Abs();
				res += "0.";
				for(var i = BigInteger.$0.Clone(); i.Less(tmp); i.Inc())
					res += '0';
				res += ms;
			}
		}
		return res;
	}
	
	this.ToExponentString = function()
	{
	    var res = this.mantissa.ToString() + 'e';
	    if(this.exp.GreaterOrEqual(BigInteger.$0))
	        res += '+'
	    return res + this.exp.ToString();
	}
	
	if(value != null)
	{
		var re = new RegExp(/(-?\d+)\.?(\d+)?/);
		var re2 = new RegExp(/(0*$)/);
		var parsed = re.exec(value);
		var parsed2 = re2.exec(parsed[1]);
		this.exp = new BigInteger((-parsed[2].length).toString());
		if(parsed[2] == null || parsed[2] == "")
		{
			this.exp = this.exp.Add(new BigInteger(parsed2[1].length.toString()));
			this.mantissa = new BigInteger(parsed[1].substr(0,parsed[1].length-parsed2[1].length));
		}
		else
		{
			if(parsed[1] == "0")
				parsed[1] = "";
			this.mantissa = new BigInteger(parsed[1] + parsed[2]);
		}
		this.mantissa.Normalize();
		this.exp.Normalize();
	}
}

function Main()
{
	var X = "3";//WScript.StdIn.ReadLine();
	var Y = "75";//WScript.StdIn.ReadLine();
	var a = new Float(X);
	var b = new Float(Y);
	
	WScript.Echo(a.ToExponentString());
	WScript.Echo(b.ToExponentString());
	
	WScript.Echo("a + b = " + a.Add(b).ToString());
	WScript.Echo("a * b = " + a.Multiply(b).ToString());
}

Main();