﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCSercerRabbitMq.Model
{
    public class ResponseModel
    {
        public int ID { get; set; }
        public string MachineName { get; set; }
        public bool IsSleepRequired { get; set; }
        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }
        public string filePath {  get; set; }   
    }
}
