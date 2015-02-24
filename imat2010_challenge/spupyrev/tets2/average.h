#pragma once
#ifndef AVERAGE_H
#define AVERAGE_H

#include "database.h"
#include "algorithm.h"

class SmallAverage : public Algorithm
{
	static const int MAX_TIME = 3000;
	static const int TIME_INTERVAL = 60;

	double* roadAverage;
	double timeOffset[MAX_TIME];
	double timeOffsetCount[MAX_TIME];

	bool initialized;
	ClusterEngine* cluster;
	int TO;

public:
    SmallAverage(Database* db, ClusterEngine* cluster, int to) : Algorithm() 
    {
		this->cluster = cluster;
		this->TO = to;
		roadAverage = new double[db->getRoadCount()];
		initialized = false;
    }
    
    ~SmallAverage() 
    {
		delete[] roadAverage;
    }

	map<int, vector<double> > avg;
	map<int, double> avgRes;
	map<int, double> avgSize;

	int convertTime(int day, int hh, int mm)
	{
		day = (day + 1)%7;
		if ( day <= 3 ) day = 0;
		else if ( day <= 4 ) day = 1;
		else day = 2;

		if ( mm >= 30 && hh != 21 ) hh++;
		//if ( mm >= 30 ) hh++;
		
		return day*(60/TIME_INTERVAL)*6 + (hh - 16)*(60/TIME_INTERVAL) + (mm/TIME_INTERVAL);
	}

	inline int convertRoad(Database*db, int roadIndex)
	{
		return roadIndex;
	}

	inline int createPair(int road, int time)
	{
		return road*MAX_TIME + time;
	}

	inline double convertSpeed(Database*db, int roadIndex, int jamIndex)
	{
		double speed = db->getSpeed(roadIndex, jamIndex);
		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), db->getDay(roadIndex, jamIndex), true);

		cluster->fitMeanAndVariance(db, roadIndex, speed);

		return speed;
	}

	bool needTeach(Database* db, int roadIdx, int jamIdx)
	{
		if ( (db->getDay(roadIdx, jamIdx) + 1)%7 >= 5 ) return false;
		if ( db->getJamCount(roadIdx) >= TO ) return false;
		//if ( db->getJamCount(roadIdx) >= 1000 ) return false;
		return true;
	}

	double calcGlobalAverage(Database* db)
	{
		VD sps;
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				sps.push_back(speed);
			}
		}

		double gspeed = MedianValue(sps);

		return gspeed;
	}

	double globalAverage;

	void initRoadAverage(Database* db)
	{
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			roadAverage[i] = 0;
			double rc = 0;
			VD sps;
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				sps.push_back(speed);
				//roadAverage[i] += speed;
				rc++;
			}

			double KK = 3;
			for (int j = 0; j < KK; j++)
				sps.push_back(globalAverage);

			roadAverage[i] = MedianValue(sps);
			//if ( roadAverage[i] == 0 )
			//	OutputTimeInfo("Error at %d (%d)", db->getRoadId(i), db->getJamCount(i));

			//roadAverage[i] = (roadAverage[i] + globalAverage*KK)/(rc + KK);
			/*if ( db->getJamCount(i) <= TO && db->getJamCount(i) > 0 )
			{
				double mean, var;
				cluster->getMeanAndVariance(db, i, mean, var);
				OutputTimeInfo("%d %.3lf (%d)", db->getRoadId(i), var, db->getJamCount(i));
			}  */
		}
	}

	void initTimeOffset(Database* db)
	{
		for (int i = 0; i < MAX_TIME; i++)
			timeOffset[i] = timeOffsetCount[i] = 0;

		map<int, VD> off;
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				int t = convertTime(db->getDay(i, j), db->getH(i, j), db->getM(i, j));

				off[t].push_back(speed - roadAverage[i]);
				timeOffset[t] += speed - roadAverage[i];
				timeOffsetCount[t]++;
			}
		}

		for (int i = 0; i < MAX_TIME; i++)
			if ( timeOffsetCount[i] > 0.0 )
			{
				//timeOffset[i] = MedianValue(off[i]);
				timeOffset[i] /= timeOffsetCount[i];
				//OutputTimeInfo("%d   %.3lf", i, timeOffset[i]);
				//timeOffset[i] = 0;
			}
	}

	void initialize(Database* db)
	{
		if ( initialized ) return;
		OutputTimeInfo("initializing small average...");

		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				int t = convertTime(db->getDay(i, j), db->getH(i, j), db->getM(i, j));
				int r = convertRoad(db, i);
				int pr = createPair(r, t);
				double speed = convertSpeed(db, i, j);
				avg[pr].push_back(speed);
			}
		}

		globalAverage = calcGlobalAverage(db);

		initRoadAverage(db);
		initTimeOffset(db);

		initialized = true;
	}

	double predict(Database* db, int roadIndex, const Item& item)
	{
		//double res = cluster->predict(db, roadIndex, item, this);
		//if ( res != -1 )
		//	return res;
		Item nItem1(item.getDay(), item.getH(), 0, -1);
		double sp1 = predictRaw(db, roadIndex, nItem1);

		Item nItem2(item.getDay(), min(item.getH() + 1, 21), 0, -1);
		//Item nItem2(item.getDay(), item.getH() + 1, 0, -1);
		double sp2 = predictRaw(db, roadIndex, nItem2);

		double t = double(item.getM())/60.0;
		return sp1 * (1.0 - t) + sp2 * t;
		//return predictRaw(db, roadIndex, item);
	}

	double predictRaw(Database* db, int roadIndex, const Item& item)
	{
		assert(roadIndex != -1);
		assert(roadAverage[roadIndex] >= 0.0);

		int t = convertTime(item.getDay(), item.getH(), item.getM());
		int r = convertRoad(db, roadIndex);
		int pr = createPair(r, t);
		double speed, sc;
		if ( avgRes.find(pr) == avgRes.end() )
		{
			vector<double> sps = avg[pr];
			//speed = AverageValue(sps);
			speed = MedianValue(sps);
			sc = (double)sps.size();
			avgRes[pr] = speed;
			avgSize[pr] = sc;
			//avg.erase(pr);
		}
		else
		{
			speed = avgRes[pr];
			sc = avgSize[pr];
		}

		double K2 = 100;
//		double mean, var;
//		cluster->getMeanAndVariance(db, roadIndex, mean, var);
//		if ( var < 14.5 ) K2 = 50;
		
		//TODO need this???
		//if ( Variance(avg[pr]) <= 5.0 && sc >= 10.0 ) K2 = 10;

		speed = (speed*sc + (roadAverage[roadIndex] + timeOffset[t])*K2)/(sc + K2);

		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), item.getDay(), false);
		speed = max(speed, 0.0);

		return speed;
	}
};

