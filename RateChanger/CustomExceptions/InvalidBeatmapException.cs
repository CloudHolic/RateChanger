using System;
using System.Runtime.Serialization;

namespace RateChanger.CustomExceptions
{
    [Serializable]
    public class InvalidBeatmapException : Exception
    {
        public InvalidBeatmapException()
        {
        }

        public InvalidBeatmapException(string message) : base(message)
        {

        }

        public InvalidBeatmapException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //  This constructor is needed for serialization.
        protected InvalidBeatmapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}