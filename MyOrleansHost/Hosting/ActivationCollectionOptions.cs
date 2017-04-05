using System;
using System.Collections.Generic;

namespace Orleans.Hosting
{
    public class ActivationCollectionOptions
    {
        public TimeSpan DefaultCollectionAgeLimit { get; set; } = TimeSpan.FromHours(2);

        public Dictionary<string, TimeSpan> GrainOverrides { get; set; } = new Dictionary<string, TimeSpan>();

        public ActivationCollectionOptions AddOverride<TGrain>(TimeSpan ageLimit)
        {
            GrainOverrides.Add(typeof(TGrain).FullName, ageLimit);
            return this;
        }

        internal void Validate()
        {
            // Can validate values
        }
    }
}
