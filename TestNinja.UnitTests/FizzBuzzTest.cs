using NUnit.Framework;
using TestNinja.Fundamentals;

namespace TestNinja.UnitTests
{
    [TestFixture]
    public class FizzBuzzTest
    {
        [Test]
        public void GetOutput_InputIsDevisibleBy3and5_ReturnFizzBuzz()
        {
           var result =  FizzBuzz.GetOutput(15);
           Assert.That(result, Is.EqualTo("FizzBuzz"));
        }

        [Test]
        public void GetOutput_InputIsDevisibleBy3Only_ReturnFizz()
        {
            var result =  FizzBuzz.GetOutput(3);
            Assert.That(result, Is.EqualTo("Fizz"));
        }

        [Test]
        public void GetOutput_InputIsDevisibleBy5only_ReturnBuzz()
        {
            var result =  FizzBuzz.GetOutput(5);
            Assert.That(result, Is.EqualTo("Buzz"));
        }

        [Test]
        public void GetOutput_InputIsNotDevisibleBy3or5_ReturnNumber()
        {
            var result =  FizzBuzz.GetOutput(1);
            Assert.That(result, Is.EqualTo("1"));
        }
    }
}