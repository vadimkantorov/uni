#define _USE_MATH_DEFINES
#include <cstdio>
#include <cmath>
#include <functional>
#include <algorithm>
#include <vector>

using namespace std;

const double EPS = 1e-2;
const double alpha = 2 + 0.1*7;
const double a = 0, b = 1;
double h;
int n;
const double e = M_E;

double q(double x)
{
	return alpha*x*(1-x)+2 + 2*alpha;
}

double f(double xn, double yn, double dyn)
{
	return yn + q(xn);
}

bool LE(double x, double y)
{
	return x < y || abs(x - y) <= EPS;
}

double SQR(double x)
{
	return x*x;
}

pair<vector<double>, vector<double>> Analytical()
{
	vector<double> xs, ys;
	for(double x = a; LE(x, b) ; x += h)
	{
		double y = alpha*(x - 1)*x + exp(-x) + exp(x) -2;
		xs.push_back(x);
		ys.push_back(y);
	}
	return make_pair(xs, ys);
}

pair<vector<double>, vector<double>> SolveCauchyWithRK4(double mu) // first - y, second - y'
{
	double y0 = alpha + mu;
	double dy0 = mu;
	
	vector<double> ys, dys;
	dys.push_back(dy0);
	ys.push_back(y0);

	for(double x = a + h; LE(x, b); x += h)
	{
		double xn = x - h;
		double yn = ys.back();
		double dyn = dys.back();
		double k1 = h*f(xn, yn, dyn);
		double k2 = h*f(xn + h/2, yn + h*dyn/2 + h*k1/8, dyn + k1/2);
		double k3 = h*f(xn + h/2, yn + h*dyn/2 + h*k1/8, dyn + k2/2);
		double k4 = h*f(xn + h, yn + h*dyn + h*k3/2, dyn + k3);
		double nextY = yn + h*(dyn + (k1 + k2 + k3)/6);
		double nextDY = dyn + (k1 + 2*k2 + 2*k3 + k4)/6;

		ys.push_back(nextY);
		dys.push_back(nextDY);
	}
	return make_pair(ys, dys);
}

double Deviation(double mu)
{
	auto cauchy = SolveCauchyWithRK4(mu);
	return cauchy.first.back() + cauchy.second.back() -2*e - alpha + 2;
}

double SecantMethod(double x0, double x1, double (*f)(double))
{
	while(abs(x1 - x0) > EPS)
	{
		double tmp = x1;
		double f0 = f(x0);
		double f1 = f(x1);
		x1 = x1 - (x1-x0)*f1/(f1-f0);
		x0 = tmp;
	}
	return x1;
}

vector<double> TridiagonalAlgorithm()
{
	int xxx = n-1;
	int n = xxx;
	double x0 = a, xn = b;

	vector<double> a(n+1), b(n+1), c(n+1), d(n+1);
	c[0] = 0;
	a[0] = -2 - 2*h;
	b[0] = 2 - SQR(h);
	d[0] = -2*h*alpha + q(x0 + h)*SQR(h);

	c[n] = SQR(h) - 2;
	a[n] = 2 + 2*h;
	b[n] = 0;
	d[n] = 2*h*(2*e + alpha -2) - SQR(h)*q(xn-h);

	for(int i = 1; i < n; i++)
	{
		c[i] = 1;
		a[i] = -2 - SQR(h);
		b[i] = 1;
		d[i] = q(x0 + i*h)*SQR(h);
	}

	vector<double> m(n+1), k(n+1);
	m[0] = -b[0] / a[0];
	k[0] = d[0] / a[0];
	for(int i = 1; i <= n; i++)
	{
		double den = c[i]*m[i-1] + a[i];
		m[i] = -b[i]/den;
		k[i] = (d[i] - c[i]*k[i-1])/den;
	}
	

	vector<double> y(n+1);
	y[n] = k[n];
	for(int i = n-1; i >= 0; i--)
		y[i] = m[i]*y[i+1] + k[i];
	return y;
}

vector<double> TridiagonalAlgorithmTest()
{
	int n = 3;
	vector<double> a(n+1), b(n+1), c(n+1), d(n+1);
	c[0] = 0;
	a[0] = 8;
	b[0] = -2;
	d[0] = 6;

	c[1] = -1;
	a[1] = 6;
	b[1] = -2;
	d[1] = 3;

	c[2] = 2;
	a[2] = 10;
	b[2] = -4;
	d[2] = 8;

	c[n] = -1;
	a[n] = 6;
	b[n] = 0;
	d[n] = 5;

	
	vector<double> m(n+1), k(n+1);
	m[0] = -b[0] / a[0];
	k[0] = d[0] / a[0];
	for(int i = 1; i <= n; i++)
	{
		double den = c[i]*m[i-1] + a[i];
		m[i] = -b[i]/den;
		k[i] = (d[i] - c[i]*k[i-1])/den;
	}
	

	vector<double> y(n+1);
	y[n] = k[n];
	for(int i = n-1; i >= 0; i--)
		y[i] = m[i]*y[i+1] + k[i];
	return y;
}

double MaxDev(vector<double>& ideal, vector<double>& ys)
{
	vector<double> devs;
	for(int i = 0; i < ideal.size(); i++)
		devs.push_back(abs(ideal[i] - ys[i]));
	return *max_element(devs.begin(), devs.end());
}

void SetN(int _n)
{
	n = _n;
	h = 1.0/(n-1);
}

void Devs()
{
	freopen("devs.txt","w",stdout);
	printf("h\tI\tS\tT\n");


	double ns[] = {10,50,100};
	for(int i = 0; i < 3; i++)
	{
		SetN(ns[i]);
		auto ideal = Analytical().second;
		auto trid = TridiagonalAlgorithm();
		double mu0 = SecantMethod(-10000, 10000, Deviation);
		auto solution = SolveCauchyWithRK4(mu0).first;
		
		printf(
			"%f\t%f\t%f\t%f\n",
			h, MaxDev(ideal, ideal), MaxDev(ideal, solution), MaxDev(ideal, trid));
	}
}

void Plot()
{
	printf("N:\t");
	scanf("%d",&n);
	freopen("output.txt","w",stdout);
	
	SetN(10);
	auto ideal = Analytical();
	
	auto trid = TridiagonalAlgorithm();
	double mu0 = SecantMethod(-10000, 10000, Deviation);
	auto solution = SolveCauchyWithRK4(mu0);
	freopen("output.txt","w",stdout);
	printf("X\tI\tS\tT\n");
	for(int i = 0; i < ideal.first.size(); i++)
	{
		printf("%f\t%f\t%f\t%f\n",ideal.first[i], ideal.second[i], solution.first[i], trid[i]);
	}
}

int main()
{
	Plot();
	Devs();
}