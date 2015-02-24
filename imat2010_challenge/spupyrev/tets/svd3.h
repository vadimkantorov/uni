#pragma once
#ifndef SVD3_H
#define SVD3_H

#include <vector>
#include <algorithm>
#include <cmath>
using namespace std;

#include "algorithm.h"
#include "database.h"
#include "defs.h"
#include "random_utils.h"
#include "input_output.h"

class SVD3Algorithm : public Algorithm
{
	static const int TIME_INTERVAL = 20;
	static const int INTERVALS_PER_DAY = (60*6)/TIME_INTERVAL;

	int ROAD_IDX;
	int timeCount;
	int dayCount;

	bool initialized;

	//Algorithm* avgAlgorithm;

public:
    SVD3Algorithm(Database* db, int ROAD_IDX) : Algorithm() 
    {
		//this->avgAlgorithm = avgAlgorithm;
		this->ROAD_IDX = ROAD_IDX;

		timeCount = INTERVALS_PER_DAY;
		initTimeIndexes(db);

		initialized = false;
		initializeFeatures(db);
    }

	void initializeFeatures(Database* db)
	{
		values = new double*[timeCount];
		for (int i = 0; i < timeCount; i++)
			values[i] = new double[dayCount];
		valuesSet = new bool*[timeCount];
		for (int i = 0; i < timeCount; i++)
			valuesSet[i] = new bool[dayCount];
	}
    
    ~SVD3Algorithm() 
    {
		for (int i = 0; i < timeCount; i++)
		{
			delete[] values[i];
			delete[] valuesSet[i];
		}

		delete[] values;
		delete[] valuesSet;
    }

	void initialize(Database* db)
	{
		if ( initialized ) return;
		initialized = true;
		
		buildValues(db);
		teach(db);
	}

	void initTimeIndexes(Database* db)
	{
		for (int i = 0; i < 100; i++)
			dayIndex[i] = revDayIndex[i] = -1;
		dayCount = 0;

		for (int j = 0; j < db->getJamCount(ROAD_IDX); j++)
		{
			int dd = db->getJam(ROAD_IDX, j)->getDay();
			if ( dayIndex[dd] == -1 )
			{
				dayIndex[dd] = dayCount++;
				revDayIndex[dayCount - 1] = dd;
			}
		}
	}

	int dayIndex[100];
	int revDayIndex[100];
	double** values;
	bool** valuesSet;

	void buildValues(Database* db)
	{
		vector<vector<VD> > data = vector<vector<VD> >(timeCount, vector<VD>(dayCount, VD()));

		for (int j = 0; j < db->getJamCount(ROAD_IDX); j++)
		{
			if ( (db->getDay(ROAD_IDX, j) + 1)%7 >= 5 ) continue;

			int ti = timeIndex(db->getJam(ROAD_IDX, j));
			int di = dayIndex[db->getDay(ROAD_IDX, j)];
			double speed = db->getSpeed(ROAD_IDX, j);
			speed = Item::turnSpeed(speed, db->getRoadLLS(ROAD_IDX), db->getDay(ROAD_IDX, j), true);
			data[ti][di].push_back(speed);
		}

		int totalElements = (dayCount - 1)*timeCount + (2*60)/TIME_INTERVAL;
		int filledElements = 0;
		for (int i = 0; i < timeCount; i++)
		{
			for (int j = 0; j < dayCount; j++)
			{
				if ( data[i][j].empty() )
				{
					values[i][j] = -1;
					valuesSet[i][j] = false;
				}
				else
				{
					values[i][j] = AverageValue(data[i][j]);
					//values[i][j] = MedianValue(data[i][j]);
					valuesSet[i][j] = true;
					filledElements++;
				}
			}
		}

		//OutputTimeInfo("Teaching %d road (filled = %d, missed = %d elements, jams = %d)", db->getRoadId(ROAD_IDX), filledElements, totalElements - filledElements, db->getJamCount(ROAD_IDX));
		//outputVariance();
	}

	inline int timeIndex(const Item* item)
	{
		int t = (item->getH() - 16)*(60/TIME_INTERVAL) + item->getM()/TIME_INTERVAL;
		return t;
	}

	double dist[100][100];
	int nearest[100][100];

