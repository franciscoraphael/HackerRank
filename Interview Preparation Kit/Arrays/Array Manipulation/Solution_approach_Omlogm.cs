using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class Solution {
    // Complete the arrayManipulation function below.
    static long arrayManipulation(int n, int m, int[][] queries) {
        // NOTE: after a few attempts receiving timeouts, I've took a look at discussion tab to seek for someone else claiming against the timeout settings, but inevitably I've saw the solution posted there by 'amansbhandari' (genius solution, btw). So, influenced by his/her solution I'm implementing the same idea of 'modifiers' but without an n-sized array, this because 'n' can be 100x greater than 'm', in worst case scenarios, and I think in real life problems the amount of memory needed would be greater than a simple 'long' for each item. The expected complexity is O(m * log m).

        long max = 0;
        
        SortedDictionary<int,long> fakeArray = new SortedDictionary<int,long>();

        for (int i=0; i<m; i++) {
            // bit optimization: a '0' valued query will make no difference on resultant ones
            int a = queries[i][0];
            int b = queries[i][1];
            int k = queries[i][2];

            if (queries[i][2] <= 0) continue;

            if (!fakeArray.ContainsKey(a)) {
                fakeArray.Add(a, k);
            }
            else {
                fakeArray[a] += k;
            }

            if (b < n) {
                b++;
                if (!fakeArray.ContainsKey(b)) {
                    fakeArray.Add(b, -1 * k);
                }
                else {
                    fakeArray[b] -= k;
                }
            }
        }

        long current = 0;
        foreach(long modifier in fakeArray.Values){
            current += modifier;
            if (current > max) max = current;
        }

        return max;
    }

    static void Main(string[] args) {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string[] nm = Console.ReadLine().Trim().Split(' ');
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);
        int[][] queries = new int[m][];
        for (int i = 0; i < m; i++) {
            queries[i] = Array.ConvertAll(Console.ReadLine().Trim().Split(' '), queriesTemp => int.Parse(queriesTemp));
        }

        long result = arrayManipulation(n, m, queries);
        textWriter.WriteLine(result);
        textWriter.Flush();
        textWriter.Close();
    }
}
