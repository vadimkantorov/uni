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
	rm -f $(OBJ)
	rm -f main
	rm -f main.exe	
	