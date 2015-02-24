#pragma once

struct counter{
  int ch;
  int val;
  counter(int _ch = 0, int _val = 0) : ch(_ch), val(_val) {}
};

enum MODE{CODER, DECODER, OTHER};
