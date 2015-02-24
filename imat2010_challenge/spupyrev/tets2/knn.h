#pragma once
#ifndef KNN_H
#define KNN_H

#include "database.h"
#include "algorithm.h"

class KNN : public Algorithm
{
	static const int MAX_TIME = 3000;
	static const int TIME_INTERVAL = 50;
	static const int MAX_TIME_INTERVAL = 50;

	int FROM, TO;

	bool initialized;
	ClusterEngine* cluster;

public:
    KNN(Database* db, int from, int to, ClusterEngine* cluster) : Algorithm() 
    {
		initialized = false;
		roadAverage = new double[db->getRoadCount()];
		FROM = from;
		TO = to;
		this->cluster = cluster;
    }
    
    ~KNN() 
    {
		delete[] roadAverage;
    }

	bool needTeach(Database* db, int roadIndex, int jamIdx)
	{
		if ( (db->getDay(roadIndex, jamIdx) + 1)%7 >= 5 ) return false;
		if ( db->getJamCount(roadIndex) >= TO ) return false;
		if ( db->getJamCount(roadIndex) < FROM ) return false;
		return true;
	}

	double daysDist[100][100];
	double daysDist2[100][100];

	map<int, vector<pair<int, double> > > nearest;

	void readNearest(Database* db)
	{
		string fileName = "nearest.dist2";
		OutputTimeInfo("Reading nearest data (%s)...", fileName.c_str());
		int tot = 0;

		FILE* f = fopen(fileName.c_str(), "r");
		int n;
		fscanf(f, "%d", &n);
		for (int i = 0; i < n; i++)
		{
			vector<pair<int, double> > v;
			int id, k;
			if ( fscanf(f, "%d %d", &id, &k) != 2 ) break;
			for (int j = 0; j < k; j++)
			{
				int id2;
				double d;
				fscanf(f, "%d %lf", &id2, &d);
				if ( db->getRoadIndex(id) != -1 && db->getRoadIndex(id2) != -1 )
					v.push_back(make_pair(db->getRoadIndex(id2), d));
			}

			if ( db->getRoadIndex(id) != -1 && !v.empty() )
			{
				nearest[db->getRoadIndex(id)] = v;
				int jc = db->getJamCount(db->getRoadIndex(id));
				if ( jc <= TO && jc >= FROM ) tot++;
			}

		}
		fclose(f);
		OutputTimeInfo("done");
		OutputTimeInfo("found %d n-roads", tot);
	}


	void readDaysDistances(Database* db)
	{
		readDaysDistances(db, "days.dist", daysDist);
		readDaysDistances(db, "days.dist2", daysDist2);
	}

	void readDaysDistances(Database* db, string fileName, double daysDist[][100])
	{
		OutputTimeInfo("Reading days distances (%s)...", fileName.c_str());
		FILE* f = fopen(fileName.c_str(), "r");
		for (int i = 0; i < 31; i++)
		{
			int day;
			fscanf(f, "%d", &day);
			assert(day == i+11);
			for (int j = 0; j < 31; j++)
			{
				double tmp;
				fscanf(f, "%lf", &tmp);
				daysDist[i][j] = tmp;
				if ( i==j )
					assert(tmp == 0.0);
				else 
					assert(tmp != 0.0);
			}
		}

		fclose(f);
	}

	double getDaysDistance(Database* db, int roadIndex, int day1, int day2)
	{
		assert(day1 - 11 >= 0);
		assert(day2 - 11 >= 0);
		//HACK!!!!!!
		assert(day1 == 41 || day2 == 41);
		if ( day1 != 41 ) swap(day1, day2);

		double var = cluster->getVar(db, roadIndex);

		if ( var < 14.5 )
		{
			double ret = daysDist[day1 - 11][day2 - 11];
			//if ( ret >= 11.0 ) ret = 30.0;
			return ret;
		}
		else
		{
			double ret = daysDist2[day1 - 11][day2 - 11];
			if ( ret >= 16.3 ) ret = 35.0;
			return ret;
		}
	}

	void initialize(Database* db)
	{
		if ( initialized ) return;
		OutputTimeInfo("initializing knn...");

		globalAverage = 50.3;
		readNearest(db);
		readDaysDistances(db);
		initRoadAverage(db);
		initTimeOffset(db);

		initialized = true;
	}

	double* roadAverage;
	double timeOffset[MAX_TIME];
	double globalAverage;

