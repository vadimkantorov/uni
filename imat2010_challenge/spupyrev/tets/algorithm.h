#pragma once
#ifndef ALGORITHM_H
#define ALGORITHM_H

#include "database.h"

class Algorithm
{

public:
    Algorithm() {}
    virtual ~Algorithm() {}
    
    virtual void initialize(class Database* db) = 0;
    virtual double predict(class Database* db, int roadIndex, const struct Item& item) = 0;
	virtual double predictRaw(class Database* db, int roadIndex, const struct Item& item) 
	{
		//assert(false);
		return 0;
	}
};

#endif
