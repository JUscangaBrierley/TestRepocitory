using System;
using System.Runtime.Serialization;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Moq;

namespace Brierley.ClientDevUtilities.TestingUtilities.Expressions
{
    public class ExpressionUtility
    {
        public static ParameterList GetSimpleParameterList(params object[] parameters)
        {
            //At this time this class only has an internal constructor this method creates an instance without calling a constructor
            ParameterList parms = (ParameterList)FormatterServices.GetUninitializedObject(typeof(ParameterList));
            Expression[] expressions = new Expression[parameters.Length];
            int counter = 0;
            foreach(object obj in parameters)
            {
                Mock<Expression> mock = new Mock<Expression>();
                mock.Setup(x => x.evaluate(It.IsAny<ContextObject>())).Returns(obj);
                expressions[counter] = mock.Object;
                counter++;
            }
            parms.Expressions = expressions;
            return parms;
        }

        public static Expression GetSimpleSingleExpression(object parameter)
        {
            Mock<Expression> mock = new Mock<Expression>();
            mock.Setup(x => x.evaluate(It.IsAny<ContextObject>())).Returns(parameter);
            return mock.Object;
        }
    }
}
