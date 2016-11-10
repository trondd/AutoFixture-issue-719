using System;
using System.Linq.Expressions;
using Ploeh.AutoFixture;
using Xunit;

namespace AutoFixture.Bug
{
    public interface IRule
    {
        bool Evaluate(string arg);
    }

    public class Rule : IRule
    {
        public Expression<Func<string, bool>> Condition;

        public Rule(Expression<Func<string, bool>> condition)
        {
            Condition = condition;
        }

        public bool Evaluate(string arg)
        {
            return Condition.Compile().Invoke(arg);
        }
    }

    public class MyRule : Rule
    {
        public MyRule() : base(s => InternalCondition(s))
        {
        }

        private static bool InternalCondition(string s)
        {
            throw new Exception();
        }
    }

    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            var fixture = new Fixture();

            var sut = fixture.Create<MyRule>();

            // Inspection of the sut.Condition field reveals that the has changed to {Param_0 => true}
            // Question is why AutoFixture is altering this value?

            Assert.Throws<Exception>(() => sut.Evaluate(string.Empty));
        }
    }
}