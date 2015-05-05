using Xunit;

namespace changeTracker.Tests
{
    public class ChangeTrackerUnitTests
    {
        [Fact]
        public void detects_simple_value_change()
        {
            var objToBeChanged = new Person { FName = "Anders", LName = "And" };

            var sut = new ChangeTracker<Person>(objToBeChanged);
            var hasBeenChanged = sut
              .Set(o => o.FName, "Donald")
              .Execute();

            Assert.True(hasBeenChanged);
            Assert.Equal("Donald", objToBeChanged.FName);
        }

        [Fact]
        public void do_not_detect_simple_value_change_to_same_value()
        {
            var objToBeChanged = new Person { FName = "Anders", LName = "And" };

            var sut = new ChangeTracker<Person>(objToBeChanged);
            var hasBeenChanged = sut
              .Set(o => o.FName, "Anders")
              .Execute();

            Assert.False(hasBeenChanged);
        }

        [Fact]
        public void chaining_of_set_method_works()
        {
            var objToBeChanged = new Person { FName = "Anders", LName = "And" };

            var sut = new ChangeTracker<Person>(objToBeChanged);
            var hasBeenChanged = sut
              .Set(o => o.FName, "Donald")
              .Set(o => o.LName, "Duck")
              .Execute();

            Assert.True(hasBeenChanged);
        }

        [Fact]
        public void detect_change_of_referencetypes()
        {
            var objToBeChanged = new Person { FName = "Anders", LName = "And" };
            var child = new Person { FName = "Rip", LName = "And" };

            var sut = new ChangeTracker<Person>(objToBeChanged);
            var hasBeenChanged = sut
              .Set(o => o.Child, child)
              .Execute();

            Assert.True(hasBeenChanged);
            Assert.Equal("Rip", objToBeChanged.Child.FName);
            Assert.Equal("And", objToBeChanged.Child.LName);
        }

        [Fact]
        public void do_not_detect_referencetype_change_when_equal()
        {
            var objToBeChanged = new Person { FName = "Anders", LName = "And" };
            var child1 = new Person { FName = "Rip", LName = "And" };
            var child2 = new Person { FName = "Rip", LName = "And" };
            objToBeChanged.Child = child1;

            var sut = new ChangeTracker<Person>(objToBeChanged);
            var hasBeenChanged = sut
              .Set(o => o.Child, child2)
              .Execute();

            Assert.False(hasBeenChanged);
        }

        [Fact]
        public void detect_change_in_nested_properties()
        {
            var objToBeChanged = new Person { FName = "Anders", LName = "And" , Child = new Person { FName = "Rip", LName = "And" }};
            
            var sut = new ChangeTracker<Person>(objToBeChanged);
            var hasBeenChanged = sut
              .Set(o => o.Child.LName, "duck")
              .Execute();

            Assert.True(hasBeenChanged);
            Assert.Equal("duck", objToBeChanged.Child.LName);
        }

        class Person
        {
            public override int GetHashCode()
            {
                return -1;
            }

            public string FName { get; set; }
            public string LName { get; set; }
            public Person Child { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Person) obj);
            }

            bool Equals(Person other)
            {
                return string.Equals(FName, other.FName) && string.Equals(LName, other.LName) && Equals(Child, other.Child);
            }
        }
    }

}
