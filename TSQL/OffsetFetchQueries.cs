using System.Linq;
using NUnit.Framework;

namespace TSQL
{
    [TestFixture]
    public class OffsetFetchQueries
    {
        [Test]
        public void OffsetFetchIsUsedForSkippingAndFetchingElements()
        {
            const string query = @"SELECT empid, orderid, orderdate, custid
FROM Sales.Orders
ORDER BY orderdate DESC
OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(25));
        }

        [Test]
        public void OffsetFetchMustBeUsedAlongWithAnOrderByClause()
        {
            const string query = @"SELECT empid, orderid, orderdate, custid
FROM Sales.Orders
OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY";

            Assert.That(() => QueryHelper.ExecuteQuery(query), Throws.Exception);
        }

        [Test]
        public void OffsetMustBeSpecifiedIfUsingFetch()
        {
            const string query = @"SELECT empid, orderid, orderdate, custid
FROM Sales.Orders
FETCH NEXT 25 ROWS ONLY";

            Assert.That(() => QueryHelper.ExecuteQuery(query), Throws.Exception);
        }

        [Test]
        public void OffsetCanBeUsedWithoutFetch()
        {
            const string query = @"SELECT empid, orderid, orderdate, custid
FROM Sales.Orders
ORDER BY orderdate DESC
OFFSET 50 ROWS";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(780));
        }

        [Test]
        public void InAnOffsetFetchRowAndRowsAreInterchangeable()
        {
            const string query = @"SELECT empid, orderid, orderdate, custid
FROM Sales.Orders
ORDER BY orderdate DESC
OFFSET 50 ROWS FETCH NEXT 1 ROW ONLY";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(1));
        }

        [Test]
        public void InAnOffsetFetchNextAndFirstAreInterchangeable()
        {
            const string query = @"SELECT empid, orderid, orderdate, custid
FROM Sales.Orders
ORDER BY orderdate DESC
OFFSET 50 ROWS FETCH FIRST 10 ROWS ONLY";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(10));
        }
    }
}