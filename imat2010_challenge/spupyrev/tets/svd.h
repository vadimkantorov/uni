#pragma once
#ifndef SVD_H
#define SVD_H

#include <vector>
#include <algorithm>
#include <cmath>
using namespace std;

#include "algorithm.h"
#include "database.h"
#include "defs.h"
#include "random_utils.h"

class SVDAlgorithm : public Algorithm
{
	static const int MAX_FEATURES = 40;
	static const int MAX_EPOCH = 1000;
	static const int TIME_INTERVAL = 10;
	static const int INTERVALS_PER_DAY = (60*6)/TIME_INTERVAL;

	double** roadFeature;
	double** timeFeature;
	double** cache;
	double** cacheTail;
	int roadCount;
	int timeCount;

	double lrate;
	double K;
	double SCALE;
	double INIT_VALUE;
	bool initialized;
	Algorithm* avgAlgorithm;

public:
    SVDAlgorithm(Database* db, Algorithm* avgAlgorithm) : Algorithm() 
    {
		this->avgAlgorithm = avgAlgorithm;
		roadCount = db->getRoadCount() * INTERVALS_PER_DAY;
		timeCount = db->getDayCount();

		initialized = false;
		initializeFeatures(db);
    }

	void initializeFeatures(Database* db)
	{
		SCALE = 100;
		lrate = 0.001;
		K = 0.015;
		INIT_VALUE = 0.1;

		roadFeature = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
		{
			roadFeature[i] = new double[MAX_FEATURES];
			for (int j = 0; j < MAX_FEATURES; j++)
				roadFeature[i][j] = randDouble(-INIT_VALUE, INIT_VALUE);
		}

		timeFeature = new double*[timeCount];
		for (int i = 0; i < timeCount; i++)
		{
			timeFeature[i] = new double[MAX_FEATURES];
			for (int j = 0; j < MAX_FEATURES; j++)
				timeFeature[i][j] = randDouble(-INIT_VALUE, INIT_VALUE);
		}

		cache = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
			cache[i] = new double[timeCount];

		cacheTail = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
			cacheTail[i] = new double[timeCount];
	}
    
    ~SVDAlgorithm() 
    {
		for (int i = 0; i < roadCount; i++)
		{
			delete[] roadFeature[i];
			delete[] cache[i];
			delete[] cacheTail[i];
		}
		for (int i = 0; i < timeCount; i++)
			delete[] timeFeature[i];

		delete[] roadFeature;
		delete[] timeFeature;
		delete[] cache;
		delete[] cacheTail;
    }

	void initialize(Database* db)
	{
		if ( initialized ) return;
		initialized = true;
		
		teach(db);
	}

	inline int roadIndex(int roadI, const Item* item)
	{
		int t = (item->getH() - 16)*(60/TIME_INTERVAL) + item->getM()/TIME_INTERVAL;
		int ret = roadI*INTERVALS_PER_DAY + t;
		return ret;
	}

	inline int timeIndex(const Item* item)
	{
		return item->getDay() - 11;
	}

	double buildSpeed(Database* db, int roadIdx, int jamIdx, Item* item)
	{
		double speed = db->getSpeed(roadIdx, jamIdx);
		//speed = Item::turnSpeed(speed, db->getDay(roadIdx, jamIdx), true);
		//speed = (speed - avgAlgorithm->predict(db, roadIdx, *item))/SCALE;
		speed = (speed - 57.0)/SCALE;
		return speed;
	}

	double rebuildSpeed(Database* db, int ri, int ti, const Item& item)
	{
		//int roadIdx = ri/(INTERVALS_PER_DAY);
		//int day = ti + 11;

		double speed = predict(ri, ti, MAX_FEATURES - 1, false, false) * SCALE;
		//speed += avgAlgorithm->predict(db, roadIdx, item);
		speed += 57.0;
		//speed = Item::turnSpeed(speed, day, false);
		return speed;
	}

	bool needTeach(Database* db, int roadIdx, int jamIdx)
	{
		//return true;
		if ( db->getH(roadIdx, jamIdx) != 21 ) return false;
		return ( (db->getDay(roadIdx, jamIdx) + 1)%7 < 5 );
	}

