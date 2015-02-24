#include <cstdio>
#include <utility>
#include <cmath>
using namespace std;

double a, b, h;

bool LT(double _a, double _b)
{
	return _b >= _a + h/2;
}

bool LE(double _a, double _b)
{
	return LT(_a, _b) || abs(_b-_a) <= h/2;
}

double f(double x)
{
	return sin(sqrt((1 + x*x)/3));
}

double g(double u)
{
	return f(0.9*u + 1.9)*0.9;
}

double RectangleIntegrate()
{
	double res = 0;
	for(double x = a+h; LE(x, b); x += h)
		res += f(x-h/2);
	return res*h;
}

double GregoryIntegrate()
{
	double p2 = h/2*(f(a) + f(b));
	for(double x = a+h; LT(x, b); x += h)
		p2 += h*f(x);
	
	double p24 = (-3*f(a) + 4*f(a + h) - f(a + 2*h) + f(b - 2*h) - 4*f(b - h) + 3*f(b));
	p24 *= h/24;
	return p2 + p24;



	/*double p2 = 0;
	for(double x = a+h; x < b; x += h)
		p2 += f(x);
	p2 *= 2;
	p2 += f(a) + f(b);
	p2 *= h/2;

	double p24 = 0;
	p24 += (-3*f(a) + 4*f(a + h) - f(a + 2*h) + f(b - 2*h) - 4*f(b - h) + 3*f(b));
	p24 *= h/24;
	return p2 + p24;*/
}

double GaussIntegrate()
{
	return g(1/sqrt(3.0)) + g(-1/sqrt(3.0));
}

pair<double, double> Integrate(double (*f)(), double h0)
{
	h = h0/2;
	double S2n = f();
	
	h = h0;
	double Sn = f();
	return make_pair(S2n, Sn);
}

void PrintResults(char method[], double (*f)(), double h0, int den)
{
	pair<double, double> res = Integrate(f, h0);
	pair<double, double> res2 = Integrate(f, h0*2);
	printf("%s| Sn: %f\tS2n: %f\tSn/2: %f\nRh/2: %f\tRh: %f\n", 
		method, 
		res.second, //Sn
		res.first, //S2n
		res2.second,
		abs(res.first - res.second)/den, //Rh/2
		abs(res2.first - res2.second)/den
		);
}

int main()
{
	a = 1;
	b = 2.8;
	double hs[] = {0.1, 0.05, 0.025};
	
	for(int i = 0; i < 3; i++)
	{
		double h0 = hs[i];
		printf("h = %f\n", h0);
		PrintResults("Rect", RectangleIntegrate, h0, 3);
		PrintResults("Greg", GregoryIntegrate, h0, 15);
		puts("");
	}

	printf("Gauss: %.7f\n", GaussIntegrate());
}