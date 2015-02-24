#pragma once
#ifndef CLUSTER_H
#define CLUSTER_H

#include <vector>
#include <algorithm>
#include <cmath>
#include <set>
using namespace std;

#include "algorithm.h"
#include "database.h"
#include "defs.h"
#include "random_utils.h"

class ClusterEngine
{
	static const int TIME_INTERVAL = 20;
	static const int INTERVALS_PER_HOUR = 60/TIME_INTERVAL;
	static const int INTERVALS_PER_DAY = 6*INTERVALS_PER_HOUR;

	int roadCount;
	int timeCount;
	int dayCount;

public:
    ClusterEngine(Database* db)
    {
		values = 0;
		nearestInitialized = false;
		initialized = false;
		//initialize(db);

		for (int i = 0; i < Database::MAX_ROADS; i++)
			roadMean[i] = roadVar[i] = -1;
    }

    ~ClusterEngine() 
    {
		if ( values != 0 )
		{
			for (int i = 0; i < roadCount; i++)
			{
				if ( values[i] == 0 ) continue;
				for (int j = 0; j < timeCount; j++)
				{
					if ( values[i][j] == 0 ) continue;

					delete[] values[i][j];
					delete[] valuesSet[i][j];
				}
				delete[] values[i];
				delete[] valuesSet[i];
			}

			delete[] values;
			delete[] valuesSet;
		}
    }

	bool needCluster(Database* db, int idx)
	{
		return true;
		int jc = db->getJamCount(idx);
		return ( jc >= 1900 );
	}

	inline int timeIndex(const Item* item)
	{
		int t = (item->getH() - 16)*INTERVALS_PER_HOUR + item->getM()/TIME_INTERVAL;
		return t;
	}

	inline int dayIndex(const Item* item)
	{
		return item->getDay() - 11;
	}

	bool initialized;
	double*** values;
	bool*** valuesSet;

	void initialize(Database* db)
	{
		if ( initialized ) return;
		initialized = true;
		OutputTimeInfo("initializing values...");

		roadCount = db->getRoadCount();
		dayCount = db->getDayCount();
		timeCount = INTERVALS_PER_DAY;

		values = new double**[roadCount];
		valuesSet = new bool**[roadCount];
		for (int i = 0; i < roadCount; i++)
		{
			values[i] = 0;
			valuesSet[i] = 0;
		}

		int cnt = 0;
		for (int i = 0; i < db->getRoadCount(); i++)
			if ( needCluster(db, i) )
			{
				initializeValues(db, i);
				cnt++;
			}

		OutputTimeInfo("clustered %d roads", cnt);
		//outputRoadValues(db);
	}

	void initializeValues(Database* db, int roadIdx)
	{
		values[roadIdx] = new double*[timeCount];
		valuesSet[roadIdx] = new bool*[timeCount];
		for (int i = 0; i < timeCount; i++)
		{
			values[roadIdx][i] = new double[dayCount];
			valuesSet[roadIdx][i] = new bool[dayCount];
		}

		vector<vector<VD> > data = vector<vector<VD> >(timeCount, vector<VD>(dayCount, VD()));

		for (int j = 0; j < db->getJamCount(roadIdx); j++)
		{
			//if ( (db->getDay(roadIdx, j)+1)%7 >=5 ) continue;

			int ri = timeIndex(db->getJam(roadIdx, j));
			int ti = dayIndex(db->getJam(roadIdx, j));
			double speed = db->getSpeed(roadIdx, j);
			speed = Item::turnSpeed(speed, db->getRoadLLS(roadIdx), db->getDay(roadIdx, j), true);

			data[ri][ti].push_back(speed);
		}

		int totalElements = (dayCount - 1)*timeCount + (2*60)/TIME_INTERVAL;
		int filledElements = 0;
		for (int i = 0; i < timeCount; i++)
		{
			for (int j = 0; j < dayCount; j++)
			{
				if ( data[i][j].empty() )
				{
					values[roadIdx][i][j] = -1;
					valuesSet[roadIdx][i][j] = false;
				}
				else
				{
					//values[roadIdx][i][j] = AverageValue(data[i][j]);
					values[roadIdx][i][j] = MedianValue(data[i][j]);
					valuesSet[roadIdx][i][j] = true;
					filledElements++;
				}
			}
		}

		if ( db->getRoadCount() <= 200 )
			OutputTimeInfo("Road %d (filled = %d, missed = %d elements, jams = %d)", db->getRoadId(roadIdx), filledElements, totalElements - filledElements, db->getJamCount(roadIdx));
	}