	inline double convertSpeed(Database*db, int roadIndex, int jamIndex)
	{
		double speed = db->getSpeed(roadIndex, jamIndex);
		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), db->getDay(roadIndex, jamIndex), true);

		return speed;
	}

	void initRoadAverage(Database* db)
	{
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			roadAverage[i] = 0;
			VD sps;
			vector<pair<double, double> > prob;
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				sps.push_back(speed);

				int skipDay = (db->getSkippedDay() == -1 ? 41 : db->getSkippedDay());
				double sim = getDaysDistance(db, i, db->getDay(i, j), skipDay);
				if ( sim == 0.0 ) 
				{
					sim = 6;
				}
				prob.push_back(make_pair(speed, 1.0/Sqr2(sim*sim)));
			}

			prob.push_back(make_pair(globalAverage, 0.001));

			double KK = 3;
			for (int j = 0; j < KK; j++)
				sps.push_back(globalAverage);

			//roadAverage[i] = MedianValue(sps);
			roadAverage[i] = ProbabilityMedian(prob);
		}
	}

	void initTimeOffset(Database* db)
	{
		map<int, VD> to;
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			for (int j = 0; j < db->getJamCount(i); j++)
			{
				if ( !needTeach(db, i, j) ) continue;

				double speed = convertSpeed(db, i, j);
				int t = db->getH(i, j) - 16;

				to[t].push_back(speed - roadAverage[i]);
			}
		}

		for (int i = 0; i < MAX_TIME; i++)
		{
			VD off = to[i];
			if ( off.empty() ) timeOffset[i] = 0;
			else timeOffset[i] = AverageValue(off);
		}
	}

	int timeDiff(const Item& item1, const Item& item2)
	{
		int t1 = item1.getH()*60 + item1.getM();
		int t2 = item2.getH()*60 + item2.getM();

		return Abs(t1 - t2);
	}

	bool isNearest(const Item& item1, const Item& item2, int timeInterval)
	{
		return timeDiff(item1, item2) <= timeInterval;
	}

	//returns pair<roadIndex, jamIndex>
	void findNearestSpeeds(Database* db, int roadIndex, const Item& item, vector<pair<int, int> >& res, int timeInterval)
	{
		for (int i = 0; i < db->getJamCount(roadIndex); i++)
		{
			if ( !needTeach(db, roadIndex, i) ) continue;
			if ( !isNearest(item, *db->getJam(roadIndex, i), timeInterval) ) continue;

			res.push_back(make_pair(roadIndex, i));
		}
	}

	static const int MIN_COUNT = 500;
	vector<pair<int, int> > findNeighNearestSpeeds(Database* db, int roadIndex, const Item& item, int timeInterval)
	{
		double LIMIT = 7.5;

		vector<pair<int, double> > neig = nearest[roadIndex];
		neig.push_back(make_pair(roadIndex, 0.1));

		vector<pair<int, int> > res;
		for (int i = 0; i < (int)neig.size(); i++)
		{
			if ( neig[i].second > LIMIT ) continue;

			findNearestSpeeds(db, neig[i].first, item, res, timeInterval);
		}

		return res;
	}

	vector<pair<int, int> > findSelfNearestSpeeds(Database* db, int roadIndex, const Item& item, int timeInterval)
	{
		vector<pair<int, int> > res;
		findNearestSpeeds(db, roadIndex, item, res, timeInterval);

		return res;
	}


	double constructSpeed(Database* db, int roadIndex, const Item& item, vector<pair<int, int> >& values)
	{
		double var = cluster->getVar(db, roadIndex);
		
		double res = 0;
		vector<pair<double, double> > prob;
		for (int i = 0; i < (int)values.size(); i++)
		{
			Item* it = db->getJam(values[i].first, values[i].second);
			int day = db->getDay(values[i].first, values[i].second);
			double dd = getDaysDistance(db, roadIndex, day, item.getDay());
			dd /= 7.0;

			double sim;
			if ( var < 14.5 )
				sim = (dd == 0.0 ? 0.1 : 1.0/(Sqr2(dd*dd)));
			else
				sim = (dd == 0.0 ? 0.1 : 1.0/(Sqr2(dd*dd*dd)));
			
			//double tdiff = timeDiff(*it, item);
			//tdiff = sqrt(max(tdiff, 5.0));
			//sim /= tdiff;

			double speed = db->getSpeed(values[i].first, values[i].second);
			speed = Item::turnSpeed(speed, db->getRoadLLS(roadIndex), day, true);

			prob.push_back(make_pair(speed, sim));
		}

		double sc = (double)values.size();
		if ( sc > 0.0 )
		{
			res = ProbabilityMedian(prob);
		}

		double K2 = 15;
		res = (res*sc + (roadAverage[roadIndex] + timeOffset[item.getH()-16])*K2)/(sc + K2);

		res = Item::turnSpeed(res, db->getRoadLLS(roadIndex), item.getDay(), false);
		return res;
	}

	double predict(Database* db, int roadIndex, const Item& item)
	{
		assert(db->getJamCount(roadIndex) <= TO);
//		assert(db->getJamCount(roadIndex) >= FROM);

		int timeInterval;
		if ( item.getH() == 18 ) timeInterval = 60;//40
		else if ( item.getH() == 19 ) timeInterval = 60;//40
		else if ( item.getH() == 20 ) timeInterval = 60;//50
		else if ( item.getH() == 21 ) timeInterval = 60;//60
		
		//if ( db->getJamCount(roadIndex) >= 2500 ) timeInterval = 40;
/*		double var = cluster->getVar(db, roadIndex);
		if ( var <= 10.0 )
		{
			if ( item.getH() == 18 ) timeInterval = 60;//40
			else if ( item.getH() == 19 ) timeInterval = 60;//40
			else if ( item.getH() == 20 ) timeInterval = 60;//50
			else if ( item.getH() == 21 ) timeInterval = 60;//60
		}*/
		

		vector<pair<int, int> > tmp = findSelfNearestSpeeds(db, roadIndex, item, timeInterval);
		//vector<pair<int, int> > tmp = findNeighNearestSpeeds(db, roadIndex, item, timeInterval);

		//OutputInfo("%d  ", tmp.size());
		return constructSpeed(db, roadIndex, item, tmp);
	}

	double predictRaw(Database* db, int roadIndex, const Item& item)
	{
		return -1;
	}
};


#endif
