using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace IndexingService
{
	public class MorphologyAnalyzer
	{
		public MorphologyAnalyzer(string serviceIp)
		{
			this.serviceIp = serviceIp;
		}

		public Paradigm[] Analyze(string word)
		{
			using(var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				socket.Connect(IPAddress.Parse(serviceIp), 1234);
				Encoding cp1251 = Encoding.GetEncoding(1251);
				socket.Send(cp1251.GetBytes(word + "\n"));
				var buffer = new byte[1];
				var message = new List<byte>();
				while(true)
				{
					socket.Receive(buffer);
					byte b = buffer.First();
					if(b == '\n')
						break;
					message.Add(b);
				}
				XDocument xDoc = XDocument.Parse(cp1251.GetString(message.ToArray()));
				var result =
					from p in xDoc.Root.Elements("paradigm")
					select
						new Paradigm
							{
								Lemma = p.Element("lemma").Value.ToLower(),
								PartOfSpeech = p.Element("ps").Value.ToLower(),
								Inflections = p
									.Element("inflections")
									.Elements("i")
									.Select(x => x.Value.ToLower())
									.OrderBy(x => x == word.ToLower() ? 0 : 1)
									.ToArray(),
							};
				return result.ToArray();
			}
		}

		private readonly string serviceIp;
	}
}