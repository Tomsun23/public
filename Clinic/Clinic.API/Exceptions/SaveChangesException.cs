namespace Clinic.API.Exceptions
{
    public class SaveChangesException : Exception
    {
        public SaveChangesException(string message) : base(message) { }
    }
}
