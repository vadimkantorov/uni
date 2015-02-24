#pragma once 
#ifndef COMMONUTILS_H_
#define COMMONUTILS_H_

#include <vector>
#include <string>
#include <algorithm>
using namespace std;

vector<string> SplitNotNull(const string& s, const string& c);

double AverageValue(const vector<double>& v);
double MedianValue(const vector<double>& v);
double Variance(const vector<double>& v);
double Sum(const vector<double>& v);
double ProbabilityMedian(vector<pair<double, double> >& arr);

#endif
