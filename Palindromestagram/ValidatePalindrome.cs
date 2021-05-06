using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Palindromestagram
{
    public static class ValidatePalindrome
    {
        [FunctionName("ValidatePalindrome")]
        public static async Task<ValidatedPalindrome> ValidatePalindromeFunctionAsync([ActivityTrigger] string palindrome, ILogger log)
        {

            ValidatedPalindrome validatedPalindrome = new ValidatedPalindrome()
            {
                Word = palindrome,
                IsValid = isPalindrome(palindrome),
                Score = palindrome.Length
            };

            return validatedPalindrome; 
            
        }

        public static bool isPalindrome(string palindrome)
        {
            string first = palindrome.Substring(0, palindrome.Length / 2);
            char[] arr = palindrome.ToCharArray();

            Array.Reverse(arr);

            string temp = new string(arr);
            string second = temp.Substring(0, temp.Length / 2);

            return first.ToLower().Equals(second.ToLower());
        }
    }
}