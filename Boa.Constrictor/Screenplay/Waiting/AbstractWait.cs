﻿using Boa.Constrictor.Logging;
using System.Diagnostics;

namespace Boa.Constrictor.Screenplay
{
    /// <summary>
    /// Waits for a desired state.
    /// If the desired state does not happen within the time limit, then an exception is thrown.
    /// 
    /// If the actor has the SetTimeouts ability, then the ability will be used to calculate timeouts.
    /// Otherwise, DefaultTimeout will be used.
    /// </summary>
    public abstract class AbstractWait
    {
        #region Constants

        /// <summary>
        /// The default timeout value.
        /// Use this if an override timeout value is not provided,
        /// And if the actor does not have the SetTimeouts ability.
        /// </summary>
        public const int DefaultTimeout = 30;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor.
        /// (Use static methods for public construction.)
        /// </summary>
        protected AbstractWait()
        {
            TimeoutSeconds = null;
            AdditionalSeconds = 0;
            ActualTimeout = -1;
            SuppressLogs = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The timeout override in seconds.
        /// If null, use the standard timeout value.
        /// </summary>
        public int? TimeoutSeconds { get; protected set; }

        /// <summary>
        /// An additional amount to add to the timeout.
        /// </summary>
        public int AdditionalSeconds { get; protected set; }

        /// <summary>
        /// The actual timeout used.
        /// </summary>
        public int ActualTimeout { get; set; }

        /// <summary>
        /// If true, do not print log messages below "Warning" severity while waiting.
        /// This is set to true by default.
        /// </summary>
        public bool SuppressLogs { get; protected set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// If the actor has the SetTimeouts ability, then the ability will be used to calculate timeouts.
        /// Otherwise, DefaultTimeout will be used.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        protected int CalculateTimeout(IActor actor)
        {
            int timeout = actor.HasAbilityTo<SetTimeouts>()
                ? actor.Using<SetTimeouts>().CalculateTimeout(TimeoutSeconds)
                : TimeoutSeconds ?? DefaultTimeout;

            return timeout + AdditionalSeconds;
        }

        /// <summary>
        /// Waits until the question's answer value meets the condition.
        /// If the expected condition is not met within the time limit, then an exception is thrown.
        /// Returns the actual awaited value.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <returns></returns>
        protected void WaitForValue(IActor actor)
        {
            // Set variables
            bool satisfied = false;
            ActualTimeout = CalculateTimeout(actor);

            // Adjust log level if necessary (to avoid too many messages)
            LogSeverity original = actor.Logger.LowestSeverity;
            if (SuppressLogs && actor.Logger.LowestSeverity < LogSeverity.Warning)
                actor.Logger.LowestSeverity = LogSeverity.Warning;

            // Start the timer
            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                // Repeatedly check if the condition is satisfied until the timeout is reached
                do
                {
                    satisfied = EvaluateCondition(actor);
                }
                while (!satisfied && timer.Elapsed.TotalSeconds < ActualTimeout);
            }
            finally
            {
                // Return the log level to normal, no matter what goes wrong
                if (SuppressLogs)
                    actor.Logger.LowestSeverity = original;

                // Stop the timer
                timer.Stop();
            }

            // Verify successful waiting
            if (!satisfied)
                ThrowWaitException();
        }

        /// <summary>
        /// Evaluate the condition.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <returns></returns>
        protected abstract bool EvaluateCondition(IActor actor);

        /// <summary>
        /// Throw the waiting exception if condition is not satisfied
        /// </summary>
        protected abstract void ThrowWaitException();

        /// <summary>
        /// Returns a description of the task.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = $"wait until the condition is satisfied";

            if (ActualTimeout >= 0)
                s += $" for up to {ActualTimeout}s";
            else if (TimeoutSeconds != null)
                s += $" for up to {TimeoutSeconds + AdditionalSeconds}s";

            return s;
        }

        #endregion
    }
}
