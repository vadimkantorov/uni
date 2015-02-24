#pragma comment(linker, "/STACK:16000000");
#include <cstdio>
#include <stack>
#include <utility>

using namespace std;

const int N = 1000;

int n,m;
bool M[N][N];
pair<int,int> P[N][N];

int dx[] = {0,  0, -1, 1};
int dy[] = {-1, 1,  0, 0};

bool check(int i, int j)
{
	return i >= 1 && j >= 1 && i <= n && j <= m;
}

void DFS(pair<int, int> start)
{
	for(int i = 0; i < 4; i++)
	{
		int nx = start.first + dy[i];
		int ny = start.second + dx[i];

		if(check(nx, ny) && !M[nx][ny] && P[nx][ny].first == 0)
		{
			P[nx][ny] = start;

			DFS(make_pair(nx,ny));
		}
	}
}

int main()
{
	freopen("in.txt","r",stdin);
	freopen("out.txt","w",stdout);

	scanf("%d%d",&n,&m);

	for(int i = 1; i <= n; i++)
		for(int j = 1; j <= m; j++)
			scanf("%d", &M[i][j]);

	pair<int, int> from, to;
	scanf("%d%d%d%d",&from.first,&from.second,&to.first,&to.second);

	P[from.first][from.second] = make_pair(-1,-1);
	DFS(from);

	if(P[to.first][to.second].first == 0)
		puts("N");
	else
	{
		puts("Y");
		
		stack<pair<int, int>> res;
		while(P[to.first][to.second].first != -1)
		{
			res.push(to);
			to = P[to.first][to.second];
		}
		res.push(from);

		while(!res.empty())
		{
			pair<int, int> v = res.top();
			printf("%d %d\n",v.first, v.second);
			res.pop();
		}
	}
	
	return 0;
}