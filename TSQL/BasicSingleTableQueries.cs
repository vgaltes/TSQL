using System.Linq;
using NUnit.Framework;

namespace TSQL
{
    [TestFixture]
    public class BasicSingleTableQueriesHelper
    {
        [Test]
        public void ShouldSelectFromATable()
        {
            // The order is not guaranteed. In the case of SQLServer it returns the physical order
            const string query = @"SELECT orderid, custid, empid, orderdate, freight
FROM Sales.Orders";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(830));
        }

        [Test]
        public void BasicFilterUsingWhere()
        {
            // custid != 71 and custid == NULL are discarded
            const string query = @"SELECT orderid, custid, empid, orderdate, freight
FROM Sales.Orders
WHERE custid = 71";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(31));
        }

        [Test]
        public void ShouldOnlyGetCustomersInARegion()
        {
            const string query = @"SELECT custid, country, region, city
FROM Sales.Customers
WHERE region = N'WA'";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(3));
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
        public void ShouldGetCustomersWithNoRegion()
        {
            const string query = @"SELECT custid, country, region, city
FROM Sales.Customers
WHERE region IS NULL";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(60));
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

        [Test]
        public void BasicGrouping()
        {
            const string query = @"SELECT empid, YEAR(orderdate)
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(27));
        }

        [Test]
        public void SumOfElementsThatDontParticipateInTheGroupByIsAllowed()
        {
            const string query = @"SELECT empid, YEAR(orderdate), SUM(freight) as totalfreight
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(27));
        }

        [Test]
        public void CountOfElementsThatDontParticipateInTheGroupByIsAllowed()
        {
            const string query = @"SELECT empid, YEAR(orderdate), COUNT(*) as numrows
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(27));
        }

        [Test]
        public void AverageOfElementsThatDontParticipateInTheGroupByIsAllowed()
        {
            const string query = @"SELECT empid, YEAR(orderdate), AVG(freight) as avgfreight
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(27));
        }

        [Test]
        public void MinOfElementsThatDontParticipateInTheGroupByIsAllowed()
        {
            const string query = @"SELECT empid, YEAR(orderdate), MIN(freight) as avgfreight
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(27));
        }

        [Test]
        public void MaxOfElementsThatDontParticipateInTheGroupByIsAllowed()
        {
            const string query = @"SELECT empid, YEAR(orderdate), MAX(freight) as avgfreight
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(27));
        }

        [Test]
        public void SelectOfElementsThatDontParticipateInTheGroupByIsNotAllowed()
        {
            const string query = @"SELECT empid, YEAR(orderdate), freight
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)";

            Assert.That(() => QueryHelper.ExecuteQuery(query), Throws.Exception);
        }

        [Test]
        public void HavingIsAGroupFilter()
        {
            const string query = @"SELECT empid, YEAR(orderdate)
FROM Sales.Orders
GROUP BY empid, YEAR(orderdate)
HAVING COUNT(*) > 10";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(26));
        }

        [Test]
        public void DisticntCanBeUsedInASelect()
        {
            const string query = @"SELECT DISTINCT empid, YEAR(orderdate)
FROM Sales.Orders
WHERE custid = 71";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(16));
        }

        [Test]
        public void YouCantUseAliasesCreatedInSelectInOtherExpresionsWithinTheSameSelectClause()
        {
            const string query = @"SELECT orderid, YEAR(orderdate) as orderyear, orderyear + 1 as nextyear
FROM Sales.Orders
WHERE custid = 71";

            Assert.That(() => QueryHelper.ExecuteQuery(query), Throws.Exception);
        }

        [Test]
        public void OrderBySortTheRowsInTheOutputForPresentationPurposes()
        {
            // ORDER BY is the last clause to be processed. Therefor is the only clause
            // where you can use aliases defined in the select clause
            const string query = @"SELECT empid, YEAR(orderdate) as orderyear
FROM Sales.Orders
WHERE custid = 71
ORDER BY empid, orderyear";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(31));

            // SQL calls an ordered result a cursor, not a table
        }

        [Test]
        public void OrderByDescSortTheRowsInDescendingWayInTheOutputForPresentationPurposes()
        {
            // ASC is the default
            const string query = @"SELECT empid, YEAR(orderdate) as orderyear
FROM Sales.Orders
WHERE custid = 71
ORDER BY empid, orderyear DESC";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(31));
        }

        [Test]
        public void TopIsAPropietaryTSQLFeatureThatLimitTheResultsReturned()
        {
            const string query = @"SELECT TOP 5 empid, YEAR(orderdate) as orderyear
FROM Sales.Orders
WHERE custid = 71";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(5));
        }

        [Test]
        public void YouCanSpecifyAPercentageWhenUsingTop()
        {
            const string query = @"SELECT TOP 5 PERCENT empid, YEAR(orderdate) as orderyear
FROM Sales.Orders
WHERE custid = 71";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void YouCanWithTiesWhenUsingTopToReturnTiesInTheOrderByClause()
        {
            const string query = @"SELECT TOP 5 WITH TIES empid, orderid, orderdate, custid
FROM Sales.Orders
ORDER BY orderdate DESC";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(8));
        }
    }
}