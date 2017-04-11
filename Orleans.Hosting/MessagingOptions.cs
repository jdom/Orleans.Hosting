using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Orleans.Hosting
{
    public class MessagingOptions
    {
        /// <summary>
        /// The ResponseTimeout attribute specifies the default timeout before a request is assumed to have failed.
        /// </summary>
        public TimeSpan ResponseTimeout { get; set; } = Debugger.IsAttached ? TimeSpan.FromMinutes(30) : TimeSpan.FromSeconds(30);
        /// <summary>
        /// The MaxResendCount attribute specifies the maximal number of resends of the same message.
        /// </summary>
        public int MaxResendCount { get; set; } = 2;
        /// <summary>
        /// The ResendOnTimeout attribute specifies whether the message should be automaticaly resend by the runtime when it times out on the sender.
        /// Default is false.
        /// </summary>
        public bool ResendOnTimeout { get; set; } = false;

        /// <summary>
        /// The DropExpiredMessages attribute specifies whether the message should be dropped if it has expired, that is if it was not delivered 
        /// to the destination before it has timed out on the sender.
        /// Default is true.
        /// </summary>
        public bool DropExpiredMessages { get; set; } = true;

        /// <summary>
        /// The list of serialization providers
        /// </summary>
        public List<TypeInfo> SerializationProviders { get; } = new List<TypeInfo>();

        /// <summary>
        /// Gets the fallback serializer, used as a last resort when no other serializer is able to serialize an object.
        /// </summary>
        public TypeInfo FallbackSerializationProvider { get; set; }
    }
}