class MidAverage : public Algorithm
{
	static const int MAX_TIME = 12000;
	static const int TIME_INTERVAL = 60;

	double* roadAverage;
	double timeOffset[MAX_TIME];
	double timeOffsetCount[MAX_TIME];

	int FROM, TO;

	bool initialized;
	ClusterEngine* cluster;

public:
    MidAverage(Database* db, ClusterEngine* cluster, int from, int to) : Algorithm() 
    {
		this->cluster = cluster;
		this->FROM = from;
		this->TO = to;
		roadAverage = new double[db->getRoadCount()];
		initialized = false;
    }
    
    ~MidAverage() 
    {
		delete[] roadAverage;
    }

	map<int, vector<double> > avg;
	map<int, double> avgRes;
	map<int, double> avgSize;

	int convertTime(int day, int hh, int mm)
	{
		day = (day + 1)%7;
		if ( day <= 3 ) day = 0;
		else if ( day <= 4 ) day = 1;
		else day = 2;

		if ( mm > 30 && hh != 21) hh++;
		//if ( mm > 30 ) hh++;
		
		return day*(60/TIME_INTERVAL)*6 + (hh - 16)*(60/TIME_INTERVAL) + (mm/TIME_INTERVAL);
	}

	inline int convertRoad(Database*db, int roadIndex)
	{
		return roadIndex;
	}

	inline int createPair(int road, int time)
	{
		return road*MAX_TIME + time;
	}

