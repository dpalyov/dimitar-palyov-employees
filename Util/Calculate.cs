using System;

namespace dimitar_palyov_employees.Util
{
    public static class Calculate
    {
        public static bool IsIntersecting(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return startA <= endB && startB <= endA;
        }

        public static int IntersectionDays(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            var from = Math.Max(startA.Ticks, startB.Ticks);
            var to = Math.Min(endA.Ticks, endB.Ticks);

            DateTime fromDate = new DateTime(from);
            DateTime toDate = new DateTime(to);

            return Math.Abs(toDate.Subtract(fromDate).Days);

        }
    }
}