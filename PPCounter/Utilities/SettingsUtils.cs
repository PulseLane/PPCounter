using PPCounter.Settings;
using System;
using System.Collections.Generic;

namespace PPCounter.Utilities
{
    static class SettingsUtils
    {
        public static long GetPreferredOrderNumber(List<PPCounters> counterOrder)
        {
            long order = 0;

            for (int i = 0; i < counterOrder.Count; i++)
            {
                order += (long) counterOrder[i] * (long) Math.Pow(10, counterOrder.Count - 1 - i);
            }

            return order;
        }

        public static List<PPCounters> GetCounterOrder(long orderNumber, int numCounters)
        {
            List<PPCounters> ppCounters = new List<PPCounters>(new PPCounters[numCounters]);

            for (int i = 0; i < numCounters; i++)
            {
                long pow = (long) Math.Pow(10, numCounters - 1 - i);
                long enumValue = orderNumber / pow;
                orderNumber %= pow;

                ppCounters[i] = (PPCounters) enumValue;
            }

            return ppCounters;
        }
    }
}
