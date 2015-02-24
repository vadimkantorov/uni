#pragma once
#ifndef SVD2_H
#define SVD2_H

#include <vector>
#include <algorithm>
#include <cmath>
using namespace std;

#include "algorithm.h"
#include "database.h"
#include "defs.h"
#include "random_utils.h"
#include "input_output.h"

class SVD2Algorithm : public Algorithm
{
	static const int MAX_FEATURES = 10;
	static const int MAX_EPOCH = 1000;
	static const int TIME_INTERVAL = 30;
	static const int INTERVALS_PER_DAY = (60*6)/TIME_INTERVAL;

	int ROAD_IDX;
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
    SVD2Algorithm(Database* db, int ROAD_IDX, Algorithm* avgAlgorithm) : Algorithm() 
    {
		this->ROAD_IDX = ROAD_IDX;
		this->avgAlgorithm = avgAlgorithm;

		roadCount = INTERVALS_PER_DAY;
		initTimeIndexes(db);

		initialized = false;
		initializeFeatures(db);
    }

	void initializeFeatures(Database* db)
	{
		roadFeature = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
		{
			roadFeature[i] = new double[MAX_FEATURES];
			for (int j = 0; j < MAX_FEATURES; j++)
				roadFeature[i][j] = 0;
		}

		timeFeature = new double*[timeCount];
		for (int i = 0; i < timeCount; i++)
		{
			timeFeature[i] = new double[MAX_FEATURES];
			for (int j = 0; j < MAX_FEATURES; j++)
				timeFeature[i][j] = 0;
		}

		cache = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
			cache[i] = new double[timeCount];

		cacheTail = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
		{
			cacheTail[i] = new double[timeCount];
			for (int j = 0; j < timeCount; j++)
				cacheTail[i][j] = 0;
		}

		values = new double*[roadCount];
		for (int i = 0; i < roadCount; i++)
			values[i] = new double[timeCount];
		valuesSet = new bool*[roadCount];
		for (int i = 0; i < roadCount; i++)
			valuesSet[i] = new bool[timeCount];
	}
    
    ~SVD2Algorithm() 
    {
		for (int i = 0; i < roadCount; i++)
		{
			delete[] roadFeature[i];
			delete[] cache[i];
			delete[] cacheTail[i];
			delete[] values[i];
			delete[] valuesSet[i];
		}
		for (int i = 0; i < timeCount; i++)
			delete[] timeFeature[i];

		delete[] roadFeature;
		delete[] timeFeature;
		delete[] cache;
		delete[] cacheTail;
		delete[] values;
		delete[] valuesSet;
    }

	void initialize(Database* db)
	{
		if ( initialized ) return;
		initialized = true;

		SCALE = 100;
		lrate = 0.001;
		K = 0.01;
		INIT_VALUE = 0.1;

		buildValues(db);

		buildFeatures(db);
		teach(db);
	}

	void initTimeIndexes(Database* db)
	{
		for (int i = 0; i < 100; i++)
			dayIndex[i] = revDayIndex[i] = -1;
		timeCount = 0;

		for (int j = 0; j < db->getJamCount(ROAD_IDX); j++)
		{
			if ( !needTeach(db, j) ) continue;

			int dd = (db->getJam(ROAD_IDX, j)->getDay() - 11);
			if ( dayIndex[dd] == -1 )
			{
				dayIndex[dd] = timeCount++;
				revDayIndex[timeCount - 1] = dd + 11;
			}
		}
	}

	int dayIndex[100];
	int revDayIndex[100];
	double** values;
	bool** valuesSet;
	bool hasTeachData;

	void buildFeatures(Database* db)
	{
		hasTeachData = true;
		double score = db->scorePart(this, ROAD_IDX);
		if ( !hasTeachData ) return;

		OutputTimeInfo("Initial result: %.5lf (RMSE=%.6lf)", score, RMSE(db));

		for (int i = 0; i < roadCount; i++)
			for (int j = 0; j < MAX_FEATURES; j++)
				roadFeature[i][j] = randDouble(-INIT_VALUE, INIT_VALUE);

		for (int i = 0; i < timeCount; i++)
			for (int j = 0; j < MAX_FEATURES; j++)
				timeFeature[i][j] = randDouble(-INIT_VALUE, INIT_VALUE);
	}

