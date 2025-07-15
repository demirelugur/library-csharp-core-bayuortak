namespace BayuOrtak.Core.UnitTest.Extensions
{
    using BayuOrtak.Core.Extensions;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    [TestFixture]
    public class PropertyInfoExtensionsTests
    {
        [Table("TestClass")]
        private class TestClass
        {
            [Key]
            public int Id { get; set; }
            public string Name { get; set; }
            [Column("custom_col")]
            public string Custom { get; set; }
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int AutoId { get; set; }
            [NotMapped]
            public string NotMappedProp { get; set; }
            public int ReadOnly { get; }
            public virtual ICollection<TestClass> Children { get; set; }
        }
        [Test]
        public void IsMappedProperty_StandardProperty_ReturnsTrue()
        {
            var prop = typeof(TestClass).GetProperty("Name");
            Assert.That(prop.IsMappedProperty(), Is.True);
        }
        [Test]
        public void IsMappedProperty_NotMapped_ReturnsFalse()
        {
            var prop = typeof(TestClass).GetProperty("NotMappedProp");
            Assert.That(prop.IsMappedProperty(), Is.False);
        }
        [Test]
        public void IsMappedProperty_VirtualICollection_ReturnsFalse()
        {
            var prop = typeof(TestClass).GetProperty("Children");
            Assert.That(prop.IsMappedProperty(), Is.False);
        }
        [Test]
        public void IsMappedProperty_Null_ReturnsFalse()
        {
            PropertyInfo prop = null;
            Assert.That(prop.IsMappedProperty(), Is.False);
        }
        [Test]
        public void IsMappedProperty_ReadOnlyOrWriteOnly_ReturnsFalse()
        {
            var prop = typeof(TestClass).GetProperty("ReadOnly");
            Assert.That(prop.IsMappedProperty(), Is.False);
        }
        [Test]
        public void IsPK_KeyAttribute_ReturnsTrue()
        {
            var prop = typeof(TestClass).GetProperty("Id");
            Assert.That(prop.IsPK(), Is.True);
        }
        [Test]
        public void IsPK_NoKeyAttribute_ReturnsFalse()
        {
            var prop = typeof(TestClass).GetProperty("Name");
            Assert.That(prop.IsPK(), Is.False);
        }
        [Test]
        public void GetColumnName_ColumnAttribute_ReturnsCustomName()
        {
            var prop = typeof(TestClass).GetProperty("Custom");
            Assert.That(prop.GetColumnName(), Is.EqualTo("custom_col"));
        }
        [Test]
        public void GetColumnName_NoColumnAttribute_ReturnsPropertyName()
        {
            var prop = typeof(TestClass).GetProperty("Name");
            Assert.That(prop.GetColumnName(), Is.EqualTo("Name"));
        }
        [Test]
        public void GetDatabaseGeneratedOption_None_ReturnsNone()
        {
            var prop = typeof(TestClass).GetProperty("AutoId");
            Assert.That(prop.GetDatabaseGeneratedOption(), Is.EqualTo(DatabaseGeneratedOption.None));
        }
        [Test]
        public void GetDatabaseGeneratedOption_NoAttribute_ReturnsNull()
        {
            var prop = typeof(TestClass).GetProperty("Name");
            Assert.That(prop.GetDatabaseGeneratedOption(), Is.Null);
        }
    }
}