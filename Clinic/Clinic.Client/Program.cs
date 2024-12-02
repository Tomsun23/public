using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Clinic.Client
{
    public class Patient
    {
        public PersonName Name { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Active { get; set; }
    }

    public class PersonName
    {
        public string? Use { get; set; }
        public string Family { get; set; }
        public string[]? Given { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Specify uri of the web api as command line parameter. Press any key.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Generating records...");

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(args[0]);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string[] genders = { "male", "female", "other", "unknown" };
            string[] actives = { "false", "true" };
            string[] uses = { "unknown", "official", "non-official" };
            string[] families = { "Иванов", "Петров", "Сидоров", "Смирнов", "Попов", "Стрельцов" };
            string[,] names = { { "Александр", "Игорь", "Максим", "Никита", "Сергей" }, { "Мария", "Алена", "Ольга", "Елена", "Кристина" } };
            string[,] middles = { { "Иванович", "Егорович", "Валентинович", "Петрович", "Эдуардович" }, { "Геннадьевна", "Олеговна", "Владимировна", "Витальевна", "Александровна" } };
            string[] postfixes = { "", "а" };

            int cnt = 0;
            Random rnd = new Random();
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    int gender = rnd.Next(genders.Length);

                    Patient patient = new Patient
                    {
                        Name = new PersonName()
                        {
                            Family = families[rnd.Next(families.Length)] + (gender < 2 ? postfixes[gender] : postfixes[rnd.Next(postfixes.Length)]),
                            Use = uses[rnd.Next(uses.Length)],
                            Given = new string[] { (gender < 2 ? names[gender,rnd.Next(names.GetLength(1))] : names[rnd.Next(names.GetLength(0)),rnd.Next(names.GetLength(1))]),
                                (gender < 2 ? middles[gender,rnd.Next(middles.GetLength(1))] : middles[rnd.Next(middles.GetLength(0)),rnd.Next(middles.GetLength(1))]) }
                        },
                        Gender = genders[gender],
                        Active = actives[rnd.Next(actives.Length)],
                        BirthDate = DateTime.UtcNow.AddDays(-1 * rnd.NextDouble() * 80 * 365),
                    };
                    HttpResponseMessage response = client.PostAsJsonAsync("api/patients", patient).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    cnt++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine($"Generated {cnt.ToString()} patient records.");
            Console.ReadKey();
        }
    }
}
