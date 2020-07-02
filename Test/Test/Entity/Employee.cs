using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Entity
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        // A random number generator that helps tp generate
        // Employee property values.
        static Random rand = new Random(42);

        // Possible random first names.
        static readonly string[] firstNames = { "Tom", "Mike", "Ruth", "Bob", "John" };
        // Possible random last names.
        static readonly string[] lastNames = { "Jones", "Smith", "Johnson", "Walker" };

        // Creates an Employee object that contains random
        // property values.
        public static Employee Random(int id)
        {
            return new Employee
            {
                EmployeeID = id,
                LastName = lastNames[rand.Next() % lastNames.Length],
                FirstName = firstNames[rand.Next() % firstNames.Length]
            };
        }
    }
}