	double getValue(Database* db, int roadIdx, const Item& item)
	{
		double speed = getValueRaw(db, roadIdx, item);
		if ( speed == -1 ) return -1;
		speed = Item::turnSpeed(speed, db->getRoadLLS(roadIdx), item.getDay(), false);
		return speed;
	}

	double getValueRaw(Database* db, int roadIdx, const Item& item)
	{
		initialize(db);

		int ti = timeIndex(&item);
		int di = dayIndex(&item);

		double prevSpeed;
		if ( !valuesSet[roadIdx][ti][di] ) prevSpeed = -1;
		else prevSpeed = values[roadIdx][ti][di];

		return prevSpeed;

		double nextSpeed;
		if ( ti+1 >= INTERVALS_PER_DAY || !valuesSet[roadIdx][ti+1][di] ) nextSpeed = -1;
		else nextSpeed = values[roadIdx][ti+1][di];

		if ( prevSpeed == -1 && nextSpeed == -1 ) return -1;
		if ( prevSpeed == -1 ) return nextSpeed;
		if ( nextSpeed == -1 ) return prevSpeed;

		double t = double(item.getM()%TIME_INTERVAL)/double(TIME_INTERVAL);
		double speed = prevSpeed*(1.0 - t) + t*nextSpeed;
		return speed;
	}

