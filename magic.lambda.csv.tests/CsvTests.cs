/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using Xunit;
using magic.node;

namespace magic.lambda.csv.tests
{
    public class CsvTests
    {
        [Fact]
        public void FromCsvSimpleObject()
        {
            var signaler = Common.GetSignaler();
            var node = new Node("", @"name, age
Thomas,55");
            signaler.Signal("csv2lambda", node);
            Assert.Equal("name", node.Children.First().Children.First().Name);
            Assert.Equal("Thomas", node.Children.First().Children.First().Value);
        }

        [Fact]
        public void FromLambdaSimpleObject()
        {
            var signaler = Common.GetSignaler();
            var node = new Node("");
            node.Add(new Node(".", null, new Node[] { new Node("Name", "Thomas"), new Node("Age", 55) }));
            signaler.Signal("lambda2csv", node);
            Assert.Equal("Name,Age\r\n\"Thomas\",55\r\n", node.Value);
        }

        [Fact]
        public void FromLambdaAndBackAgainWithTyping()
        {
            var result = Common.Evaluate(@"
.data
   .
      name:Thomas
      age:int:55
   .
      name:John
      age:67
lambda2csv:x:-/*
add:x:+
   get-nodes:x:@lambda2csv/*
csv2lambda:x:@lambda2csv
");
            Assert.Equal("name", result.Children.Last().Children.First().Children.First().Name);
            Assert.Equal("Thomas", result.Children.Last().Children.First().Children.First().Value);
            Assert.Equal("age", result.Children.Last().Children.First().Children.Skip(1).First().Name);
            Assert.Equal(55, result.Children.Last().Children.First().Children.Skip(1).First().Value);
            Assert.Equal("name", result.Children.Last().Children.Skip(1).First().Children.First().Name);
            Assert.Equal("John", result.Children.Last().Children.Skip(1).First().Children.First().Value);
            Assert.Equal("age", result.Children.Last().Children.Skip(1).First().Children.Skip(1).First().Name);
            Assert.Equal(67, result.Children.Last().Children.Skip(1).First().Children.Skip(1).First().Value);
        }

        [Fact]
        public void FromLambdaAndBackAgainWithNullValues()
        {
            var result = Common.Evaluate(@"
.data
   .
      name:[NULL]
      age:int:55
   .
      name:John
      age:[NULL]
lambda2csv:x:-/*
add:x:+
   get-nodes:x:@lambda2csv/*
csv2lambda:x:@lambda2csv
");
            Assert.Equal("name", result.Children.Last().Children.First().Children.First().Name);
            Assert.Null(result.Children.Last().Children.First().Children.First().Value);
            Assert.Equal("age", result.Children.Last().Children.First().Children.Skip(1).First().Name);
            Assert.Equal(55, result.Children.Last().Children.First().Children.Skip(1).First().Value);
            Assert.Equal("name", result.Children.Last().Children.Skip(1).First().Children.First().Name);
            Assert.Equal("John", result.Children.Last().Children.Skip(1).First().Children.First().Value);
            Assert.Equal("age", result.Children.Last().Children.Skip(1).First().Children.Skip(1).First().Name);
            Assert.Null(result.Children.Last().Children.Skip(1).First().Children.Skip(1).First().Value);
        }
    }
}
