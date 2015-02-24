#include <cstdio>
#include <algorithm>
#include <vector>

using namespace std;

double SQR(double x)
{
	return x*x;
}

double f(double x, double y)
{
	return -20*SQR(y)*(x-0.4);
}

double df(double x, double y)
{
	return -20*(2*y*f(x, y)*(x-0.4)+SQR(y));
	//return -20*SQR(y);
}

const double Y0 = 0.5;
const double a = 0;
const double b = 1;
double h;

vector<double> SolveODE(double (*ynext)(double xk, double yk))
{
	vector<double> res;
	res.push_back(Y0);
	for(double x = h; x <= b; x += h)
	{
		double yk = res.back();
		double xk = x - h;
		double yn = ynext(xk, yk);
		res.push_back(yn);
	}
	return res;
}

double RK4(double xk, double yk)
{
	double k1 = h*f(xk, yk);
	double k2 = h*f(xk + h/2, yk + k1/2);
	double k3 = h*f(xk + h/2, yk + k2/2);
	double k4 = h*f(xk + h, yk + k3);
	return yk + k1/6 + k2/3 + k3/3 + k4/6;
}

double Taylor2(double xk, double yk)
{
	return yk + h*f(xk, yk) + 0.5*SQR(h)*df(xk, yk);
}

double EulerExplicit(double xk, double yk)
{
	return yk + h*f(xk, yk);
}

double Analytical(double xk, double yk)
{
	double x = xk+h;
	return 1/(10*SQR(x) - 8*x + 2);
}



pair<vector<double>, vector<double>> ExtractPrev(vector<double>& res, double x, int k)
{
	vector<double> xs, ys;

	for(int i = 1; i <= k+1; i++)
	{
		xs.push_back(x - i*h);
		ys.push_back(res[res.size()-i]);
	}

	return make_pair(xs, ys);
}

vector<double> SimpsonImplicit3()
{
	const int order = 3;
	vector<double> res = SolveODE(RK4);
	res.resize(order+1); //0, h, 2h, 3h,
	for(double x = (order+1)*h; x <= b; x += h)
	{
		pair<vector<double>, vector<double>> hh = ExtractPrev(res, x, order);
		vector<double>& xs = hh.first;
		vector<double>& ys = hh.second;

		double p = ys[3] + 4*h/3*(
			+2*f(xs[0], ys[0]) 
			-1*f(xs[1], ys[1])
			+2*f(xs[2], ys[2])
			);

		double c = ys[1] + h/3*(
			+1*f(x, p)
			+4*f(xs[0], ys[0])
			+1*f(xs[1], ys[1])
			);
		
		res.push_back(c);
	}
	return res;
}

vector<double> SimpsonImplicitK2()
{
	const int order = 3;
	vector<double> res = SolveODE(RK4);
	res.resize(order+1); //0, h, 2h, 3h,
	for(double x = (order+1)*h; x <= b; x += h)
	{
		pair<vector<double>, vector<double>> hh = ExtractPrev(res, x, order);
		vector<double>& xs = hh.first;
		vector<double>& ys = hh.second;

		double c;
		
		double A = ys[1] + h*(4.0/3*f(xs[0], ys[0]) + 1.0/3*f(xs[1], ys[1]));
		if(abs(x-0.4) <= 1e-5)
		{
			c = A;
		}
		else
		{
			double B = -20*h/3.0*(x - 0.4);
			double c1 = (1 - sqrt(1 - 4*B*A))/(2*B);
			double c2 = (1 - sqrt(1 - 4*B*A))/(2*B);
		
			if(abs(c1 - ys[0]) < abs(c2 - ys[0]))
				c = c1;
			else
				c = c2;
		}
		res.push_back(c);
	}
	return res;
}


vector<double> SimpsonImplicit5()
{
	const int order = 5;
	vector<double> res = SolveODE(RK4);
	res.resize(order+1); //0, h, 2h, 3h, 4h, 5h
	for(double x = (order+1)*h; x <= b; x += h)
	{
		pair<vector<double>, vector<double>> hh = ExtractPrev(res, x, order);
		vector<double>& xs = hh.first;
		vector<double>& ys = hh.second;

		double p = ys[5] + 3*h/10*(
			+11*f(xs[0], ys[0]) 
			-14*f(xs[1], ys[1])
			+26*f(xs[2], ys[2])
			-14*f(xs[3], ys[3])
			+11*f(xs[4], ys[4]));

		double c = ys[3] + 2*h/45*(
			+7*f(x, p)
			+32*f(xs[0], ys[0])
			+12*f(xs[1], ys[1])
			+32*f(xs[2], ys[2])
			+7*f(xs[3], ys[3])
			);
		
		res.push_back(c);
	}
	return res;
}

double MaxDev(vector<double>& ideal, vector<double>& ys)
{
	vector<double> devs;
	for(int i = 0; i < ideal.size(); i++)
		devs.push_back(abs(ideal[i] - ys[i]));
	return *max_element(devs.begin(), devs.end());
}

void Plot()
{
	h = 0.1;
	vector<double> taylor = SolveODE(Taylor2);
	vector<double> euler = SolveODE(EulerExplicit);
	vector<double> ideal = SolveODE(Analytical);
	vector<double> simp3 = SimpsonImplicit3();
	vector<double> simp5 = SimpsonImplicit5();
	vector<double> simpk2 = SimpsonImplicitK2();
	
	freopen("output.txt","w",stdout);
	printf("X\tI\tT\tE\tS3\tS5\tK2\n");
	for(int i = 0; i < ideal.size(); i++)
	{
		printf(
			//"x: %.2f I: %.2f\tT: %.2f\tE: %.2f\tS3: %.2f\tS5: %.2f\n", 
			"%f\t%f\t%f\t%f\t%f\t%f\t%f\n",
			a + h*i, ideal[i], taylor[i], euler[i], simp3[i], simp5[i], simpk2[i]);
	}
}

void Devs()
{
	freopen("devs.txt","w",stdout);
	printf("h\tI\tT\tE\tS3\tS5\tK2\n");

	double hs[] = {0.1, 0.01, 0.001};
	for(int i = 0; i < 3; i++)
	{
		h = hs[i];
		vector<double> taylor = SolveODE(Taylor2);
		vector<double> euler = SolveODE(EulerExplicit);
		vector<double> ideal = SolveODE(Analytical);
		vector<double> simp3 = SimpsonImplicit3();
		vector<double> simp5 = SimpsonImplicit5();
		vector<double> simpk2 = SimpsonImplicitK2();

		printf(
			"%f\t%f\t%f\t%f\t%f\t%f\t%f\n",
			h, MaxDev(ideal, ideal), MaxDev(ideal, taylor), MaxDev(ideal, euler), MaxDev(ideal, simp3), MaxDev(ideal, simp5), MaxDev(ideal, simpk2));
	}
}

int main()
{
	Devs();
	Plot();
}