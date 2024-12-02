using Clinic.API.Enums;

namespace Clinic.API
{
    public abstract class BasePartProcessor
    {
        public abstract ProcessorResultEnum ExecuteParse(ref string parameter);
        public abstract ProcessorResultEnum PrepareDateTimeParts(ref int[] start, ref int[] end, int i);
    }
}
