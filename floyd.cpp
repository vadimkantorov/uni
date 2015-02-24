#include <cstdio>
#include <vector>
#include <algorithm>

using namespace std;

const int N = 100;
const int INF = 1e+9;

struct Graph
{
	struct Edge
	{
		int Vertex;
		int Weight;
	};
	
	vector<Edge> M[N];
	int Parent[N];
	long long Distance[N];

	void AddEdge(int u, int v, int w)
	{
		Edge e = {v, w};
		M[u].push_back(e);
	}

	void FindShortestPaths(int source, int n)
	{
		fill_n(Parent, N, -1);
		fill_n(Distance, N, INF);
		
		Distance[source] = 0;
		for(int i = 1; i < n; i++)
		{
			for(int u = 1; u <= n; u++)
			{
				for(int j = 0; j < M[u].size(); j++)
				{
					int v = M[u][j].Vertex;
					long long newd = long long(u == source ? 1 : Distance[u])*M[u][j].Weight;

					if(Distance[v] > newd)
					{
						Distance[v] = newd;
						Parent[v] = u;
					}
				}
			}
		}
	}
};

Graph g;

int main()
{
	freopen("in.txt","r",stdin);
	freopen("out.txt","w",stdout);

	int n,s,t;
	scanf("%d",&n);

	for(int i = 1; i <= n; i++)
	{
		int v, w;
		while(true)
		{
			scanf("%d",&v);

			if(v == 0)
				break;

			scanf("%d",&w);
			g.AddEdge(v, i, w);
		}
	}

	scanf("%d%d",&s,&t);

	g.FindShortestPaths(s,n);

	if(g.Parent[t] != -1)
	{
		puts("Y");

		vector<int> path;
		for(int cur = t; cur != -1; cur = g.Parent[cur])
			path.push_back(cur);
		
		for(int i = path.size()-1; i >= 0; i--)
			printf("%d ",path[i]);
		printf("\n%lld",g.Distance[t]);
	}
	else
	{
		puts("N");
	}
	
	return 0;
}