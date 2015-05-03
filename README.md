# ChangeTracker
Simple library to simplify change detection.

Uses lambdas and expresssion trees to make it easy to write and detect if a bunch of changes to the same object, 
actually changed anything.

##Example
```c#
using System;
using System.Linq.Expressions;

namespace changeTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var l = new Person { FName = "Anders", LName = "And"};
            var sub = new Person { FName = "Rip", LName = "and" };
            l.child = sub;

            //change everything you wan't to change via the changetracker
            var c = new ChangeTracker<Person>(l);
            c
             .Set(i => i.Child.FName, "Rap")
             .Set(i => i.LName, "Duck");
             // etc. etc. (chaining not required)

            Console.WriteLine(c.HasBeenChanged); //prints true

            c.ResetHasBeenChanged();
            c.Set(i => i.LName, "Duck"); //nochange!!
            Console.WriteLine(c.HasBeenChanged); //prints false

            Console.WriteLine("press key");
            Console.ReadKey();
        }
    }

    internal class Person
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public Person Child { get; set; }
    }
}

```
