#!/usr/bin/perl -w

open(DFA,"<","dfa.txt");
open(STDIN,"<","text.txt");

while(<DFA>)
{
	($from, $by, $to, $fun) = /^(\d+) (.) (\d+) ?(.*)$/;
	$dfa{$from}{$by} = [$to,$fun];
}

chomp($text = <>);

$state = 0;
for(split //, $text)
{
	if($pos = $dfa{$state}{$_})
	{
		$state =  $pos->[0];
		eval($pos->[1]);
	}
	else
	{
		last;
	}
}

print "\nLast state: $state";