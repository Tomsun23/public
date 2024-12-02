using Clinic.API.Enums;

namespace Clinic.API
{
    public class TimeProcessor : ParameterProcessor
    {
        public TimeProcessor()
        {
            _processors.Add(new DateTimePartProcessor(2, new char[] { ':' }));
            _processors.Add(new DateTimePartProcessor(2, new char[] { ':' }));
            _processors.Add(new DateTimePartProcessor(2, new char[] { '.' }));
            _processors.Add(new DateTimePartProcessor(4, new char[] { 'Z', '+', '-', }));
        }
        public override ProcessorResultEnum PrepareDateTimeParts(ref int[] start, ref int[] end, int i)
        {
            int j = 0;
            if (
                _processors[j].PrepareDateTimeParts(ref start, ref end, i + j++) == ProcessorResultEnum.Stop ||
                _processors[j].PrepareDateTimeParts(ref start, ref end, i + j++) == ProcessorResultEnum.Stop ||
                _processors[j].PrepareDateTimeParts(ref start, ref end, i + j++) == ProcessorResultEnum.Stop ||
                _processors[j].PrepareDateTimeParts(ref start, ref end, i + j++) == ProcessorResultEnum.Stop
            )
                return ProcessorResultEnum.Stop;
            return ProcessorResultEnum.Success;
        }

        public override ProcessorResultEnum ExecuteParse(ref string parameter)
        {
            for (int i = 0; i < _processors.Count; i++)
            {
                ProcessorResultEnum partResult = _processors[i].ExecuteParse(ref parameter);
                switch (partResult)
                {
                    case ProcessorResultEnum.Invalid:
                        return partResult;
                    case ProcessorResultEnum.Stop:
                        if (i == 1)
                            return ProcessorResultEnum.Invalid;
                        return partResult;
                    default:
                        break;
                }
            }
            return ProcessorResultEnum.Success;
        }
    }
}
