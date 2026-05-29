using System.ComponentModel.DataAnnotations;

namespace MedicalCallServer.Models
{
    public enum CallType { DoctorAtHome, Ambulance }

    public enum TCallStatus
    {
        csNew = 1, csPending = 2, csFromInsurance = 3,
        csInWork = 4, csCancelled = 5, csCompleted = 6
    }

     public static class CallStatusHelper
    {
        public static string GetName(TCallStatus s) => s switch
        {
            TCallStatus.csNew => "Новая заявка",
            TCallStatus.csPending => "На согласовании",
            TCallStatus.csFromInsurance => "Заявка от страховой",
            TCallStatus.csInWork => "Взята в работу бригадой",
            TCallStatus.csCancelled => "Отменена",
            TCallStatus.csCompleted => "Завершена",
            _ => s.ToString()
        };
    }

    public class MedicalAssistanceCall
    {
        #region Вызов
        public int Id { get; set; }

        [Required]
        public int StatusId { get; set; }
        
        public string Status => CallStatusHelper.GetName((TCallStatus)StatusId);

        [Required] public CallType TypeOfCall { get; set; }
        [Required] public int Number { get; set; }
        #endregion

        #region Пациент
        [Required] public string PatientName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        [Required] public string AddressStreet { get; set; } = string.Empty;
        public string? AddressDetails { get; set; }
        public string? ContactInfo { get; set; }
        #endregion

        #region Диагноз и помощь
        public string? Complaints { get; set; }
        public string? Comment { get; set; }
        public string? Diagnosis { get; set; }
        public string? Conclusion { get; set; }
        public string? Note { get; set; }
        #endregion

        #region Время
        [Required]
        public DateTime? CallDate { get; set; }
        [Required]
        public DateTime? ReceptionTime { get; set; } 
        public DateTime? TransferTime { get; set; }  
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public TimeSpan? WorkDuration { get; set; }  
        #endregion

        #region Бригада
        public string? BrigadeNumber { get; set; }
        public string? Doctor { get; set; }
        public string? Paramedic { get; set; }
        public string? Driver { get; set; }
        public string? Dispatcher1 { get; set; }
        public string? Dispatcher2 { get; set; }
        #endregion

        #region Оплата и страховка
        [Required] public string PaymentType { get; set; } = string.Empty;
        public string? InsuranceNumber { get; set; }
        public string? Customer { get; set; }
        public string? CustomerRepresentative { get; set; }
        public decimal? Cost { get; set; }
        public int? MKADDistance { get; set; }
        #endregion

        public bool IsWaiting { get; set; }
    }

}