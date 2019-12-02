using Agility.Sdk.Model.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAgility.Sdk;

namespace KtaCase
{
    class Jobs
    {
        public void Test(string sessionId)
        {
            var j = new JobService();

            while (true)
            {
                Debugger.Break();

                var jf = new JobFilter4();
                jf.MaxNumberToRetrieve = 100;



                var jobs = j.GetJobs4(sessionId, jf);

                int jobCount = jobs.JobCount;
                Trace.WriteLine($"Found {jobCount} jobs");
                Trace.WriteLine(ReflectionHelper.PropertyList(jf));
            }
            
        }
    }
}
