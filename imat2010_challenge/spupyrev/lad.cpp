#pragma warning(disable:4996)
#pragma warning(disable:4267)
#pragma warning(disable:4244)

#include <boost/numeric/ublas/matrix.hpp>
#include <boost/numeric/ublas/io.hpp>

#include <boost/numeric/ublas/vector.hpp>
#include <boost/numeric/ublas/vector_proxy.hpp>
#include <boost/numeric/ublas/matrix.hpp>
#include <boost/numeric/ublas/triangular.hpp>
#include <boost/numeric/ublas/lu.hpp>
#include <boost/numeric/ublas/io.hpp>

#include <cstdio>
#include <cmath>
#include <vector>
#include <map>

using namespace std;

const char* ANSWER_FILE = "generated_answer.txt";
const char* VALUES_FILE = "generated_values.txt";
bool generateAnswer = true;

typedef vector<double> VD;
typedef vector<VD> VVD;

struct Item
{
	int id;
	int day;
	int hh;
	int mm;

	inline double getOrderK() const
	{
		int tt = (hh - 18)*15 + (mm - 2)/4;
		double kt = 1.0 + 0.1*double(tt);
		return kt;
	}
};

vector<Item> items;
map<int, double> edgeLen;

void ReadEdgeData()
{
	FILE* f = fopen("E:\\C++Projects\\yandexmath\\data_init\\edge_data.txt", "r");
	int id;
	double len, speed;

	while ( fscanf(f, "%d %lf %lf", &id, &len, &speed) == 3 )
	{
		edgeLen[id] = len;
	}
	
	fclose(f);
}

double Error(const VVD& data, const VD& arr)
{
	double res = 0;
	for (int j = 0; j < (int)arr.size(); j++)
		res += abs(data[0][j] - arr[j]);

	res /= (double)arr.size();

	return res;
}

void Read(const char* fileName, VVD& data)
{
	FILE* f = fopen(fileName, "r");
	vector<double> tmp;
	tmp.reserve(1024);
	bool firstItems = (items.size() == 0);

	for (int i=0;;i++)
	{
		double t;
		Item item;
		if ( fscanf(f, "%d %d %d:%d %lf", &item.id, &item.day, &item.hh, &item.mm, &t) != 5 ) break;

		//assert(edgeLen[item.id] != edgeLen.end());
		t *= item.getOrderK()*edgeLen[item.id]/120.0;

		tmp.push_back(t);

		if ( firstItems )
			items.push_back(item);
		else
			assert(items[i].id == item.id);
	}
	fclose(f);

	data.push_back(tmp);
	cerr<<"Read "<<tmp.size()<<" elements from "<<fileName<<". ";
	cerr<<"Error: "<<Error(data, data.back())<<"\n";
}

VVD ReadData()
{
	ReadEdgeData();

	VVD data;
	Read("data36/ans-36.txt", data);
	Read("data36/64.061-36.txt", data);
	Read("data36/64.334-36.txt", data);
	Read("data36/64.406-36.txt", data);
	Read("data36/average-36.txt", data);
	Read("data36/givenaverage-36.txt", data);
	Read("data36/offset-36.txt", data);
	Read("data36/cluster-36.txt", data);
	Read("data36/cluster2-36.txt", data);
	Read("data36/cluster3-36.txt", data);
	//Read("data36/svd-36.txt", data);

	return data;
}

VD Combine(const VVD& data, const VD& k)
{
	assert((int)data.size() == (int)k.size() + 1);

	VD res(data[0].size(), 0);
	for (int i = 0; i < (int)data[0].size(); i++)
	{
		double t = 0;
		for (int j = 1; j < (int)data.size(); j++)
			t += data[j][i]*k[j-1];

		res[i] = t;
	}

	return res;
}

/* Matrix inversion routine.
Uses lu_factorize and lu_substitute in uBLAS to invert a matrix */
template<class T>
bool invert(const boost::numeric::ublas::matrix<T>& input, boost::numeric::ublas::matrix<T>& inverse) 
{
	using namespace boost::numeric::ublas;
	typedef permutation_matrix<std::size_t> pmatrix;
	// create a working copy of the input
	matrix<T> A(input);
	// create a permutation matrix for the LU-factorization
	pmatrix pm(A.size1());

	// perform LU-factorization
	int res = lu_factorize(A, pm);
	if( res != 0 ) return false;

	// create identity matrix of "inverse"
	inverse.assign(boost::numeric::ublas::identity_matrix<T>(A.size1()));

	// backsubstitute to get the inverse
	lu_substitute(A, pm, inverse);

	return true;
}

