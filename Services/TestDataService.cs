using MedicalCallServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicalCallServer.Services
{
    public class TestDataService : ITestDataService
    {
        private const int DefaultTestRecordsCount = 10;
        private const CallType DefaultCallType = CallType.Ambulance;

        private static readonly Random _random = new();
        private static readonly object _lock = new();

        private static List<MedicalAssistanceCall>? _cachedCalls;
        private static List<TCallStatus>? _cachedStatuses;

        private static int _nextId = 1;
        private static readonly DateTime _today = DateTime.Today;

        public List<MedicalAssistanceCall> GetTestCalls()
        {
            lock (_lock)
            {
                if (_cachedCalls == null)
                    _cachedCalls = GenerateCalls();
                return new List<MedicalAssistanceCall>(_cachedCalls);
            }
        }

        public List<TCallStatus> GetTestStatuses()
        {
            lock (_lock)
            {
                if (_cachedStatuses == null)
                    _cachedStatuses = Enum.GetValues<TCallStatus>().ToList();
                return new List<TCallStatus>(_cachedStatuses);
            }
        }

        private List<MedicalAssistanceCall> GenerateCalls()
        {
            var calls = new List<MedicalAssistanceCall>();
            var statuses = GetTestStatuses();

            var paymentTypes = new[] { "Наличный", "Безналичный" };
            var doctors = new[] { "Петров А.С.", "Иванов М.В.", "Сидорова Е.П." };
            var paramedics = new[] { "Козлов В.П.", "Смирнов И.В.", "Николаев Д.С." };
            var drivers = new[] { "Федоров А.А.", "Морозов К.И." };
            var dispatchers = new[] { "Иванова Е.А.", "Николаев Д.С.", "Сидорова М.И." };

            for (int i = 0; i < DefaultTestRecordsCount; i++)
            {
                var status = statuses[_random.Next(statuses.Count)];
                var birthYear = _random.Next(_today.Year - 80, _today.Year - 18);

                // Базовое время вызова (Прием вызова)
                var receptionHour = _random.Next(8, 20);
                var receptionTimeSpan = TimeSpan.FromHours(receptionHour) + TimeSpan.FromMinutes(_random.Next(0, 60));
                var receptionTime = _today.Add(receptionTimeSpan);

                DateTime? transferTime = null;
                DateTime? departureTime = null;
                DateTime? arrivalTime = null;
                DateTime? completionTime = null;
                TimeSpan? workDuration = null;
                decimal? cost = null; // По умолчанию null, так как в модели тип decimal?

                // Если вызов НЕ отменен и НЕ на согласовании
                if (status != TCallStatus.csCancelled && status != TCallStatus.csPending)
                {
                    transferTime = receptionTime.AddMinutes(_random.Next(3, 10));
                    departureTime = receptionTime.AddMinutes(_random.Next(5, 20));
                    arrivalTime = departureTime.Value.AddMinutes(_random.Next(15, 45));

                    if (status == TCallStatus.csCompleted)
                    {
                        completionTime = arrivalTime.Value.AddMinutes(_random.Next(20, 90));
                        workDuration = completionTime.Value - departureTime.Value;
                        cost = _random.Next(1500, 5000);
                    }
                }

                calls.Add(new MedicalAssistanceCall
                {
                    Id = _nextId++,
                    Number = 1000 + i,
                    StatusId = (int)status,
                    TypeOfCall = DefaultCallType,
                    PatientName = GenerateRandomName(),
                    DateOfBirth = new DateTime(birthYear, _random.Next(1, 13), _random.Next(1, 29)),
                    Age = _today.Year - birthYear,
                    AddressStreet = GenerateRandomAddress(),
                    AddressDetails = $"кв. {_random.Next(1, 100)}, подъезд {_random.Next(1, 5)}",
                    ContactInfo = $"+7 (999) {_random.Next(100, 999)}-{_random.Next(1000, 9999)}",

                    // Время
                    CallDate = _today,
                    ReceptionTime = receptionTime,
                    TransferTime = transferTime,
                    DepartureTime = departureTime,
                    ArrivalTime = arrivalTime,
                    CompletionTime = completionTime,
                    WorkDuration = workDuration,

                    // Диагнозы
                    Complaints = GenerateRandomComplaints(),
                    Diagnosis = status == TCallStatus.csCompleted ? GenerateRandomDiagnosis() : null,
                    Conclusion = status == TCallStatus.csCompleted ? GenerateRandomResult() : null,
                    Comment = "Тестовый комментарий",
                    Note = null,

                    // Оплата и страховка
                    PaymentType = paymentTypes[_random.Next(paymentTypes.Length)],
                    InsuranceNumber = $"POL-{_random.Next(100000, 999999)}",
                    Customer = GenerateRandomCustomer(),
                    CustomerRepresentative = _random.Next(2) == 0 ? "Иванов И.И. (родственник)" : null,
                    Cost = cost,
                    MKADDistance = _random.Next(0, 2) == 1 ? _random.Next(1, 30) : null, // За пределами МКАД или null

                    // Бригада
                    BrigadeNumber = $"Б-{_random.Next(1, 10):D2}",
                    Doctor = doctors[_random.Next(doctors.Length)],
                    Paramedic = paramedics[_random.Next(paramedics.Length)],
                    Driver = drivers[_random.Next(drivers.Length)],
                    Dispatcher1 = dispatchers[_random.Next(dispatchers.Length)],
                    Dispatcher2 = dispatchers[_random.Next(dispatchers.Length)],
                    IsWaiting = false
                });
            }
            return calls;
        }

        private string GenerateRandomName()
        {
            var names = new[]
            {
                (first: "Иван", last: "Иванов", middle: "Иванович"),
                (first: "Пётр", last: "Петров", middle: "Петрович"),
                (first: "Сергей", last: "Сидоров", middle: "Сергеевич"),
                (first: "Анна", last: "Иванова", middle: "Ивановна"),
                (first: "Мария", last: "Петрова", middle: "Петровна"),
                (first: "Елена", last: "Сидорова", middle: "Сергеевна")
            };

            var n = names[_random.Next(names.Length)];
            return $"{n.last} {n.first} {n.middle}";
        }

        private string GenerateRandomAddress() =>
            $"ул. {new[] { "Ленина", "Гагарина", "Победы", "Мира" }[_random.Next(4)]}, д. {_random.Next(1, 150)}";
        private string GenerateRandomComplaints() =>
            new[] { "Температура 38.5, головная боль", "Головокружение, слабость", "Аллергическая реакция" }[_random.Next(3)];
        private string GenerateRandomDiagnosis() =>
            new[] { "ОРВИ", "Гипертонический криз", "Острый бронхит" }[_random.Next(3)];
        private string GenerateRandomResult() =>
            new[] { "Назначено лечение", "Госпитализация", "Рекомендации" }[_random.Next(3)];
        private string GenerateRandomCustomer() =>
            new[] { "Частное лицо", "СК Альфа", "Поликлиника №5" }[_random.Next(3)];
    }
}
