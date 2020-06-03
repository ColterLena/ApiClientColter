using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConsoleTables;

namespace ApiClientColter
{
    class Program
    {
        class Brewery
        {
            public int id { get; set; }
            public string name { get; set; }
            public string city { get; set; }

            public string state { get; set; }
            public string country { get; set; }
        }
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var client = new HttpClient();
            var responseAsStream = await client.GetStreamAsync("https://api.openbrewerydb.org/breweries");
            var breweries = await JsonSerializer.DeserializeAsync<List<Brewery>>(responseAsStream);

            var userHasChosenToQuit = false;

            while (userHasChosenToQuit == false)
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to the brewery database. Please select one of the following options: ");
                Console.WriteLine();
                Console.Write("(V)iew a list of breweries with supporting information, (F)ind a brewery by entering in the associated ID number, F(I)nd breweries by entering in a state, or (Q)uit this application ");
                var choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "V":
                        var table = new ConsoleTable("ID", "Name", "City", "State", "Country");

                        foreach (var brewery in breweries)
                        {
                            table.AddRow(brewery.id, brewery.name, brewery.city, brewery.state, brewery.country);
                        }
                        table.Write();
                        break;

                    case "F":
                        Console.Write("Enter the ID for the brewery you would like to search for: ");
                        try
                        {
                            var id = int.Parse(Console.ReadLine());
                            var url = $"https://api.openbrewerydb.org/breweries/{id}";
                            var responseAsStreamForChoiceF = await client.GetStreamAsync(url);
                            var breweryForChoiceF = await JsonSerializer.DeserializeAsync<Brewery>(responseAsStreamForChoiceF);
                            var tableForChoiceF = new ConsoleTable("ID", "Name", "City", "State", "Country");

                            tableForChoiceF.AddRow(breweryForChoiceF.id, breweryForChoiceF.name, breweryForChoiceF.city, breweryForChoiceF.state, breweryForChoiceF.country);
                            tableForChoiceF.Write();
                        }
                        catch (HttpRequestException)
                        {
                            Console.WriteLine("That ID could not be found.");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("That ID could not be found.");
                        }
                        break;

                    case "I":

                        Console.Write("Enter a state and receive a list of breweries: ");
                        try
                        {
                            var state = Console.ReadLine();
                            var url = $"https://api.openbrewerydb.org/breweries?by_state={state}";
                            var responseAsStreamForChoiceI = await client.GetStreamAsync(url);
                            var breweryForChoiceI = await JsonSerializer.DeserializeAsync<List<Brewery>>(responseAsStreamForChoiceI);
                            var tableForChoiceI = new ConsoleTable("ID", "Name", "City", "State", "Country");

                            foreach (var brewery in breweryForChoiceI)
                            {
                                tableForChoiceI.AddRow(brewery.id, brewery.name, brewery.city, brewery.state, brewery.country);
                            }

                            tableForChoiceI.Write();
                        }
                        catch (HttpRequestException)
                        {
                            Console.WriteLine("That state could not be found.");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("That state could not be found.");
                        }
                        break;

                    case "Q":
                        userHasChosenToQuit = true;
                        break;

                }
            }
        }
    }
}
