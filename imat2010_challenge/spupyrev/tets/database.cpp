#pragma warning(disable:4996)

#include <cassert>
#include <vector>
#include <algorithm>
#include <map>
using namespace std;

#include "database.h"
#include "input_output.h"
#include "time_utils.h"
#include "common_utils.h"
#include "defs.h"

const char* DATABASE_HOME = "H:\\Projects\\SlowMotion\\imat2010\\";

Database::Database(int skippedDay_)
{
	skippedDay = skippedDay_;

	loadEdges();
	loadNodes();
	loadJams(skippedDay_);

	//check();
}

Database::~Database()
{
	for (int i = 0; i < roadCount; i++)
	{
		for (int j = 0; j < itemsCount[i]; j++)
			delete items[i][j];
		if ( items[i] != 0 )
			delete[] items[i];
	}
}

void Database::check()
{
	assert(roadId[0] == 317744);
	assert(roadLength[0] == 39.93);

	Item item(20, 18, 20, -1);
	assert(item.getH() == 18);
	assert(item.getM() == 20);
	assert(item.getDay() == 20);
}

vector<Item> fixItems(vector<Item>& curItems, int curRoadId)
{
	vector<Item> goodItems;
	VD sps;
	int curTime = -1;

	for (int i = 0; i < (int)curItems.size(); i++)
	{
		if ( curItems[i].time == curTime )
		{
			sps.push_back(curItems[i].speed);
		}
		else
		{
			if ( !sps.empty() ) 
				goodItems.push_back(Item(curTime, MedianValue(sps)));
			sps.clear();
			sps.push_back(curItems[i].speed);
			curTime = curItems[i].time;
		}
	}

	if ( !sps.empty() ) 
		goodItems.push_back(Item(curTime, MedianValue(sps)));

	return goodItems;
}

void Database::createRoadData(vector<Item>& curItems, int curRoadId)
{
	while ( roadId[roadCount] != curRoadId )
	{
		itemsCount[roadCount] = 0;
		items[roadCount] = 0;
		roadCount++;
	}

	curItems = fixItems(curItems, curRoadId);

	items[roadCount] = new Item*[curItems.size()];
	itemsCount[roadCount] = (int)curItems.size();
	for (int i = 0; i < itemsCount[roadCount]; i++)
	{
		items[roadCount][i] = new Item(curItems[i].time, curItems[i].speed);
		dayCount = max(dayCount, curItems[i].getDay() - 10);
	}
	curItems.clear();
	roadCount++;
}

void Database::loadJams(int skippedDay)
{
	char fileName[256];
	sprintf(fileName, "%sjams.txt", DATABASE_HOME);
	OutputTimeInfo("Loading jams (day=%d) from '%s'...", skippedDay, fileName);

	FILE* f = OpenFile("r", fileName);

	char* buf = new char[128];
	char* buf_init = buf;
	int line = 0;

	int curRoadId = -1;
	vector<Item> curItems;
	roadCount = jamCount = dayCount = 0;

	while ( ReadLine(buf, f, 128) )
	{
		int id, day, hh, mm, speed;

		buf = ReadInt(buf, id);
		buf = ReadInt(buf, day);
		buf = ReadInt(buf, hh);
		buf = ReadInt(buf, mm);
		buf = ReadInt(buf, speed);

		if ( day == 41 && hh >= 18 ) 
		{
			buf = buf_init;
			line++;
			continue;
		}

		if ( day == skippedDay && hh >=18 ) 
		{
			//skip
			taskItems.push_back(Item(day, hh, mm, (double)speed));
			taskIds.push_back(id);
		}
		else
		{
			jamCount++;
			if ( curRoadId != id )
			{
				if ( curRoadId != -1 )
					createRoadData(curItems, curRoadId);
				curRoadId = id;
			}

			curItems.push_back(Item(day, hh, mm, (double)speed));
		}

		line++;
		if ( line%10000000 == 0 )
			OutputTimeInfo("Read %d jam elements", line);
		buf = buf_init;
	}

	if ( !curItems.empty() )
	{
		createRoadData(curItems, curRoadId);
	}

	delete[] buf;
	fclose(f);
	OutputTimeInfo("jams loaded (%d lines)", line);
}

