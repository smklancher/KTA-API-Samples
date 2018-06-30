using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtaCase
{
    public class FieldValidationOutput
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Validation
    {
        public FieldValidationOutput CompareStrings(string s1, string s2)
        {
            var result = new FieldValidationOutput();
            result.IsValid = (s1 == s2);
            result.ErrorMessage = $"{s1} {(result.IsValid ? "==" : "!=")} {s2}";
            return result;
        }



        public FieldValidationOutput CompareDoubles(double d1, double d2, string errorMsgIfDifferent)
        {
            // comparing doubles can produce unexpected results.
            // to understand more: https://floating-point-gui.de/errors/comparison/

            var result = new FieldValidationOutput();
            result.IsValid = NearlyEqual(d1, d2, 0.0001);
            if (!result.IsValid)
            {
                if (string.IsNullOrWhiteSpace(errorMsgIfDifferent))
                {
                    result.ErrorMessage=$"Values are different: {d1.ToString()}, {d2.ToString()}";
                }
                else
                {
                    result.ErrorMessage = errorMsgIfDifferent;
                }
            }
            return result;
        }


        /// <summary>
        /// https://stackoverflow.com/a/3875619/221018
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool NearlyEqual(double a, double b, double epsilon)
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < Double.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
    }
}