	void buildValues(Database* db)
	{
		vector<vector<VD> > data = vector<vector<VD> >(roadCount, vector<VD>(timeCount, VD()));

		for (int j = 0; j < db->getJamCount(ROAD_IDX); j++)
		{
			if ( !needTeach(db, j) ) continue;

			int ri = roadIndex(db->getJam(ROAD_IDX, j));
			int ti = timeIndex(db->getJam(ROAD_IDX, j));
			double speed = buildSpeed(db, j, db->getJam(ROAD_IDX, j));
			data[ri][ti].push_back(speed);
		}

		int totalElements = (timeCount - 1)*roadCount + (2*60)/TIME_INTERVAL;
		int filledElements = 0;
		for (int i = 0; i < roadCount; i++)
		{
			for (int j = 0; j < timeCount; j++)
			{
				if ( data[i][j].empty() )
				{
					values[i][j] = -1;
					valuesSet[i][j] = false;
				}
				else
				{
					//values[i][j] = AverageValue(data[i][j]);
					values[i][j] = MedianValue(data[i][j]);
					valuesSet[i][j] = true;
					filledElements++;
				}
			}
		}

		OutputTimeInfo("Teaching %d road (filled = %d, missed = %d elements, jams = %d)", db->getRoadId(ROAD_IDX), filledElements, totalElements - filledElements, db->getJamCount(ROAD_IDX));
	}

	void outputData(Database* db)
	{
		int skipDay = 34;
		//FILE* f = OpenFile("w", "gen_data/%d_%d_%d", db->getRoadId(ROAD_IDX), TIME_INTERVAL, skipDay);
		FILE* f2 = OpenFile("w", "gen_data/%d_%d_%d_ans", db->getRoadId(ROAD_IDX), TIME_INTERVAL, skipDay);

		for (int j = 0; j < timeCount; j++)
			for (int i = 0; i < roadCount; i++)
			{
				int day = revDayIndex[j];
				int hh = i/(60/TIME_INTERVAL) + 16;
				int mm = i%((60/TIME_INTERVAL)) * TIME_INTERVAL;

				if ( day == skipDay && hh >= 18)
				{
					//assert(valuesSet[i][j]==false);
					fprintf(f2, "%d %d %d:%02d %.3lf\n", db->getRoadId(ROAD_IDX), day, hh, mm, values[i][j]*SCALE);
					continue;
				}

				//fprintf(f, "%d %d %d:%02d %.3lf\n", db->getRoadId(ROAD_IDX), day, hh, mm, values[i][j]*SCALE);
			}
		//fclose(f);
		fclose(f2);
	}

	inline int roadIndex(const Item* item)
	{
		int t = (item->getH() - 16)*(60/TIME_INTERVAL) + item->getM()/TIME_INTERVAL;
		return t;
	}

	inline int timeIndex(const Item* item)
	{
		return dayIndex[item->getDay() - 11];
	}

	double buildSpeed(Database* db, int jamIdx, Item* item)
	{
		double speed = db->getSpeed(ROAD_IDX, jamIdx);
		//speed = Item::turnSpeed(speed, db->getDay(ROAD_IDX, jamIdx), true);
		speed = (speed - avgAlgorithm->predictRaw(db, ROAD_IDX, *item))/SCALE;
		//speed = (speed - 57.0)/SCALE;
		return speed;
	}

	double rebuildSpeed(Database* db, int ri, int ti, const Item& item)
	{
		double speed = 0;
		if ( ti != -1)
			speed = predict(ri, ti, MAX_FEATURES - 1, false, false) * SCALE;
		else
			hasTeachData = false;
		speed += avgAlgorithm->predictRaw(db, ROAD_IDX, item);
		//speed += 57.0;
		//speed = Item::turnSpeed(speed, item.getDay(), false);
		return speed;
	}

	bool needTeach(Database* db, int jamIdx)
	{
		//return true;
		//if ( db->getH(ROAD_IDX, jamIdx) > 18 ) return false;
		return ( (db->getDay(ROAD_IDX, jamIdx) + 1)%7 < 5 );
	}

