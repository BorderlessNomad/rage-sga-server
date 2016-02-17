using Xunit;
using Xunit.Abstractions;

namespace SocialGamificationAssetTests
{
    public class TestClass
    {
        private readonly ITestOutputHelper output;

        public TestClass(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void MyTest()
        {
            var temp = "my class!";
            output.WriteLine("This is output from {0}", temp);
        }

        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        public void MyFirstTheory(int value)
        {
            Assert.True(IsOdd(value));
        }

        bool IsOdd(int value)
        {
            return value % 2 == 1;
        }
    }
}