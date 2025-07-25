﻿using AppService.Core.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Core.Responses
{
    public class ApiResponse<T>
    {

        public ApiResponse(T data)
        {
            Data = data;

        }

        public T Data { get; set; }



        public Metadata Meta { get; set; }

        public decimal Total1 { get; set; }
        
        public decimal Total2 { get; set; }
        
        public decimal Total3 { get; set; }
        public decimal Total4 { get; set; }

    }
}
