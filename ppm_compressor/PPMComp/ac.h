#pragma once

#include <cstdio>
#include <vector>

#include "common.h"
#include "bitio.h"
#include "coder.h"

using std::vector;

class ArithmeticCoder{
public:
  ArithmeticCoder(FILE* infile, FILE* outfile, MODE coding_mode);
  ~ArithmeticCoder();
  void encode(vector<counter> & counters, int ch);
  int decode(vector<counter> & counters);
private:
  FILE * infile;
  FILE * outfile;
  MODE coding_mode;
};
