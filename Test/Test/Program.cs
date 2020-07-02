using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.Entity;
using Test.Respository;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var batchDataPipeline = new BatchBlockPipeline<Employee>(10, EmployeeRepository.InsertEmployees, timeTrigger: 3);

            for (int i = 0; i < 100; i++)
            {
                batchDataPipeline.PostValue(Employee.Random(i));
                if (i % 3 == 0 || i % 7 == 0 || i % 9 == 0)
                {
                    Thread.Sleep(5000);
                }
            }
            Console.ReadKey();
        }
    }

}
