use strict;
use warnings;

use Socket;

my $port = 43;
my @whoisServers = (
	"whois.arin.net",
	"whois.ripe.net",
	"whois.apnic.net",
	"whois.lacnic.net",
	"whois.afrinic.net"
);

my $userHost = $ARGV[0];

if($#ARGV != 0
   || $userHost !~ /^(?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(?=\.?\d)\.)){4}$/)
{
	die "Usage: whois.pl IPv4\r\n";
}

my $whoisServer = $whoisServers[3];
my $whoisAddr = inet_aton($whoisServer);

socket S, PF_INET, SOCK_STREAM, getprotobyname 'tcp';
connect S, sockaddr_in($port, $whoisAddr);
send S, "$userHost\r\n", 0;

my $res = "";
while(defined(my $line = <S>))
{
	$res .= $line;
}
close S;

if($res =~ /^country:\s+(\w+)$/mi)
{
	print $1;
}
else
{
	print "Unable to resolve country";
}