	void teach(Database* db)
	{
		OutputTimeInfo("SVD of matrix %d*%d", roadCount, timeCount);

		for (int f = 0; f < MAX_FEATURES; f++)
		{
			//fill cacheTail
			for (int i = 0; i < roadCount; i++)
				for (int j = 0; j < timeCount; j++)
				{
					if ( f == 0 )
					{
						cacheTail[i][j] = 0;
						for (int f1 = f+1; f1 < MAX_FEATURES; f1++)
							cacheTail[i][j] += roadFeature[i][f1]*timeFeature[j][f1];
					}
					else
					{
						cacheTail[i][j] -= roadFeature[i][f]*timeFeature[j][f];
					}
				}

			for (int e = 0; e < MAX_EPOCH; e++)
			{
				if ( e == 0 )
				{
					double rmse = RMSE(db);
					OutputTimeInfo("Teaching %d(%d)-th epoch, %d(%d) feature (LAD=%.5lf  RMSE=%.5lf)...", e+1, MAX_EPOCH, f+1, MAX_FEATURES, LAD(db), rmse);
					OutputTimeInfo("Partitial result: %.5lf", db->scorePart(this, -1));

					//debugOutputFeatures();
				}

				for (int i = 0; i < db->getRoadCount(); i++)
				{
					for (int j = 0; j < db->getJamCount(i); j++)
					{
						if ( !needTeach(db, i, j) ) continue;

						int ri = roadIndex(i, db->getJam(i, j));
						int ti = timeIndex(db->getJam(i, j));
						double speed = buildSpeed(db, i, j, db->getJam(i, j));

						int kLimit = 10;
						//if ( j == db->getSkippedDay()-11 ) kLimit = 4000;

						for (int k = 0; k < kLimit; k++)
							teach(f, ri, ti, speed);
					}
				}

			}
			for (int i = 0; i < roadCount; i++)
				for (int j = 0; j < timeCount; j++)
					cache[i][j] = predict(i, j, f, true, false);
		}
	}

	void debugOutputFeatures()
	{
		OutputInfo("\n");
		/*for (int i = 0; i < 1; i++)
		{
			for (int j = 0; j < MAX_FEATURES; j++)
				OutputInfo("%.3lf ", roadFeature[i][j]);
			OutputInfo("\n");
		}  */

		OutputInfo("\n");
		for (int i = 0; i < 1; i++)
		{
			for (int j = 0; j < MAX_FEATURES; j++)
				OutputInfo("%.3lf ", timeFeature[i][j]);
			OutputInfo("\n");
		}  
	}

	/*void debugOutputMatrix(Database* db)
	{
		OutputInfo("\n");
		for (int i = 0; i < roadCount; i++)
		{
			for (int j = 0; j < timeCount; j++)
			{
				double speed = rebuildSpeed(db, i, j);
				OutputInfo("%.3lf ", speed);
			}
			OutputInfo("\n");
		}
	}  */

	void teach(int f, int ri, int ti, double speed)
	{
		double pr = predict(ri, ti, f, true, true);
		assert( pr == pr );
		double error = (speed - pr);
		if ( Abs(error) < 1e-8 ) return;

		double tmpRoad = roadFeature[ri][f];
		double tmpTime = timeFeature[ti][f];

		roadFeature[ri][f] += lrate * (error*tmpTime - K*tmpRoad);
		timeFeature[ti][f] += lrate * (error*tmpRoad - K*tmpTime);
	}

	double predict(int ri, int ti, int f, bool useCache, bool withTrailing)
	{
		assert(ri != -1);
		double res = roadFeature[ri][f] * timeFeature[ti][f];
		if ( !useCache )
		{
			for (int i = 0; i < f; i++)
			{
				res += roadFeature[ri][i] * timeFeature[ti][i];
			}
		}
		else
		{
			if ( f > 0 )
			{
				res += cache[ri][ti];
			}
		}

		if ( withTrailing )
			res += cacheTail[ri][ti];

		return res;
	}

	double LAD(Database* db)
	{
		double res = 0;
		for (int i = 0; i < db->getRoadCount(); i++)
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				int ri = roadIndex(i, db->getJam(i, j));
				int ti = timeIndex(db->getJam(i, j));
				double speed = buildSpeed(db, i, j, db->getJam(i, j));
				double ans = predict(ri, ti, MAX_FEATURES-1, false, false);

				res += Abs(ans - speed);
			}

		res /= (double)db->getJamCount();

		return res;
	}  

	double RMSE(Database* db)
	{
		double res = 0;
		for (int i = 0; i < db->getRoadCount(); i++)
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				int ri = roadIndex(i, db->getJam(i, j));
				int ti = timeIndex(db->getJam(i, j));
				double speed = buildSpeed(db, i, j, db->getJam(i, j));
				double ans = predict(ri, ti, MAX_FEATURES-1, false, false);

				res += Sqr2(ans - speed);
			}

		res /= (double)db->getJamCount();

		return res;
	}

	double predict(Database* db, int roadIdx, const Item& item)
	{
		assert(roadIdx != -1);

		int ri = roadIndex(roadIdx, &item);
		int ti = timeIndex(&item);
		
		assert(ri != -1);
		assert(ti != -1);
			
		double res = rebuildSpeed(db, ri, ti, item);
		if ( res < 1e-6 ) res = 0.0;
		return res;
	}
};

#endif
