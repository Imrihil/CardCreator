using System;
using System.Collections.Generic;
using System.Text;

namespace CardCreator.Features.System
{
    public static class IntegerExtensions
    {
        public static string ToOrdinal(this int number)
        {
            switch (number % 10)
            {
                case 1:
                    {
                        if (number == 11) return $"{number}th";
                        else return $"{number}st";
                    }
                case 2:
                    {
                        if (number == 12) return $"{number}th";
                        else return $"{number}nd";
                    }
                case 3:
                    {
                        if (number == 13) return $"{number}th";
                        else return $"{number}rd";
                    }
                default:
                    return $"{number}th";
            }
        }
    }
}
