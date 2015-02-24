#include "common_utils.h"
#include "defs.h"

#include <cassert>
#include <cmath>

vector<string> SplitNotNull(const string& s, const string& c)
{
	vector<string> result;
	string tec = "";
	for (int i = 0;i <= (int)s.length(); i++)
	{
		if ( i == (int)s.length() || (int)c.find(s[i]) != -1 ) 
		{
			if ( tec.length()>0 ) result.push_back(tec);
			tec = "";
		}
		else tec += s[i];
	}
	return result;
}

double Sum(const vector<double>& v)
{
	double sum = 0;
	for (int i = 0; i < (int)v.size(); i++)
		sum += v[i];
	return sum;
}

double AverageValue(const vector<double>& v)
{
	double av = Sum(v);
	if ( !v.empty() )
		av /= (double)v.size();
	return av;
}

double MedianValue(const vector<double>& v)
{
	if ( v.empty() ) return 0;
	if ( v.size() == 1 ) return v[0];

	vector<double> tmp = v;
	sort(tmp.begin(), tmp.end());
	int sz = (int)tmp.size();

	if ( sz%2 == 0 )
		return (tmp[sz/2 - 1] + tmp[sz/2])/2.0;
	else
		return tmp[sz/2];
}

double Variance(const vector<double>& v)
{
	if ( v.empty() ) return 0;
	double var = 0, cnt = 0;
	double mean = AverageValue(v);
	for (int j = 0; j < (int)v.size(); j++)
	{
		var += Sqr2(v[j] - mean);
		cnt++;
	}

	if ( cnt > 0 ) var /= cnt;
	var = sqrt(var);
	return var;
}

//pair<value, probability>
double ProbabilityMedian(vector<pair<double, double> >& arr)
{
	sort(arr.begin(), arr.end());
	double sum = 0;
	for (int i = 0; i < (int)arr.size(); i++)
	{
		sum += arr[i].second;
	}

	assert(sum >= 0.0);
	double tec = 0;
	for (int i = 0; i < (int)arr.size(); i++)
	{
		tec += arr[i].second/sum;
		if ( tec >= 0.5 ) return arr[i].first;
	}

	assert(false);
	return -1;
}
