#include "ac.h"

ArithmeticCoder::ArithmeticCoder(FILE* infile, FILE* outfile, MODE coding_mode)
  : infile(infile), outfile(outfile), coding_mode(coding_mode)
{
  if (coding_mode == CODER)
  {
    initialize_output_bitstream();
    initialize_arithmetic_encoder();
  }
  else if (coding_mode == DECODER)
  {
    initialize_input_bitstream();
    initialize_arithmetic_decoder(infile);
  }
  else throw "Арифметическое кодирование отказывается работать в таких условиях";
}

ArithmeticCoder::~ArithmeticCoder()
{
  if (coding_mode == CODER)
  {
    flush_arithmetic_encoder(outfile);
    flush_output_bitstream(outfile);
  }
}

SYMBOL CreateSymbol(vector<counter>& counters)
{
  SYMBOL s = {0, 0, 0};
  for (int i = 0; i < (int) counters.size(); i++)
  {
    unsigned long long old_scale = s.scale;
    s.scale += (unsigned long long)counters[i].val;
    if (s.scale <= old_scale)
      throw "Overflow";
  }
  return s;
}

SYMBOL RecalcSymbol(counter& counter, SYMBOL prev)
{
  SYMBOL s = {prev.high_count, prev.high_count + counter.val, prev.scale};
  return s;
}

void ArithmeticCoder::encode(vector<counter> & counters, int ch)
{
  SYMBOL s(CreateSymbol(counters));
  for (size_t i = 0; i < counters.size(); i++)
  {
    s = RecalcSymbol(counters[i], s);
    if (counters[i].ch == ch)
      break;
  }
  encode_symbol(outfile, &s);
}

int ArithmeticCoder::decode(vector<counter> & counters)
{
  SYMBOL s(CreateSymbol(counters));
  unsigned long long count = get_current_count(&s);
  for (size_t i = 0; i < counters.size(); i++)
  {
    s = RecalcSymbol(counters[i], s);
    if (count >= s.low_count && count < s.high_count)
    {
      remove_symbol_from_stream(infile, &s);
      return counters[i].ch;
    }
  }
  throw "o_O";
}