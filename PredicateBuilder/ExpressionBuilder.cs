using ExpressionGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace System.Linq.Expressions
{
    public class ExpressionBuilder<T>
    {
        private readonly StringBuilder _jsonDocument = new StringBuilder();
        private readonly Conditions conditions = new Conditions();
        public ExpressionBuilder<T> Or()
        {
            return this;
        }
        public ExpressionBuilder<T> And()
        {
            return this;
        }
        public ExpressionBuilder<T> Not()
        {
            return this;
        }
        public ExpressionBuilder<T> Rule(Expression<Func<T, object>> name, Operator @operator, params object[] values)
        {
            var n = GetMemberName(name);
            Debug.WriteLine(n);

            conditions.DynamicRules.Add(new DynamicRule()
            {
                Field = n,
                Label = n,
                Operator = @operator,
                Type = Type.Boolean,
                Values = values.Select(x => x.ToString()).ToArray()
            }); 
            return this;
        }


        public ExpressionBuilder<T> RuleGroup(ExpressionBuilder<T> dynamicRuleBuilder)
        {
            return this;
        }
        public Func<T, bool> Build()
        {
            var jsonExpressionParser = new JsonExpressionParser();
            var predicate = jsonExpressionParser.ParsePredicateOf<T>(GetJsonDocument());
            return predicate;
        }

        public Func<T, bool> BuildJson(JsonDocument jsonDocument)
        {
            var jsonExpressionParser = new JsonExpressionParser();
            var predicate = jsonExpressionParser.ParsePredicateOf<T>(jsonDocument);
            return predicate;
        }

        private JsonDocument GetJsonDocument()
        {
            return JsonDocument.Parse("");
        }

        private string GetMemberName<TSource, TProperty>(Expression<Func<TSource, TProperty>> property)
        {
            if (Equals(property, null))
            {
                throw new NullReferenceException("Property is required");
            }

            MemberExpression expr;

            if (property.Body is MemberExpression)
            {
                expr = (MemberExpression)property.Body;
            }
            else if (property.Body is UnaryExpression)
            {
                expr = (MemberExpression)((UnaryExpression)property.Body).Operand;
            }
            else
            {
                const string format = "Expression '{0}' not supported.";
                string message = string.Format(format, property);

                throw new ArgumentException(message, "Property");
            }

            return expr.Member.Name;
        }
    }

    public class DynamicRule
    {
        public string Label { get; set; }
        public string Field { get; set; }
        public Operator Operator { get; set; }
        public Type Type { get; set; }
        public string[] Values { get; set; }

    }

    public class Conditions
    {
        public AndOrOperator AndOrOperator { get; set; }
        public List<DynamicRule> DynamicRules { get; set; }
        public Conditions()
        {
            DynamicRules = new List<DynamicRule>();
        }
    }

    public enum Type
    {
        Number,
        String,
        Boolean
    }
    public enum AndOrOperator
    {
        And,
        Or
    }

    public enum Operator
    {
        In,
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanEqual,
        LessThan,
        LessThanEqual,
        IsNull,
        NotIsNull,
        Between,
        NotBetween,
        Contains,
        StartWith,
        EndsWith,
    }
}
