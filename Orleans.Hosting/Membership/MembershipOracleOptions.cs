using System;

namespace Orleans.Hosting.Membership
{
    public class MembershipOracleOptions
    {
        /// <summary>
        /// Global switch to disable silo liveness protocol (should be used only for testing).
        /// The LivenessEnabled attribute, if provided and set to "false", suppresses liveness enforcement.
        /// If a silo is suspected to be dead, but this attribute is set to "false", the suspicions will not propagated to the system and enforced,
        /// This parameter is intended for use only for testing and troubleshooting.
        /// In production, liveness should always be enabled.
        /// Default is true (eanabled)
        /// </summary>
        public bool LivenessEnabled { get; set; } = true;
        /// <summary>
        /// The number of seconds to periodically probe other silos for their liveness or for the silo to send "I am alive" heartbeat  messages about itself.
        /// </summary>
        public TimeSpan ProbeTimeout { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// The number of seconds to periodically fetch updates from the membership table.
        /// </summary>
        public TimeSpan TableRefreshTimeout { get; set; } = TimeSpan.FromSeconds(60);
        /// <summary>
        /// Expiration time in seconds for death vote in the membership table.
        /// </summary>
        public TimeSpan DeathVoteExpirationTimeout { get; set; } = TimeSpan.FromSeconds(120);
        /// <summary>
        /// The number of seconds to periodically write in the membership table that this silo is alive. Used ony for diagnostics.
        /// </summary>
        public TimeSpan IAmAliveTablePublishTimeout { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// The number of seconds to attempt to join a cluster of silos before giving up.
        /// </summary>
        public TimeSpan MaxJoinAttemptTime { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// The number of missed "I am alive" heartbeat messages from a silo or number of un-replied probes that lead to suspecting this silo as dead.
        /// </summary>
        public int NumMissedProbesLimit { get; set; }
        /// <summary>
        /// The number of silos each silo probes for liveness.
        /// </summary>
        public int NumProbedSilos { get; set; }
        /// <summary>
        /// The number of non-expired votes that are needed to declare some silo as dead (should be at most NumMissedProbesLimit)
        /// </summary>
        public int NumVotesForDeathDeclaration { get; set; }
        /// <summary>
        /// The number of missed "I am alive" updates  in the table from a silo that causes warning to be logged. Does not impact the liveness protocol.
        /// </summary>
        public int NumMissedTableIAmAliveLimit { get; set; }
        /// <summary>
        /// Whether to use the gossip optimization to speed up spreading liveness information.
        /// </summary>
        public bool UseLivenessGossip { get; set; }
        /// <summary>
        /// Whether new silo that joins the cluster has to validate the initial connectivity with all other Active silos.
        /// </summary>
        public bool ValidateInitialConnectivity { get; set; }
    }
}
