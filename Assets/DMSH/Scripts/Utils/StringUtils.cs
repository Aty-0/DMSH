using System.Text;
using UnityEngine;

namespace Scripts.Utils
{
    public static class StringUtils
    {
        // Default zeros count
        public const int UI_ZEROS_SCORE_TEXT = 8;

        // Basic string tool to fill string with number also zeros 
        public static string GetNumberWithZeros(int num)
        {
            // Initialize empty string
            var resultText = new StringBuilder();
            // Fill string by UI_ZEROS_SCORE_TEXT count subtract number length 
            var numLength = Mathf.Max(0, Mathf.Floor(Mathf.Log10(num) + 1));
            for (var i = 0; i <= UI_ZEROS_SCORE_TEXT - numLength; i++)
            {
                resultText.Append("0");
            }
            // Add number
            resultText.Append(num);
            return resultText.ToString();
        }
    }
}