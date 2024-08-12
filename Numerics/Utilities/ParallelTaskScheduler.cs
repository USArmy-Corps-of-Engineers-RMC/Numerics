/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Numerics.Utilities
{
    /// <summary>
    /// Custom TaskScheduler that processes work items in batches, where 
    /// each batch is processed by a ThreadPool thread, in parallel.
    /// </summary>
    public class ParallelTaskScheduler : TaskScheduler
    {
        [ThreadStatic]
        private static bool currentThreadIsProcessingItems;

        private int maxDegreeOfParallelism;

        private volatile int runningOrQueuedCount;

        private readonly LinkedList<Task> tasks = new LinkedList<Task>();

        /// <summary>
        /// Initialize new parallel tasks scheduler.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The max degree of parallelism.</param>
        public ParallelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
                throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }
        /// <inheritdoc/>
        public ParallelTaskScheduler() : this(Environment.ProcessorCount) { }

        /// <inheritdoc/>
        public override int MaximumConcurrencyLevel
        {
            get { return maxDegreeOfParallelism; }
        }

        /// <inheritdoc/>
        protected override bool TryDequeue(Task task)
        {
            lock (tasks) return tasks.Remove(task);
        }

        /// <inheritdoc/>
        protected override bool TryExecuteTaskInline(Task task,
            bool taskWasPreviouslyQueued)
        {
            if (!currentThreadIsProcessingItems) return false;

            if (taskWasPreviouslyQueued) TryDequeue(task);

            return base.TryExecuteTask(task);
        }

        /// <inheritdoc/>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);
                if (lockTaken) return tasks.ToArray();
                else throw new NotSupportedException();
            }
            finally { if (lockTaken) Monitor.Exit(tasks); }
        }

        /// <inheritdoc/>
        protected override void QueueTask(Task task)
        {
            lock (tasks) tasks.AddLast(task);

            if (runningOrQueuedCount < maxDegreeOfParallelism)
            {
                runningOrQueuedCount++;
                RunTasks();
            }
        }

        /// <summary>
        /// Runs the work on the ThreadPool.
        /// </summary>
        private void RunTasks()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                List<Task> taskList = new List<Task>();

                currentThreadIsProcessingItems = true;
                try
                {
                    while (true)
                    {
                        lock (tasks)
                        {
                            if (tasks.Count == 0)
                            {
                                runningOrQueuedCount--;
                                break;
                            }

                            var t = tasks.First.Value;
                            taskList.Add(t);
                            tasks.RemoveFirst();
                        }
                    }

                    if (taskList.Count == 1)
                    {
                        base.TryExecuteTask(taskList[0]);
                    }
                    else if (taskList.Count > 0)
                    {
                        var batches = taskList.GroupBy(
                            task => taskList.IndexOf(task) / maxDegreeOfParallelism);

                        foreach (var batch in batches)
                        {
                            batch.AsParallel().ForAll(task =>
                                base.TryExecuteTask(task));
                        }
                    }
                }
                finally { currentThreadIsProcessingItems = false; }

            }, null);
        }
    }
}
