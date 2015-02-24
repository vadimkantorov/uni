#!/usr/bin/perl -w

chomp($inputFile = <>);
open STDIN, "<$inputFile";
open STDOUT, ">out.txt";
binmode(STDIN);

$cols = 16;

for($lines = 0; !eof(STDIN); $lines++)
{
	printf("%06X ", $lines*$cols);
	
	$read = read STDIN, $data, $cols;
	for(split //, $data)
	{
		print uc(unpack("H2", $_))." ";
	}
	
	print "   "x($cols-$read);
	
	for(split //, $data)
	{
		print ord($_) >= 32? $_ : '.';
	}
	print "\n";
}