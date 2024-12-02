using Clinic.API.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clinic.API
{

    public class ParameterProcessor: IParameterProcessor
    {
        protected List<IParameterProcessor> _processors = new List<IParameterProcessor>();
        public void Add(IParameterProcessor partProcessor) 
        {
            _processors.Add(partProcessor);
        }

        public virtual ProcessorResultEnum ExecuteParse(ref string parameter) 
        {
            try
            {
                foreach (IParameterProcessor partProcessor in _processors)
                {
                    ProcessorResultEnum partResult = partProcessor.ExecuteParse(ref parameter);
                    switch (partResult)
                    {
                        case ProcessorResultEnum.Invalid:
                        case ProcessorResultEnum.Stop:
                            return partResult;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) 
            {
                return ProcessorResultEnum.Invalid;
            }
            return ProcessorResultEnum.Success;
        }

        public virtual ProcessorResultEnum PrepareDateTimeParts(ref int[] start, ref int[] end, int i)
        {
            for(int j = 0; j < _processors.Count; j++)
            {
                if(_processors[j].PrepareDateTimeParts(ref start, ref end, i + j) == ProcessorResultEnum.Stop)
                    return ProcessorResultEnum.Stop;
            }
            return ProcessorResultEnum.Success;
        }

    }
}
