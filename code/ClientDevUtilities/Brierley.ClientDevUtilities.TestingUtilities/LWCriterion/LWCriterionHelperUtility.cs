using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Data = Brierley.FrameWork.Data;
using Brierley.ClientDevUtilities.TestingUtilities.Reflection;
using System.Collections;

namespace Brierley.ClientDevUtilities.TestingUtilities.LWCriterion
{
    public static class LWCriterionHelperUtility
    {
        public enum CriteriaComponentField
        {
            Name,
            Value,
            Predicate,
            Operator
        }

        public static Type critCompType = typeof(Data.LWCriterion).Assembly.GetType("Brierley.FrameWork.Data.CriteriaComponent");

        public static string GetLWCriterionFieldName(Data.LWCriterion instance, int row)
        {
            return GetLWCriterionField(instance, row, CriteriaComponentField.Name).ToString();
        }

        public static object GetLWCriterionFieldValue(Data.LWCriterion instance, int row)
        {
            return GetLWCriterionField(instance, row, CriteriaComponentField.Value);
        }

        public static Data.LWCriterion.Predicate GetLWCriterionFieldPredicate(Data.LWCriterion instance, int row)
        {
            return (Data.LWCriterion.Predicate)GetLWCriterionField(instance, row, CriteriaComponentField.Predicate);
        }

        public static Data.LWCriterion.OperatorType GetLWCriterionFieldOperator(Data.LWCriterion instance, int row)
        {
            return (Data.LWCriterion.OperatorType)GetLWCriterionField(instance, row, CriteriaComponentField.Operator);
        }

        public static object GetLWCriterionField(Data.LWCriterion instance, int row, CriteriaComponentField field)
        {
            IList criteriaField = (IList)ReflectionUtilities.GetInstanceField(typeof(Data.LWCriterion), instance, "criteria");
            return ReflectionUtilities.GetInstanceProperty(critCompType, criteriaField[row], field.ToString());
        }
    }
}
