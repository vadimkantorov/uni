#define _CRT_SECURE_NO_DEPRECATE
#include <cstdio>
#include <cstring>
#include <vector>
#include <string>
//#include <fcntl.h> // for _O_BINARY
using namespace std;

const int ALPH_SIZE = 256;
const int ESCAPE_SYMBOL = -1;

int MAX_CONTEXT;
int TRIE_SIZE;
bool UPDATE_EXCLUSION;
string ESCAPE_PROB;
int SEE_DEPTH;
double RECENCY_SCALING;
double SCALING_COEF;
int SCALING_FREQ;

enum {CODER, DECODER, OTHER} mode;
FILE * infile;
FILE * outfile;
FILE * logfile;

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
    if (!strcmp(str, "SEE_DEPTH")){
      sscanf(val, "%d", &SEE_DEPTH);
    }
    if (!strcmp(str, "RECENCY_SCALING")){
      sscanf(val, "%lf", &RECENCY_SCALING);
    }
    if (!strcmp(str, "SCALING_COEF")){
      sscanf(val, "%lf", &SCALING_COEF);
    }
    if (!strcmp(str, "SCALING_FREQ")){
      sscanf(val, "%d", &SCALING_FREQ);
    }
  }
}

struct counter{
  int ch;
  int val;
  counter(int _ch = 0, int _val = 0) : ch(_ch), val(_val) {}
};

struct ArithmeticCoder{
  FILE * infile;
  FILE * outfile;
  ArithmeticCoder(FILE* _infile, FILE* _outfile) : infile(_infile), outfile(_outfile) {}
  void encode(vector<counter> & counters, int ch) { fprintf(outfile, "%d\n", ch); }
  int decode(vector<counter> & counters) { int zzz; fscanf(infile, "%d", &zzz); return zzz;}
};

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
  sons.push_back(down_link(ch, (int)nodes.size() - 1));
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

void add_escape_probability(vector<counter> & counters){
  if (ESCAPE_PROB == "PPMA"){
    counters.push_back(counter(ESCAPE_SYMBOL, 1));
  }
}

int process_node(int node, int cur, bool exclusions[ALPH_SIZE]){
  vector<counter> & counters = nodes[node].counters;
  vector<counter> good_counters;
  for (int i=0; i<(int)counters.size(); i++){
    int zch = counters[i].ch;
    if (!exclusions[zch]){
      good_counters.push_back(counters[i]);
      if (zch == nodes[node].last_encoded){
        good_counters.back().val = int(good_counters.back().val * RECENCY_SCALING);
      }
    }
    exclusions[zch] = true;
  }
  add_escape_probability(good_counters);
  if (mode == CODER){
    if (!exclusions[cur]) cur = ESCAPE_SYMBOL;
    AC->encode(good_counters, cur);
  }
  else{
    cur = AC->decode(good_counters);
  }
  return cur;
}

int process(vector<int> & path, int cur = ESCAPE_SYMBOL){
  bool exclusions[ALPH_SIZE];
  memset(exclusions, false, sizeof(exclusions));
  for (int i=(int)path.size() - 1; i>=0; i--){
    int coded = process_node(path[i], cur, exclusions);
    nodes[path[i]].num_been++;
    if (coded != ESCAPE_SYMBOL){
      nodes[path[i]].last_encoded = cur;
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
  if (symbols % SCALING_FREQ != 0) return;
  if (symbols % 10000 == 0) fprintf(stderr, "%d\n", symbols);
  for (int i=1; i<(int)nodes.size(); i++){
    vector<counter> & counters = nodes[i].counters;
    for (int j=0; j<(int)counters.size(); j++){
      counters[j].val = int(counters[j].val * SCALING_COEF);
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
        fprintf(outfile, "%d\n", symbols - 1);
        break;
      }
      unsigned char cur = fgetc(infile);
      vector<int> path = build_path(tail);
      process(path, int(unsigned char(cur)));
      update(path, int(unsigned char(cur)));
      update_tail(tail, cur);
      scale_all_counters(symbols);
    }
  }
  else if (mode == DECODER){
    int ZZZ;
    fscanf(infile, "%d", &ZZZ);
    ZZZ--;
    for (int symbols=1; symbols <= ZZZ; symbols++){
      // if () break;
      vector<int> path = build_path(tail);
      if (symbols == 150){
        int z=2;
      }
      int cur = process(path);
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
  //_set_fmode(_O_BINARY);
  infile = fopen(argv[2], "rb");
  outfile = fopen(argv[3], "wb");
  //_set_fmode(_O_TEXT);
  AC = new ArithmeticCoder(infile, outfile);
  logfile = fopen("zzzlog.txt", "w");
  run_ppm();
  fclose(infile);
  fclose(outfile);
  fclose(logfile);
  return 0;
}