	void outputRoadNearest(Database* db)
	{
		initialize(db);

		FILE* f = fopen("nearest.dist_small_t", "w");
		fprintf(f, "%d\n", db->getRoadCount());
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			if ( 15 <= db->getJamCount(i) && db->getJamCount(i) <= 50000 )
				outputRoadNearest(db, i, f);
		}
		fclose(f);
	}

	void outputRoadNearest(Database* db, int roadIdx, FILE* f)
	{
		double LIMIT = 5.0;

		vector<pair<double, int> > pr;
		for (int i = 0; i < db->getRoadCount(); i++)
			if ( i != roadIdx )
			{
				//double d = calcRoadDistanceSmall(db, roadIdx, i);
				double d = calcRoadDistance(roadIdx, i);
				if ( d <= LIMIT )
					pr.push_back(make_pair(d, i));
			}

		if ( pr.empty() ) return;

		sort(pr.begin(), pr.end());
		int sz = (int)pr.size();
		sz = min(sz, 100);
		fprintf(f, "%d %d", db->getRoadId(roadIdx), sz);
		for (int i = 0; i < sz ; i++)
		{
			fprintf(f, " %d %.3lf", db->getRoadId(pr[i].second), pr[i].first);
		}
		fprintf(f, "\n");
	}

	void outputDayDistances(Database* db)
	{
		initialize(db);

		for (int i = 0; i < dayCount; i++)
		{
			OutputInfo("%d", i + 11);
			for (int j = 0; j < dayCount; j++)
			{
				VD sps;
				for (int r1 = 0; r1 < db->getRoadCount(); r1++)
					//for (int r2 = 0; r2 < db->getRoadCount(); r2++)
					{
						double d = calcDayDistance(r1, r1, i, j);
						if ( d >= 1e6 ) continue;

						sps.push_back(d);
					}

				double sum = AverageValue(sps);
				//double sum = MedianValue(sps);
				OutputInfo(" %.3lf", sum);
			}

			OutputInfo("\n");
		}
	}
	
	double calcDayDistance(int roadIdx1, int roadIdx2, int day1, int day2)
	{
		VD diff;
		for (int i = 0; i < timeCount; i++)
		{
			if ( !valuesSet[roadIdx1][i][day1] || !valuesSet[roadIdx2][i][day2] ) continue;

			int hh = i/(60/TIME_INTERVAL) + 16;
			//if ( hh != 21 ) continue;
			if ( hh >= 18 ) continue;

			double sp1 = values[roadIdx1][i][day1];
			double sp2 = values[roadIdx2][i][day2];

			diff.push_back(Abs(sp1 - sp2));
		}

		if ( (int)diff.size() < 4.0 ) return 1e6;
		return MedianValue(diff);
		//return AverageValue(diff);
	}

	double calcRoadDistance(int roadIdx1, int roadIdx2)
	{
		VD diff;
		for (int i = 0; i < dayCount; i++)
		{
			//if ( i+11 < 27 ) continue;

			double d = calcDayDistance(roadIdx1, roadIdx2, i, i);
			if ( d < 1e6 )
			{
				diff.push_back(d);
			}
		}

		if ( (int)diff.size() <= 4 ) return 1e6;
		//return MedianValue(diff);
		return AverageValue(diff);
	}

	double calcRoadDistanceSmall(Database* db, int roadIdx1, int roadIdx2)
	{
		double err = 0, cnt = 0;
		//double mean1, var1, mean2, var2;
		//getMeanAndVariance(db, roadIdx1, mean1, var1);
		//getMeanAndVariance(db, roadIdx2, mean2, var2);

		for (int i = 0; i < db->getJamCount(roadIdx1); i++)
		{
			double speed = db->getSpeed(roadIdx1, i);// - mean1;
			//speed = Item::turnSpeed(speed, db->getDay(roadIdx1, i), true);

			double speed2 = getValue(db, roadIdx2, *db->getJam(roadIdx1, i));// - mean2;
			if ( speed2 == -1 ) continue;

			err += Abs(speed - speed2);
			cnt++;
		}

		if ( cnt <= 10 ) return 1e6;
		err /= cnt;
		return err;
	}

	void outputRoadValues(Database* db)
	{
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			outputRoadValues(db, i);
		}
	}

	void outputRoadValues(Database* db, int roadIdx)
	{
		initialize(db);

		OutputTimeInfo("Building values for road %d...", db->getRoadId(roadIdx));

		FILE* f1 = OpenFile("w", "days_avg.%d", db->getRoadId(roadIdx));
		FILE* f2 = OpenFile("w", "values_avg.%d", db->getRoadId(roadIdx));

		for (int i = 0; i < dayCount; i++)
		{
			for (int j = 0; j < timeCount; j++)
			{
				int hh = j/(60/TIME_INTERVAL) + 16;
				int mm = j%((60/TIME_INTERVAL)) * TIME_INTERVAL;
				Item item(i + 11, hh, mm, -1);

				int ttime = (item.getDay() - 11)*6*15 + (item.getH() - 16)*15 + (item.getM() / 4);

				if ( valuesSet[roadIdx][j][i] )
				{
					fprintf(f1, "%d\n", ttime);
					fprintf(f2, "%.3lf\n", values[roadIdx][j][i]);
				}
			}
		}
		fclose(f1);
		fclose(f2);
	}

	void outputRoadValues2(Database* db, int roadIdx)
	{
		OutputTimeInfo("Building values for road %d...", db->getRoadId(roadIdx));

		FILE* f1 = OpenFile("w", "days.%d", db->getRoadId(roadIdx));
		FILE* f2 = OpenFile("w", "values.%d", db->getRoadId(roadIdx));

		for (int i = 0; i < db->getJamCount(roadIdx); i++)
		{
			double speed = db->getSpeed(roadIdx, i);
			Item* item = db->getJam(roadIdx, i);

			int ttime = (item->getDay() - 11)*10*15 + (item->getH() - 16)*15 + (item->getM() / 4);

			fprintf(f1, "%d\n", ttime);
			fprintf(f2, "%.3lf\n", speed);
		}
		fclose(f1);
		fclose(f2);
	}

	void outputRoadValues3(Database* db, int roadIdx)
	{
		OutputTimeInfo("Building values for road %d...", db->getRoadId(roadIdx));

		FILE* f1 = OpenFile("w", "days_f.%d", db->getRoadId(roadIdx));
		FILE* f2 = OpenFile("w", "values_f.%d", db->getRoadId(roadIdx));

		for (int i = 0; i < db->getJamCount(roadIdx); i++)
		{
			Item* item = db->getJam(roadIdx, i);
			VD sps;
			for (int j = 0; j < db->getJamCount(roadIdx); j++)
			{
				Item* item2 = db->getJam(roadIdx, j);

				int t1 = item->getH()*60 + item->getM();
				int t2 = item2->getH()*60 + item2->getM();

				if ( Abs(t1 - t2) > 10 || item->getDay() != item2->getDay() ) continue;

				sps.push_back(db->getSpeed(roadIdx, j));
			}

			double speed = MedianValue(sps);

			int ttime = (item->getDay() - 11)*10*15 + (item->getH() - 16)*15 + (item->getM() / 4);

			fprintf(f1, "%d\n", ttime);
			fprintf(f2, "%.3lf\n", speed);
		}
		fclose(f1);
		fclose(f2);
	}

	void noiseReduction(Database* db, int roadIdx)
	{
		for (int i = 0; i < db->getJamCount(roadIdx); i++)
		{
			Item* item = db->getJam(roadIdx, i);
			VD sps;
			int beg = max(i-10, 0);
			int end = min(i+10, db->getJamCount(roadIdx));
			for (int j = beg; j < end; j++)
			{
				Item* item2 = db->getJam(roadIdx, j);
				if ( item->getDay() != item2->getDay() ) continue;

				int t1 = item->getH()*60 + item->getM();
				int t2 = item2->getH()*60 + item2->getM();

				if ( Abs(t1 - t2) > 20 ) continue;

				sps.push_back(db->getSpeed(roadIdx, j));
			}

			//double speed = MedianValue(sps);
			double speed = AverageValue(sps);
			//if ( sps.size() >= 5 )
				db->setSpeed(roadIdx, i, speed);
		}
	}

	void noiseReduction(Database* db)
	{
		OutputTimeInfo("Noise reduction...");
		for (int i = 0; i < db->getRoadCount(); i++)
			if ( db->getJamCount(i) >= 2200 )
				noiseReduction(db, i);
	}

	void outputAvgRoadValues(Database* db)
	{
		FILE* f1 = OpenFile("w", "days.avg");
		FILE* f2 = OpenFile("w", "values.avg");

		int ttime = 0;
		for (int i = 0; i < dayCount; i++)
		{
			for (int j = 0; j < timeCount; j++)
			{
				double val = 0, cnt = 0;
				for (int k = 0; k < roadCount; k++)
					if ( valuesSet[k][j][i] )
					{
						val += values[k][j][i];
						cnt++;
					}

				if ( cnt > 0.0 )
				{
					fprintf(f1, "%d\n", ttime);
					fprintf(f2, "%.3lf\n", val/cnt);
				}
				ttime++;
			}

			ttime += 10;
		}
		fclose(f1);
		fclose(f2);
	}

	void outputRoadPredictions(Database* db, int roadIdx, Algorithm* algo)
	{
		initialize(db);
		algo->initialize(db);

		OutputTimeInfo("Building predictions for road %d...", db->getRoadId(roadIdx));

		FILE* f1 = OpenFile("w", "pdays.%d", db->getRoadId(roadIdx));
		FILE* f2 = OpenFile("w", "pvalues.%d", db->getRoadId(roadIdx));

		for (int i = 0; i < dayCount; i++)
		{
			for (int j = 0; j < timeCount; j++)
			{
				int hh = j/(60/TIME_INTERVAL) + 16;
				int mm = j%((60/TIME_INTERVAL)) * TIME_INTERVAL;
				Item item(i + 11, hh, mm, -1);

				int ttime = (item.getDay() - 11)*10*15 + (item.getH() - 16)*15 + (item.getM() / 4);

				if ( item.getDay() == 36 && hh >= 18 )
				{
					double val = algo->predict(db, roadIdx, item);

					fprintf(f1, "%d\n", ttime);
					fprintf(f2, "%.3lf\n", val);
				}
			}
		}
		fclose(f1);
		fclose(f2);
	}

	void outputRoadPredictions(Database* db, Algorithm* avgAlgorithm)
	{
		FILE* f = OpenFile("w", "predictions.diff");

		for (int i = 0; i < db->getRoadCount(); i++)
			for (int j = 0; j < timeCount; j++)
			{
				int hh = j/INTERVALS_PER_HOUR + 16;
				int mm = (j%INTERVALS_PER_HOUR) * TIME_INTERVAL;
				Item it(41, hh, mm, -1);
				if ( !valuesSet[i][j][dayIndex(&it)] ) continue;

				double val = values[i][j][dayIndex(&it)];
				double ans = avgAlgorithm->predict(db, i, it);

				fprintf(f, "%d %d %d:%02d %.3lf\n", db->getRoadId(i), it.getDay(), it.getH(), it.getM(), val - ans);
			}

		fclose(f);
	}



	void outputLLS(Database* db)
	{
		FILE* f = fopen("lls.txt", "w");
		double sum = 0, cnt = 0;
		for (int i = 0; i < db->getRoadCount(); i++)
		{
			double b1, b2;
			calcLLS(db, i, b1, b2);
			fprintf(f, "%d %.3lf (%d)\n", db->getRoadId(i), -b2, db->getJamCount(i));

			//if ( db->getJamCount(i) <= 2000 && db->getJamCount(i) >= 500 )
			{
				sum += -b2;
				cnt++;
			}
		}

		sum /= cnt;
		OutputTimeInfo("Avg b2: %.3lf", sum);
		fclose(f);
	}

	void calcLLS(Database* db, int roadIdx, double& b1, double& b2)
	{
		double sumx = 0, sumx2 = 0, sumy = 0, sumxy = 0;
		for (int i = 0; i < db->getJamCount(roadIdx); i++)
		{
			//if ( db->getDay(roadIdx, i) == db->getSkippedDay() ) continue;

			double x = db->getJam(roadIdx, i)->getDay() - 11;
			double y = db->getSpeed(roadIdx, i);

			sumx += x;
			sumy += y;
			sumx2 += Sqr2(x);
			sumxy += x*y;
		}

		double n = db->getJamCount(roadIdx);
		if ( n <= 1300 || n*sumx2 - sumx*sumx == 0.0)
		{
			b1 = b2 = -0.12;
		}
		else
		{
			if ( n*sumx2 - sumx*sumx == 0.0 )
				OutputTimeInfo("Wrong lls at %d (%.3lf %.3lf)", db->getRoadId(roadIdx), sumx, sumx2);

			b2 = (n*sumxy - sumx*sumy)/(n*sumx2 - sumx*sumx);
			b1 = sumy/n - b2*sumx/n;
		}

		assert(b1 == b1);
		assert(b2 == b2);
	}

	void outputRoadMeanAndVariance(Database* db)
	{
		FILE* f = OpenFile("w", "mean_var.txt");

		for (int i = 0; i < db->getRoadCount(); i++)
		{
			double mean, var;
			getMeanAndVariance(db, i, mean, var);

			fprintf(f, "%d %.3lf %.3lf\n", db->getRoadId(i), mean, var);
		}

		fclose(f);
	}

	double roadMean[Database::MAX_ROADS];
	double roadVar[Database::MAX_ROADS];

	double getMean(Database* db, int roadIdx)
	{
		if ( roadMean[roadIdx] == -1 )
		{
			VD sps;
			for (int j = 0; j < db->getJamCount(roadIdx); j++)
			{
				if ( (db->getDay(roadIdx, j) + 1)%7 >= 4 ) continue;

				double speed = db->getSpeed(roadIdx, j);
				sps.push_back(speed);
			}

			roadMean[roadIdx] = AverageValue(sps);
		}
		
		return roadMean[roadIdx];
	}

	double getVar(Database* db, int roadIdx)
	{
		if ( roadVar[roadIdx] == -1 )
		{
			double mean = getMean(db, roadIdx);
			VD sps;
			for (int j = 0; j < db->getJamCount(roadIdx); j++)
			{
				if ( (db->getDay(roadIdx, j) + 1)%7 >= 4 ) continue;

				double speed = db->getSpeed(roadIdx, j);
				sps.push_back(Sqr2(speed - mean));
			}
			roadVar[roadIdx] = AverageValue(sps);
			roadVar[roadIdx] = sqrt(roadVar[roadIdx]);
		}

		return roadVar[roadIdx];
	}

	void getMeanAndVariance(Database* db, int roadIndex, double& mean, double& var)
	{
		mean = getMean(db, roadIndex);
		var = getVar(db, roadIndex);
	}

	void fitMeanAndVariance(Database* db, int roadIndex, double& speed)
	{
		double mean = getMean(db, roadIndex);
		double var = getVar(db, roadIndex);

		var = max(var, 20.0);

		if ( db->getJamCount(roadIndex) <= 30 ) return;

		speed = min(speed, mean + var);
		speed = max(speed, mean - var);
	}

	void clusterDays(Database* db, int roadIdx)
	{
		/*VVD dist = VVD(dayCount, VD(dayCount, -1));
		for (int i = 0; i < dayCount; i++)
			for (int j = 0; j < dayCount; j++)
			{
				dist[i][j] = calcDayDistance(roadIdx1, roadIdx2, i, j);
			}*/

		FILE* f = OpenFile("w", "roads%d.dist", db->getRoadId(roadIdx));
		
		for (int j = 0; j < dayCount; j++)
		{
			if ( (j+12)%7 >= 5 ) continue;

			fprintf(f, "%d", j+11);
			for (int k = 0; k < dayCount; k++)
			{
				if ( (k+12)%7 >= 5 ) continue;

				double dist = calcDayDistance(roadIdx, roadIdx, j, k);
				fprintf(f, " %.4lf", dist);
			}
			fprintf(f, "\n");
		}
		fclose(f);
	}

	bool nearestInitialized;
	map<int, vector<pair<int, double> > > nearest;

	void readNearest(Database* db)
	{
		initialize(db);
		OutputTimeInfo("Reading nearest data...");

		FILE* f = fopen("nearest.dist2", "r");
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
				{
					v.push_back(make_pair(db->getRoadIndex(id2), d));
					//if ( db->getJamCount(db->getRoadIndex(id2)) )
					//OutputInfo("%d\n", id2);
				}
			}

			if ( db->getRoadIndex(id) != -1 && !v.empty() )
			{
				nearest[db->getRoadIndex(id)] = v;
				//OutputInfo("%d\n", id);
			}
		}
		fclose(f);
		OutputTimeInfo("done");
	}

	vector<pair<int, double> > getNeighbors(Database* db, int roadIdx)
	{
		if ( !nearestInitialized )
		{
			readNearest(db);
			nearestInitialized = true;
		}

		return nearest[roadIdx];
	}

	double predict(Database* db, int roadIdx, const Item& item)
	{
		vector<pair<int, double> > neig = getNeighbors(db, roadIdx);
		double LIMIT = 7.5;

		if ( neig.empty() ) 
		{
			//OutputInfo("%d\n", db->getRoadId(roadIdx));
			return -1;
		}

		neig.insert(neig.begin(), make_pair(roadIdx, 0.1));
		int ti = timeIndex(&item);

		VD sps;
		double cnt2 = 0;
		for (int i = 0; i < (int)neig.size() && i < 50; i++)
		{
			if ( neig[i].second > LIMIT ) continue;

			bool useRoad = false;
			for (int j = 0; j < dayCount; j++)
			{
				if ( valuesSet[neig[i].first] == 0 )
					OutputTimeInfo("NULL at %d (%d, %d)", neig[i].first, db->getRoadId(neig[i].first), db->getJamCount(neig[i].first));

				assert(valuesSet[neig[i].first] != 0);
				if ( (j+12)%7 >= 4 ) continue;

				double speed = -1;
				if ( valuesSet[neig[i].first][ti][j] )
				{
					speed = values[neig[i].first][ti][j];
				}

				//Item nItem(j+11, item.getH(), item.getM(), -1);
				//speed = getValueRaw(db, neig[i].first, nItem);

				if ( speed == -1 ) continue;
				sps.push_back(speed);
				useRoad = true;
			}

			if ( useRoad ) cnt2++;
		}

		if ( (int)sps.size() <= 10 || cnt2 <= 3 ) 
		{
			return -1;
		}

		double avg = AverageValue(sps);
		//double avg = MedianValue(sps);
		avg = Item::turnSpeed(avg, db->getRoadLLS(roadIdx), item.getDay(), false);
		return avg;
	}
};

#endif
