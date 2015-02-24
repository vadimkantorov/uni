#pragma comment(linker,"/stack:16000000")

#include <cstdio>
#include <cstdlib>
#include <cmath>
#include <algorithm>
#include <string>
using namespace std;

struct BigInteger
{
	static const int SIZE = 4000;
	static const int BASE = 100000;
	static const int BASELEN = 5;

	__int64 M[SIZE];
	int size;

	BigInteger(__int64 val = 0)
	{
		fill_n(M,SIZE,0);

		M[0] = val;
		size = 3;
		Normalize();
	}

	BigInteger(const string& s)
	{
		fill_n(M,SIZE,0);
		
		size = 0;
		for(int pos = s.length() - BASELEN; pos >= 0; pos -= BASELEN)
			M[size++] = atoi(s.substr(pos,BASELEN).c_str());
		
		if(s.length()%BASELEN != 0)
			M[size++] = atoi(s.substr(0,s.length()%BASELEN).c_str());

		Normalize();
	}

	static BigInteger PowTen(int power)
	{
		BigInteger res;
		res.M[power/BASELEN] = pow(10.0,power%BASELEN);
		res.size = power/BASELEN+1;
		return res;
	}

	void Print(bool newLine = false)
	{
		char tmp[15];
		string fmt = "%." + string(itoa(BASELEN,tmp,10)) + "I64d";
		printf("%d",M[size-1]);
		for(int i = size-2; i >= 0; i--)
			printf(fmt.c_str(),M[i]);
		
		if(newLine)
			puts("");
	}

	void Normalize()
	{
		int rem = 0;
		for(int i = 0; i < size; i++)
		{
			int tmp = (M[i] + rem)/BASE;
			M[i] = (M[i] + rem)%BASE;
			rem = tmp;
			if (M[i] < 0) 
			{
				M[i] += BASE;
				rem--;
			}
		}

		while(M[size-1] == 0)
			size--;

		if(size == 0)
			size == 1;
	}

	bool operator==(BigInteger& x)
	{
		if(size != x.size)
			return false;
		
		for(int i = size-1; i >= 0; i--)
		{
			if(M[i] != x.M[i])
				return false;
		}

		return true;
	}

	bool operator!=(BigInteger& x)
	{
		return !(*this == x);
	}
	
	BigInteger operator+(BigInteger& x)
	{
		BigInteger res(max(size,x.size));
		res.size = max(size,x.size) + 1;
		
		for(int i = 0; i < res.size; i++)
			res.M[i] = M[i] + x.M[i];

		res.Normalize();
		return res;
	}

	BigInteger operator-(BigInteger& x)
	{
		BigInteger res;
		res.size = max(size,x.size) + 1;

		for(int i = 0; i < res.size; i++)
			res.M[i] = M[i] - x.M[i];

		res.Normalize();
		return res;
	}

	BigInteger operator*(BigInteger& x)
	{
		BigInteger res;
		res.size = size + x.size + 2;

 		for(int i = 0; i < size; i++)
			for(int j = 0; j < x.size; j++)
				res.M[i+j] += M[i]*x.M[j];

		res.Normalize();
		return res;
	}

	BigInteger operator/(BigInteger& x)
	{
		BigInteger res;
		res.size = size - x.size + 1;//max(size - x.size, size - x.size + 1);//1);

		for(int i = res.size-1; i >= 0; i--)
		{
			int l = 0, r = BASE - 1;
			while(l < r)
			{
				int mid = (l+r)/2;
				res.M[i] = mid;
				
				BigInteger tmp = (x*res);
				
				if(tmp < (*this))
					l = mid+1;
				else
					r = mid;
			}
			res.M[i] = l;
			if(*this < x*res)
			{
				if(res.M[i] == 1)
					res.size--;
				res.M[i]--;
			}
		}

		res.Normalize();
		return res;
	}

	BigInteger operator/(int x)
	{
		BigInteger res = *this;
		__int64 mod = 0;
		
		for(int i = size - 1; i >= 0; i--)
		{
			int tmp = mod * BASE + res.M[i];
			res.M[i] = tmp / x;
			mod = tmp % x;
		}
		
		res.Normalize();
		return res;
	}

	bool operator<(BigInteger& x)
	{
		if(size != x.size)
			return size < x.size;
		
		for(int i = size-1; i >= 0; i--)
		{
			if(M[i] != x.M[i])
				return M[i] < x.M[i];
		}

		return false;
	}

