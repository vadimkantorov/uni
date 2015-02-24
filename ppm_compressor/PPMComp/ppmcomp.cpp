#define _CRT_SECURE_NO_DEPRECATE
#include <cstdio>
#include <cstring>
#include <vector>
#include <string>
#include <cmath>
#include <algorithm>

#include "ac.h"
#include "common.h"

using namespace std;

const int ALPH_SIZE = 256;
const int ESCAPE_SYMBOL = -1;

int MAX_CONTEXT;
int TRIE_SIZE;
bool UPDATE_EXCLUSION;
string ESCAPE_PROB;
double RECENCY_SCALING;
double SCALING_COEF;
int SCALING_FREQ;
MODE mode;

FILE * infile;
FILE * outfile;
FILE * auxfile;

void print_counters(vector<counter>& counters, int sym, int symnum, int step)
{
  return;
  fprintf(stderr, "symnum = %d, step = %d, len = %d, sym = %d\n", symnum, step, counters.size(), sym);
  for (int i = 0; i < (int) counters.size(); i++)
  {
    fprintf(stderr, "  ch = %d, val = %d\n", counters[i].ch, counters[i].val);
  }
  fputs("===\n", stderr);
}

void read_ini_file(){
  FILE * inif = fopen("params.ini", "r");
  char str[100], val[100];
  while (fscanf(inif, "%s = %s\n", &str, &val) > 0){
    if (!strcmp(str, "MAX_CONTEXT")){
      sscanf(val, "%d", &MAX_CONTEXT);
    }
    if (!strcmp(str, "TRIE_SIZE")){
      sscanf(val, "%d", &TRIE_SIZE);
    }
    if (!strcmp(str, "UPDATE_EXCLUSION")){
      UPDATE_EXCLUSION = (val[0]=='t' || val[0]=='T' || val[0]=='1');
    }
    if (!strcmp(str, "ESCAPE_PROB")){
      ESCAPE_PROB = string(val);
    }
    if (!strcmp(str, "RECENCY_SCALING")){
      sscanf(val, "%lf", &RECENCY_SCALING);
    }
    if (!strcmp(str, "SCALING")){
      sscanf(val, "%lf-%d", &SCALING_COEF, &SCALING_FREQ);
    }
  }
}

ArithmeticCoder * AC;

struct down_link{
  int ch;
  int id;
  down_link(int _ch = 0, int _id = 0) : ch(_ch), id(_id) {}
};

struct node{
  vector<counter> counters;
  vector<down_link> sons;
  int num_been;
  int num_escape;
  int last_encoded;
  node(){
    last_encoded = ESCAPE_SYMBOL;
    num_been = 0;
    num_escape = 0;
  }
};

int total_nodes;
vector<node> nodes;

int get_son(int cv, int ch){
  vector<down_link> & sons = nodes[cv].sons;
  for (int i=0; i<(int)sons.size(); i++){
    if (sons[i].ch == ch) return sons[i].id;
  }
  nodes.push_back(node());
  nodes[cv].sons.push_back(down_link(ch, (int)nodes.size() - 1));
  return (int)nodes.size() - 1;
}

vector<int> build_path(const string & tail){
  vector<int> path;
  path.push_back(0);
  int cv = 1;
  path.push_back(cv);
  for (int i=(int)tail.size() - 1; i>=0; i--){
    int nv = get_son(cv, int(unsigned char(tail[i])));
    path.push_back(nv);
    cv = nv;
  }
  return path;
}

