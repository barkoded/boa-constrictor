﻿using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using RestSharp;

namespace Boa.Constrictor.RestSharp.UnitTests
{
#pragma warning disable 0618

    [TestFixture]
    public class ParameterDataTest
    {
        [Test]
        public void GetParameterDataList()
        {
            var parameters = new List<Parameter>()
            {
                new HeaderParameter("p1", "hello"),
                new GetOrPostParameter("p2", "goodbye"),
            };

            var list = ParameterData.GetParameterDataList(parameters);
            list.Count.Should().Be(2);
            list[0].Name.Should().Be("p1");
            list[0].Value.Should().Be("hello");
            list[0].Type.Should().Be(ParameterType.HttpHeader.ToString());
            list[1].Name.Should().Be("p2");
            list[1].Value.Should().Be("goodbye");
            list[1].Type.Should().Be(ParameterType.GetOrPost.ToString());
        }

        [Test]
        public void GetParameterDataListEmpty()
        {
            ParameterData.GetParameterDataList(new List<Parameter>()).Should().BeEmpty();
        }
    }

#pragma warning restore 0618
}