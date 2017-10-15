using System;


namespace PriceTick
{
    class Program
    {
        public abstract class Security
        {
            protected decimal[] PriceRange;
            protected decimal[] TickSize;

            public virtual void setdata() { }

            public int inPriceRange(decimal p)
            {
                int i;
                for (i = 0; i < PriceRange.Length; i++)
                {
                    if (p < PriceRange[i]) 
                        return i;
                }
                return i;
            }
            // return tick size of a given price 
            public decimal getTickSize(decimal p)
            {
                int idx = inPriceRange(p);
                return TickSize[idx];
            }
            //check if the given price is valid 
            public bool isValidPrice(decimal p)
            {
                int idx = inPriceRange(p);
                if (p>=0 && p % TickSize[idx] == 0)
                    return true;
                return false;
            }

            public int TicksBetween(decimal p1, decimal p2)
            {
                int i, count = 0;
                int idx1, idx2;
                decimal low, high;

                if (!isValidPrice(p1) || !isValidPrice(p2))
                {
                    System.Console.WriteLine("!! {0} is not a valid price, please check your input.", isValidPrice(p1) ? p2 : p1);
                    return -1;
                }

                if (p1 > p2)
                {
                    low = p2;
                    high = p1;
                }
                else
                {
                    low = p1;
                    high = p2;
                }
                idx1 = inPriceRange(low);
                idx2 = inPriceRange(high);


                if (idx1 == idx2)
                    count = (int)((high - low) / TickSize[idx1]);
                else
                {
                    decimal curr_tick = TickSize[idx1];

                    count = (int)((PriceRange[idx1] - curr_tick - low) / curr_tick);
                    count += (int)((high - PriceRange[idx2 - 1]) / getTickSize(high)) + 1;
                    for (i = idx1 + 1; i < idx2; i++)
                    {
                        curr_tick = TickSize[i];
                        count += (int)((PriceRange[i] - curr_tick - PriceRange[i - 1]) / curr_tick) + 1;
                    }
                }

                return count;
            }

            public decimal TickMove(decimal price, int ticks)
            {
                decimal p = price;
                decimal curr_tick = getTickSize(price);
                int curr_idx = inPriceRange(price);
                int i; // = Math.Abs(ticks);

                if (ticks == 0) return price;

                if (!isValidPrice(price))
                {
                    System.Console.WriteLine("!! {0} is not a valid price, please check your input.", price);
                    return -1;
                }

                for (i = Math.Abs(ticks); i > 0; i--)
                {
                    if (ticks > 0)
                    {
                        p += curr_tick;
                        if (inPriceRange(p) != curr_idx)
                        {
                            curr_tick = getTickSize(p + curr_tick);
                            curr_idx++;
                        }
                    }
                    else
                    {
                        p -= curr_tick;
                        if (inPriceRange(p - curr_tick) != curr_idx)
                        {
                            curr_tick = getTickSize(p - curr_tick);
                            curr_idx--;
                        }
                    }
                }
                return p;
            }
        }

        public class CB : Security
        {
            private decimal[] price_range = { 150m, 1000m };
            private decimal[] tick_size = { 0.05m, 1m, 5m };

            public override void setdata()
            {
                PriceRange = price_range;
                TickSize = tick_size;
            }
            public CB()
            {
                setdata();
            }
             
        }
        
        public class ETF : Security
        {
            private decimal[] price_range = { 50m };
            private decimal[] tick_size = { 0.01m, 0.05m };

            public override void setdata()
            {
                PriceRange = price_range;
                TickSize = tick_size;
            }
            public ETF()
            {
                setdata();
            }
        }

        public class TX : Security
        {
            private decimal[] price_range = { 50m, 500m };
            private decimal[] tick_size = { 0.1m, 0.5m, 1m };

            public override void setdata()
            {
                PriceRange = price_range;
                TickSize = tick_size;
            }
            public TX()
            {
                setdata();
            }
        }
        
        static void Main(string[] args)
        {

            CB cb = new CB();
            Console.WriteLine("== test CB =====");
            Console.WriteLine("give price 149.9, idx = {0}, tick_size={1:n}", cb.inPriceRange(149.9m), cb.getTickSize(149.9m));
            Console.WriteLine("give price 100 and 101, ticks  = {0}", cb.TicksBetween(100m, 101m));
            Console.WriteLine("give price 153.5 and 155, ticks  = {0}", cb.TicksBetween(153.5m, 155m));

            Console.WriteLine("give price 149.9 and 150, ticks  = {0}", cb.TicksBetween(149.9m, 150m));
            Console.WriteLine("give price 150 and 999, ticks  = {0}", cb.TicksBetween(150m, 999m));
            Console.WriteLine("give price 150 and 1000, ticks  = {0}", cb.TicksBetween(150m, 1000m));
            Console.WriteLine("give price 149.9 and 1005, ticks  = {0}", cb.TicksBetween(149.9m, 1005m));
            Console.WriteLine("give price 149.9 and 1003, ticks  = {0}", cb.TicksBetween(149.9m, 1003m));

            Console.WriteLine("price 100.5 move 3 ticks  = {0:n}", cb.TickMove(100.5m, 3));
            Console.WriteLine("price 149.9 move 5 ticks  = {0:n}", cb.TickMove(149.9m, 5));
            Console.WriteLine("price 998 move 5 ticks  = {0:n}", cb.TickMove(998m, 5));


            Console.WriteLine("== test ETF =====");
            ETF etf = new ETF();
            Console.WriteLine("give price 40 and 41, ticks  = {0}", etf.TicksBetween(40m, 41m));
            Console.WriteLine("give price 60 and 62, ticks  = {0}", etf.TicksBetween(60m, 62m));
            Console.WriteLine("price 50.2 move -5 ticks  = {0:n}", etf.TickMove(50.2m, -5));
            Console.WriteLine("price 49.98 move 5 ticks  = {0:n}", etf.TickMove(49.98m, 5));


            Console.WriteLine("== test X =====");
            TX tx = new TX();
            Console.WriteLine("give price 49.5 and 51, ticks  = {0}", tx.TicksBetween(49.5m, 51m));
            Console.WriteLine("give price 498.5 and 501, ticks  = {0}", tx.TicksBetween(498.5m, 501m));
            Console.WriteLine("price 49.6 move 8 ticks  = {0:n}", tx.TickMove(49.6m, 8));
            Console.WriteLine("price 498.5 move 6 ticks  = {0:n}", tx.TickMove(498.5m, 6));
            Console.WriteLine("price 51.5 move -6 ticks  = {0:n}", tx.TickMove(51.5m, -6));
            Console.WriteLine("price 505 move -6 ticks  = {0:n}", tx.TickMove(505m, -6));
            
            Console.Read();
        }
    }
}
