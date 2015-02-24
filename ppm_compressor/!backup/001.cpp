#define _CRT_SECURE_NO_DEPRECATE
#include <cstdio>
#include <cstring>
#include <vector>
#include <string>
//#include <fcntl.h> // for _O_BINARY
using namespace std;

const int ALPH_SIZE = 128;

int TRIE_DEPTH;
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
    if (!strcmp(str, "TRIE_DEPTH")){
      sscanf(val, "%d", &TRIE_DEPTH);
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
  char ch;
  int val;
  counter(char _ch = 0, int _val = 0) : ch(_ch), val(_val) {}
};

struct ArithmeticCoder{
  FILE * infile;
  FILE * outfile;
  ArithmeticCoder(FILE* _infile, FILE* _outfile) : infile(_infile), outfile(_outfile) {}
  void encode(vector<counter> & counters, char ch) { fprintf(outfile, "%d\n", (int)ch); }
  char decode(vector<counter> & counters) { int zzz; fscanf(infile, "%d", &zzz); return char(zzz);}
};

ArithmeticCoder * AC;

struct down_link{
  char ch;
  int id;
  down_link(char _ch = 0, int _id = 0) : ch(_ch), id(_id) {}
};

struct node{
  vector<counter> counters;
  vector<down_link> sons;
  int num_been;
  int num_escape;
  char last_encoded;
  node(){
    last_encoded = 0;
    num_been = 0;
    num_escape = 0;
  }
};

int total_nodes;
vector<node> nodes;

int get_son(int cv, char ch){
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
    int nv = get_son(cv, tail[i]);
    path.push_back(nv);
    cv = nv;
  }
  return path;
}

void add_escape_probability(vector<counter> & counters){
  if (ESCAPE_PROB == "PPMA"){
    counters.push_back(counter(0, 1));
  }
}

bool encode_node(int node, char ch, bool exclusions[ALPH_SIZE]){
  vector<counter> & counters = nodes[node].counters;
  vector<counter> good_counters;
  for (int i=0; i<(int)counters.size(); i++){
    char zch = counters[i].ch;
    if (!exclusions[zch]){
      good_counters.push_back(counters[i]);
      if (zch == nodes[node].last_encoded){
        good_counters.back().val = int(good_counters.back().val * RECENCY_SCALING);
      }
    }
    exclusions[zch] = true;
  }
  add_escape_probability(good_counters);
  if (!exclusions[ch]) ch = 0;
  AC->encode(good_counters, ch);
  return (ch != 0);
}

void encode(vector<int> & path, char cur){
  bool exclusions[ALPH_SIZE];
  memset(exclusions, false, sizeof(exclusions));
  for (int i=(int)path.size() - 1; i>=0; i--){
    bool success = encode_node(path[i], cur, exclusions);
    nodes[path[i]].num_been++;
    if (success){
      nodes[path[i]].last_encoded = cur;
      return;
    }
    else{
      nodes[path[i]].num_escape++;
    }
  }
}

bool decode_node(int node, char & cur, bool exclusions[ALPH_SIZE]){
  vector<counter> & counters = nodes[node].counters;
  vector<counter> good_counters;
  for (int i=0; i<(int)counters.size(); i++){
    char zch = counters[i].ch;
    if (!exclusions[zch]){
      good_counters.push_back(counters[i]);
      if (zch == nodes[node].last_encoded){
        good_counters.back().val = int(good_counters.back().val * RECENCY_SCALING);
      }
    }
    exclusions[zch] = true;
  }
  add_escape_probability(good_counters);
  char ch = AC->decode(good_counters);
  cur = ch;
  return (ch != 0);
}


char decode(vector<int> & path){
  bool exclusions[ALPH_SIZE];
  memset(exclusions, false, sizeof(exclusions));
  for (int i=(int)path.size() - 1; i>=0; i--){
    char cur;
    bool success = decode_node(path[i], cur, exclusions);
    nodes[path[i]].num_been++;
    if (success){
      nodes[path[i]].last_encoded = cur;
      return cur;
    }
    else{
      nodes[path[i]].num_escape++;
    }
  }
  return 0;
}

bool update_node(int node, char ch){
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

void update(vector<int> & path, char cur){
  for (int i=(int)path.size() - 1; i>=0; i--){
    bool success = update_node(path[i], cur);
    if (success && UPDATE_EXCLUSION) return;
  }
}

void scale_all_counters(){
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
  for (int ch=1; ch<ALPH_SIZE; ch++){
    minus_one.counters.push_back(counter((char)ch, 1));
    minus_one.sons.push_back(down_link((char)ch, 1));
  }
  nodes.push_back(minus_one);
  nodes.push_back(zero);
  total_nodes = 2;
  string tail = "";
  if (mode == CODER){
    for (int symbols=1; ; symbols++){
      char cur = fgetc(infile);
      if (cur == EOF){
        fprintf(outfile, "%d\n", symbols - 1);
        break;
      }
      vector<int> path = build_path(tail);
      encode(path, cur);
      update(path, cur);

      if ((int)tail.size() == TRIE_DEPTH) tail = tail.substr(1);
      tail += cur;
      if (symbols % SCALING_FREQ == 0){
        scale_all_counters();
      }
    }
  }
  else if (mode == DECODER){
    int ZZZ;
    fscanf(infile, "%d", &ZZZ);
    for (int symbols=1; symbols <= ZZZ; symbols++){
      // if () break;
      vector<int> path = build_path(tail);
      char cur = decode(path);
      fprintf(outfile, "%c", cur);
      update(path, cur);
      
      if ((int)tail.size() == TRIE_DEPTH) tail = tail.substr(1);
      tail += cur;
      if (symbols % SCALING_FREQ == 0){
        scale_all_counters();
      }
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