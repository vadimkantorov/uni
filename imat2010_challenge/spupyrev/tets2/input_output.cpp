#pragma warning(disable:4996)
#pragma warning(disable:4018)

#include <cstdio>
#include <string>
#include <cstdarg>
#include <iostream>
#include <string.h>
#include <ctype.h>
using namespace std;

FILE* OpenFile(const char* mode, const char* filePattern, ...)
{
    char* buffer = new char [1024];
    char* fileName = new char [1024];
    
    va_list ap;
    va_start(ap, filePattern);
    std::vsprintf(buffer, filePattern, ap);
    va_end(ap);

	sprintf(fileName, "%s", buffer);
	FILE* f = fopen(fileName, mode);
	if ( f==0 )
		cerr<<"Can't open file "<<string(fileName)<<"\n";

	delete[] buffer;
	delete[] fileName;

	return f;
}

bool ReadLine(char* buf, FILE* f = stdin, int max_len = 1024)
{
	if ( fgets(buf, max_len, f)==NULL ) return false;
	if ( strlen(buf)==0 ) return ReadLine(buf, f, max_len);
	return true;
}

void WriteLine(char* buf, FILE* f = stdout)
{
	fputs(buf, f);
}

char* ReadWord(char* buf, char* res)
{
	while ( *buf && isspace(*buf) ) buf++;
	while ( *buf && !isspace(*buf) ) 
	{
		*res = *buf;
		res++;
		buf++;
	}
	*res = 0;
	return buf;
}

char* ReadInt(char* buf, int& res)
{
	res = 0;
	while ( *buf && !isdigit(*buf) ) buf++;
	while ( *buf && isdigit(*buf) )
	{
		res *= 10;
		res += (*buf-'0');
		buf++;
	}
	return buf;
}

char* ReadDouble(char* buf, double& res)
{
	res = 0;
	while ( *buf && !isdigit(*buf) ) buf++;
	while ( *buf && isdigit(*buf) )
	{
		res *= 10.0;
		res += (*buf-'0');
		buf++;
	}
	if ( *buf=='.' ) buf++;
	double frac = 0.0, pow10 = 1.0;
	while ( *buf && isdigit(*buf) )
	{
		pow10 *= 10.0;
		frac *= 10.0;
		frac += (*buf-'0');
		buf++;
	}

	res += frac/pow10;

	return buf;
}
