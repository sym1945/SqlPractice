﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MyService();

            var repo = new TestRepository();
            repo.SelectAll();
            
        }
    }
}
