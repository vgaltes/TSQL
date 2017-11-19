using System.Linq;
using NUnit.Framework;

namespace TSQL
{
    [TestFixture]
    public class CaseExpressions
    {
        [Test]
        public void BasicCase()
        {
            /*
             * Case is a scalar expression that returns a value based on a conditional logic.
             * It is allowed wherever a scalar expression is allowed (SELECT, WHERE, HAVING, ORDER BY and CHECK) 
             * If no value in the list is equal to the tested value, the expression returns
             * the value that appears in the else clause. If there is no else clause, null is returned
             */
            const string query = @"SELECT productid, productname, categoryid,
    CASE categoryid 
        WHEN 1 THEN 'Beverages'
        WHEN 2 THEN 'Condiments'
        WHEN 3 THEN 'Confections'
        ELSE 'Other'
    END as categoryname
FROM Production.Products";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(77));
        }

        [Test]
        public void YouCanSpecifyPredicatesInTheWhenClauses()
        {
            // Searched case
            const string query = @"SELECT orderid, custid, val,
    CASE 
        WHEN val < 1000.00 THEN 'Cheap'
        WHEN val BETWEEN 1000.00 AND 3000.00 THEN 'Medium'
        WHEN val > 3000.0 THEN 'Expensive'
        ELSE 'Unknown'
    END as valuecategory
FROM Sales.OrderValues";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(830));
        }

        [Test]
        public void IsNullAsAbbreviationOfACase()
        {
            // Non standard
            const string query = @"SELECT orderid, custid, val, ISNULL(val, '0.00')
FROM Sales.OrderValues";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(830));
        }

        [Test]
        public void ValuesInsideIsNullMustBeOfTheSameType()
        {
            // Non standard
            const string query = @"SELECT orderid, custid, val, ISNULL(val, 'Unknowns')
FROM Sales.OrderValues";

            Assert.That(() => QueryHelper.ExecuteQuery(query), Throws.Exception);
        }

        [Test]
        public void IifAsAbbreviationOfACase()
        {
            // Non standard
            // IIF(conditional, true value, false value)
            const string query = @"SELECT orderid, custid, val, IIF(val > 3000.0, 'Expensive', 'false')
FROM Sales.OrderValues";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(830));
        }

        [Test]
        public void ValuesInsideIifMustBeOfTheSameType()
        {
            // Non standard
            const string query = @"SELECT orderid, custid, val, IIF(val > 3000.0, 'Expensive', 0.00)
FROM Sales.OrderValues";

            Assert.That(() => QueryHelper.ExecuteQuery(query), Throws.Exception);
        }

        [Test]
        public void ChooseAsAbbreviationOfACase()
        {
            // Non standard
            const string query = @"SELECT CHOOSE ( 3, 'Manager', 'Director', 'Developer', 'Tester' ) AS Result;";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.First().Result, Is.EqualTo("Developer"));
        }

        [Test]
        public void CoalesceAsAbbreviationOfACase()
        {
            // Standard
            const string query = @"SELECT custid, country, COALESCE(region, 'Unknown') as region, city
FROM Sales.Customers
WHERE region IS NULL";

            var results = QueryHelper.ExecuteQuery(query);

            Assert.That(results.Count(), Is.EqualTo(60));
            Assert.That(results.All(r => r.region == "Unknown"), Is.True);
        }
    }
}