void Database::loadEdges()
{
	char fileName[256];
	sprintf(fileName, "%sedge_data.txt", DATABASE_HOME);
	OutputTimeInfo("Loading edges from '%s'...", fileName);

	FILE* f = OpenFile("r", fileName);

	char* buf = new char[128];
	char* buf_init = buf;
	int line = 0;
	for (int i = 0; i < MAX_ROADS_ID; i++)
		roadIndex[i] = -1;

	while ( ReadLine(buf, f, 128) )
	{
		int id;
		double length, speed;

		buf = ReadInt(buf, id);
		buf = ReadDouble(buf, length);
		buf = ReadDouble(buf, speed);

		roadIndex[id] = line;
		roadId[line] = id;
		roadLength[line] = length;
		roadAvgSpeed[line] = speed;

		line++;
		buf = buf_init;
	}

	fclose(f);
	OutputTimeInfo("edges loaded (%d lines)", line);

	//loadLLS();
}

void Database::loadLLS()
{
	FILE* f = OpenFile("r", "lls.txt");

	char* buf = new char[128];
	char* buf_init = buf;
	for (int i = 0; i < MAX_ROADS; i++)
		roadLLS[i] = 0;

	while ( ReadLine(buf, f, 128) )
	{
		int id;
		double b2;

		buf = ReadInt(buf, id);
		buf = ReadDouble(buf, b2);

		if ( roadIndex[id] != -1 )
			roadLLS[roadIndex[id]] = b2;

		buf = buf_init;
	}  

	delete[] buf;	 
	fclose(f);
}

void Database::loadNodes()
{
}

void Database::outputDeatailErrors(vector<pair<double, pair<double, int> > >& errors)
{
	FILE* fError = OpenFile("w", "errors.txt");
	sort(errors.begin(), errors.end());
	map<int, vector<double> > maxErrors;

	for (int i = (int)errors.size() - 1; i >= 0; i--)
	{
		int ind = errors[i].second.second;
		Item item = taskItems[ind];
		int roadId = taskIds[ind];
		int ri = roadIndex[roadId];
		double ans = errors[i].second.first;
		fprintf(fError, "%.5lf (ans=%.5lf, my=%.5lf)", errors[i].first, item.speed, ans);
		fprintf(fError, " %d %d %d:%02d %d\n", roadId, item.getDay(), item.getH(), item.getM(), getJamCount(ri));

		if ( maxErrors[roadId].size() < 50 )
			maxErrors[roadId].push_back(errors[i].first);
	}
	fclose(fError);

	vector<pair<double, int> > mev;
	for (map<int, vector<double> >::iterator it = maxErrors.begin(); it != maxErrors.end(); it++)
	{
		double av = Sum((*it).second);
		mev.push_back(make_pair(av, (*it).first));
	}
	sort(mev.begin(), mev.end());
	fError = OpenFile("w", "errors_sum.txt");
	for (int i = (int)mev.size() - 1; i >= 0; i--)
	{
		fprintf(fError, "%.3lf (%d, %d) %d\n", mev[i].first, (int)maxErrors[mev[i].second].size(), getJamCount(roadIndex[mev[i].second]), mev[i].second);
	}
	fclose(fError);
}