	static BigInteger GCD(BigInteger a, BigInteger b)
	{
		BigInteger res = 1;
		while(a != b)
		{
			if(a.M[0]%2 == 0 && b.M[0]%2 == 0)
			{
				a = a/2;
				b = b/2;
				res = res*BigInteger(2);//TODO
			}
			else if(a.M[0]%2 == 0 && b.M[0]%2 == 1)
			{
				a = a/2;
			}
			else if(a.M[0]%2 == 1 && b.M[0]%2 == 0)
			{
				b = b/2;
			}
			else
			{
				if(a < b)
					b = b - a;
				else
					a = a - b;
			}
		}
		res = res * a;
		return res;
	}
};

struct Rational
{
	BigInteger num,den;
	int sign;

	Rational(BigInteger num, BigInteger den = BigInteger(1))
		: num(num), den(den)
	{
		sign = 1;
	}
	
	Rational operator-()
	{
		Rational tmp(*this);
		tmp.sign = -sign;
		return tmp;
	}

	Rational& Truncate(int acc)
	{
		num.size = den.size = acc;
		return *this;
	}

	Rational operator-(Rational& r)
	{
		if(r.sign < sign)//r < 0 && *this > 0
			return (*this + -r);
		else if(sign < r.sign)//r > 0 && *this < 0
			return -(-(*this) + r);
		else if(sign == r.sign)
		{
			if(sign > 0)
			{
				if(num*r.den < r.num*den)
					return -Rational(r.num*den - num*r.den,den*r.den);
				else
					return Rational(num*r.den - r.num*den,den*r.den);
			}
			else
				return -r + *this;
		}
	}

	Rational operator+(Rational& r)
	{
		if(r.num == BigInteger())
			return *this;
		if(num == BigInteger())
			return r;

		if(sign == r.sign)
		{
			Rational res(num*r.den + r.num*den,den*r.den);
			res.sign = sign;
			return res;
		}
		else if(sign < r.sign)
			return (r - (-*this));
		else
			return (r + *this);
	}

	Rational& operator+=(Rational& r)
	{
		Rational sum(*this + r);
		num = sum.num;
		den = sum.den;
		return *this;
	}

	Rational operator/(Rational& r)
	{
		Rational res(num*r.den,den*r.num);
		res.sign = sign*r.sign;
		return res;
	}

	Rational operator*(Rational& r)
	{
		Rational res(num*r.num,den*r.den);
		res.sign = sign*r.sign;
		
		BigInteger gcd = BigInteger::GCD(res.num, res.den);
		
		res.num = res.num/gcd;
		res.den = res.den/gcd;
		return res;
	}

	bool operator<(Rational& r)
	{
		if(sign < r.sign)
			return true;
		return num*r.den < r.num*den;
	}

	Rational Abs()
	{
		Rational res = *this;
		res.sign = 1;
		return res;
	}

	void Print(int acc)
	{
		if(sign < 0)
			putchar('-');
		BigInteger integral = num/den;
		integral.Print();
		putchar('.');
		Rational cur(integral);
		for(int i = 1; i <= acc; i++)
		{
			int l = 0, r = 9;
			BigInteger den10 = BigInteger::PowTen(i);
			while(l < r)
			{
				int mid = (l+r)/2;
				Rational t(mid,den10);
				if(cur + t < *this)
					l = mid + 1;
				else
					r = mid;
			}
			if(*this < cur + Rational(l,den10))
				l--;

			cur += Rational(l,BigInteger::PowTen(i));
			putchar('0'+(l));
		}
		puts("");
	}
};

Rational F(Rational& r)
{
	return r*r*r - Rational(2)*r - Rational(5);
}

Rational DerF(Rational& r)
{
	return Rational(3)*r*r - Rational(2);
}

void main()
{
	//BigInteger a = 1000000000000, b = 3;
	//(a-b).Print();
	Rational EPS(1,BigInteger::PowTen(42));
	Rational x(2);
	while(true)
	{
		
		Rational f = F(x);
		Rational derF = DerF(x);
		Rational tmp = x - (f/derF);
		tmp.Print(42);
		if((tmp - x).Abs() < EPS)
		{
			x = tmp;
			break;
		}
		x = tmp;
	}
	x.Print(42);
}