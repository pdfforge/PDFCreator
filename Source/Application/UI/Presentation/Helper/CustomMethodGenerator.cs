using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class CustomMethodGenerator
    {
        public Func<T1, TReturn> ConvertFromGenericToSpecificGetter<T1, TReturn, TGeneric>(Expression<Func<T1, TGeneric>> expressionGetter)
        {
            // return x => (TReturn)((object) expressionGetter.Compile().Invoke(x));
            var returnType = typeof(TReturn);
            var returnTarget = Label(returnType, "returnTarget");
            var convertValueExpression = Convert(expressionGetter.Body, returnType);
            var returnLabelExpression = Label(returnTarget, convertValueExpression);

            return Lambda<Func<T1, TReturn>>(returnLabelExpression, expressionGetter.Parameters[0]).Compile();
        }

        public Action<T1, TSetterType> GenerateSetterFromGetter<T1, TSetterType, TGeneric>(Expression<Func<T1, TGeneric>> expressionGetter)
        {
            var newValue = Parameter(typeof(TSetterType));
            var convertValueExpression = Convert(newValue, typeof(TGeneric));
            var assignValueExpression = Assign(expressionGetter.Body, convertValueExpression);

            return Lambda<Action<T1, TSetterType>>(assignValueExpression, expressionGetter.Parameters[0], newValue).Compile();
        }
    }
}
