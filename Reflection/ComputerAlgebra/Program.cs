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
                return (((MethodCallExpression)expression).Arguments[0].NodeType != ExpressionType.Constant)
                    ? Expression.Call(typeof(Math).GetMethod("Cos"), parameter)
                    : (Expression)Expression.Constant(0.0);
            if (expression.NodeType == ExpressionType.Constant)
                 return Expression.Constant(0.0);
            if (expression.NodeType == ExpressionType.Parameter)
                return Expression.Constant(1.0);
            binExpression = (BinaryExpression)expression;
            if (binExpression.NodeType == ExpressionType.Add)
            {
                left = Recurse(binExpression.Left, parameter);
                right = Recurse(binExpression.Right, parameter);
                if (left.NodeType == ExpressionType.Constant)
                    if ((double)((ConstantExpression)left).Value == 0.0)
                        return right;
                if(right.NodeType == ExpressionType.Constant)
                    if ((double)((ConstantExpression)right).Value == 0.0)
                        return left;
                return Expression.Add(left,right);
            }
            else if (binExpression.NodeType == ExpressionType.Multiply)
            {
                Expression leftDf = Recurse(binExpression.Left, parameter);
                Expression rightDf = Recurse(binExpression.Right, parameter);
                if (leftDf.NodeType == ExpressionType.Constant)
                    if ((double)((ConstantExpression)leftDf).Value == 0.0)
                        left = Expression.Constant(0.0);
                    else if ((double)((ConstantExpression)leftDf).Value == 1.0)
                        left = binExpression.Right;
                if (left == null && binExpression.Right.NodeType == ExpressionType.Constant)
                    left = ((double)((ConstantExpression)binExpression.Right).Value == 0.0)
                        ? Expression.Constant(0.0)
                        : ((double)((ConstantExpression)binExpression.Right).Value == 1.0)
                        ? leftDf
                        : Expression.Multiply(leftDf, binExpression.Right);
                else if (left == null)
                    left = Expression.Multiply(leftDf, binExpression.Right);
                if (rightDf.NodeType == ExpressionType.Constant)
                    if ((double)((ConstantExpression)rightDf).Value == 0.0)
                        right = Expression.Constant(0.0);
                    else if ((double)((ConstantExpression)rightDf).Value == 1.0)
                        right = binExpression.Left;
                if (right == null && binExpression.Left.NodeType == ExpressionType.Constant)
                    right = ((double)((ConstantExpression)binExpression.Left).Value == 0.0)
                        ? Expression.Constant(0.0)
                        : ((double)((ConstantExpression)binExpression.Left).Value == 1.0)
                        ? rightDf
                        : Expression.Multiply(binExpression.Left, rightDf);
                else if (right == null)
                    right = Expression.Multiply(binExpression.Left, rightDf);
                if (left.NodeType == ExpressionType.Constant)
                    if ((double)((ConstantExpression)left).Value == 0.0) 
                        return right;
                if (right.NodeType == ExpressionType.Constant)
                    if ((double)((ConstantExpression)right).Value == 0.0)
                        return left;
                return Expression.Add(left,right);
            }
            return expression;
        }
        public static Func<double, double> Differentiate(Expression<Func<double, double>> exp)
        {
            var dfExpression = Expression.Lambda<Func<double, double>>(Recurse(exp.Body, exp.Parameters[0]), exp.Parameters);
            Console.WriteLine(dfExpression);
            return dfExpression.Compile();
        }
        static void Main(string[] args)
        {
            Expression<Func<double, double>> f = x => 2*x*x*Math.Sin(x);
            Console.WriteLine(f);
            var compiled = Differentiate(f);
            Console.WriteLine(compiled.Invoke(12));
            Console.ReadLine();
        }
    }
}
