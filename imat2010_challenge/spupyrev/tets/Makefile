# Variables

CXX = g++
CXXFLAGS = -Wall -O2

OBJ = main.o time_utils.o input_output.o common_utils.o random_utils.o database.o 

# Targets

main: $(OBJ)
	$(CXX) $(CXXFLAGS) -o main $(OBJ)


main.o: main.cpp
	$(CXX) $(CXXFLAGS) -c main.cpp
	


clean :
	del common_utils.o
	del database.o
	del main.o
	del random_utils.o
	del time_utils.o
	del main.0
	del main.exe	
	