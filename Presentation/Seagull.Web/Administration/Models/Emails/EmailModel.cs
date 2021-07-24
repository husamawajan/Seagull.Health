using System;
using System.Web.Mvc;
//using FluentValidation.Attributes;                                                                             
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Core.Domain.Emails;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seagull.Admin.Models.Emails
{
    //[Validator(typeof(EmailValidator))]                                                                 
    public class EmailModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.Id")]
        [AllowHtml]
        public int Id { get; set; } //Id
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.From")]
        [AllowHtml]
        public string From { get; set; } //From
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.To")]
        [AllowHtml]
        public string To { get; set; } //To
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; } //Subject
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.Msg")]
        [AllowHtml]
        public string Msg { get; set; } //Msg
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.EmailDate")]
        [AllowHtml]
        public DateTime? EmailDate { get; set; } //EmailDate
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.OperatorId")]
        [AllowHtml]
        public int? OperatorId { get; set; } //OperatorId
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.Type")]
        [AllowHtml]
        public int? Type { get; set; } //Type
        [SeagullResourceDisplayName("Admin.Seagull.TRC.Emails.Fields.Flag")]
        [AllowHtml]
        public int? Flag { get; set; } //Flag

    }
}