	double calcDistance(int ti, int tj)
	{
		if ( ti == tj ) return 1e6;
		VD sps;
		for (int i = 0; i < timeCount; i++)
			if ( valuesSet[i][ti] && valuesSet[i][tj] )
			{
				int hh = i/(60/TIME_INTERVAL) + 16;
				//if ( hh < 17 || hh >= 18 ) continue;
				if ( hh >= 18 ) continue;

				sps.push_back(Abs(values[i][ti] - values[i][tj]));
			}

		if ( (int)sps.size() == 0 )
			return 1e6;

		double res = AverageValue(sps);
		//double res = MedianValue(sps);
		if ( res < 1.0 ) 
			res = 1.0;
		return res;
	}

	void teach(Database* db)
	{
		for (int i = 0; i < dayCount; i++)
			for (int j = 0; j < dayCount; j++)
			{
				dist[i][j] = calcDistance(i, j);
				assert(dist[i][j] == dist[i][j]);
				assert(dist[i][j] > 1e-6);
			}

		for (int i = 0; i < dayCount; i++)
		{
			vector<pair<double, int> > pr;
			for (int j = 0; j < dayCount; j++)
			{
				pr.push_back(make_pair(dist[i][j], j));
			}

			sort(pr.begin(), pr.end());
			for (int j = 0; j < dayCount; j++)
				nearest[i][j] = pr[j].second;
		}
	}

	double predict(Database* db, int ti, int di, const Item& item)
	{
		int MIN_NEIGH = 10;		  
		int MAX_NEIGH = 25;		  
		double DIST = 7.5;
		return predict(db, ti, di, item, MIN_NEIGH, MAX_NEIGH, DIST);
	}

	double predict(Database* db, int ti, int di, const Item& item, int MIN_NEIGH, int MAX_NEIGH, double DIST)
	{
		if ( MIN_NEIGH > MAX_NEIGH ) MIN_NEIGH = MAX_NEIGH;

		vector<int> nr;
		for (int i = 0; i < dayCount; i++)
			if ( valuesSet[ti][nearest[di][i]] )
			{
				if ( dist[di][nearest[di][i]] > DIST ) break;
				nr.push_back(nearest[di][i]);
				if ( (int)nr.size() >= MAX_NEIGH )  //break;
					if ( i+1 < dayCount && dist[di][nearest[di][i+1]] > dist[di][nearest[di][i]] + DIST/10.0 ) break;
			}

		if ( (int)nr.size() < MIN_NEIGH && DIST < 100.0)
		{
			return predict(db, ti, di, item, MIN_NEIGH+1, MAX_NEIGH, 1.5*DIST);
			OutputTimeInfo("wrong day %d", item.getDay());
			return -1;
		} 
		if ( nr.empty() ) return -1;
		assert(nr.size() > 0);

		double sumSim = 0, res = 0;
		//OutputInfo("For day %d and road %d used: ", item.getDay(), db->getRoadId(ROAD_IDX));
		for (int i = 0; i < (int)nr.size(); i++)
		{
			//OutputInfo(" %d (%.3lf)", revDayIndex[nr[i]], dist[di][nr[i]]);
			assert(dist[di][nr[i]] > 1e-6);

			double sim = 1.0 / Sqr2(dist[di][nr[i]]);
			sumSim += sim;
		}
		//OutputInfo("\n");

		for (int i = 0; i < (int)nr.size(); i++)
		{
			double sim = 1.0 / Sqr2(dist[di][nr[i]]);
			res += sim * values[ti][nr[i]];
		}

		assert(sumSim > 1e-6);
		res /= sumSim;

		return res;
	}

	double predict(Database* db, int roadIdx, const Item& item)
	{
		assert(roadIdx != -1);

		int ti = timeIndex(&item);
		int di = dayIndex[item.getDay()];
		
		if ( di == -1 ) return -1;
		assert(di != -1);
			
		double res = predict(db, ti, di, item);
		res = Item::turnSpeed(res, db->getRoadLLS(roadIdx), item.getDay(), false);
		return res;
	}

	double predictRaw(Database* db, int roadIndex, const Item& item)
	{
		assert(false);
		return -1;
	}
};

#endif
