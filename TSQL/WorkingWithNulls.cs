using System.Linq;
using NUnit.Framework;

namespace TSQL
{
    [TestFixture]
    public class WorkingWithNulls
    {
        [Test]
        public void ShouldGetCustomersWithNoRegion()
        {
            const string query = @"SELECT custid, country, region, city
FROM Sales.Customers
WHERE region IS NULL";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(60));
        }

        [Test]
        public void ShouldOnlyGetCustomersOutsideARegion_NoNullsReturned()
        {
            const string query = @"SELECT custid, country, region, city
FROM Sales.Customers
WHERE region <> N'WA'";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(28));
        }

        [Test]
        public void ShouldOnlyGetCustomersOutsideARegion_NullsReturned()
        {
            const string query = @"SELECT custid, country, region, city
FROM Sales.Customers
WHERE region <> N'WA' OR region IS NULL";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(88));
        }
    }
}