#pragma once
#ifndef INPUT_H_
#define INPUT_H_

#include <stdio.h>

FILE* OpenFile(const char* mode, const char* filePattern, ...);

bool ReadLine(char* buf, FILE* f = stdin, int max_len = 1024);
char* ReadWord(char* buf, char* res);
char* ReadInt(char* buf, int& res);
char* ReadDouble(char* buf, double& res);

void WriteLine(char* buf, FILE* f = stdout);

#endif
