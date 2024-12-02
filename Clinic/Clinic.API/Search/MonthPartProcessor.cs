using Clinic.API.Enums;

namespace Clinic.API
{
    public class MonthPartProcessor : DateTimePartProcessor
    {
        public MonthPartProcessor(int length, char[] closingChars) : base(length, closingChars)
        {
        }
        public override ProcessorResultEnum PrepareDateTimeParts(ref int[] start, ref int[] end, int i)
        {
            if (base.PrepareDateTimeParts(ref start, ref end, i) == ProcessorResultEnum.Stop)
                return ProcessorResultEnum.Stop;
            end[i + 1] = DateTime.DaysInMonth(start[i - 1], _value);
            return ProcessorResultEnum.Success;
        }
    }
}
