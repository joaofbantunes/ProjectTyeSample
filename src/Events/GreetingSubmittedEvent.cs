using System;

namespace Events
{
    public class GreetingSubmittedEvent
    {
        public string Name { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
