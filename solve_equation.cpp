/*
Method: Binary serarch
Root: 1.3920974731
Iterations: 18

Method: Secant method
Root: 1.3920937532
Iterations: 8
*/

#include <cstdio>
#include <cmath>
#include <utility>
using namespace std;

const double EPS = 5e-6;
const double L = 1, R = 2;

double f(double x)
{
	return 0.5*log10(x) - 0.1/x;
}

double df(double x)
{
	return 0.1/(x*x) + 1/(2*log(10.0)*x);
}

pair<double, int> SecantMethod()
{
	double m = df(R);
	double x0 = L, x1 = R;
	int iterations = 0;
	while(abs(x1)/m > EPS && x1 != x0)
	{
		double next = x1 - f(x1)*(x1 - x0)/(f(x1)-f(x0));
		x0 = x1;
		x1 = next;
		iterations++;
	}
	
	return make_pair(x1, iterations);
}

pair<double, int> BinarySearch()
{
	int iterations = 0;
	double l = L, r = R;
	while(abs(r-l) > EPS)
	{
		double mid = (l+r)/2;
		if(f(mid) > 0)
			r = mid;
		else
			l = mid;
		iterations++;
	}
	return make_pair(r, iterations);
}

void PrintResults(char method[], pair<double, int> solution)
{
	printf("Method: %s\nRoot: %.10f\nIterations: %d\n\n", method, solution.first, solution.second);
}

int main()
{
	PrintResults("Binary serarch", BinarySearch());
	PrintResults("Secant method", SecantMethod());
	
}