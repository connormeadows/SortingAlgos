using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;

namespace SortingAlgos
{
    class Sort
    {
        private List<int> unsorted;
        private List<int> sorted;
        bool isSorted = false;

        // Just here because of convention
        public Sort()
        {
            unsorted = new List<int>();
            sorted = new List<int>();
        }

        // You'll always use this one
        public Sort(List<int> arr)
        {
            unsorted = arr;
            sorted = arr;
        }

        // Since the class is mostly for comparing performance I thought 
        // this constructor could be useful
        public Sort(List<int> un, List<int> done)
        {
            unsorted = un;
            sorted = done;
            isSorted = true;
        }

        public bool SortStatus()
        {
            return isSorted;
        }

        // A lot of algos need to swap things so I made it a method.
        private void Swap(int i, int j)
        {
            int temp = sorted[i];
            sorted[i] = sorted[j];
            sorted[j] = temp;
        }

        // Bubble sort is a cutie, that's for sure. Abismally slow, but adorable.
        public List<int> Bubble(bool ascending = true)
        {
            sorted = unsorted;

            for(int i = 0; i < sorted.Count; i++)
            {
                for(int j = 1; j < unsorted.Count; j++)
                {
                    if ((sorted[j] < sorted[j - 1] && ascending) || (sorted[j] > sorted[j - 1] && !ascending))
                        Swap(j, j - 1);
                }
            }

            isSorted = true;
            return sorted;
        }

        // Step through the indices, select what goes next.
        public List<int> Selection(bool ascending = true)
        {
            sorted = unsorted;
            int ind;

            for(int i = 0; i < sorted.Count; i++)
            {
                ind = i;

                for(int j = i; j < sorted.Count; j++)
                {
                    if((sorted[j] < sorted[ind] && ascending) || (sorted[j] > sorted[ind] && !ascending))
                    {
                        ind = j;
                    }
                }

                Swap(i, ind);
            }

            isSorted = true;
            return sorted;
        }

        // Inserts each value where it's supposed to go. 
        // This one is pretty speedy when all values are close to where they need to be
        public List<int> Insertion(bool ascending = true)
        {
            sorted = unsorted;

            int j, hold;

            for (int i = 1; i < sorted.Count; ++i)
            {
                // We're "holding" onto this value.
                hold = sorted[i];

                // Just used a for loop because the typical implementation with the while loop has an
                // instatiation of j to i-1, condition, and decrement anyway
                for(j = i-1; j >= 0 && ((sorted[j] > hold && ascending) || (sorted[j] < hold && !ascending)); j--)
                {
                    sorted[j + 1] = sorted[j];
                }
                sorted[j + 1] = hold;
            }

            isSorted = true;
            return sorted;
        }

        // The classic "Divide and conquer" mergesort. 1.5 years ago this took me a week to write,
        // today it took 30 minutes. I'm patting myself on the back for that one.
        public List<int> MergeSort(bool ascending = true)
        {
            sorted = unsorted;

            sorted = Split(sorted, ascending);

            isSorted = true;
            return sorted;
        }

        private List<int> Split(List<int> arr, bool ascending)
        {
            if (arr.Count > 1)
            {
                List<int> left, right;
                int middle = arr.Count / 2;
                left = Split(arr.GetRange(0, middle), ascending);
                right = Split(arr.GetRange(middle, arr.Count - middle), ascending);
                arr = Merge(left, right, ascending);
            }

            return arr;
        }

        private List<int> Merge(List<int> l, List<int> r, bool asc)
        {
            List<int> merged = new List<int>(l.Count + r.Count);

            int lInd = 0, rInd = 0, mInd = 0;
            while(lInd < l.Count || rInd < r.Count)
            {
                if(lInd >= l.Count)
                {
                    merged.Add(r[rInd++]);
                }
                else if(rInd >= r.Count)
                {
                    merged.Add(l[lInd++]);
                }
                else if((l[lInd] <= r[rInd] && asc) || (l[lInd] >= r[rInd] && !asc))
                {
                    merged.Add(l[lInd++]);
                }
                else
                {
                    Console.WriteLine("mInd: {0}, mLength: {1}, rInd: {2}, rLength: {3}", mInd, merged.Count, rInd, r.Count);
                    merged.Add(r[rInd++]);
                }
            }

            return merged;
        }

        // Going with the first, middle, last partition method. (Taking the middle of those three to be the partition)
        public List<int> QuickSort(bool ascending = true)
        {
            sorted = unsorted;

            sorted = QSHelper(sorted, ascending);

            isSorted = true;
            return sorted;
        }

