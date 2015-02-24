#pragma warning (disable:4996)
#include <map>
#include <cassert>
using namespace std;

#include "database.h"
#include "algorithm.h"
#include "time_utils.h"
#include "common_utils.h"
#include "svd.h"
#include "svd2.h"
#include "svd3.h"
#include "cluster.h"
#include "average.h"
#include "graph_cluster.h"
#include "knn.h"


class AverageAlgorithm : public Algorithm
{
	bool initialized;
	LargeAverage* largeAvg;
	//MidAverage* midAvg;
	KNN* midAvg;
	SmallAverage* smallAvg;
	ClusterEngine* cluster;

public:
    AverageAlgorithm(Database* db, ClusterEngine* cluster) : Algorithm() 
    {
		this->cluster = cluster;
		initialized = false;
		largeAvg = 0;
		midAvg = 0;
		smallAvg = 0;

		UPPER_LIMIT = 12000;
		LOWER_LIMIT = 500;
    }
    
    ~AverageAlgorithm() 
    {
		delete largeAvg;
		delete midAvg;
		delete smallAvg;
    }

	void initialize(Database* db)
	{
		if ( initialized ) return;

		initialized = true;
	}

	bool useSVD;
	bool useCluster;
	int LOWER_LIMIT;
	int UPPER_LIMIT;

	double predict(Database* db, int roadIndex, const Item& item)
	{
		double jc = (double)db->getJamCount(roadIndex);
		double A = 480;
		double B = 520;

		if ( midAvg == 0 )
		{
			midAvg = new KNN(db, LOWER_LIMIT, UPPER_LIMIT, cluster);
			midAvg->initialize(db);
		}

		if ( smallAvg == 0 )
		{
			smallAvg = new SmallAverage(db, cluster, LOWER_LIMIT);
			smallAvg->initialize(db);
		}

		if ( jc > B )
		{
			return midAvg->predict(db, roadIndex, item);
		}

		return 50.0;
		if ( jc < A )
		{
			return smallAvg->predict(db, roadIndex, item);
		}

		double mid = midAvg->predict(db, roadIndex, item);
		double small = smallAvg->predict(db, roadIndex, item);

		double K = (jc - A)/(B - A);
		double res = small*(1.0 - K) + mid*K;

		return res;
	}

};

int main()
{
	InitRand(123);
	InitTime("log");

	Database* database = new Database(-1);
	OutputTimeInfo("Roads: %d,   Jams: %d,   Days: %d", database->getRoadCount(), database->getJamCount(), database->getDayCount());

	//SVDCluster* svdCluster = new SVDCluster();
	//svdCluster->outputPredictions(database, database->getRoadIndex(320259));

	ClusterEngine* cluster = new ClusterEngine(database);
	//cluster->outputRoadMeanAndVariance(database);
	//cluster->noiseReduction(database);
	//cluster->outputDayDistances(database);
	//cluster->outputRoadValues2(database, database->getRoadIndex(567167));
	//cluster->outputRoadValues2(database, database->getRoadIndex(654062));
	//cluster->outputRoadValues2(database, database->getRoadIndex(920370));
	//cluster->outputRoadValues2(database, database->getRoadIndex(920373));
	//cluster->outputRoadValues2(database, database->getRoadIndex(906859));
	//cluster->outputRoadValues2(database, database->getRoadIndex(818716));
	//cluster->outputRoadValues2(database, database->getRoadIndex(465256));
	//cluster->outputRoadValues2(database, database->getRoadIndex(380369));
	//cluster->outputRoadNearest(database);
	//cluster->outputRoadMeanAndVariance(database);

	AverageAlgorithm* algo = new AverageAlgorithm(database, cluster);
	//algo->useSVD = false;
	//SVDAlgorithm* algo2 = new SVDAlgorithm(database, algo);
	//KNN* algo = new KNN(database, 500, 2200);
	//cluster->outputRoadPredictions(database, database->getRoadIndex(567167), algo);
	
	OutputTimeInfo("Result: %.5lf", database->score(algo, 0));


	delete database;
	delete algo;
	delete cluster;
	
	FinalizeTime();
	return 0;
}
