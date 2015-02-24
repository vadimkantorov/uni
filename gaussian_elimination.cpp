#include <valarray>
#include <algorithm>
#include <cstdio>
using namespace std;

int n,m;
valarray<double> M[100];

const double EPS = 1e-5;
bool NEQ(double x, double y)
{
	return abs(x-y) >= EPS;
}

bool GT(double x, double y)
{
	return x > y + EPS;
}

void InitMatrix()
{
	m = n+1;
	for(int i = 0; i < n; i++)
	{
		M[i].resize(m);
		
		for(int j = 1; j < m; j++)
			scanf("%lf",&M[i][j]);
		
		scanf("%lf",&M[i][0]);

		M[i] /= M[i].max();
	}
}

void PrintMatrix()
{
	for(int i = 0; i < n; i++)
	{
		for(int j = 1; j < m; j++)
			printf("%lf\t",M[i][j]);
		printf("| %lf\n",M[i][0]);
	}
	puts("");
}

void PrintAnswer()
{
	for(int i = 0; i < n; i++)
		printf("X%d = %lf\n",i+1,M[i][0]);
}

bool FirstNonZeroColumn(int& col, int& row)
{
	for(; col < m; col++)
	{
		int bestRow = -1;
		for(int j = row; j < n; j++)
		{
			if(NEQ(M[j][col],0))
			{
				if(bestRow == -1 || GT(M[j][col],M[bestRow][col]))
					bestRow = j;
			}
		}
		if(bestRow != -1)
		{
			row = bestRow;
			return true;
		}
	}
	col = -1;
	row = -1;
	return false;
}

bool HasSolutions()
{
	for(int i = 0; i < n; i++)
	{
		bool zero = true;
		for(int j = 1; j < m; j++)
		{
			if(NEQ(M[i][j],0))
			{
				zero = false;
				break;
			}
		}
		if(zero && NEQ(M[i][0],0))
			return false;
	}
	return true;
}

bool HasOneSolution()
{
	for(int i = 0; i < n; i++)
		for(int j = 1; j < m; j++)
		{
			if(j == i+1 && NEQ(M[i][j],1))
				return false;
			if(j != i+1 && NEQ(M[i][j],0))
				return false;
		}
	return true;
}

void main()
{
	freopen("input.txt","r",stdin);
	
	scanf("%d",&n);
	InitMatrix();
	PrintMatrix();

	int col = 1, row = 0;
	while(true)
	{
		int nonZeroRow = row;
		if(!FirstNonZeroColumn(col, nonZeroRow))
			break;

		swap(M[row],M[nonZeroRow]);

		M[row] /= M[row][col];
		for(int i = row+1; i < n; i++)
			M[i] -= (M[row]*M[i][col]);
		
		col++;
		row++;
	}

	if(HasSolutions())
	{
		for(int i = n-1; i >= 0; i--)
			for(int j = i-1; j >= 0; j--)
				M[j] -= M[i]*M[j][1+i];

		PrintMatrix();
		if(HasOneSolution())
			PrintAnswer();
		else
			puts("Infinite number of solutiions");
	}
	else
		puts("No solution");
}