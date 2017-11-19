using System.Linq;
using NUnit.Framework;

namespace TSQL
{
    [TestFixture]
    public class Predicates
    {
        [Test]
        public void InChecksWhetherAValueIsAtLeastEqualToOneElementOfASet()
        {
            const string query = @"SELECT empid, orderid
FROM Sales.Orders
WHERE orderid IN (10248, 10249, 10250)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(3));
        }

        [Test]
        public void BetweenChecksWhetherAValueIsOnAnInclusiveRange()
        {
            const string query = @"SELECT empid, orderid
FROM Sales.Orders
WHERE orderid BETWEEN 10248 AND 10250";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(3));
        }

        [Test]
        public void LikeChecksWhetherAStringMeetsTheSpecifiedPattern()
        {
            // N stands for National and is used to denote that a character string 
            // is of a Unicode data type (NCHAR or NVARCHAR), as opposed to a regular
            // character data type (CHAR, VARCHAR)
            const string query = @"SELECT empid, firstname, lastname
FROM HR.Employees
WHERE lastname LIKE N'D%'";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void YouCanUseComparisionOperators()
        {
            // =, >, <, >=, <=, <> -> standard
            // !=, !>, !< -> non standard
            const string query = @"SELECT empid, orderid
FROM Sales.Orders
WHERE orderdate >= '20160101'";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(270));
        }

        [Test]
        public void YouCanCombineLogicalExpressionsWithAnd()
        {
            const string query = @"SELECT empid, orderid
FROM Sales.Orders
WHERE orderdate >= '20160101' AND empid IN (1, 3, 5)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(93));
        }

        [Test]
        public void YouCanCombineLogicalExpressionsWithOr()
        {
            const string query = @"SELECT empid, orderid
FROM Sales.Orders
WHERE orderdate >= '20160101' OR empid IN (1, 3, 5)";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(469));
        }
    }
}