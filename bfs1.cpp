#pragma comment(linker, "/STACK:16000000");
#include <cstdio>
#include <vector>
#include <queue>
using namespace std;

struct Graph
{
	static const int N = 2000;
	
	vector<int> M[N];
	int color[N];
	
	void AddEdge(int u, int v)
	{
		M[u].push_back(v);
	}

	vector<vector<int>> DivideIntoTwoParts(int n)
	{
		vector<vector<int>> res;
		
		if(BFS(1))
		{
			res.resize(2,vector<int>());
			for(int i = 1; i <= n; i++)
				res[color[i]-1].push_back(i);
		}

		return res;
	}

	bool BFS(int start)
	{
		queue<int> Q;
		Q.push(start);
		color[start] = 1;

		while(!Q.empty())
		{
			int u = Q.front();
			Q.pop();

			for(int i = 0; i < M[u].size(); i++)
			{
				int v = M[u][i];

				if(color[v] == 0)
				{
					color[v] = 3-color[u];
					Q.push(v);
				}
				else if(color[v] != 3-color[u])
					return false;
			}
		}

		return true;
	}
};

Graph g;

int main()
{
	freopen("in.txt","r",stdin);
	freopen("out.txt","w",stdout);

	int n;
	scanf("%d",&n);

	for(int i = 1; i <= n; i++)
	{
		while(true)
		{
			int v;
			scanf("%d", &v);
			if(v == 0)
				break;
			g.AddEdge(i,v);
		}
	}

	vector<vector<int>> res = g.DivideIntoTwoParts(n);
	if(res.empty())
		puts("N");
	else
	{
		puts("Y");
		for(int k = 0; k < 2; k++)
		{
			for(int i = 0; i < res[k].size(); i++)
				printf("%d ",res[k][i]);
			if(k == 0)
				puts("0");
		}
	}
	
	return 0;
}