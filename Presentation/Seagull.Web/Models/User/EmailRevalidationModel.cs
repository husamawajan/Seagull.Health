﻿using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.User
{
    public partial class EmailRevalidationModel : BaseSeagullModel
    {
        public string Result { get; set; }
    }
}