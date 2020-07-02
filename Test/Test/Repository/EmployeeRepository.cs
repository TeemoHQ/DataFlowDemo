using System;
using System.Collections.Generic;
using System.Text;
using Test.Entity;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Test.Respository
{
    public class EmployeeRepository
    {
        /// <summary>
        /// 执行次数
        /// </summary>
        private static int Count;
        public static void InsertEmployees(Employee[] employees)
        {
            Console.WriteLine($"线程Id：{Thread.CurrentThread.ManagedThreadId} 第{Interlocked.Increment(ref Count)}次  总数：{ employees.Count()},数据：{JsonConvert.SerializeObject(employees.Select(p=>p.EmployeeID))}开始执行批量插入");
            //todo: db op
            return;
        }
    }
}
