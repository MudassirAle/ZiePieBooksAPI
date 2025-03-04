using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class StatementConfiguration
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public int OnboardingId { get; set; }
        public int PlatformAccountId { get; set; }
        public bool IsFirstRowHeader { get; set; }
        public int NoOfAmountColumns { get; set; }
        public string DateFormat { get; set; } = string.Empty;
        public List<ColumnConfiguration> Columns { get; set; } = new List<ColumnConfiguration>();
    }
    public class StatementConfigurationDTO
    {
        public int BusinessId { get; set; }
        public int OnboardingId { get; set; }
        public int PlatformAccountId { get; set; }
        public bool IsFirstRowHeader { get; set; }
        public int NoOfAmountColumns { get; set; }
        public string DateFormat { get; set; } = string.Empty;
    }
    public class ColumnConfigurationDTO
    {
        public int StatementConfigurationId { get; set; }
        public List<ColumnConfiguration> Columns { get; set; } = new List<ColumnConfiguration>();
    }
    public class ColumnConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}