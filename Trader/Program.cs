using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Trader
{
    class Program
    {

        static void Main(string[] args)
        {
            LogUtils.Debug("Started!");
            string lastFetchId = "";
            var lastOrder = DateTime.Now;
            while (true)
            {
                Fetch(ref lastFetchId);

                if (DateTime.Now.Subtract(lastOrder).TotalSeconds>=10)
                {
                    PlaceOrder(lastOrder);
                    lastOrder = DateTime.Now;
                }
                Thread.Sleep(5000);//10 secs
            }
        }

        private static void PlaceOrder(DateTime local_time)
        {
            using (var conn = DbUtils.OpenSqlConnection())
            {
                var res = DbUtils.Query<dynamic>(conn, @"select sum(cast(is_buy as int)) as buys
                                                        ,count(1) as tot 
                                                        from dbo.trades 
                                                        where local_time > dateadd(minute,-20,getdate())",
                     new { local_time },
                     System.Data.CommandType.Text).First();
                Console.WriteLine($"last 20 minutes buys {res.buys} / {res.tot}");
                if (res.tot > 0)
                {
                    if ((int)res.buys > ((double)res.tot) / 2)
                        Console.WriteLine("BUY!");
                    else
                        Console.WriteLine("SELL!");
                }
            }
        }

        private static void Fetch(ref string last)
        {

            var res = WebApiUtils.Send<dynamic>(HttpMethod.Get, $"https://api.kraken.com/0/public/Trades?pair=XBTUSD&since={last}");
            last = res.result.last;
            var buys = 0;
            var tot = 0;
            using (var conn = DbUtils.OpenSqlConnection())
            {
                foreach (var line in res.result.XXBTZUSD)
                {
                    var p = new
                    {
                        price = (decimal)line[0],
                        volume = (decimal)line[1],
                        trade_time = ParseUnixTime((double)line[2]),
                        is_buy = line[3] == "b",
                        is_market = line[4] == "m",
                        misc = line[5] as string
                    };
                    tot++;
                    buys+=p.is_buy?1:0;
                    //Console.WriteLine(JsonUtils.Serialize(p));
                    DbUtils.Execute(conn, @"
INSERT INTO [dbo].[Trades]
           ([price]
           ,[volume]
           ,[trade_time]
           ,[is_buy]
           ,[is_market]
           ,[misc]
            ,local_time)
     select @price
           ,@volume
           ,@trade_time
           ,@is_buy
           ,@is_market
           ,@misc
            ,getdate() as local_time", p, System.Data.CommandType.Text);

                }
            }
            Console.WriteLine($"buys {buys} / {tot}");
        }

        private static DateTime ParseUnixTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}