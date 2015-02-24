#include <cstdio>
#include <algorithm>
#include <vector>
using namespace std;

const int N = 1000;

int n, k;
vector<int> g[N];
int mt[N];
bool used[N];

bool kuhn (int v) {
	if (used[v])  return false;
	used[v] = true;
	for (size_t i=0; i<g[v].size(); ++i) {
		int to = g[v][i];
		if (mt[to] == -1 || kuhn (mt[to])) {
			mt[to] = v;
			return true;
		}
	}
	return false;
}

int main()
{
	freopen("input.txt","r",stdin);
	freopen("output.txt","w",stdout);

	scanf("%d",&k);

	for(int i = 0; i < k; i++)
	{
		int x;
		while(scanf("%d",&x) > 0 && x != 0)
		{
			g[i].push_back(k + x);
			g[k+x].push_back(i);
		}
	}

	fill_n(mt, k, -1);
	for (int i=k; i<N; ++i) {
		if(!g[i].empty())
		{
			fill_n(used, N, false);
			kuhn (i);
		}
	}

	if(count(mt, mt+k, -1) > 0)
		puts("N");
	else
	{
		puts("Y");
		for (int i=0; i<k; ++i)
			printf ("%d ", mt[i]-k);
	}
	
	return 0;
}