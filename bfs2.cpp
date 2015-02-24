#include <iostream>
#include <vector>
#include <stack>
#include <queue>
using namespace std;

struct Graph
{
	static const int N = 100;
	
	vector<int> M[N];
	bool visited[N];
	int parent[N];
	int source;

	void AddEdge(int u, int v)
	{
		M[u].push_back(v);
		M[v].push_back(u);
	}

	void BFS(int start)
	{
		source = start;

		queue<int> Q;
		Q.push(start);
		visited[start] = true;

		while(!Q.empty())
		{
			int u = Q.front();
			Q.pop();

			for(int i = 0; i < M[u].size(); i++)
			{
				int v = M[u][i];
				if(!visited[v])
				{
					visited[v] = true;
					parent[v] = u;
					Q.push(v);
				}
			}
		}
	}

	void PrintShortestPath(int to)
	{
		stack<int> s;

		for(int v = to; v != source; v = parent[v])
			s.push(v);
		s.push(source);
		
		while(!s.empty())
		{
			int v = s.top();
			s.pop();

			cout << v << endl;
		}
	}
};

Graph g;

void main()
{
	freopen("input.txt","r",stdin);
	
	int from, to, n, m;
	cin >> from >> to;
	cin >> n >> m;
	for(int i = 0; i < m; i++)
	{
		int u,v;
		cin >> u >> v;
		g.AddEdge(u,v);
	}

	g.BFS(from);
	g.PrintShortestPath(to);
}