	void teach(Database* db)
	{
		if ( !hasTeachData ) return;
		//if ( calcVariance(db->getSkippedDay()) <= 0.1 ) return;
		//OutputTimeInfo("SVD of matrix %d*%d", roadCount, timeCount);

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

			if ( f%10 == 0 )
			{
				//OutputTimeInfo("Teaching %d(%d) feature (RMSE=%.6lf)...", f+1, MAX_FEATURES, RMSE(db));
				//OutputTimeInfo("Partitial result: %.5lf", db->scorePart(this, ROAD_IDX));

				//debugOutputMatrix(db);
			}  
			for (int e = 0; e < MAX_EPOCH; e++)
			{
				//vector<int> pi = randPermutation(roadCount);
				for (int ii = 0; ii < roadCount; ii++)
				{
					int i = ii;//pi[ii];
					//vector<int> pj = randPermutation(timeCount);
					for (int jj = 0; jj < timeCount; jj++)
					{
						int j = jj;//pj[jj];
						if ( !valuesSet[i][j] ) continue;

						int kLimit = 10;
						//if ( j == db->getSkippedDay()-11 ) kLimit = 4000;

						for (int k = 0; k < kLimit; k++)
							teach(f, i, j, values[i][j]);
					}
				}
			}

			for (int i = 0; i < roadCount; i++)
				for (int j = 0; j < timeCount; j++)
					cache[i][j] = predict(i, j, f, true, false);
		}		
		
		OutputTimeInfo("Final result:   %.5lf (RMSE=%.6lf)", db->scorePart(this, ROAD_IDX), RMSE(db));

		//debugOutputMatrix(db);
	}

	void debugOutputFeatures()
	{
		OutputInfo("\n");
		for (int i = 0; i < roadCount; i++)
		{
			OutputInfo("%.3lf ", roadFeature[i][0]);
		} 
		OutputInfo("\n");

		for (int i = 0; i < timeCount; i++)
		{
			OutputInfo("%.3lf ", timeFeature[i][0]);
		}  
		OutputInfo("\n");
	}

	void debugOutputMatrix(Database* db)
	{
		OutputInfo("\n");
		for (int i = 0; i < roadCount; i++)
		{
			for (int j = 0; j < timeCount; j++)
				if ( valuesSet[i][j] )
				{
					int hh = i/(60/TIME_INTERVAL) + 16;
					int mm = i%((60/TIME_INTERVAL)) * TIME_INTERVAL;

					Item item(revDayIndex[j], hh, mm, -1);
					double speed = rebuildSpeed(db, i, j, item);
					double avg = avgAlgorithm->predictRaw(db, ROAD_IDX, item);
					double des = values[i][j]*SCALE + avg;
					OutputInfo("%d:%02d %.0lf--%.0lf (%.0lf) ", hh, mm, speed, avg, des);
				}
				else
				{
					int hh = i/(60/TIME_INTERVAL) + 16;
					int mm = i%((60/TIME_INTERVAL)) * TIME_INTERVAL;

					Item item(revDayIndex[j], hh, mm, -1);
					double speed = rebuildSpeed(db, i, j, item);
					double avg = avgAlgorithm->predictRaw(db, ROAD_IDX, item);
					OutputInfo("%d:%02d %.0lf--%.0lf (gg) ", hh, mm, speed, avg);
				}

			OutputInfo("\n");
		}
		//OutputInfo("\n");
	}

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

	double RMSE(Database* db)
	{
		double res = 0, cnt = 0;
		for (int i = 0; i < roadCount; i++)
			for (int j = 0; j < timeCount; j++)
			{
				if ( !valuesSet[i][j] ) continue;

				double ans = predict(i, j, MAX_FEATURES-1, false, false);

				res += Sqr2(ans - values[i][j]);
				cnt++;
			}

		res /= cnt;

		return res;
	} 

	double predict(Database* db, int roadIdx, const Item& item)
	{
		assert(roadIdx != -1);

		int ri = roadIndex(&item);
		int ti = timeIndex(&item);
		
		assert(ri != -1);

		if (false && valuesSet[ri][ti] )
		{
			double sp = values[ri][ti] * SCALE;
			sp += avgAlgorithm->predictRaw(db, roadIdx, item);
			return sp;
		}

		//return 0;

		double res = rebuildSpeed(db, ri, ti, item);
		if ( res < 1e-6 ) res = 0.0;
		return res;
	}
};

#endif
