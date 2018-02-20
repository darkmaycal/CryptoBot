﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.AIClass
{
    public static class CompairCoins
    {
        public static List<MainStrategy> CoinCompare(Dictionary<string, TransforfOrders> arg1, Dictionary<string, TransforfOrders> arg2, Stock mainStockEx, Stock secoondStockEx)
        {
            if (arg1 == null || arg2 == null)
            {
                return new List<MainStrategy>();
            }
            Dictionary<string, TransforfOrders> main = arg1.Where(d => arg2.ContainsKey(d.Key)).ToDictionary(x => x.Key, y => new TransforfOrders()
            {
                asks = y.Value.asks.Count != 0 ? y.Value.asks.Take(30).ToDictionary(c => c.Key, v => v.Value) : y.Value.asks,
                bids = y.Value.bids.Count != 0 ? y.Value.bids.Take(30).ToDictionary(c => c.Key, v => v.Value) : y.Value.bids
            });
            Dictionary<string, TransforfOrders> second = arg2.Where(d => arg1.ContainsKey(d.Key)).ToDictionary(x => x.Key, y => new TransforfOrders()
            {
                asks = y.Value.asks.Count != 0 ? y.Value.asks.Take(30).ToDictionary(c => c.Key, v => v.Value) : y.Value.asks,
                bids = y.Value.bids.Count != 0 ? y.Value.bids.Take(30).ToDictionary(c => c.Key, v => v.Value) : y.Value.bids
            });
            #region сравнение валют и запись из в стратегию

            List<MainStrategy> MainSt = new List<MainStrategy>();

            foreach (KeyValuePair<string, TransforfOrders> i in main)
            {
                TransforfOrders temp;

                if (second.ContainsKey(i.Key))
                {
                    temp = second[i.Key];
                }
                else
                {
                    continue;
                }
                if (temp.asks.Count == 0)
                {
                    continue;
                }
                if (temp.bids.Count == 0)
                {
                    continue;
                }
                if (i.Value.asks.Count == 0)
                {
                    continue;
                }
                if (i.Value.bids.Count == 0)
                {
                    continue;
                }
                if (IsProfit(temp.bids.First().Key, i.Value.asks.First().Key))
                {
                    MainStrategy TempSt = new MainStrategy();
                    TempSt.MarketName = i.Key;
                    TempSt.BuyStockEX = mainStockEx;
                    TempSt.SellStockEX = secoondStockEx;
                    if (temp.asks.Count == 0)
                    {
                        continue;
                    }
                    if (temp.bids.Count == 0)
                    {
                        continue;
                    }
                    if (i.Value.asks.Count == 0)
                    {
                        continue;
                    }
                    if (i.Value.bids.Count == 0)
                    {
                        continue;
                    }
                    //buy on [livecoin] main sell on  [poloniex] several
                    while (IsProfit(temp.bids.First().Key, i.Value.asks.First().Key))
                    {

                        if (i.Value.asks.First().Value > temp.bids.First().Value)
                        {
                            //записать в стратегию значение цены и количество в цикле пока профит
                            if (TempSt.StrategyBuy.ContainsKey(i.Value.asks.First().Key))
                            {
                                TempSt.StrategyBuy[i.Value.asks.First().Key] += temp.bids.First().Value;
                            }
                            else
                            {
                                TempSt.StrategyBuy.Add(i.Value.asks.First().Key, temp.bids.First().Value);
                            }
                            if (TempSt.StrategySell.ContainsKey(temp.bids.First().Key))
                            {
                                TempSt.StrategySell[temp.bids.First().Key] += temp.bids.First().Value;
                            }
                            else
                            {
                                TempSt.StrategySell.Add(temp.bids.First().Key, temp.bids.First().Value);
                            }

                            //отнять в списках большее значение
                            i.Value.asks[i.Value.asks.First().Key] -= temp.bids.First().Value;
                            //удалить из списков меньшее значение
                            temp.bids.Remove(temp.bids.First().Key);

                        }
                        else if (i.Value.asks.First().Value < temp.bids.First().Value)
                        {
                            //записать в стратегию значение цены и количество
                            if (TempSt.StrategyBuy.ContainsKey(i.Value.asks.First().Key))
                            {
                                TempSt.StrategyBuy[i.Value.asks.First().Key] += i.Value.asks.First().Value;
                            }
                            else
                            {
                                TempSt.StrategyBuy.Add(i.Value.asks.First().Key, i.Value.asks.First().Value);
                            }
                            if (TempSt.StrategySell.ContainsKey(temp.bids.First().Key))
                            {
                                TempSt.StrategySell[temp.bids.First().Key] += i.Value.asks.First().Value;
                            }
                            else
                            {
                                TempSt.StrategySell.Add(temp.bids.First().Key, i.Value.asks.First().Value);
                            }
                            //отнять в списках большее значение
                            temp.bids[temp.bids.First().Key] -= i.Value.asks.First().Value;
                            //удалить из списков меньшее значение
                            i.Value.asks.Remove(i.Value.asks.First().Key);
                        }
                        if (temp.asks.Count == 0)
                        {
                            break;
                        }
                        if (temp.bids.Count == 0)
                        {
                            break;
                        }
                        if (i.Value.asks.Count == 0)
                        {
                            break;
                        }
                        if (i.Value.bids.Count == 0)
                        {
                            break;
                        }
                        //равные значения будут нужны для возврата валюты 
                        //else if (temp.bids.First()[1] == i.Value.asks.First()[1])
                        //{
                        //    //записать в стратегию значение цены и количество
                        //    MainSt.StrategyBuy.Data[temp.bids.First()[0]] += i.Value.asks.First()[1];
                        //    MainSt.StrategySell.Data[i.Value.asks.First()[0]] += i.Value.asks.First()[1];

                        //    //удалить из списков меньшее значение
                        //    temp.bids.First().Clear();
                        //    i.Value.asks.First().Clear();
                        //}

                    }
                    MainSt.Add(TempSt);
                }
                else if (IsProfit(i.Value.bids.First().Key, temp.asks.First().Key))
                {
                    //buy on [poloniex] main sell on [livecoin] several
                    MainStrategy TempSt = new MainStrategy();
                    TempSt.MarketName = i.Key;
                    TempSt.BuyStockEX = secoondStockEx;
                    TempSt.SellStockEX = mainStockEx;

                    //buy on [livecoin] main sell on  [poloniex] several
                    while (IsProfit(i.Value.bids.First().Key, temp.asks.First().Key))
                    {

                        if (temp.asks.First().Value > i.Value.bids.First().Value)
                        {
                            //записать в стратегию значение цены и количество в цикле пока профит
                            if (TempSt.StrategyBuy.ContainsKey(temp.asks.First().Key))
                            {
                                TempSt.StrategyBuy[temp.asks.First().Key] += i.Value.bids.First().Value;
                            }
                            else
                            {
                                TempSt.StrategyBuy.Add(temp.asks.First().Key, i.Value.bids.First().Value);
                            }
                            if (TempSt.StrategySell.ContainsKey(i.Value.bids.First().Key))
                            {
                                TempSt.StrategySell[i.Value.bids.First().Key] += i.Value.bids.First().Value;
                            }
                            else
                            {
                                TempSt.StrategySell.Add(i.Value.bids.First().Key, i.Value.bids.First().Value);
                            }

                            //отнять в списках большее значение
                            temp.asks[temp.asks.First().Key] -= -i.Value.bids.First().Value;
                            //удалить из списков меньшее значение
                            i.Value.bids.Remove(i.Value.bids.First().Key);


                        }
                        else if (temp.asks.First().Value < i.Value.bids.First().Value)
                        {
                            //записать в стратегию значение цены и количество
                            if (TempSt.StrategyBuy.ContainsKey(temp.asks.First().Key))
                            {
                                TempSt.StrategyBuy[temp.asks.First().Key] += temp.asks.First().Value;
                            }
                            else
                            {
                                TempSt.StrategyBuy.Add(temp.asks.First().Key, temp.asks.First().Value);
                            }
                            if (TempSt.StrategySell.ContainsKey(i.Value.bids.First().Key))
                            {
                                TempSt.StrategySell[i.Value.bids.First().Key] += temp.asks.First().Value;
                            }
                            else
                            {
                                TempSt.StrategySell.Add(i.Value.bids.First().Key, temp.asks.First().Value);
                            }
                            //отнять в списках большее значение
                            i.Value.bids[i.Value.bids.First().Key] -= temp.asks.First().Value;
                            //удалить из списков меньшее значение
                            temp.asks.Remove(temp.asks.First().Key);
                        }
                        if (temp.asks.Count == 0)
                        {
                            break;
                        }
                        if (temp.bids.Count == 0)
                        {
                            break;
                        }
                        if (i.Value.asks.Count == 0)
                        {
                            break;
                        }
                        if (i.Value.bids.Count == 0)
                        {
                            break;
                        }
                    }
                    MainSt.Add(TempSt);
                }

            }
            return MainSt;
            #endregion
        }
        public static MainStrategy ComparePrice(TransforfOrders arg1, TransforfOrders arg2, Stock mainStockEx, Stock secoondStockEx, string MarketName)
        {
            if (arg1 == null || arg2 == null)
            {
                return null;
            }
            TransforfOrders main = new TransforfOrders()
            {
                asks = arg2.asks.Take(30).ToDictionary(c => c.Key, v => v.Value),
                bids = arg2.bids.Take(30).ToDictionary(c => c.Key, v => v.Value)
            };
            TransforfOrders second = new TransforfOrders()
            {
                asks = arg1.asks.Take(30).ToDictionary(c => c.Key, v => v.Value),
                bids = arg1.bids.Take(30).ToDictionary(c => c.Key, v => v.Value)
            };
            #region сравнение валют и запись из в стратегию

            if (IsProfit(second.bids.First().Key, main.asks.First().Key))
            {
                MainStrategy TempSt = new MainStrategy();
                TempSt.MarketName = MarketName;
                TempSt.BuyStockEX = mainStockEx;
                TempSt.SellStockEX = secoondStockEx;

                //buy on [livecoin] main sell on  [poloniex] several
                while (IsProfit(second.bids.First().Key, main.asks.First().Key))
                {

                    if (main.asks.First().Value > second.bids.First().Value)
                    {
                        //записать в стратегию значение цены и количество в цикле пока профит
                        if (TempSt.StrategyBuy.ContainsKey(main.asks.First().Key))
                        {
                            TempSt.StrategyBuy[main.asks.First().Key] += second.bids.First().Value;
                        }
                        else
                        {
                            TempSt.StrategyBuy.Add(main.asks.First().Key, second.bids.First().Value);
                        }
                        if (TempSt.StrategySell.ContainsKey(second.bids.First().Key))
                        {
                            TempSt.StrategySell[second.bids.First().Key] += second.bids.First().Value;
                        }
                        else
                        {
                            TempSt.StrategySell.Add(second.bids.First().Key, second.bids.First().Value);
                        }

                        //отнять в списках большее значение
                        main.asks[main.asks.First().Key] -= second.bids.First().Value;
                        //удалить из списков меньшее значение
                        second.bids.Remove(second.bids.First().Key);

                    }
                    else if (main.asks.First().Value < second.bids.First().Value)
                    {
                        //записать в стратегию значение цены и количество
                        if (TempSt.StrategyBuy.ContainsKey(main.asks.First().Key))
                        {
                            TempSt.StrategyBuy[main.asks.First().Key] += main.asks.First().Value;
                        }
                        else
                        {
                            TempSt.StrategyBuy.Add(main.asks.First().Key, main.asks.First().Value);
                        }
                        if (TempSt.StrategySell.ContainsKey(second.bids.First().Key))
                        {
                            TempSt.StrategySell[second.bids.First().Key] += main.asks.First().Value;
                        }
                        else
                        {
                            TempSt.StrategySell.Add(second.bids.First().Key, main.asks.First().Value);
                        }
                        //отнять в списках большее значение
                        second.bids[second.bids.First().Key] -= main.asks.First().Value;
                        //удалить из списков меньшее значение
                        main.asks.Remove(main.asks.First().Key);
                    }

                    if (second.asks.Count == 0)
                    {
                        break;
                    }
                    if (second.bids.Count == 0)
                    {
                        break;
                    }
                    if (main.asks.Count == 0)
                    {
                        break;
                    }
                    if (main.bids.Count == 0)
                    {
                        break;
                    }

                    //равные значения будут нужны для возврата валюты 
                    //else if (temp.bids.First()[1] == i.Value.asks.First()[1])
                    //{
                    //    //записать в стратегию значение цены и количество
                    //    MainSt.StrategyBuy.Data[temp.bids.First()[0]] += i.Value.asks.First()[1];
                    //    MainSt.StrategySell.Data[i.Value.asks.First()[0]] += i.Value.asks.First()[1];

                    //    //удалить из списков меньшее значение
                    //    temp.bids.First().Clear();
                    //    i.Value.asks.First().Clear();
                    //}

                }
                return TempSt;
            }
            return new MainStrategy();
            #endregion
        }
        private static bool IsProfit(decimal Bids, decimal Asks)
        {
            if (Bids - Asks > 0)
            {
                return true;
            }
            return false;
        }

        public static MainStrategy CompareByDollar(IEnumerable<KeyValuePair<string, TransforfOrders>> Dollar, IEnumerable<KeyValuePair<string, TransforfOrders>> BTC, Stock StockEx)
        {
            return null;

        }
    }



}
