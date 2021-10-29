﻿using Boa.Constrictor.Screenplay;
using Boa.Constrictor.WebDriver;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Boa.Constrictor.UnitTests.WebDriver
{
    public class ClassesTest : BaseWebLocatorQuestionTest
    {
        #region Tests

        [Test]
        public void TestOneClass()
        {
            var element = new Mock<IWebElement>();
            WebDriver.Setup(x => x.FindElements(It.IsAny<By>())).Returns(new List<IWebElement> { element.Object }.AsReadOnly());
            WebDriver.Setup(x => x.FindElement(It.IsAny<By>()).GetAttribute(It.IsAny<string>())).Returns("test");

            Actor.AsksFor(Classes.Of(Locator)).Should().BeEquivalentTo(new string[] { "test" });
        }

        [Test]
        public void TestZeroClass()
        {
            WebDriver.Setup(x => x.FindElements(It.IsAny<By>())).Returns(new List<IWebElement>().AsReadOnly());
            WebDriver.Setup(x => x.FindElement(It.IsAny<By>()).GetAttribute(It.IsAny<string>())).Returns(string.Empty);

            Actor.Invoking(x => x.AsksFor(Classes.Of(Locator))).Should().Throw<WaitingException<bool>>();
        }

        [Test]
        public void TestMultipleClass()
        {
            var elementOne = new Mock<IWebElement>();
            var elementTwo = new Mock<IWebElement>();
            var elementThree = new Mock<IWebElement>();
            WebDriver.Setup(x => x.FindElements(It.IsAny<By>()))
                .Returns(new List<IWebElement> { elementOne.Object, elementTwo.Object, elementThree.Object }.AsReadOnly());
            WebDriver.Setup(x => x.FindElement(It.IsAny<By>()).GetAttribute(It.IsAny<string>())).Returns("i my me");

            Actor.AsksFor(Classes.Of(Locator)).Should().BeEquivalentTo(new string[] { "i", "my", "me" });
        }

        #endregion
    }
}
