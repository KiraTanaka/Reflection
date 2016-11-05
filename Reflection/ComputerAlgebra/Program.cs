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
        public static Expression SimplificationExpression(Expression left, Expression right)
        {
            Expression simplifiedExpression = null;
            Func<Expression, Expression> simplifyConstant = (x) =>
             {
                 if (x.NodeType == ExpressionType.Constant)
                     if ((double)((ConstantExpression)x).Value == 0.0)
                         return Expression.Constant(0.0);
                     else if ((double)((ConstantExpression)x).Value == 1.0)
                         return right;
                 return null;
             };
            simplifiedExpression = simplifyConstant(left);
            simplifiedExpression = simplifiedExpression ?? simplifyConstant(right);
            return simplifiedExpression ?? Expression.Multiply(left, right);
            /*if (left.NodeType == ExpressionType.Constant)
                if ((double)((ConstantExpression)left).Value == 0.0)
                    simplifiedExpression = Expression.Constant(0.0);
                else if ((double)((ConstantExpression)left).Value == 1.0)
                    simplifiedExpression = right;
            if (simplifiedExpression == null)
                if (right.NodeType == ExpressionType.Constant)
                    simplifiedExpression = ((double)((ConstantExpression)right).Value == 0.0)
                        ? Expression.Constant(0.0)
                        : ((double)((ConstantExpression)right).Value == 1.0)
                        ? left
                        : Expression.Multiply(left, right);
                else
                    simplifiedExpression = Expression.Multiply(left, right);*/
        }
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
                left = SimplificationExpression(leftDf, binExpression.Right);
                right = SimplificationExpression(rightDf, binExpression.Left);
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
            Expression<Func<double, double>> f = x => (10 + Math.Sin(x))*x;
            Console.WriteLine(f);
            var compiled = Differentiate(f);
            Console.WriteLine(compiled.Invoke(12));
            Console.ReadLine();
        }
    }
}
