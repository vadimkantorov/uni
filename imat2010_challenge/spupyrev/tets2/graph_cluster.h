#pragma once
#ifndef GRAPHCLUSTER_H
#define GRAPHCLUSTER_H

#include <vector>
#include <algorithm>
#include <cmath>
#include <set>
using namespace std;

#include "algorithm.h"
#include "database.h"
#include "defs.h"
#include "random_utils.h"

class DummyAverage: public Algorithm
{
	double avgSpeed;
public:
    DummyAverage(Database* db, int roadIndex) : Algorithm() 
    {
		double sum = 0, cnt = 0;
		for (int i = 0; i < db->getJamCount(roadIndex); i++)
		{
			if ( (db->getDay(roadIndex, i) + 1)%7 >= 5 ) continue;

			sum += db->getSpeed(roadIndex, i);
			cnt++;
		}

		if ( cnt > 0 ) sum /= cnt;
		avgSpeed = sum;
    }
    
    ~DummyAverage() 
    {
    }

	void initialize(Database* db)
	{
	}

	double predict(Database* db, int roadIndex, const Item& item)
	{
		return avgSpeed;
	}
};

class SVDCluster
{
	static const int TIME_INTERVAL = 10;

public:
	SVDCluster()
	{
	}

	~SVDCluster()
	{
	}

	void outputPredictions(Database* db, int roadIndex)
	{
		FILE* f1 = OpenFile("w", "pdays.%d", db->getRoadId(roadIndex));
		FILE* f2 = OpenFile("w", "pvalues.%d", db->getRoadId(roadIndex));

		DummyAverage da(db, roadIndex);
		SVD2Algorithm svd(db, roadIndex, &da);

		svd.initialize(db);

		for (int day = 0; day < db->getDayCount(); day++)
		{
			for (int hh = 16; hh < 22; hh++)
				for (int mm = 0; mm < 60; mm += TIME_INTERVAL)
				{
					Item item(day+11, hh, mm, -1);
					double speed = svd.predict(db, roadIndex, item);
					if ( speed == 0.0 ) continue;

					int ttime = (item.getDay() - 11)*6*15 + (item.getH() - 16)*15 + (item.getM() / 4);

					fprintf(f1, "%d\n", ttime);
					fprintf(f2, "%.3lf\n", speed);
				}
		}

		fclose(f1);
		fclose(f2);
	}
};

#endif
