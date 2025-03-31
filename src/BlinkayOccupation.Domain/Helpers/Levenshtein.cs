namespace BlinkayOccupation.Domain.Helpers
{
    public sealed class Levenshtein
    {
        private readonly int[] _costs;
        private readonly string _storedValue;

        /// <summary>
        ///     Creates a new instance with a value to test other values against
        /// </summary>
        /// <param name="value">Value to compare other values to.</param>
        public Levenshtein(string value)
        {
            _storedValue = value;
            _costs = new int[_storedValue.Length];
        }

        /// <summary>
        ///     Compares a value to the stored value. Not thread safe.
        /// </summary>
        /// <returns>Difference. 0 complete match.</returns>
        public int DistanceFrom(ReadOnlySpan<char> value)
        {
            if (_costs.Length == 0)
            {
                return value.Length;
            }

            var costs = _costs;
            var storedValue = _storedValue;
            var storedValueLength = storedValue.Length;

            // Add indexing for insertion to first row
            for (var i = 0; i < costs.Length;)
            {
                costs[i] = ++i;
            }

            for (var i = 0; i < value.Length; ++i)
            {
                // cost of the first index
                var cost = i;
                var previousCost = i;
                var currentChar = value[i];

                for (var j = 0; j < storedValueLength; ++j)
                {
                    var currentCost = cost;
                    cost = costs[j];

                    if (currentChar != storedValue[j])
                    {
                        if (previousCost < currentCost)
                        {
                            currentCost = previousCost;
                        }

                        if (cost < currentCost)
                        {
                            currentCost = cost;
                        }

                        ++currentCost;
                    }

                    costs[j] = currentCost;
                    previousCost = currentCost;
                }
            }

            return costs[^1];
        }
    }
}
