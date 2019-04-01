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

    public class Interval : IComparable<Interval> {
        public uint Begin { get; private set; }
        public uint End { get; private set; }
        public ulong Value { get; private set; }

        public Interval(uint begin, uint end, ulong value){
            this.Begin = begin;
            this.End = end;
            this.Value = value;
        }

        public Interval(int begin, int end, int value) : this((uint)begin, (uint)end, (ulong)value) { }

        public Interval(uint begin, uint end) : this(begin, end, 0){}

        public int CompareTo(Interval other) {
            return this.Begin.CompareTo(other.Begin);
        }

        public bool Contains(uint point) {
            return this.Begin <= point && point <= this.End;
        }

        public bool Contains(Interval other) {
            return this.Contains(other.Begin) && this.Contains(other.End);
        }

        public bool Intersects(Interval other) {
            return this.Contains(other.Begin) || this.Contains(other.End) || other.Contains(this.Begin) || other.Contains(this.End);
        }

        public Interval Intersection(Interval other) {
            if (!Intersects(other)) return null;

            return new Interval(GetMaxBetween(this.Begin, other.Begin), GetMinBetween(this.End, other.End), this.Value + other.Value);
        }

        public List<Interval> Except(Interval other) {
            if (!this.Intersects(other)) return new List<Interval>(){ this };

            List<Interval> result = new List<Interval>();
            if (this.Begin < other.Begin) result.Add(new Interval(this.Begin, other.Begin-1, this.Value));
            if (this.End > other.End) result.Add(new Interval(other.End+1, this.End, this.Value));

            return result;
        }
    }

    static Interval GetIntervalBetweenAdjacentProccessedIntervals(Interval first, Interval second) {
            if (first == null || second == null) return null;
            if (first.End + 1 == second.Begin) return null;
            return new Interval(first.End + 1, second.Begin - 1);
    }

    static Interval GetIntervalBetweenLastProccessedIntervalAndTheEndOfImaginaryArray(Interval lastProcessedInterval, uint endOfImaginaryArray) {
        uint begin = lastProcessedInterval.End + 1;
        if (begin > endOfImaginaryArray) return null;
        return new Interval(begin, endOfImaginaryArray);
    }

    static T GetMaxBetween<T>(T first, T second) where T : IComparable, IComparable<T>
    {
        return first.CompareTo(second) > 0 ? first : second;
    }

    static T GetMinBetween<T>(T first, T second) where T : IComparable, IComparable<T>  {
        return first.CompareTo(second) < 0 ? first : second;
    }

    // Complete the arrayManipulation function below.
    static ulong arrayManipulation(uint n, uint m, uint[][] queries) {
        // approach: array with 'n' items will not be created, instead only manipulations over the 'm' queries will be made
        // rational: O(m^2) is better than O(n*m)

        ulong max = 0;

        // processedIntervals = "merged queries", or "equivalent set of intervals with  no intersection between each other"
        // the number of processed intervals is potentially greater than the number of queries
        SortedSet<Interval> processedIntervals = new SortedSet<Interval>();

        // the below lists are necessary to avoid changes on 'processedIntervals' set during its exploration
        List<Interval> intervalsToRemoveFromProcessed = new List<Interval>();
        List<Interval> intervalsToAddInProcessed = new List<Interval>();

        for (int i=0; i<m; i++) {
            Interval current = new Interval((uint)queries[i][0], (uint)queries[i][1], (ulong)queries[i][2]);

            // optimization: a '0' valued query will make no difference on resultant ones
            if (current.Value == 0) continue;
            
            intervalsToRemoveFromProcessed.Clear();
            intervalsToAddInProcessed.Clear();

            if (i == 0) {
                intervalsToAddInProcessed.Add(current);
            }
            else {
                Interval lastChecked = null;
                Interval betweenLastAndCurrent = null;
                foreach (Interval processed in processedIntervals) {
                    // optimization: avoid run all intervals again unnecessarily
                    if (current.Begin > processed.End) {
                        lastChecked = processed;
                        continue;
                    }

                    betweenLastAndCurrent = GetIntervalBetweenAdjacentProccessedIntervals(lastChecked, processed);

                    if (betweenLastAndCurrent != null) {
                        intervalsToAddInProcessed.Add(betweenLastAndCurrent.Intersection(current));
                    }

                    Interval intersection = processed.Intersection(current);
                    if (intersection != null) {
                        intervalsToRemoveFromProcessed.Add(processed);
                        intervalsToAddInProcessed.Add(intersection);
                        
                        List<Interval> notAffected = processed.Except(intersection);
                        intervalsToAddInProcessed.AddRange(notAffected);
                    }
                    
                    lastChecked = processed;

                    // optimization: avoid run all intervals again unnecessarily
                    if (current.End <= processed.End) break;
                }

                if (intervalsToAddInProcessed.Count == 0 || current.End > lastChecked.End) {
                    Interval fromTheLastUntilTheEnd = GetIntervalBetweenLastProccessedIntervalAndTheEndOfImaginaryArray(lastChecked, n);
                    if (fromTheLastUntilTheEnd != null)                     intervalsToAddInProcessed.Add(fromTheLastUntilTheEnd.Intersection(current));
                }
            }

            // process intervalsToRemoveFromProcessed
            foreach(var toRemove in intervalsToRemoveFromProcessed){
                if (toRemove != null) processedIntervals.Remove(toRemove);
            }
            
            // process intervalsToAddInProcessed
            foreach(var toAdd in intervalsToAddInProcessed){
                if (toAdd != null) {
                    processedIntervals.Add(toAdd);
                    max = GetMaxBetween(max, toAdd.Value);
                }
            }
        }

        return max;
    }

    static void Main(string[] args) {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string[] nm = Console.ReadLine().Trim().Split(' ');

        uint n = uint.Parse(nm[0]);

        uint m = uint.Parse(nm[1]);

        uint[][] queries = new uint[m][];

        for (int i = 0; i < m; i++) {
            queries[i] = Array.ConvertAll(Console.ReadLine().Trim().Split(' '), queriesTemp => uint.Parse(queriesTemp));
        }

        ulong result = arrayManipulation(n, m, queries);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}

