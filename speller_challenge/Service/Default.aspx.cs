using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Speller;

public class SpellerService : System.Web.UI.Page
{
    public void Page_Load()
    {
        if (speller == null)
        {
            speller = new NorvigSpeller(GetWords());
        }
        Response.ContentType = "text/plain;charset=utf-8";
        var query = Request.QueryString["q"];
        if (query == null)
        {
            throw new ArgumentNullException("query");
        }
        Response.Write(speller.CorrectQuery(query) + "\t1.0");
    }
    
    private IEnumerable<string> GetWords()
    {
        return
            new Regex("[a-z]+")
            .Matches(File.ReadAllText(Server.MapPath("dict.txt")).ToLower())
            .Cast<Match>()
            .Select(x => x.Value);
    }

    private static ISpeller speller;
}