using Clinic.API.Enums;

namespace Clinic.API
{
    public class DateTimePartProcessor: BasePartProcessor, IParameterProcessor
    {
        protected int _length;
        protected char[] _closingChars;
        protected int _value;
        protected ProcessorResultEnum _state = ProcessorResultEnum.Stop;

        public ProcessorResultEnum State => _state;

        public DateTimePartProcessor()
        {
        }
        public DateTimePartProcessor(int length, char[] closingChars)
        {
            _length = length;
            _closingChars = closingChars;
        } 
        public override ProcessorResultEnum ExecuteParse(ref string parameter) 
        {
            if(string.IsNullOrEmpty(parameter))
                return _state = ProcessorResultEnum.Stop;
            if (parameter.Length < _length)
                return _state = ProcessorResultEnum.Invalid;
            else if(parameter.Length > _length) 
            {
                if (Array.IndexOf<char>(_closingChars, parameter[_length]) < 0)
                    return _state = ProcessorResultEnum.Invalid;
            }
            if (!int.TryParse(parameter.Substring(0, _length), out _value))
                return _state = ProcessorResultEnum.Invalid;
            if (parameter.Length > _length + 1)
                parameter = parameter.Substring(_length + 1);
            else
                parameter = "";

            return _state = ProcessorResultEnum.Success;
        }

        public override ProcessorResultEnum PrepareDateTimeParts(ref int[] start, ref int[] end, int i) 
        {
            if (_state == ProcessorResultEnum.Stop)
                return _state;
            start[i] = _value;
            end[i] = _value;
            return ProcessorResultEnum.Success;
        }
    }
}
