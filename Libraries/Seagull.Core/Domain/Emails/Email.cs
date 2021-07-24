using Seagull.Core.Domain.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Seagull.Core.Domain.Emails
{
    /// <summary>
    /// Represents a Email
    /// </summary>
    public partial class Email : BaseEntity
    {
        public int Id { get; set; } //Id
        public string From { get; set; } //From
        public string To { get; set; } //To
        public string Subject { get; set; } //Subject
        public string Msg { get; set; } //Msg
        public DateTime? EmailDate { get; set; } //EmailDate
        public int? OperatorId { get; set; } //OperatorId
        public int? Type { get; set; } //Type
        public bool Flag { get; set; } //Flag

    }
}
