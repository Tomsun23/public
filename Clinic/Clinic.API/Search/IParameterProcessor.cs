using Clinic.API.Enums;

namespace Clinic.API
{
    public interface IParameterProcessor
    {
        ProcessorResultEnum ExecuteParse(ref string parameter);
        ProcessorResultEnum PrepareDateTimeParts(ref int[] start, ref int[] end, int i);
    }
}
