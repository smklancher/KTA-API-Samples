using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAgility.Sdk;

namespace KtaCase
{
    public class JobNote
    {
        /// <summary>
        /// Repeatedly creates job notes, which is intended to demonstrate an error when they are added in the same millisecond (PK error). Case 24032484
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="jobId"></param>
        /// <param name="numberOfNotes"></param>
        public void RepeatedJobNote(string sessionId, string jobId, int numberOfNotes)
        {
            JobService js = new JobService();
            for (int i = 0; i < numberOfNotes; i++)
            {
                js.AddJobNote(
                    sessionId,
                    new Agility.Sdk.Model.Jobs.JobIdentity() { Id = jobId },
                    new Agility.Sdk.Model.Server.NoteInfo() { NoteText = $"Test note {DateTime.Now.ToLongTimeString()}", TypeId = "9E298F8BC47F49B3BAD91B852862B4D6" }
                );
            }
        }
    }
}