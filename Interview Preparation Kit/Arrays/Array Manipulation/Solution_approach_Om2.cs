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
    public class Interval {
        public int Begin { get; private set; }
        public int End { get; private set; }
        public long Value { get; private set; }

        public Interval(int begin, int end, long value){
            this.Begin = begin;
            this.End = end;
            this.Value = value;
        }

        public Interval(int begin, int end) : this(begin, end, 0){}

        public bool Contains(int point) {
            return this.Begin <= point && point <= this.End;
        }

        public bool Contains(Interval other) {
            return other != null && this.Contains(other.Begin) && this.Contains(other.End);
        }

        public bool Intersects(Interval other) {
            return other != null && this.Contains(other.Begin) || this.Contains(other.End) || other.Contains(this.Begin) || other.Contains(this.End);
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

    // Needed only for the purpose of having an O(1) "insertion method"
    public class ListNode {
        public Interval Value { get; set; }
        public ListNode Next { get; set; }

        public ListNode (Interval value) : this (value, null) {}

        public ListNode (Interval value, ListNode next) {
            this.Value = value;
            this.Next = next;
        }
    }

    static T GetMaxBetween<T>(T first, T second) where T : IComparable, IComparable<T>
    {
        return first.CompareTo(second) > 0 ? first : second;
    }

    static T GetMinBetween<T>(T first, T second) where T : IComparable, IComparable<T>  {
        return first.CompareTo(second) < 0 ? first : second;
    }

    // Complete the arrayManipulation function below.
    static long arrayManipulation(int n, int m, int[][] queries) {
        // approach: array with 'n' items will not be created, instead only some processing over the 'm' queries will be made
        // rational: O(m^2) is potentially faster than O(n*m)
        // more on "https://github.com/franciscoraphael/HackerRank/blob/master/ArrayManipulation_approach_Om2.cs"

        long max = 0;

        ListNode processedIntervalsHead = new ListNode(new Interval(1, n, 0));

        for (int i = 0; i < m; i++) {
            Interval processing = new Interval(queries[i][0],queries[i][1],queries[i][2]);
            for (ListNode processedNode = processedIntervalsHead; processedNode != null; processedNode = processedNode.Next) {
                Interval processed = processedNode.Value;

                if (processing.End < processed.Begin) break;
                if (processing.Begin > processed.End) continue;

                Interval intersection = processing.Intersection(processed); // intersection already have as its value the sum of each given Interval
                if (intersection == null) continue;

                max = GetMaxBetween(max, intersection.Value);
                
                if (processing.Contains(processed)) { // means that 'intersection' and 'processed' have the same positions range
                    processedNode.Value = intersection;
                }
                else {
                    List<Interval> resultantIntervals = processed.Except(processing);
                    int positionToInsert = (intersection.Begin < resultantIntervals[0].Begin) ? 0 : 1;
                    resultantIntervals.Insert(positionToInsert, intersection);

                    processedNode.Value = resultantIntervals[0];
                    processedNode.Next = new ListNode(resultantIntervals[1], processedNode.Next);
                    processedNode = processedNode.Next;

                    if (resultantIntervals.Count > 2) {
                        processedNode.Next = new ListNode(resultantIntervals[2], processedNode.Next);
                        processedNode = processedNode.Next;
                    }
                }
            }
        }

        return max;
    }

    static void Main(string[] args) {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string[] nm = Console.ReadLine().Split(' ');

        int n = Convert.ToInt32(nm[0]);

        int m = Convert.ToInt32(nm[1]);

        int[][] queries = new int[m][];

        for (int i = 0; i < m; i++) {
            queries[i] = Array.ConvertAll(Console.ReadLine().Split(' '), queriesTemp => Convert.ToInt32(queriesTemp));
        }

        long result = arrayManipulation(n, m, queries);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}
