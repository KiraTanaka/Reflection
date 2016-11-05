using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComputerAlgebra
{
    class Program
    {
        public static Expression Recurse(Expression expression,ParameterExpression parameter)
        {
            BinaryExpression binExpression = null;
            Expression left = null, right = null;
            if (expression.NodeType == ExpressionType.Call)
                if (((MethodCallExpression)expression).Arguments[0].NodeType != ExpressionType.Constant)
                    return Expression.Call(typeof(Math).GetMethod("Cos"), parameter);
                else
                    return Expression.Constant(0.0);
            if (expression.NodeType == ExpressionType.Constant)
                return Expression.Constant(0.0);
            if (expression.NodeType == ExpressionType.Parameter)
                return Expression.Constant(1.0);
            binExpression = (BinaryExpression)expression;
            if (binExpression.NodeType == ExpressionType.Multiply)
            {
                if (binExpression.Left.NodeType == ExpressionType.Parameter && binExpression.Right.NodeType == ExpressionType.Parameter)
                    return Expression.Multiply(Expression.Constant(2.0), parameter);
                if (binExpression.Left.NodeType == ExpressionType.Parameter && binExpression.Right.NodeType == ExpressionType.Constant ||
                    binExpression.Right.NodeType == ExpressionType.Parameter && binExpression.Left.NodeType == ExpressionType.Constant)
                    return (binExpression.Left.NodeType == ExpressionType.Constant) ? binExpression.Left : binExpression.Right;
            }
            left = Recurse(binExpression.Left,parameter);
            right = Recurse(binExpression.Right,parameter);
            return expression.NodeType == ExpressionType.Add ? Expression.Add(left, right) 
                : (expression.NodeType == ExpressionType.Multiply ? Expression.Multiply(left, right)
                : expression);
        }
        public static Func<double, double> Simple(Expression<Func<double, double>> exp)
        {
            Expression<Func<double, double>> df =null;
            df = Expression.Lambda<Func<double, double>>(Recurse(exp.Body, exp.Parameters[0]),exp.Parameters);
            Console.WriteLine(df);
            return df.Compile();
        }
        static void Main(string[] args)
        {
            var compiled = Simple(x => Math.Sin(x)+10);
            Console.WriteLine(compiled.Invoke(3.14159));
            Console.ReadLine();
        }
    }
}
