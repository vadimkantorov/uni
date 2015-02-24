#include <algorithm>
#include <vector>
#include <utility>
#include <cmath>
#include <iostream>
using namespace std;

typedef pair<double, double> Point;

Point points[100];
int n,k;

double Square(vector<int>& take)
{
	double res = 0;
	for(int i = 0; i < take.size(); i++)
	{
		Point& f = points[take[i]];
		Point& s = points[take[(i+1)%take.size()]];

		res += (s.first - f.first)*(s.second + f.second);
	}
	return abs(res/2.0);
}

vector<int> perm;
bool used[50];
double maxSquare;

void EnumeratePermutations()
{
	if(perm.size() == k)
	{
		maxSquare = max(maxSquare, Square(perm));
		return;
	}
	
	for(int i = 0; i < n; i++)
	{
		if(!used[i])
		{
			used[i] = true;
			perm.push_back(i);
			EnumeratePermutations();
			perm.pop_back();
			used[i] = false;
		}
	}
}

void main()
{
	freopen("input.txt","r",stdin);
	
	cin >> n >> k;
	for(int i = 0; i < n; i++)
	{
		double a,b;
		cin >> a >> b;
		points[i] = make_pair(a,b);
	}

	EnumeratePermutations();
	cout << maxSquare;
}