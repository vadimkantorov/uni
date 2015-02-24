#pragma once
#ifndef DATABASE_H
#define DATABASE_H

#include <vector>
using namespace std;

#include "algorithm.h"
#include "time_utils.h"

class Database;

struct Item
{
	//min from 11th day, 16:00
	int time;
	double speed;

	Item() {}
	Item(int time, double speed):time(time), speed(speed) {}
	Item(int day, int hh, int mm, double speed):speed(speed) 
	{
		time = (day-11)*6*60 + (hh-16)*60 + mm;
	}

	static double turnSpeed(double speed, double lls, int day, bool up)
	{
		//if ( lls < 0 ) lls = -lls;
		//lls = Abs(lls);
		lls = 0.09;
		day = day - 11;
		if ( up )
			speed += lls*double(day);
		else
			speed -= lls*double(day);

		speed = max(speed, 0.0);
		             
		return speed;
	}

	inline double getOrderK() const
	{
		int tt = (getH() - 18)*15 + (getM() - 2)/4;
		double kt = 1.0 + 0.1*double(tt);
		return kt;
	}


	inline int getDay() const
	{
		return (time/(6*60) + 11);
	}

	inline int getH() const
	{
		return ((time%(6*60))/60 + 16);
	}

	inline int getM() const
	{
		return (time%60);
	}
};

class Database
{
public:
	static const int MAX_ROADS = 86228;
	static const int MAX_ROADS_ID = 953792;
	static const int MAX_JAMS = 29209061;

private:
	int skippedDay;
	vector<Item> taskItems;
	vector<int> taskIds;

	Item** items[MAX_ROADS];
	int itemsCount[MAX_ROADS];
	int roadId[MAX_ROADS];
	int roadIndex[MAX_ROADS_ID];
	double roadLength[MAX_ROADS];
	double roadAvgSpeed[MAX_ROADS];

	double roadLLS[MAX_ROADS];

	int roadCount;
	int jamCount;
	int dayCount;


	void loadJams(int skippedDay);
	void createRoadData(vector<Item>&, int);
	void loadEdges();
	void loadNodes();
	void check();
	void outputDeatailErrors(vector<pair<double, pair<double, int> > >& errors);

public:
	void loadLLS();
    Database(int skippedDay);
    ~Database();

	inline int getDayCount() const
	{
		return dayCount;
	}

	inline int getRoadCount() const
	{
		return roadCount;
	}

	inline int getJamCount() const
	{
		return jamCount;
	}

	inline int getSkippedDay() const
	{
		return skippedDay;
	}

	inline int getJamCount(int roadIndex) const
	{
		return itemsCount[roadIndex];
	}

	inline int getRoadId(int roadIndex) const
	{
		return roadId[roadIndex];
	}

	inline int getRoadIndex(int roadId) const
	{
		return roadIndex[roadId];
	}

	inline Item* getJam(int roadIndex, int jamIndex) const
	{
		return items[roadIndex][jamIndex];
	}

	inline double getSpeed(int roadIndex, int jamIndex) const
	{
		return items[roadIndex][jamIndex]->speed;
	}

	inline void setSpeed(int roadIndex, int jamIndex, double value)
	{
		items[roadIndex][jamIndex]->speed = value;
	}

	inline int getDay(int roadIndex, int jamIndex) const
	{
		return items[roadIndex][jamIndex]->getDay();
	}

	inline int getH(int roadIndex, int jamIndex) const
	{
		return items[roadIndex][jamIndex]->getH();
	}

	inline int getM(int roadIndex, int jamIndex) const
	{
		return items[roadIndex][jamIndex]->getM();
	}

	inline double getRoadAvgSpeed(int roadIndex) const
	{
		return roadAvgSpeed[roadIndex];
	}

	inline double getRoadLLS(int roadIndex) const
	{
		return roadLLS[roadIndex];
	}

	double score(Algorithm* algorithm, int withDetails = -1);
	double scorePart(Algorithm* algorithm, int roadIdx);
};

#endif
