#pragma once
#ifndef _DEFS_H_
#define _DEFS_H_

#include <vector>
#include <string>
using namespace std;

template<class T> T Abs(const T& t)
{
	if ( t>0 ) return t;
	return -t;
}

template<class T> T Sqr2(const T& t)
{
	return ((t)*(t));
}

typedef vector<int> VI;

typedef double LD;
typedef vector<LD> VD;
typedef vector<VD > VVD;
typedef vector<int> VI;
typedef vector<VI > VVI;

typedef vector<string> VS;

#endif
