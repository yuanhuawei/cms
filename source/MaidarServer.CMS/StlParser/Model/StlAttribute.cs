﻿using System;

namespace MaiDarServer.CMS.StlParser.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StlAttribute : Attribute
    {
        public string Usage { get; set; }
        public string Description { get; set; }
    }
}
