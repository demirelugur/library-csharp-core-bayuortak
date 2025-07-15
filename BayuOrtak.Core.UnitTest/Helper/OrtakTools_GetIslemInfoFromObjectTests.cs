namespace BayuOrtak.Core.UnitTest.Helper
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [TestFixture]
    public class OrtakTools_GetIslemInfoFromObjectTests
    {
        [Test]
        public void GetIslemInfoFromObject_AnonymousObject_ReturnsFormattedString()
        {
            var dt = new DateTime(2024, 6, 1, 12, 30, 45);
            var obj = new { isldate = dt, isluser = "user1" };
            var result = _get.GetIslemInfoFromObject(obj, "yyyy-MM-dd HH:mm:ss");
            Assert.That(result, Is.EqualTo("2024-06-01 12:30:45, user1"));
        }
        [Test]
        public void GetIslemInfoFromObject_AnonymousObject_EmptyUser_ReturnsDateOnly()
        {
            var dt = new DateTime(2024, 6, 1, 12, 30, 45);
            var obj = new { isldate = dt, isluser = "" };
            var result = _get.GetIslemInfoFromObject(obj, "yyyy-MM-dd HH:mm:ss");
            Assert.That(result, Is.EqualTo("2024-06-01 12:30:45"));
        }
        [Test]
        public void GetIslemInfoFromObject_AnonymousObject_EmptyDate_ReturnsUserOnly()
        {
            var obj = new { isldate = DateTime.MinValue, isluser = "user1" };
            var result = _get.GetIslemInfoFromObject(obj, "yyyy-MM-dd HH:mm:ss");
            Assert.That(result, Is.EqualTo("user1"));
        }
        [Test]
        public void GetIslemInfoFromObject_Null_ReturnsEmptyString()
        {
            var result = _get.GetIslemInfoFromObject(null);
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void GetIslemInfoFromObject_JToken_ReturnsFormattedString()
        {
            var dt = new DateTime(2024, 6, 1, 12, 30, 45);
            var jtoken = JObject.FromObject(new { isldate = dt, isluser = "user2" });
            var result = _get.GetIslemInfoFromObject(jtoken, "yyyy-MM-dd HH:mm:ss");
            Assert.That(result, Is.EqualTo("2024-06-01 12:30:45, user2"));
        }
        [Test]
        public void GetIslemInfoFromObject_JToken_MissingFields_ReturnsEmptyString()
        {
            var jtoken = JObject.FromObject(new { });
            var result = _get.GetIslemInfoFromObject(jtoken);
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void GetIslemInfoFromObject_IFormCollection_ReturnsFormattedString()
        {
            var dict = new Dictionary<string, StringValues>
            {
                { "isldate", new StringValues("2024-06-01 12:30:45") },
                { "isluser", new StringValues("user3") }
            };
            var form = new FormCollection(dict);
            var result = _get.GetIslemInfoFromObject(form, "yyyy-MM-dd HH:mm:ss");
            Assert.That(result, Does.Contain("user3"));
            Assert.That(result, Does.Contain("2024"));
        }
    }
}