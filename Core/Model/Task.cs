using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Task
    {
        public int ID { get; set; }
        public int OnboardingId { get; set; }
        public bool PasswordWorking { get; set; }
        public string TicketStatus { get; set; } = string.Empty;
    }
    public class ValidatePasswordDTO
    {
        public int OnboardingId { get; set; }
        public bool PasswordWorking { get; set; }
    }
}