	inline double convertSpeed(Database*db, int roadIndex, int jamIndex)
	{
		double speed = db->getSpeed(roadIndex, jamIndex);
		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), db->getDay(roadIndex, jamIndex), true);

		//cluster->fitMeanAndVariance(db, roadIndex, speed);

		return speed;
	}

	bool needTeach(Database* db, int roadIdx, int jamIdx)
	{
		if ( (db->getDay(roadIdx, jamIdx) + 1)%7 >= 5 ) return false;
		if ( db->getJamCount(roadIdx) >= TO ) return false;
		if ( db->getJamCount(roadIdx) < FROM ) return false;
		return true;
	}

	double calcGlobalAverage(Database* db)
	{
		VD sps;
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				sps.push_back(speed);
			}
		}

		double gspeed = MedianValue(sps);
		OutputTimeInfo("GlobalAverage for median: %.3lf", gspeed);

		return gspeed;
	}

	double globalAverage;

	void initRoadAverage(Database* db)
	{
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			roadAverage[i] = 0;
			VD sps;
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				sps.push_back(speed);
				//roadAverage[i] += speed;
			}

			//double KK = 5;
			double KK = 3;
			for (int j = 0; j < KK; j++)
				sps.push_back(globalAverage);

			roadAverage[i] = MedianValue(sps);
			//roadAverage[i] = (roadAverage[i] + globalAverage*KK)/(rc + KK);
		}
	}

	void initTimeOffset(Database* db)
	{
		for (int i = 0; i < MAX_TIME; i++)
			timeOffset[i] = timeOffsetCount[i] = 0;

		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				int t = convertTime(db->getDay(i, j), db->getH(i, j), db->getM(i, j));

				timeOffset[t] += speed - roadAverage[i];
				timeOffsetCount[t]++;
			}
		}

		for (int i = 0; i < MAX_TIME; i++)
			if ( timeOffsetCount[i] > 0.0 )
			{
				timeOffset[i] /= timeOffsetCount[i];
				//timeOffset[i] = 0;
				//OutputInfo("%d %.3lf\n", i, timeOffset[i]);
			}
	}

	void initialize(Database* db)
	{
		if ( initialized ) return;
		OutputTimeInfo("initializing mid average...");

		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				int t = convertTime(db->getDay(i, j), db->getH(i, j), db->getM(i, j));
				int r = convertRoad(db, i);
				int pr = createPair(r, t);
				double speed = convertSpeed(db, i, j);
				avg[pr].push_back(speed);
			}
		}

		globalAverage = calcGlobalAverage(db);

		initRoadAverage(db);
		initTimeOffset(db);

		initialized = true;
	}

	double predict(Database* db, int roadIndex, const Item& item)
	{
//		double as = cluster->predict(db, roadIndex, item, this);
//		if ( as != -1 && item.getH() == 21)
		{
		//	if ( item.getH() == 18 )
		//		OutputInfo("%d\n", db->getRoadId(roadIndex));

//			return as;
		}

		Item nItem1(item.getDay(), item.getH(), 0, -1);
		double sp1 = predictRaw(db, roadIndex, nItem1);

		Item nItem2(item.getDay(), min(item.getH() + 1, 21), 0, -1);
		//Item nItem2(item.getDay(), item.getH() + 1, 0, -1);
		double sp2 = predictRaw(db, roadIndex, nItem2);

		double t = double(item.getM())/60.0;
		return sp1 * (1.0 - t) + sp2 * t;
	}

	double predictRaw(Database* db, int roadIndex, const Item& item)
	{
		assert(roadIndex != -1);
		assert(roadAverage[roadIndex] >= 0.0);

		int t = convertTime(item.getDay(), item.getH(), item.getM());
		int r = convertRoad(db, roadIndex);
		int pr = createPair(r, t);
		double speed, sc;
		if ( avgRes.find(pr) == avgRes.end() )
		{
			vector<double> sps = avg[pr];
			//speed = AverageValue(sps);
			speed = MedianValue(sps);
			sc = (double)sps.size();
			avgRes[pr] = speed;
			avgSize[pr] = sc;
			avg.erase(pr);
		}
		else
		{
			speed = avgRes[pr];
			sc = avgSize[pr];
		}

		double K2 = 30;
		speed = (speed*sc + (roadAverage[roadIndex] + timeOffset[t])*K2)/(sc + K2);

		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), item.getDay(), false);
		return speed;
	}
};

class LargeAverage : Algorithm
{
	static const int MAX_TIME = 12000;
	static const int TIME_INTERVAL = 30;

	VI cluster;
	ClusterEngine* clusterEngine;
	int FROM;

public:

	LargeAverage(Database* db, ClusterEngine* clusterEngine, int from): Algorithm()
    {
		this->clusterEngine = clusterEngine;
		this->FROM = from;
		svd2 = 0;
		useSVD = true;
		prevRoadIndex = -1;

		for (int i = 0; i < db->getRoadCount(); i++)
			cluster.push_back(i);
    }
    
    ~LargeAverage() 
    {
    }

	map<int, vector<double> > avg;
	map<int, double> avgRes;
	map<int, double> avgSize;

	int convertTime(int day, int hh, int mm)
	{
		day = (day + 1)%7;
		if ( day <= 3 ) day = 0;
		else if ( day <= 4 ) day = 1;
		else day = 2;
		
		return day*(60/TIME_INTERVAL)*6 + (hh - 16)*(60/TIME_INTERVAL) + (mm/TIME_INTERVAL);
	}

	inline int convertRoad(Database*db, int roadIndex)
	{
		assert(cluster[roadIndex] != -1);
		return cluster[roadIndex];
	}

	inline int createPair(int road, int time)
	{
		return road*MAX_TIME + time;
	}

