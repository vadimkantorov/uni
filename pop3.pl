use strict;
use warnings;

use Socket;
use Encode;
use MIME::QuotedPrint;
use MIME::Base64;
use Term::ReadKey;

if (@ARGV != 2) 
{
    die "Usage: pop.pl <pop3-server> <userName>\n";
}

my ($server, $userName) = @ARGV;
my $port = 110;

ReadMode("noecho");
print "Please enter password: \n";
chomp(my $password = <STDIN>);
#my $password = "kantorovvadim";
ReadMode("original");

my $serverAddr = inet_aton($server);

handleError("create socket to $server", sub {socket SOCKET, PF_INET, SOCK_STREAM, getprotobyname('tcp') });

testResponseFirstLine("connect to $server", sub {connect SOCKET, sockaddr_in($port, $serverAddr);});
testResponseFirstLine("send the user name to $server", sub {send SOCKET, "USER $userName\n", 0;});

my $passResp = testResponseFirstLine("send the password to $server", sub {send SOCKET, "PASS $password\n", 0;});
my ($messageCnt) = $passResp =~ /^\+OK\s(\d+)/;

print "Total: $messageCnt\n";
print "-----------------------\n";
print "#\t\tFrom\n";

for(my $i = 1; $i <= $messageCnt; $i++)
{
	testResponseFirstLine("receive the message #$i", sub {send SOCKET, "TOP $i\n", 0;});
	my $response = "";
	do
	{
      recv SOCKET, $_, 600, 0;
      $response .= $_;
    }
	while (!/\r?\n\.\r?\n$/m);
	
	my ($from) = $response =~ /\nfrom:\s*(.*?)\n/i;
    decodeFromField($from);
	
	print "$i\t\t$from\n";
}

shutdown SOCKET, 2;

sub handleError
{
	$_[1]->() or die "Cannot $_[0]. The error: $!";
}

sub decodeFromField {
  my ($p,$q);
  $_[0] =~ s/=\?(.*?)\?(.)\?(.*?)\?=/  
    
	$p = (lc $2 eq 'b')? decode_base64($3): decode_qp($3);
    eval {$q = encode 'cp866', decode $1, $p };
    $@? $p:$q
  
  /gex;
  $_[0];
}

sub testResponseFirstLine
{
	handleError(@_);
	my $response = <SOCKET>;
	
	die "An error occured. The server returned '$response' while trying to $_[0]" unless $response =~ /^\+OK/;
	return $response;
};