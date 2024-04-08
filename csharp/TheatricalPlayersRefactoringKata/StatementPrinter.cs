using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private (Play, int, int) CalculatePlay(Performance perf, Dictionary<string, Play> plays)
        {
            var play = plays[perf.PlayID];
            var price = 0;
            price = CalculateSingelAmount(perf, play);
            return (
                play, price, perf.Audience
            );
        }

        private static int CalculateSingelAmount(Performance perf, Play play)
        {
            int thisAmount;
            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = 40000;
                    if (perf.Audience > 30)
                    {
                        thisAmount += 1000 * (perf.Audience - 30);
                    }

                    break;
                case "comedy":
                    thisAmount = 30000;
                    if (perf.Audience > 20)
                    {
                        thisAmount += 10000 + 500 * (perf.Audience - 20);
                    }

                    thisAmount += 300 * perf.Audience;
                    break;
                default:
                    throw new Exception("unknown type: " + play.Type);
            }

            return thisAmount;
        }

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            var toReturn = new List<(Play, int, int)>();
            CultureInfo cultureInfo = new CultureInfo("en-US");
            var totalAmount = CalculateInvoice(invoice, plays, toReturn, out var volumeCredits);

            foreach (var pay in toReturn)
            {
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", pay.Item1.Name,
                    Convert.ToDecimal(pay.Item2 / 100), pay.Item3);
            }

            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += String.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }


        private int CalculateInvoice(Invoice invoice, Dictionary<string, Play> plays, List<(Play, int, int)> toReturn,
            out int volumeCredits)
        {
            var totalAmount = 0;
            volumeCredits = 0;

            foreach (var perf in invoice.Performances)
            {
                var pay = CalculatePlay(perf, plays);
                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == pay.Item1.Type) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);

                // print line for this order

                totalAmount += pay.Item2;
                toReturn.Add(pay);
            }

            return totalAmount;
        }

        public string PrintHTML(Invoice invoice, Dictionary<string, Play> plays)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.AppendLine("<html>");
            resultBuilder.AppendLine("<body>");
            resultBuilder.AppendLine(string.Format("<h1>Statement for {0}</h1>", invoice.Customer));
            var toReturn = new List<(Play, int, int)>();
            CultureInfo cultureInfo = new CultureInfo("en-US");
            var totalAmount = CalculateInvoice(invoice, plays, toReturn, out var volumeCredits);

            resultBuilder.AppendLine("<tr>");
            resultBuilder.AppendLine("<th>play</th>");
            resultBuilder.AppendLine("<th>seats</th>");
            resultBuilder.AppendLine("<th>cost</th>");
            resultBuilder.AppendLine("</tr>");

            foreach (var pay in toReturn)
            {
                resultBuilder.AppendLine("<tr>");
                resultBuilder.AppendLine(String.Format("<td>{0}</td>", pay.Item1.Name));
                resultBuilder.AppendLine(String.Format("<td>{0}</td>", pay.Item3));
                resultBuilder.AppendLine(String.Format(cultureInfo, "<td>{0}</td>", Convert.ToDecimal(pay.Item2 / 100)));
                resultBuilder.AppendLine("</tr>");
            }
            resultBuilder.AppendLine("<table>");

            resultBuilder.AppendLine(String.Format(cultureInfo, "<p>Amount owed is <em>{0:C}</em></p>", Convert.ToDecimal(totalAmount / 100)));
            resultBuilder.AppendLine(String.Format("<p>You earned <em>{0}</em> credits</p>", volumeCredits));
            resultBuilder.AppendLine("</body>");
            resultBuilder.AppendLine("</html>");
            
            return resultBuilder.ToString();
        }
    }
}