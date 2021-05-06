using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Palindromestagram
{
    public static class PalindromeCheckOrchestrator
    {
        [FunctionName("PalindromeCheckOrchestrator")]
        public static async Task<int> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            //Step 1: We bring in all the Palindromes
            var palindromes = context.GetInput<List<string>>(); 

            //Step 2: We check Each Palindrome -- Fan Out

            //We create an array of tasks, one task for every palindrome that needs checked.
            //We define that these tasks are all going to return a VlidatedPalindrome.
            var checkPalindromesTasks = new Task<ValidatedPalindrome>[palindromes.Count];

            //For each one of the Palidrome Tasks we created we call an activity funciton which will return a ValidatedPalidrome object. 
            for (int i = 0; i < checkPalindromesTasks.Length; i++)
            {
                checkPalindromesTasks[i] = context.CallActivityAsync<ValidatedPalindrome>("ValidatePalindrome", palindromes[i]);
            }

            //We wait for all the tasks that we need to complete to finish
            //At this point our Azure fucniton is going to go to sleep until all of our other functions comeplete.. This saves us Money!
            await Task.WhenAll(checkPalindromesTasks);

            //We extract the reusults from the tasks. 
            List<ValidatedPalindrome> validatedPalindromes = checkPalindromesTasks.Select(validatedPalindrome => validatedPalindrome.Result).ToList();


            //Step 3: We bring back all the results and either block or allow the post through and show the final score!
            var overallScore = validatedPalindromes.Where(x => x.IsValid).Select(y => y.Score).Sum();
            var invalidPalindromes = validatedPalindromes.Where(x => x.IsValid == false).Count();

            //If the post is invalid we return -1 
            if (invalidPalindromes > 0)
            {
                return -1;
            }

            //Otherwise we return the score the user earned on their post
            return overallScore; 
        }

    }
}