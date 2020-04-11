﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Helpers
{
    public class AppSettings
    {
        public string Site          { get; set; }
        public string Audience      { get; set; }
        public string ExpirenceTime { get; set; }
        public string Secret        { get; set; }
    }
}