vector<double> SolveLSD(const VVD& data)
{
	int n = (int)data[0].size(), m = (int)data.size() - 1;	
	using namespace boost::numeric::ublas;

	matrix<double> Y(n, 1);
	matrix<double> X(n, m);
	matrix<double> XT(m, n);

	for (int i=0;i<n;i++)
		Y(i, 0) = data[0][i];

	for (int i=0;i<n;i++)
		for (int j=0;j<m;j++)
			X(i, j) = data[j+1][i];

	for (int i=0;i<n;i++)
		for (int j=0;j<m;j++)
			XT(j, i) = data[j+1][i];

	matrix<double> XTX = prod(XT, X);
	matrix<double> XTY = prod(XT, Y);
	matrix<double> XTX1(m, m);

	if ( !invert(XTX, XTX1) )
		printf("Error during inverting matrix!\n");

	matrix<double> resM = prod(XTX1, XTY);

	std::vector<double> result(m, 0.0);
	for (int i=0;i<m;i++)
		result[i] = resM(i, 0);

	return result;
}

VD SolveWLSD(const VVD& data, const VD& w)
{
	//assert(false);
	assert((int)data[0].size() == (int)w.size());

	VD ww = w;
	for (int i = 0; i < (int)w.size(); i++)
	{
		assert(ww[i] >= 0.0);
		ww[i] = sqrt(ww[i]);
	}

	VVD data_new = data;
	for (int i = 0; i < (int)data.size(); i++)
	{
		for (int j = 0; j < (int)data[i].size(); j++)
			data_new[i][j] *= ww[j];
	}

	return SolveLSD(data_new);
}

VD Residuals(const VVD& data, const VD& k)
{
	VD R;
	VD tmp = Combine(data, k);
	for (int i = 0; i < (int)data[0].size(); i++)
	{
		double tr = data[0][i] - tmp[i];
		R.push_back(tr);
	}

	return R;
}

VD SolveLAD(VVD& data)
{
	VD w = VD(data[0].size(), 1.0);

	VD bestK;
	double bestRes = 1e6;
	int MAX_ITERATIONS = 50;

	double diff = 1e6;
	for (int iter = 0; iter < MAX_ITERATIONS && diff > 1e-5; iter++)
	{
		VD k = SolveWLSD(data, w);
		VD R = Residuals(data, k);

		assert(w.size() == R.size());

		printf("Weights on iter %d:", iter);
		for(int i = 0; i < w.size(); i++)
			printf("%f ", w[i]);
		puts("");
		
		for (int i = 0; i < (int)w.size(); i++)
		{
			if ( abs(R[i]) < 1e-5 ) w[i] = 0;
			else w[i] = 1.0/abs(R[i]);
		}

		double err = Error(data, Combine(data, k));
		if ( iter%10 == 0 || iter + 5 >= MAX_ITERATIONS )
			cerr<<"Error after step "<<iter+1<<": "<<err<<"\n";

		if ( bestRes > err )
		{
			bestRes = err;
			bestK = k;
		}
	}

	return bestK;
}

void OutputValues(const VD& k, const VD& res)
{
	FILE* f = fopen(VALUES_FILE, "w");
	for (int i = 0; i < (int)k.size(); i++)
		fprintf(f, "%lf\n", k[i]);
	fclose(f);

	assert(res.size() == items.size());
	f = fopen(ANSWER_FILE, "w");
	for (int i = 0; i < (int)res.size(); i++)
	{
		double speed = res[i] / (items[i].getOrderK()*edgeLen[items[i].id]/120.0);
		fprintf(f, "%d\t%d %d:%02d\t%.5lf\n", items[i].id, items[i].day, items[i].hh, items[i].mm, speed);
	}
	fclose(f);
}

int main()
{
	freopen("H:\\Projects\\SlowMotion\\NewLAD\\test.txt","r",stdin);
	freopen("H:\\Projects\\SlowMotion\\NewLAD\\SergeyLAD.txt","w",stdout);

	VVD data(3);
	for(int i = 0; i < 3; i++)
		data[i] = VD(4);
	
	for(int i = 0; i < 4; i++)
	{
		for(int j = 0; j < 3; j++)
			scanf("%lf",&data[2-j][i]);
	}

	vector<double> k = SolveLAD(data);
	
	/*VVD data = ReadData();
	vector<double> k = SolveLAD(data);
	VD res = Combine(data, k);
	printf("Final result: %.5lf\n", Error(data, res));

	OutputValues(k, res);*/

	return 0;
}
