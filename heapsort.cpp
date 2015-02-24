#include <cstdio>
#include <cstdlib>
#include <ctime>
#include <algorithm>

using namespace std;

const int N = 100000;
int A[N+5], B[N+5], C[N+5];
int m1,m2;

int rand32()
{
	return (((rand() << 15) + rand()) << 2) + (rand() >> 13);
}

int RangedRand( int range_min, int range_max)
{
	return (double)rand() / (RAND_MAX + 1) * (range_max - range_min)
		+ range_min;
}


void GenData()
{
	//srand(time(0));
	for(int i = 0; i < N; i++)
		A[i] = RangedRand(0,N/4);
}

void TimeTest(void worker(), const char* str)
{
	clock_t start = clock();
	worker();
	clock_t finish = clock();
	double dur = (double)(finish - start) / CLOCKS_PER_SEC;

	printf("%s took %lf seconds to execute\n\n",str,dur);
}

int Left(int i)
{
	return 2*i;
}

int Right(int i)
{
	return 2*i+1;
}

void Heapify(int* m, int i, int n)
{
	int left = Left(i+1)-1;
	int right = Right(i+1)-1;

	if(left < n)
	{
		int minSon = left;
		if(right < n && m[right] < m[minSon])
			minSon = right;
		
		if(m[minSon] < m[i])
		{
			swap(m[i], m[minSon]);
			Heapify(m, minSon, n);
		}
	}
}

void BuildHeap(int* m, int n)
{
	for(int i = n/2; i >= 0; i--)
		Heapify(m,i,n);
}

int ExtractMin(int* m, int n)
{
	int tmp = m[0];
	m[0] = m[n-1];
	Heapify(m,0,n);
	return tmp;
}

void HeapSort(int* m, int n)
{
	BuildHeap(m,n);
	for(int i = 0; i < n; i++)
		C[i] = ExtractMin(m,n-i);
}

void Efficient()
{
	HeapSort(A, N);
	for(int i = 0; i < N; i++)
	{
		if(m2 >= 1)
		{
			if(C[i] != C[m2-1])
				C[m2++] = C[i];
		}
		else
			C[m2++] = C[i];
	}
}

void Inefficient()
{
	for(int i = 0; i < N; i++)
	{
		bool ok = true;
		
		for(int j = 0; ok && j < m1; j++)
			if(B[j] == A[i])
				ok = false;
		
		if(ok)
			B[m1++] = A[i];
	}
}

void main()
{
	GenData();
	TimeTest(Inefficient, "Stupid square algorithm");
	TimeTest(Efficient, "Smart heap algorithm");
	sort(B,B+m1);
	if(m1 != m2 || !equal(B,B+m1,C))
		puts("ERROR");
	else
	{
		puts("OK");
		freopen("output.txt", "w", stdout);
		for(int i = 0; i < m2; i++)
			printf("%d ",C[i]);
	}
}