        // Standard QuickSort. Get a partition, smaller values to the left, larger to the right.
        // I go with the recursive implementation because it looks cleaner to me personally.
        private List<int> QSHelper(List<int> arr, bool asc)
        {
            if(arr.Count > 1)
            {
                // Just calling the mergesort helper to sort these because it's there and why not
                List<int> partitions = new List<int>(3);
                partitions.Add(arr[0]); 
                partitions.Add(arr[arr.Count / 2]); 
                partitions.Add(arr[arr.Count - 1]);
                partitions = Split(partitions, true);
                int part = partitions[1];

                List<int> left = new List<int>(), right = new List<int>(), middle = new List<int>();

                // The "middle" list is probably new, it's a good way to handle duplicates and not get caught
                // in any unfortunate infinite loops.
                foreach(int i in arr)
                {
                    if ((i < part && asc) || (i > part && !asc))
                        left.Add(i);
                    else if (i == part)
                        middle.Add(i);
                    else
                        right.Add(i);
                }

                left = QSHelper(left, asc);
                right = QSHelper(right, asc);

                left.AddRange(middle);
                left.AddRange(right);

                return left;
            }

            return arr;
        }

        // My beloved Radix sort. I love me some linear runtime.
        public List<int> Radix(bool ascending = true)
        {
            sorted = unsorted;

            // Splitting the negatives from positives. No real change in asymptotic runtime.
            // Sort the negatives in reverse order from the positives, ( the helper takes every element's
            // absolute value ) because, ya know, magnitude vs direction and whatnot.

            List<int> negatives = new List<int>(), positives = new List<int>();

            foreach(int i in sorted)
            {
                if (i < 0)
                    negatives.Add(i);
                else
                    positives.Add(i);
            }

            negatives = RHelper(negatives, 1, !ascending);
            positives = RHelper(positives, 1, ascending);

            negatives.AddRange(positives);
            sorted = negatives;

            isSorted = true;
            return sorted;
        }

        // Comp will always be a power of two, so it works as the "mask"
        // Values with the comp's bit set go right, others go left. 
        private List<int> RHelper(List<int> arr, int comp, bool asc)
        {
            // 2**30 - 1. You may be wondering, "Why not 2**31 - 1?"
            // The answer is because I was getting stackoverflow errors when I tried that.
            // C'est La Vie
            if (comp >= 1073741823)
                return arr;

            List<int> left = new List<int>(), right = new List<int>();

            for(int j = 0; j < arr.Count; j++)
            {
                int res = Math.Abs(arr[j]) & comp;
                if ((res == 0 && asc) || (res != 0 && !asc))
                {
                    left.Add(arr[j]);
                }
                else
                {
                    right.Add(arr[j]);
                }
            }

            left.AddRange(right);
            left = RHelper(left, comp * 2, asc);

            return left;
        }

        public override string ToString()
        {
            string str = "The unsorted values are\n";

            for(int i = 0; i < unsorted.Count; i++)
            {
                if (i % 15 == 0)
                    str += "\n";
                str += unsorted[i] + "  ";
            }

            if (isSorted)
            {
                str += "\n\nThe sorted values are \n";
                for (int i = 0; i < sorted.Count; i++)
                {
                    if (i % 15 == 0)
                        str += "\n";
                    str += sorted[i] + "  ";
                }
            }
            else
                str += "\nThe list has not yet been sorted\n";

            return str;
        }
    }

    // The tester class for the above sorting algorithms. That's all
    class SortTester
    {
        static void Main()
        {
            Random rnd = new Random();

            List<int> arr = new List<int>();
            int len = 10000;

            for(int i = 0; i < len; i++)
            {
                arr.Add(rnd.Next(-100, 100)*i);
            }

            Sort sorter = new Sort(arr);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            sorter.Bubble();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time for bubble was {0}", elapsedMs);

            watch = System.Diagnostics.Stopwatch.StartNew();

            sorter.Selection();

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time for selection was {0}", elapsedMs);

            watch = System.Diagnostics.Stopwatch.StartNew();

            sorter.Insertion();

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time for Insertion was {0}", elapsedMs);

            watch = System.Diagnostics.Stopwatch.StartNew();

            sorter.MergeSort();

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time for mergesort was {0}", elapsedMs);

            watch = System.Diagnostics.Stopwatch.StartNew();

            sorter.QuickSort();

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time for QuickSort was {0}", elapsedMs);

            watch = System.Diagnostics.Stopwatch.StartNew();

            sorter.Radix();

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time for radix was {0}", elapsedMs);
        }
    }
}