void add_escape_probability(vector<counter> & counters, vector<int> & path){
  int c = 0, q = (int)counters.size(), q1 = 0;
  for (int i=0; i<(int)counters.size(); i++){
    c += counters[i].val;
    q1 += (counters[i].val == 1);
  }
  int coef, eprob;
  if (ESCAPE_PROB == "PPMA"){
    coef = 1;
    eprob = 1;
  }
  else if (ESCAPE_PROB == "PPMB"){
    coef = c - (q-q1);
    eprob = c * (q-q1);
  }
  else if (ESCAPE_PROB == "PPMC"){
    coef = 1;
    eprob = q;
  }
  else if (ESCAPE_PROB == "PPMD"){
    coef = 2*c - q;
    eprob = c * q;
  }
  else if (ESCAPE_PROB == "PPMXC"){
    if (q1 > 0 && q1 < c){
      coef = c - q1;
      eprob = c * q1;
    }
    else{
      coef = 1;
      eprob = q;
    }
  }
  else if (ESCAPE_PROB.substr(0, 3) == "SEE"){
    int SEE_DEPTH = ESCAPE_PROB[4] - '0';
    double res = 0, sw = 0;
    for (int i=1; (i<(int)path.size()) && (i<=SEE_DEPTH+1); i++){
      if (nodes[path[i]].num_been == 0) continue;
      double e = double(nodes[path[i]].num_escape) / nodes[path[i]].num_been;
      double w;
      if (e == 0 || e == 1) w = 0;
      else w = e * log(1 / e) / log(2.0) + (1-e) * log(1 / (1-e)) / log(2.0);
      if (w == 0) w = 0.001;
      w = 1 / w;
      res += e * w;
      sw += w;
    }
    eprob = int(c * res);
    coef = int(sw - res);
  }
  int mx = 0;
  for (int i = 0; i < (int)counters.size(); i++)
    mx = max(mx, counters[i].val);
  if (coef > 0 && mx >= 2000000000 / coef)
  {
    int p10 = 1;
    while (mx / p10 >= 2000000000 / coef){
      p10 *= 10;
    }
    for (int i = 0; i < (int)counters.size(); i++)
    {
      counters[i].val /= p10;
      if (counters[i].val == 0) counters[i].val = 1;
    }
    eprob /= p10;
    if (eprob == 0) eprob = 1;
  }
  for (int i=0; i<(int)counters.size(); i++){
    if (counters[i].val * coef < 0){
      // just in case
      fprintf(stderr, "%d\n", counters[i].val);
      for (;;);
    }
    counters[i].val *= coef;
    if (counters[i].val == 0) counters[i].val = 1;
  }
  if (eprob == 0) eprob = 1;
  counters.push_back(counter(ESCAPE_SYMBOL, eprob));
}

int Gcd(int a, int b)
{
  while (b)
  {
    int c = a % b;
    a = b;
    b = c;
  }
  return a;
}

void normalize_counters(vector<counter>& counters)
{
  if (counters.size() <= 1)
    return;
  int g = Gcd(counters[0].val, counters[1].val);
  for (int i = 2; i < (int) counters.size(); i++)
    g = Gcd(g, counters[i].val);
  for (int i = 0; i < (int) counters.size(); i++)
    counters[i].val /= g;
}

int process_node(int node, int cur, bool exclusions[ALPH_SIZE], vector<int> & path, int symnum, int step){
  vector<counter> & counters = nodes[node].counters;
  vector<counter> good_counters;
  for (int i=0; i<(int)counters.size(); i++){
    int zch = counters[i].ch;
    if (zch < 0 || zch > 255)
      throw "FAIL";
    if (!exclusions[zch]){
      good_counters.push_back(counters[i]);
      if (zch == nodes[node].last_encoded){
        good_counters.back().val = int(good_counters.back().val * RECENCY_SCALING);
      }
    }
    exclusions[zch] = true;
  }
  add_escape_probability(good_counters, path);
  normalize_counters(good_counters);
  if (mode == CODER){
    if (!exclusions[cur]) cur = ESCAPE_SYMBOL;
    print_counters(good_counters, cur, symnum, step);
    AC->encode(good_counters, cur);
  }
  else{
    cur = AC->decode(good_counters);
    print_counters(good_counters, cur, symnum, step);
  }
  return cur;
}

