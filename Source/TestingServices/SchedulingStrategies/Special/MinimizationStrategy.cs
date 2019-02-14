
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.PSharp.IO;
using Microsoft.PSharp.TestingServices.SchedulingStrategies;
using Microsoft.PSharp.TestingServices.Tracing.Schedule;

namespace Microsoft.PSharp.TestingServices.Scheduling
{
    /// <summary>
    /// Class representing a replaying scheduling strategy.
    /// </summary>
    internal sealed class CriticalTransitionFindingStrategy : ISchedulingStrategy
    {
        #region fields

        /// <summary>
        /// The configuration.
        /// </summary>
        private Configuration Configuration;

        /// <summary>
        /// The P# program schedule trace.
        /// </summary>
        private ScheduleTrace ScheduleTrace;

        /// <summary>
        /// The suffix strategy.
        /// </summary>
        private ISchedulingStrategy SuffixStrategy;

        /// <summary>
        /// Is the scheduler that produced the
        /// schedule trace fair?
        /// </summary>
        private bool IsSchedulerFair;

        /// <summary>
        /// Is the scheduler replaying the trace?
        /// </summary>
        private bool IsReplaying;

        /// <summary>
        /// The number of scheduled steps.
        /// </summary>
        private int ScheduledSteps;

        /// <summary>
        /// Text describing a replay error.
        /// </summary>
        internal string ErrorText { get; private set; }


        /// <summary>
        /// Number of random walks to try before concluding we have executed the critical transition.
        /// </summary>
        internal int maxRandomWalks;
        private int? maxSchedulableSteps;

        /// <summary>
        /// Number of random walks to try before concluding we have executed the critical transition.
        /// </summary>
        internal int currentRandomWalks;


        /// <summary>
        /// Current left bound of critical transition ( in terms of number of steps )
        /// </summary>
        internal int leftBound;


        /// <summary>
        /// Current right bound of critical transition ( in terms of number of steps )
        /// </summary>
        internal int rightBound;

        // We only do binary 
        ///// <summary>
        ///// Are we in exponential or binary search phase?
        ///// </summary>
        //internal bool isExponentialSearch;


        /// <summary>
        /// Keep track in case of early exit - due to running out of time budget
        /// </summary>
        private int lastBugFoundSteps;

        /// <summary>
        /// 'Mid' of the binary search.
        /// </summary>
        internal int currentSearchSteps;

        #endregion

        #region public API

        // Disallow those without suffix strategy
        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="configuration">Configuration</param>
        ///// <param name="trace">ScheduleTrace</param>
        ///// <param name="externalEventTypes">Specifies a list of "external" events which are to be pruned first</param>
        ///// <param name="isFair">Is scheduler fair</param>
        //public CriticalTransitionFindingStrategy(Configuration configuration, ScheduleTrace trace, bool isFair)
        //    : this(configuration, trace, isFair, null)
        //{ }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="trace">ScheduleTrace</param>
        /// <param name="isFair">Is scheduler fair</param>
        /// <param name="suffixStrategy">The suffix strategy.</param>
        public CriticalTransitionFindingStrategy(Configuration configuration, 
            ScheduleTrace trace, 
            bool isFair, 
            ISchedulingStrategy suffixStrategy
            /*, List<Type> externalEventTypes, */ )
        {
            Configuration = configuration;
            ScheduleTrace = trace;
            ScheduledSteps = 0;
            IsSchedulerFair = isFair;
            IsReplaying = true;
            SuffixStrategy = suffixStrategy;
            ErrorText = string.Empty;

            maxRandomWalks = 5; // TODO: [t-krgov] - Accept as commandline option
            if(Configuration.MaxFairSchedulingSteps > ScheduleTrace.Count)
            {
                maxSchedulableSteps = Configuration.MaxFairSchedulingSteps + ScheduleTrace.Count; 
            }
            else
            {
                maxSchedulableSteps = 5 * ScheduleTrace.Count;
            }

            lastBugFoundSteps = -1;
            leftBound = 0;
            rightBound = ScheduleTrace.Count;
            currentSearchSteps = (leftBound + rightBound) / 2;
        }

