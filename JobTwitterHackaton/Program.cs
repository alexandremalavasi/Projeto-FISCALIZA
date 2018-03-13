using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Json;
using Tweetinvi.Logic.JsonConverters;
using Tweetinvi.Models.DTO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;

namespace JobTwitterHackaton
{
    class Program
    {
        static void Main(string[] args)
        {


            Timer t = new Timer(Rotina, null, 0, 15000);

            //Auth.SetUserCredentials("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");

           
            Console.Read();
        }

        static DataTable RetornaTabela(string sql)
        {
            DataTable tabela = new DataTable();

            MySqlConnection conn = new MySqlConnection("Server=localhost;Database=hackatontadepe;Uid=root;Pwd=root;");
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

            da.Fill(tabela);
            conn.Close();

            return tabela;
        }

        static void InsereTweet(string id, string user_name)
        {
            MySqlConnection conn = new MySqlConnection("Server=localhost;Database=hackatontadepe;Uid=root;Pwd=root;");
            conn.Open();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO TWEET (id, username) VALUES('"+ id +"','"+ user_name +"')", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        static void Rotina(Object o)
        {
            try
            {

                Auth.SetUserCredentials("2JFR4v7euBTTMRXxkzZ28AdYr",
                                       "TYSkAoQvESJn2mQg0F73wwIoAh81ilJAfDcgix2I05PuJAf7ZM",
                                       "972539919737479169-yCip0gZvfX7dOWh7QeiLLaxFthpZCRM",
                                       "FEq98QKwBdUCRUjykfafnbtNyBsyh0cM9QtWcCZS1Qnpq");

                // Get json directly
                var tweetsJson = SearchJson.SearchTweets("testetadepe");

                // Get json from ITweet objects
                var tweets = Search.SearchTweets("testetadepe");
                // JSON Convert from Newtonsoft available with Tweetinvi
                var json = JsonConvert.SerializeObject(tweets.Select(x => x.TweetDTO));
                var tweetDTOsFromJson = JsonConvert.DeserializeObject<ITweetDTO[]>(json, JsonPropertiesConverterRepository.Converters);
                var tweetsFromJson = Tweet.GenerateTweetsFromDTO(tweetDTOsFromJson);

                foreach (ITweetDTO tw in tweetDTOsFromJson)
                {


                    DataTable tabela = RetornaTabela("SELECT * FROM TWEET WHERE ID=" + tw.Id.ToString());

                    string id = tw.Id.ToString();
                    string user = tw.CreatedBy.ToString();

                    if (tabela.Rows.Count == 0)
                    {

                        InsereTweet(id, user);

                        Tweet.PublishTweet("@" + tw.CreatedBy + " Olá, meu nome é Liza, tudo bem? Você sabia que  pode fazer sua reclamação comigo através deste link?  https://m.me/FiscalizaObra  #"  + id.ToString());
                    }
                }
            }
            catch
            {

            }
            // Tweet.PublishTweet("Meu nome é Liza! ");
            Console.WriteLine("ACABOU");
        }
    }
}
