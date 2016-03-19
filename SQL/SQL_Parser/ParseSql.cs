﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SQL_Parser.Model;
using SQL_Parser.Grammar;

namespace SQL_Parser
{
    public class ParserSql
    {
        private List<Keyword> _keywords = new List<Keyword>()
        {
            new Keyword("select", "(SELECT|select)"),
            new Keyword("from", "(FROM|from)"),
            new Keyword("where", "(WHERE|where)"),
            new Keyword("space", @"(\s+)"),
            new Keyword("semicolon", "(;)")
        };


        public string ValidateSQL(string INPUT = "select cli_name from   t_client where warunek;")
        {
            // tokenize input
            var keywordTokens = TokenizeKeywords(INPUT, _keywords);
            var tokens = TokenizeCustomText(INPUT, keywordTokens);

            #region DEBUG
            /*
       Console.WriteLine(INPUT);
       Console.WriteLine("keywords");
       foreach (var token in keywordTokens)
           Console.Write("<" + token.Name + "> ");

       Console.WriteLine("\nall");
*/
            var OUTPUT = "";
            foreach (var token in tokens)
            {
                if (token.userMade)
                    OUTPUT += ("|" + token.Name + "| ");

                else
                    OUTPUT += ("<" + token.Name + "> ");
            }

            #endregion

            // validate

            var state = new Word_STATEMENT();
            var ddd = state.IsMatch(tokens);

            Console.WriteLine("end " + ddd);
            Console.Read();

            return OUTPUT;

        }

        private List<Token> TokenizeKeywords(string INPUT, List<Keyword> keywords)
        {
            var tokens = new List<Token>();

            foreach (var keyword in keywords)
            {
                foreach (Match match in new Regex(keyword.Regex).Matches(INPUT))
                {
                    tokens.Add(new Token(keyword.Name, match));
                }
            }
            return tokens.OrderBy(x => x.Index).ToList();
        }

        private List<Token> TokenizeCustomText(string INPUT, List<Token> keywordTokens)
        {
            var tokens = new List<Token>(keywordTokens);

            for (int i = 0; i < keywordTokens.Count() - 1; i++)
            {
                var token = keywordTokens.ToArray()[i];
                var nextToken = keywordTokens.ToArray()[i + 1];

                if (nextToken.Index != token.Index + token.Length)
                {
                    var index = token.Index + token.Length;
                    var length = nextToken.Index - (token.Index + token.Length);
                    var text = INPUT.Substring(index, length);
                    var newToken = new Token(text, index, length);
                    newToken.userMade = true;
                    tokens.Add(newToken);
                }
            }
            return tokens.OrderBy(x => x.Index).ToList();
        }

    }
}
