using System;

namespace Dovecote.Eventing.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class BatchOptionsAttribute : Attribute
    {
        public BatchOptionsAttribute()
        {
            Interval = 10;
        }

        public BatchOptionsAttribute(int interval)
        {
            Interval = interval;
        }

        public int Interval { get; set; }
    }
}