	inline double convertSpeed(Database*db, int roadIndex, int jamIndex)
	{
		double speed = db->getSpeed(roadIndex, jamIndex);
		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), db->getDay(roadIndex, jamIndex), true);

		return speed;
	}

	void initialize(Database* db)
	{
		OutputTimeInfo("initializing large average...");

		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( (db->getDay(i, j) + 1)%7 >= 5 ) continue;
				//if ( db->getDay(i, j) == 34 ) continue;

				int t = convertTime(db->getDay(i, j), db->getH(i, j), db->getM(i, j));
				int r = convertRoad(db, i);
				int pr = createPair(r, t);
				double speed = convertSpeed(db, i, j);
				avg[pr].push_back(speed);
			}
		}
	}

	bool useSVD;
	int prevRoadIndex;
	SVD3Algorithm* svd2;

	double predict(Database* db, int roadIndex, const Item& item)
	{
		//assert(db->getJamCount(roadIndex) >= FROM);

		if ( item.getH() >= 20 )
			return predict21(db, roadIndex, item);

		if ( item.getH() == 18 )
			return predict18(db, roadIndex, item);

		if ( item.getH() == 19 )
			return predict19(db, roadIndex, item);

		assert(false);
		return -1;
	}

	double predict19(Database* db, int roadIndex, const Item& item)
	{
		double res = clusterEngine->predict(db, roadIndex, item);
		if ( res != -1 )
			return res;

		if ( 1==1 )
		{
			if ( prevRoadIndex != roadIndex )
			{
				if ( svd2 != 0 )
				{
					delete svd2;
					svd2 = 0;
				}

				svd2 = new SVD3Algorithm(db, roadIndex);
				//svd2 = new SVD2Algorithm(db, roadIndex, this);
				svd2->initialize(db);

				prevRoadIndex = roadIndex;
			}

			if ( svd2 != 0 )
			{
				double res = svd2->predict(db, roadIndex, item);
				if ( res != -1 ) return res;
			}
		}

		return predictRaw(db, roadIndex, item);
	}

	double predict18(Database* db, int roadIndex, const Item& item)
	{
		Item nItem = Item(item.getDay(), 17, 45, -1);
		double raw = predictRaw(db, roadIndex, nItem);

		double as = clusterEngine->getValue(db, roadIndex, nItem);
		if ( as == -1 ) as = raw;

		if ( abs(as - raw) >= 20.0 )
		{
			return max(as - raw + predictRaw(db, roadIndex, item), 0.0);
		} 
		//else
		if ( 1==1 )
		{
			if ( prevRoadIndex != roadIndex )
			{
				if ( svd2 != 0 )
				{
					delete svd2;
					svd2 = 0;
				}

				svd2 = new SVD3Algorithm(db, roadIndex);
				//svd2 = new SVD2Algorithm(db, roadIndex, this);
				svd2->initialize(db);

				prevRoadIndex = roadIndex;
			}

			if ( svd2 != 0 )
			{
				double res = svd2->predict(db, roadIndex, item);
				if ( res != -1 ) return res;
			}
		}

		return predictRaw(db, roadIndex, item);
	}

	double predict21(Database* db, int roadIndex, const Item& item)
	{
		double res = clusterEngine->predict(db, roadIndex, item);
		if ( res != -1 )
			return res;

		if ( 1==1 )
		{
			if ( prevRoadIndex != roadIndex )
			{
				if ( svd2 != 0 )
				{
					delete svd2;
					svd2 = 0;
				}

				svd2 = new SVD3Algorithm(db, roadIndex);
				//svd2 = new SVD2Algorithm(db, roadIndex, this);
				svd2->initialize(db);

				prevRoadIndex = roadIndex;
			}

			if ( svd2 != 0 )
			{
				double res = svd2->predict(db, roadIndex, item);
				if ( res != -1 ) return res;
			}
		}

		return predictRaw(db, roadIndex, item);
	}

	double predictRaw(Database* db, int roadIndex, const Item& item)
	{
		assert(roadIndex != -1);

		int t = convertTime(item.getDay(), item.getH(), item.getM());
		int r = convertRoad(db, roadIndex);

		int pr = createPair(r, t);
		double speed, sc;
		if ( avgRes.find(pr) == avgRes.end() )
		{
			vector<double> sps = avg[pr];
			speed = AverageValue(sps);
			//speed = MedianValue(sps);
			sc = (double)sps.size();
			avgRes[pr] = speed;
			avgSize[pr] = sc;
			avg.erase(pr);
		}
		else
		{
			speed = avgRes[pr];
			sc = avgSize[pr];
		}

		assert(sc > 0.0);

		//OutputInfo("%d %d\n", db->getRoadId(roadIndex), (int)sc);

		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), item.getDay(), false);
		speed = max(speed, 0.0);
		return speed;
	}
};


#endif
