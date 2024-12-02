using Clinic.API.Enums;
using Clinic.API.Exceptions;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Web.Http;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Clinic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private IClinicDbContext _dbContext;
        private ParameterProcessor _parameterProcessor;

        public PatientsController(IClinicDbContext dbContext) => _dbContext = dbContext;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            if (_dbContext.Patients == null)
                return NotFound();
            return await _dbContext.Patients.Include(p => p.Name).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(Guid id)
        {
            if (_dbContext.Patients == null)
                return NotFound();
            var patient = await _dbContext.Patients.Include(p => p.Name).FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (patient == null)
                return NotFound();
            return patient;
        }

        private static PrefixesEnum? ParsePrefixesEnum(string input)
        {
            return (PrefixesEnum?)Enum.GetValues(typeof(PrefixesEnum)).OfType<object>()
                                 .FirstOrDefault(v => v.ToString() == input);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientByBirthDate(string parameter)
        {
            if (_dbContext.Patients == null)
                return NotFound();

            PrefixesEnum? prefix = null;

            if (string.IsNullOrEmpty(parameter) || parameter.Length < 4)
                return BadRequest();
            prefix = ParsePrefixesEnum(parameter.Substring(0, 2));
            if (prefix == null)
                prefix = PrefixesEnum.eq;
            else
                parameter = parameter.Substring(2);

            if (_parameterProcessor == null)
            {
                _parameterProcessor = new ParameterProcessor();
                _parameterProcessor.Add(new DateTimePartProcessor(4, new char[] { '-' }));
                _parameterProcessor.Add(new MonthPartProcessor(2, new char[] { '-' }));
                _parameterProcessor.Add(new DateTimePartProcessor(2, new char[] { 'T' }));
                _parameterProcessor.Add(new TimeProcessor());
            }

            if (_parameterProcessor.ExecuteParse(ref parameter) == ProcessorResultEnum.Invalid)
                return BadRequest();

            int[] start = new int[] { 1, 1, 1, 0, 0, 0, 0 };
            int[] end = new int[] { 9999, 12, 31, 23, 59, 59, 999 };

            _parameterProcessor.PrepareDateTimeParts(ref start, ref end, 0);

            DateTime dtStart = new DateTime(start[0], start[1], start[2], start[3], start[4], start[5], start[6], DateTimeKind.Utc);
            DateTime dtEnd = new DateTime(end[0], end[1], end[2], end[3], end[4], end[5], end[6], DateTimeKind.Utc);

            IEnumerable<Patient> patients = null;

            switch (prefix) 
            {
                case PrefixesEnum.eq:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => dtStart <= x.BirthDate && x.BirthDate <= dtEnd).ToListAsync();
                    break;
                case PrefixesEnum.ne:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => x.BirthDate < dtStart || dtEnd < x.BirthDate).ToListAsync();
                    break;
                case PrefixesEnum.lt:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => x.BirthDate < dtStart).ToListAsync();
                    break;
                case PrefixesEnum.gt:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => dtEnd < x.BirthDate).ToListAsync();
                    break;
                case PrefixesEnum.le:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => x.BirthDate <= dtEnd).ToListAsync();
                    break;
                case PrefixesEnum.ge:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => dtStart <= x.BirthDate).ToListAsync();
                    break;
                case PrefixesEnum.sa:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => x.BirthDate > dtStart).ToListAsync();
                    break;
                case PrefixesEnum.eb:
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => x.BirthDate < dtEnd).ToListAsync();
                    break;
                case PrefixesEnum.ap:
                    DateTime dtValue = dtStart.AddMilliseconds((dtEnd - dtStart).TotalMilliseconds/2);
                    int diff = (int)(Math.Abs((DateTime.UtcNow - dtValue).TotalMilliseconds)*0.1);                     
                    patients = await _dbContext.Patients.Include(p => p.Name)
                        .Where(x => Math.Abs((x.BirthDate - dtValue).TotalMilliseconds) <= diff).ToListAsync();
                    break;
                default:
                    break;
            }

            if (patients == null)
                return NotFound();
            return patients.ToList();
        }

        [HttpPost]
        public ActionResult<Guid> PostPatient(Patient patient)
        {
            if (patient.Id == Guid.Empty && patient.Name.Id == Guid.Empty)
            {
                patient.Id = patient.Name.Id = Guid.NewGuid();
            }
            else
            {
                if (patient.Id != Guid.Empty)
                    patient.Name.Id = patient.Id;
                else
                    patient.Id = patient.Name.Id;

                if (_dbContext.Patients.FirstOrDefault(x => x.Id == patient.Id) != null)
                    return BadRequest();
            }

            if (!this.ValidatePatient(patient))
                return BadRequest();

            _dbContext.Patients.Add(patient);
            _ = this.SaveChanges();

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient.Id);
        }

        [HttpPut("{id}")]
        public ActionResult<int> PutPatient(Guid id, Patient modifiedPatient)
        {
            if (id != modifiedPatient.Id)
                return BadRequest();

            var patient = _dbContext.Patients.Include(p => p.Name).FirstOrDefault(x => x.Id == modifiedPatient.Id);
            if (patient == null)
                return NotFound();

            if (!this.ValidatePatient(modifiedPatient))
                return BadRequest();

            modifiedPatient.CopyTo(patient);

            return this.SaveChanges();
        }

        [HttpDelete("{id}")]
        public ActionResult<int> DeletePatient(Guid id)
        {
            if (_dbContext.Patients == null)
                return NotFound();
            var patient = _dbContext.Patients.Include(p => p.Name).FirstOrDefault(x => x.Id == id);
            if (patient == null)
                return NotFound();
            _dbContext.Patients.Remove(patient);
            return this.SaveChanges();
        }

        private bool ValidatePatient(Patient patient) 
        {
            GenderEnum gender = GenderEnum.unknown;
            StateEnum active = StateEnum.False;

            if (string.IsNullOrEmpty(patient.Gender) || !Enum.TryParse<GenderEnum>(patient.Gender, true, out gender))
                return false;
            if (string.IsNullOrEmpty(patient.Active) || !Enum.TryParse<StateEnum>(patient.Active, true, out active))
                return false;

            patient.Gender = gender.ToString();
            patient.Active = active.ToString().ToLower();

            if (patient.BirthDate == default(DateTime) || patient.BirthDate > DateTime.UtcNow || patient.Name == null || string.IsNullOrEmpty(patient.Name.Family))
                return false;

            return true;
        }

        private int SaveChanges() 
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new SaveChangesException("Команда базы данных не повлияла на ожидаемое количество строк. Обычно это указывает на нарушение оптимистичного параллелизма; то есть строка была изменена в базе данных с момента запроса.");
            }
            catch (DbUpdateException e)
            {
                throw new SaveChangesException("Произошла ошибка при отправке обновлений в базу данных.");
            }
            catch (NotSupportedException)
            {
                throw new SaveChangesException("Предпринята попытка использовать неподдерживаемое поведение, например одновременное выполнение нескольких асинхронных команд на одном экземпляре контекста.");
            }
            catch (ObjectDisposedException)
            {
                throw new SaveChangesException("Контекст или соединение удалены.");
            }
            catch (InvalidOperationException)
            {
                throw new SaveChangesException("Произошла ошибка при попытке обработки сущностей в контексте до или после отправки команд в базу данных.");
            }
            catch (Exception)
            {
                throw new SaveChangesException("Неизвестная ошибкаю");
            }
        }
    }
}