double Database::score(Algorithm* algorithm, int withDetails)
{
	algorithm->initialize(this);

	if ( skippedDay == -1 )
	{
		//from file
		char fileName[256];
		sprintf(fileName, "%stask.txt", DATABASE_HOME);
		OutputTimeInfo("Predicting from '%s'...", fileName);

		FILE* f = OpenFile("r", fileName);
		FILE* fAns = OpenFile("w", "predictions%d.txt", withDetails);

		char* buf = new char[128];
		char* buf_init = buf;
		int line = 0;

		while ( ReadLine(buf, f, 128) )
		{
			int id, day, hh, mm;

			buf = ReadInt(buf, id);
			buf = ReadInt(buf, day);
			buf = ReadInt(buf, hh);
			buf = ReadInt(buf, mm);

			Item item(day, hh, mm, -1);
			double ans = algorithm->predict(this, roadIndex[id], item);
			assert(ans >= 0.0);
			fprintf(fAns, "%d\t%d %d:%02d\t %.3lf\n", id, day, hh, mm, ans);

			line++;
			buf = buf_init;
		}

		delete[] buf;	 
		fclose(f);
		fclose(fAns);
		OutputTimeInfo("done %d predictions", line);
		return -1;
	}
	else
	{
		//from memory
		OutputTimeInfo("Predicting from memory");

		FILE* fAns = 0;
		if ( withDetails >= 0 )
		{
			fAns = OpenFile("w", "predictions.txt", withDetails);
		}
		vector<pair<double, pair<double, int> > > errors;

		int cnt100 = (int)taskItems.size() / 10;
		double res = 0, taskCount = 0;
		for (int i = 0; i < (int)taskItems.size(); i++)
		{
			//if ( taskItems[i].getH() != 21 ) continue;

			int jc = getJamCount(roadIndex[taskIds[i]]);
			//if ( jc < 2200 ) continue;
			//if ( i == 0 || taskIds[i] != taskIds[i-1] )
			//	OutputInfo("%d\n", taskIds[i]);

			double ans = algorithm->predict(this, roadIndex[taskIds[i]], taskItems[i]);
			//if ( ans == -1 ) continue;

			assert(ans >= 0.0);

			double kt = taskItems[i].getOrderK();
			double kl = roadLength[roadIndex[taskIds[i]]]/120.0;
			double diff = Abs(taskItems[i].speed - ans);

			res += kt*kl*diff;
			//res += kl*diff;
			taskCount++;

			if ( withDetails >= 0 )
			{
				fprintf(fAns, "%d\t%d %d:%02d\t %.3lf\n", taskIds[i], taskItems[i].getDay(), taskItems[i].getH(), taskItems[i].getM(), ans);
				//fprintf(fAns, "%d\t%d %d:%02d\t ??\n", taskIds[i], taskItems[i].getDay(), taskItems[i].getH(), taskItems[i].getM());
				//fprintf(fAns, "%d\n", taskIds[i]);
				errors.push_back(make_pair(diff, make_pair(ans, i)));
			}

			if ( cnt100 > 10 && i%cnt100 == 0 )
				OutputTimeInfo("Predicted %.1lf%%%%", double(i+1)*100.0/double(taskItems.size()));
		}
		res /= taskCount;

		if ( withDetails >= 0 )
		{
			outputDeatailErrors(errors);
			fclose(fAns);
		}
		OutputTimeInfo("done %.0lf predictions", taskCount);
		return res;
	}
}

double Database::scorePart(Algorithm* algorithm, int roadIdx)
{
	double res = 0, taskCount = 0;
	for (int i = 0; i < (int)taskItems.size(); i++)
	{
		if ( roadIdx != -1 && roadIndex[taskIds[i]] != roadIdx ) continue;
		//if ( taskItems[i].getH() != 21 ) continue;

		double ans = algorithm->predict(this, roadIndex[taskIds[i]], taskItems[i]);
		assert(ans >= 0.0);
		
		double kt = taskItems[i].getOrderK();
		double kl = roadLength[roadIndex[taskIds[i]]]/120.0;
		double diff = Abs(taskItems[i].speed - ans);

		res += kt*kl*diff;
		taskCount++;
	}
	assert(taskCount > 0.0);
	res /= taskCount;
	return res;
}