        /// <summary>
        /// Returns the next choice to schedule.
        /// </summary>
        /// <param name="next">Next</param>
        /// <param name="choices">Choices</param>
        /// <param name="current">Curent</param>
        /// <returns>Boolean</returns>
        public bool GetNext(out ISchedulable next, List<ISchedulable> choices, ISchedulable current)
        {

            if (IsReplaying && ScheduledSteps >= currentSearchSteps)
            {
                IsReplaying = false;
            }
            //Console.WriteLine($"GetNext called with {ScheduledSteps} ; IsReplaying={IsReplaying}");

            if (IsReplaying)
            {
                var enabledChoices = choices.Where(choice => choice.IsEnabled).ToList();
                if (enabledChoices.Count == 0)
                {
                    next = null;
                    return false;
                }

                try
                {
                    if (ScheduledSteps >= ScheduleTrace.Count)
                    {
                        ErrorText = "Trace is not reproducible: execution is longer than trace.";
                        throw new InvalidOperationException(ErrorText);
                    }

                    ScheduleStep nextStep = ScheduleTrace[ScheduledSteps];
                    if (nextStep.Type != ScheduleStepType.SchedulingChoice)
                    {
                        ErrorText = "Trace is not reproducible: next step is not a scheduling choice.";
                        throw new InvalidOperationException(ErrorText);
                    }

                    next = enabledChoices.FirstOrDefault(choice => choice.Id == nextStep.ScheduledMachineId);
                    if (next == null)
                    {
                        ErrorText = $"Trace is not reproducible: cannot detect id '{nextStep.ScheduledMachineId}'.";
                        throw new InvalidOperationException(ErrorText);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (SuffixStrategy == null)
                    {
                        if (!Configuration.DisableEnvironmentExit)
                        {
                            Error.ReportAndExit(ex.Message);
                        }

                        next = null;
                        return false;
                    }
                    else
                    {
                        IsReplaying = false;
                        return SuffixStrategy.GetNext(out next, choices, current);
                    }
                }

                ScheduledSteps++;
                return true;
            }

            return SuffixStrategy.GetNext(out next, choices, current);
        }

        /// <summary>
        /// Returns the next boolean choice.
        /// </summary>
        /// <param name="maxValue">The max value.</param>
        /// <param name="next">Next</param>
        /// <returns>Boolean</returns>
        public bool GetNextBooleanChoice(int maxValue, out bool next)
        {
            if (IsReplaying)
            {
                ScheduleStep nextStep = null;

                try
                {
                    if (ScheduledSteps >= ScheduleTrace.Count)
                    {
                        ErrorText = "Trace is not reproducible: execution is longer than trace.";
                        throw new InvalidOperationException(ErrorText);
                    }

                    nextStep = ScheduleTrace[ScheduledSteps];
                    if (nextStep.Type != ScheduleStepType.NondeterministicChoice)
                    {
                        ErrorText = "Trace is not reproducible: next step is not a nondeterministic choice.";
                        throw new InvalidOperationException(ErrorText);
                    }

                    if (nextStep.BooleanChoice == null)
                    {
                        ErrorText = "Trace is not reproducible: next step is not a nondeterministic boolean choice.";
                        throw new InvalidOperationException(ErrorText);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (SuffixStrategy == null)
                    {
                        if (!Configuration.DisableEnvironmentExit)
                        {
                            Error.ReportAndExit(ex.Message);
                        }

                        next = false;
                        return false;
                    }
                    else
                    {
                        IsReplaying = false;
                        return SuffixStrategy.GetNextBooleanChoice(maxValue, out next);
                    }
                }

                next = nextStep.BooleanChoice.Value;
                ScheduledSteps++;
                return true;
            }

            return SuffixStrategy.GetNextBooleanChoice(maxValue, out next);
        }

        /// <summary>
        /// Returns the next integer choice.
        /// </summary>
        /// <param name="maxValue">The max value.</param>
        /// <param name="next">Next</param>
        /// <returns>Boolean</returns>
        public bool GetNextIntegerChoice(int maxValue, out int next)
        {
            if (IsReplaying)
            {
                ScheduleStep nextStep = null;

                try
                {
                    if (ScheduledSteps >= ScheduleTrace.Count)
                    {
                        ErrorText = "Trace is not reproducible: execution is longer than trace.";
                        throw new InvalidOperationException(ErrorText);
                    }

                    nextStep = ScheduleTrace[ScheduledSteps];
                    if (nextStep.Type != ScheduleStepType.NondeterministicChoice)
                    {
                        ErrorText = "Trace is not reproducible: next step is not a nondeterministic choice.";
                        throw new InvalidOperationException(ErrorText);
                    }

                    if (nextStep.IntegerChoice == null)
                    {
                        ErrorText = "Trace is not reproducible: next step is not a nondeterministic integer choice.";
                        throw new InvalidOperationException(ErrorText);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (SuffixStrategy == null)
                    {
                        if (!Configuration.DisableEnvironmentExit)
                        {
                            Error.ReportAndExit(ex.Message);
                        }

                        next = 0;
                        return false;
                    }
                    else
                    {
                        IsReplaying = false;
                        return SuffixStrategy.GetNextIntegerChoice(maxValue, out next);
                    }
                }

                next = nextStep.IntegerChoice.Value;
                ScheduledSteps++;
                return true;
            }

            return SuffixStrategy.GetNextIntegerChoice(maxValue, out next);
        }

        /// <summary>
        /// Forces the next choice to schedule.
        /// </summary>
        /// <param name="next">Next</param>
        /// <param name="choices">Choices</param>
        /// <param name="current">Curent</param>
        /// <returns>Boolean</returns>
        public void ForceNext(ISchedulable next, List<ISchedulable> choices, ISchedulable current)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forces the next boolean choice.
        /// </summary>
        /// <param name="maxValue">The max value.</param>
        /// <param name="next">Next</param>
        /// <returns>Boolean</returns>
        public void ForceNextBooleanChoice(int maxValue, bool next)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forces the next integer choice.
        /// </summary>
        /// <param name="maxValue">The max value.</param>
        /// <param name="next">Next</param>
        /// <returns>Boolean</returns>
        public void ForceNextIntegerChoice(int maxValue, int next)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepares for the next scheduling iteration. This is invoked
        /// at the end of a scheduling iteration. It must return false
        /// if the scheduling strategy should stop exploring.
        ///     In the critical specific case, We say false iff we reach nRandomwalks
        /// </summary>
        /// <returns>True to start the next iteration</returns>
        public bool PrepareForNextIteration()
        {
            ScheduledSteps = 0;
            currentRandomWalks++;
            IsReplaying = true;
            bool suffixStrategySaysWhat = true;
            if (SuffixStrategy != null)
            {
                suffixStrategySaysWhat  = SuffixStrategy.PrepareForNextIteration();
            }

            return suffixStrategySaysWhat && currentRandomWalks < maxRandomWalks ;
        }

        /// <summary>
        /// Resets the scheduling strategy. This is typically invoked by
        /// parent strategies to reset child strategies.
        /// </summary>
        public void Reset()
        {
            ScheduledSteps = 0;
            IsReplaying = true;
            SuffixStrategy?.Reset();
        }

        /// <summary>
        /// Returns the scheduled steps.
        /// </summary>
        /// <returns>Scheduled steps</returns>
        public int GetScheduledSteps()
        {
            if (SuffixStrategy != null)
            {
                return ScheduledSteps + SuffixStrategy.GetScheduledSteps();
            }
            else
            {
                return ScheduledSteps;
            }
        }

        /// <summary>
        /// True if the scheduling strategy has reached the depth
        /// bound for the given scheduling iteration.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool HasReachedMaxSchedulingSteps()
        {
            //TODO: [t-krgov] Fix this to be configurable?
            //if (SuffixStrategy != null)
            //{
            //    return SuffixStrategy.HasReachedMaxSchedulingSteps();
            //}
            //else
            //{
            //    return false;
            //}
            return GetScheduledSteps() > maxSchedulableSteps;
        }

        /// <summary>
        /// Checks if this is a fair scheduling strategy.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool IsFair()
        {
            if (SuffixStrategy != null)
            {
                return SuffixStrategy.IsFair();
            }
            else
            {
                return IsSchedulerFair;
            }
        }

        /// <summary>
        /// Returns a textual description of the scheduling strategy.
        /// </summary>
        /// <returns>String</returns>
        public string GetDescription()
        {
            if (SuffixStrategy != null)
            {
                return "Critical transition finding with suffix strategy (" + SuffixStrategy.GetDescription() + ")";
            }
            else
            {
                return "Critical transition finding ";
            }
        }

        #endregion


        #region Critical transition specific methods

        /// <summary>
        /// Manually set initial bounds for critical transition. 
        /// Default of (0, ScheduleTrace.Length is set in constructor )
        /// </summary>
        /// <param name="leftBoundStart">Initial value of leftBound for search procedure </param>
        /// <param name="rightBoundStart">Initial value of rightBound for search procedure </param>
        /// <returns></returns>
        internal void setBounds(int leftBoundStart, int rightBoundStart)
        {
            leftBound = leftBoundStart;
            rightBound = rightBoundStart;
            currentRandomWalks = 0;
            currentSearchSteps = (leftBound + rightBound) / 2;
        }


        /// <summary>
        /// returns false if the search has concluded.
        /// </summary>
        /// <param name="foundBug"></param>
        /// <returns></returns>
        internal bool updateBounds(bool foundBug)
        {
            if (foundBug)
            {
                rightBound = currentSearchSteps;
                lastBugFoundSteps = currentSearchSteps;
            }
            else
            {
                leftBound = 1 + currentSearchSteps;
            }
            currentSearchSteps = (leftBound + rightBound) / 2;
            currentRandomWalks = -1; // Sorry
            Console.WriteLine($"Updated bounds to ({leftBound},{rightBound})");
            return (leftBound < rightBound);
        }


        /// <summary>
        /// returns -1 if we've never found a bug; steps required otherwise
        /// </summary>
        /// <returns></returns>
        internal int getLastFoundBugSteps()
        {
            return lastBugFoundSteps;
        }
        #endregion
    }
}
