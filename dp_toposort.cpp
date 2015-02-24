#include <cstdio>
#include <algorithm>
#include <map>
#include <set>
#include <vector>

using namespace std;

typedef vector<int> Vertex;

struct Graph
{
	map<Vertex, vector<Vertex>> M;
	set<Vertex> visited;

	void AddEdge(Vertex u, Vertex v)
	{
		M[u].push_back(v);
	}

	vector<Vertex> TopologicalSort()
	{
		vector<Vertex> res;
		
		for(map<Vertex, vector<Vertex>>::iterator i = M.begin(); i != M.end(); i++)
			if(visited.count(i->first) == 0)
				DFS(i->first, res);

		reverse(res.begin(), res.end());
		return res;
	}

	void DFS(const Vertex& v, vector<Vertex>& res)
	{
		visited.insert(v);

		vector<Vertex>& edges = M[v];
		for(int i = 0; i < edges.size(); i++)
		{
			if(visited.count(edges[i]) == 0)
				DFS(edges[i], res);
		}

		res.push_back(v);
	}
};

bool CanPutOn(Vertex v1, Vertex v2)
{
	return v2[0] < v1[0] && v2[1] < v1[1];
}

vector<Vertex> Rotate(Vertex& v)
{
	vector<Vertex> res;
	int p2[] = {0, 1, 2};
	while(next_permutation(p2, p2+ 3))
	{
		Vertex v2(3);
		for(int k = 0; k < 3; k++)
			v2[k] = v[p2[k]];
		res.push_back(v2);
	}
	return res;
}

int main()
{
	freopen("input.txt","r",stdin);
	freopen("output.txt","w",stdout);

	int n;
	for(int t = 1; scanf("%d", &n) > 0 && n > 0; t++)
	{
		Graph g;
		vector<Vertex> rotatedBlockTypes;
		for(int i = 0; i < n; i++)
		{
			Vertex v(3);
			for(int j = 0; j < 3; j++)
				scanf("%d", &v[j]);

			vector<Vertex> rotated = Rotate(v);
			copy(rotated.begin(), rotated.end(), back_inserter(rotatedBlockTypes));
		}

		for(int i = 0; i < rotatedBlockTypes.size(); i++)
		{
			for(int j = 0; j < rotatedBlockTypes.size(); j++)
			{
				if(CanPutOn(rotatedBlockTypes[i], rotatedBlockTypes[j]))
					g.AddEdge(rotatedBlockTypes[i], rotatedBlockTypes[j]);
			}
		}

		vector<Vertex> toptop = g.TopologicalSort();
		map<Vertex, int> dp;

		for(int i = 0; i < toptop.size(); i++)
			dp[toptop[i]] = toptop[i][2];

		int h = 0;
		for(int i = 0; i < toptop.size(); i++)
		{
			int& cur = dp[toptop[i]];
			vector<Vertex>& edges = g.M[toptop[i]];
			for(int j = 0; j < edges.size(); j++)
			{
				int& u = dp[edges[j]];
				u = max(u, cur + edges[j][2]);
			}
			h = max(cur, h);
		}
		
		printf("Case %d: maximum height = %d\n",t,h);
	}
	
	return 0;
}