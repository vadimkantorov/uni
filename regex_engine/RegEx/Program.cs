using System;
using System.Collections.Generic;
using System.IO;

namespace RegEx
{
	class Program
	{
		static void Main(string[] args)
		{
			const string inputFilePath = "input.txt";
			
			if(!File.Exists(inputFilePath))
			{
				Console.WriteLine("Next time please store your regex in the <input.txt> file");
				return;
			}
			
			string regex = File.ReadAllLines(inputFilePath)[0];
			
			var parser = new RegularExpressionASTBuilder(regex);
			var exprTree = parser.BuildExpressionTree();
			
			var regLanBuilder = new RegularLanguageBuilder(exprTree);
            var lang = regLanBuilder.BuildLanguage();
			var recognizer = new LanguageRecognizer<RegularLanguageState>(lang);

            File.WriteAllText("output.txt",lang.ToString());

			Console.WriteLine("The automaton was dumped to output.txt");
            Console.WriteLine();             
            Console.WriteLine(@"You can test if a string fits the language.{0}Feel free to write some words straight here.{0}If you are tired, press Ctrl+C to exit:", Environment.NewLine);
            while(true)
            {
            	string s = Console.ReadLine();
				
				if(recognizer.Accepts(s))
					Console.WriteLine("Yahoo! The string perfectly fits! Feel free to enter another one!");
				else
					Console.WriteLine("DOH! Don't feel upset, but the string is not from this language. Nonetheless, we invite you to test another one!");
            }
		}
	}
}
