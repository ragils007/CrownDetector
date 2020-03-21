using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Msdfa.Core.Tools
{
    public class Hashing
    {
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();

            foreach (var t in hash) sb.Append(t.ToString("X2"));

            return sb.ToString();
        }

        #region Hashing Enumerables

        /*
         * Xor:
         * One downside of that is that the hash for { "x", "x" } is the same as the hash for { "y", "y" }. 
         * If that's not a problem for your situation though, it's probably the simplest solution.
         */

        public static int GetOrderIndependentHashCode_Xor<T>(IEnumerable<T> source)
        {
            var hash = 0;
            foreach (var element in source)
                hash = hash ^ EqualityComparer<T>.Default.GetHashCode(element);
            return hash;
        }

        /*
         * Addition:
         * Overflow is fine here, hence the explicit unchecked context.
         * There are still some nasty cases (e.g. {1, -1} and {2, -2}, but it's more likely to be okay, particularly with strings. 
         * In the case of lists that may contain such integers, you could always implement a custom hashing function (perhaps one 
         * that takes the index of recurrence of the specific value as a parameter and returns a unique hash code accordingly).
         */

        public static int GetOrderIndependentHashCode<T>(IEnumerable<T> source)
        {
            var hash = 0;
            foreach (var element in source)
                hash = unchecked(hash +
                                 EqualityComparer<T>.Default.GetHashCode(element));
            return hash;
        }

        /*
         * Here is an example of such an algorithm that gets around the aforementioned problem in a fairly efficient manner. 
         * It also has the benefit of greatly increasing the distribution of the hash codes generated (see the article linked 
         * at the end for some explanation). A mathematical/statistical analysis of exactly how this algorithm produces "better" 
         * hash codes would be quite advanced, but testing it across a large range of input values and plotting the results 
         * should verify it well enough.
         */

        public static int GetOrderIndependentHashCode_v2<T>(IEnumerable<T> source)
        {
            var hash = 0;
            int curHash;
            var bitOffset = 0;
            // Stores number of occurences so far of each value.
            var valueCounts = new Dictionary<T, int>();

            foreach (var element in source)
            {
                curHash = EqualityComparer<T>.Default.GetHashCode(element);
                if (valueCounts.TryGetValue(element, out bitOffset)) valueCounts[element] = bitOffset + 1;
                else valueCounts.Add(element, bitOffset);

                // The current hash code is shifted (with wrapping) one bit further left on each successive recurrence of a certain
                // value to widen the distribution. 37 is an arbitrary low prime number that helps the algorithm to smooth out the distribution.
                hash = unchecked(hash + ((curHash << bitOffset) | (curHash >> (32 - bitOffset)))*37);
            }

            return hash;
        }

        /*
         * Multiplication:
         * Which has few if benefits over addition: small numbers and a mix of positive and negative numbers they may lead to a better 
         * distribution of hash bits. As a negative to offset this "1" becomes a useless entry contributing nothing and any zero element 
         * results in a zero. You can special-case zero not to cause this major flaw.
         */

        public static int GetOrderIndependentHashCode_Multiplication<T>(IEnumerable<T> source)
        {
            var hash = 17;
            foreach (var element in source)
            {
                var h = EqualityComparer<T>.Default.GetHashCode(element);
                if (h != 0)
                    hash = unchecked(hash*h);
            }
            return hash;
        }

        /*
         * Order first
         * The other core approach is to enforce some ordering first, then use any hash combination function you like. 
         * The ordering itself is immaterial so long as it is consistent.
         * 
         * This has some significant benefits in that the combining operations possible in f can have significantly better 
         * hashing properties (distribution of bits for example) but this comes at significantly higher cost. The sort 
         * is O(n log n) and the required copy of the collection is a memory allocation you can't avoid given the desire 
         * to avoid modifying the original. GetHashCode implementations should normally avoid allocations entirely. 
         * One possible implementation of f would be similar to that given in the last example under the Addition 
         * section (e.g. any constant number of bit shifts left followed by a multiplication by a prime - you could even 
         * use successive primes on each iteration at no extra cost, since they only need be generated once).
         * 
         * That said, if you were dealing with cases where you could calculate and cache the hash and amortize the cost over 
         * many calls to GetHashCode this approach may yield superior behaviour. Also the latter approach is even more flexible 
         * since it can avoid the need to use the GetHashCode on the elements if it knows their type and instead use per byte 
         * operations on them to yield even better hash distribution. Such an approach would likely be of use only in cases 
         * where the performance was identified as being a significant bottleneck.
         * 
         * Finally, if you want a reasonably comprehensive and fairly non-mathematical overview of the subject of hash codes 
         * and their effectiveness in general, these blog posts < http://blog.roblevine.co.uk/?cat=10 > would be worthwhile reads, 
         * in particular the Implementing a simple hashing algorithm (pt II) post.

            public static int GetOrderIndependentHashCode_v3<T>(IEnumerable<T> source)
            {
                int hash = 0;
                foreach (T element in source.OrderBy(x => x, Comparer<T>.Default))
                {
                    // f is any function/code you like returning int
                    hash = f(hash, element);
                }
                return hash;
            }

         */

        #endregion
    }
}