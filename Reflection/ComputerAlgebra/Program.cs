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
        public static Expression SimplifyExpressionAndCompute(Expression left, Expression right,ExpressionType typeOperations)
        {
            Expression simplifiedExpression = null;
            Func<Expression, Expression, Expression> simplifyConstantZeroInAdd = (x, y) => 
            {
                if (x.NodeType == ExpressionType.Constant)
                    return ((double)((ConstantExpression)x).Value == 0.0) ? y : null;
                return null;
            };
             Func<Expression, Expression, Expression> simplifyConstantInMultiply = (x, y) =>
             {
                 if (x.NodeType == ExpressionType.Constant)
                 {
                     return ((double)((ConstantExpression)x).Value == 0.0) ? Expression.Constant(0.0)
                     : ((double)((ConstantExpression)x).Value == 1.0)?
                      y: null;
                 }
                 return null;
             };
            if (typeOperations == ExpressionType.Add)
            {
                simplifiedExpression = simplifyConstantZeroInAdd(left, right);
                simplifiedExpression = simplifiedExpression ?? simplifyConstantZeroInAdd(right, left);
                return simplifiedExpression ?? Expression.Add(left, right);
            }
            simplifiedExpression = simplifyConstantInMultiply(left, right);
            simplifiedExpression = simplifiedExpression ?? simplifyConstantInMultiply(right, left);
            return simplifiedExpression ?? Expression.Multiply(left, right);
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
                return SimplifyExpressionAndCompute(left, right, ExpressionType.Add);
            }
            else if (binExpression.NodeType == ExpressionType.Multiply)
            {
                Expression leftDf = Recurse(binExpression.Left, parameter);
                Expression rightDf = Recurse(binExpression.Right, parameter);
                left = SimplifyExpressionAndCompute(leftDf, binExpression.Right, ExpressionType.Multiply);
                right = SimplifyExpressionAndCompute(rightDf, binExpression.Left, ExpressionType.Multiply);
                return SimplifyExpressionAndCompute(left, right, ExpressionType.Add);  
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
            Expression<Func<double, double>> f = x => 2 * x * x * Math.Sin(x);//(10 + Math.Sin(x))*x //2 * x * x * Math.Sin(x)
            Console.WriteLine(f);
            var compiled = Differentiate(f);
            Console.WriteLine(compiled.Invoke(12));
            Console.ReadLine();
        }
    }
}