int process(vector<int> & path, int symnum, int cur = ESCAPE_SYMBOL){
  bool exclusions[ALPH_SIZE];
  memset(exclusions, false, sizeof(exclusions));
  for (int i=(int)path.size() - 1, step = 1; i>=0; i--, step++){
    int coded = process_node(path[i], cur, exclusions, path, symnum, step);
    nodes[path[i]].num_been++;
    if (coded != ESCAPE_SYMBOL){
      nodes[path[i]].last_encoded = coded;
      return coded;
    }
    else{
      nodes[path[i]].num_escape++;
    }
  }
  return ESCAPE_SYMBOL;
}

bool update_node(int node, int ch){
  if (node == 0) return true;
  vector<counter> & counters = nodes[node].counters;
  for (int i=0; i<(int)counters.size(); i++){
    if (counters[i].ch == ch){
      counters[i].val++;
      if (counters[i].val < 0)
        throw "Overflow";
      return true;
    }
  }
  counters.push_back(counter(ch, 1));
  return false;
}

void update(vector<int> & path, int cur){
  for (int i=(int)path.size() - 1; i>=0; i--){
    bool success = update_node(path[i], cur);
    if (success && UPDATE_EXCLUSION) return;
  }
}

void update_tail(string & tail, unsigned char cur){
  if ((int)tail.size() == MAX_CONTEXT) tail = tail.substr(1);
  tail += cur;
}

void scale_all_counters(int symbols){
  // if (symbols % 10000 == 0) fprintf(stderr, "%d\n", symbols);
  if (SCALING_COEF == 1.0) return;
  if (symbols % SCALING_FREQ != 0) return;
  for (int i=1; i<(int)nodes.size(); i++){
    vector<counter> & counters = nodes[i].counters;
    for (int j=0; j<(int)counters.size(); j++){
      counters[j].val = int(counters[j].val * SCALING_COEF);
      if (counters[j].val == 0) counters[j].val = 1;
    }
  }
}

void run_ppm(){
  nodes.reserve(TRIE_SIZE);
  node minus_one, zero;
  for (int ch=0; ch<ALPH_SIZE; ch++){
    minus_one.counters.push_back(counter(ch, 1));
    minus_one.sons.push_back(down_link(ch, 1));
  }
  nodes.push_back(minus_one);
  nodes.push_back(zero);
  total_nodes = 2;
  string tail = "";
  if (mode == CODER){
    for (int symbols=1; ; symbols++){
      if (feof(infile)){
        fprintf(auxfile, "%d\n", symbols - 1);
        break;
      }
      unsigned char cur = fgetc(infile);
      vector<int> path = build_path(tail);
      process(path, symbols, int(unsigned char(cur)));
      update(path, int(unsigned char(cur)));
      update_tail(tail, cur);
      scale_all_counters(symbols);
    }
  }
  else if (mode == DECODER){
    int ZZZ;
    fscanf(auxfile, "%d", &ZZZ);
    ZZZ--;
    for (int symbols=1; symbols <= ZZZ; symbols++){
      vector<int> path = build_path(tail);
      int cur = process(path, symbols);
      fprintf(outfile, "%c", (unsigned char)cur);
      update(path, cur);
      update_tail(tail, (unsigned char)cur);
      scale_all_counters(symbols);
    }
  }
}

int main(int argc, char * argv[]){
  read_ini_file();
  if (!strcmp(argv[1], "--code")) mode = CODER;
  else if (!strcmp(argv[1], "--decode")) mode = DECODER;
  else mode = OTHER;
  infile = fopen(argv[2], "rb");
  outfile = fopen(argv[3], "wb");
  auxfile = fopen((string(argv[mode == CODER ? 3 : 2]) + ":length").c_str(), mode == CODER ? "w" : "r");
  AC = new ArithmeticCoder(infile, outfile, mode);
  run_ppm();
  delete AC;
  fclose(infile);
  fclose(outfile);
  fclose(auxfile);
  return 0;
}
