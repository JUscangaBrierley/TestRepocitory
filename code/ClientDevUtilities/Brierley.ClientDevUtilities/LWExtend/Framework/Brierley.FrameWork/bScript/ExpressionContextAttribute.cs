using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.bScript
{
	[Flags]
	public enum WizardCategories
	{
		Profile = 1, 
		Tier = 2,
		Points = 4,
		Attributes = 8,
		Behavior = 16, 
		Operator = 32, 
		LogicalOperator = 64, 
		MathOperator = 128, 
		Function = 256, 
		Survey = 512, 
		Content = 1024,
		Dates = 2048
	}

	[Flags]
	public enum ExpressionContexts
	{
		All = 32767,
		Content = 1,
		Survey = 2,
		Email = 4,
		Member = 8, 
		Campaign = 16
	}


	public enum ExpressionTypes
	{
		Constant = 1,
		Function = 2,
		Operator = 3
	}


	[Flags]
	public enum ExpressionApplications
	{
		All = 32767, 
		Strings = 1,
		Numbers = 2,
		Dates = 4,
		Booleans = 8, 
		Objects = 16, 
		Content = 32,
		Survey = 64, 
		Member = 128, 
		Custom = 256, 
		Campaign = 512
	}

	/// <summary>
	/// defines popup helpers for building scripts...
	/// </summary>
	public enum ParameterHelpers
	{
        None = 0,
		AttributeSet = 1, 
		Attribute = 2, 
		Tier = 3, 
		DateFormat = 4, 
		ConfigurationKey = 5, 
		PointType = 6, 
		PointEvent = 7, 
		PromotionCode = 8, 
		Boolean = 9, 
		TierProperty = 10, 
		ProductProperty = 11, 
		Language = 12, 
		Channel = 13, 
		StoreProperty = 14,
		ExprWizSet = 15, 
		Bonus = 16,
		GlobalAttributeSet = 17,
        CSAgentRole = 18,
        CSAgentFunction = 19,
        OwnerType = 20,
        SocialPublisher = 21,
        SocialSentiment = 22,
        SocialProperty = 23, 
		CampaignStepName = 24, 
		CampaignAttribute = 25,
        PointsSummaryType = 26,
        Reward = 27,
		EnvironmentKey = 28,
		RuleTriggerIssueType = 29,
        MemberTierProperty = 30,
        ContentType = 31,
        ContentAttributeName = 32,
        ContentTypeSearch = 33,
        MemberStatus = 34,
        MemberProperty = 35
    }

	/// <summary>
	/// Attribute class used to provide information on bScript expression classes and enums.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ExpressionContextAttribute : System.Attribute
	{
		/// <summary>
		/// Type of bScript entity, e.g., constant, function, operator, etc.
		/// </summary>
		public ExpressionTypes ExpressionType { get; set; }

		/// <summary>
		/// Friendly name of this bScript entity.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Friendly description of this bScript entity.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Contexts in which this bScript function is not applicable.
		/// </summary>
		public ExpressionContexts ExcludeContext { get; set; }

		/// <summary>
		/// Applications to which this bScript entity applies.
		/// </summary>
		public ExpressionApplications ExpressionApplication { get; set; }

		/// <summary>
		/// Return values for this bScript entity type.
		/// </summary>
		public ExpressionApplications ExpressionReturns { get; set; }
		
		/// <summary>
		/// The description of the expression for the expression wizard
		/// </summary>
		public string WizardDescription { get; set; }

		/// <summary>
		/// If true, the function is only shown when using the "Advanced" wizard
		/// </summary>
		public bool AdvancedWizard { get; set; }

		public WizardCategories WizardCategory { get; set; }

		/// <summary>
		/// If true, a member is required in the ContentObject.Owner when the function is evaluated.
		/// </summary>
		public bool EvalRequiresMember { get; set; }
		
		public ExpressionContextAttribute()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
	public sealed class ExpressionParameterAttribute : System.Attribute
	{
		public int Order { get; set; }
		public string Name { get; set; }
		public ExpressionApplications Type { get; set; }
		public bool Optional { get; set; }
		public string WizardDescription { get; set; }
		public ParameterHelpers Helpers { get; set; }

		/// <summary>
		/// Allow a semicolon separated list for the parameter value?
		/// </summary>
		public bool AllowMultiple { get; set; }

		public ExpressionParameterAttribute()
		{
		}
	}

	/// <summary>
	/// Attribute class used to define the assmebly name the bScript expression resides in, when the expression
	/// resides outside of Brierley.FrameWork.dll.
	/// </summary>
	/// <remarks>
	/// The bScript expression builder can reflect over an expression class and obtain its <see cref="!:ExpressionContextAttribute">ExpressionContextAttribute</see>, 
	/// if it exists. This attribute should be used on <see cref="Brierley.FrameWork.bScript.FunctionNames">Brierley.FrameWork.bScript.FunctionNames</see> to describe
	/// the name of the assembly the expression resides in, in order for the expression builder to locate and reflect over its attributes.
	/// </remarks>
	/// <example>
	/// [ExpressionAssemblyAttribute("Brierley.MyLib.dll", "Brierley.MyLib.bScript.FooBar")]
    /// FOOBAR,
	/// </example>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class ExpressionAssemblyAttribute : System.Attribute
	{
		/// <summary>
		/// The assembly file name where the expression resides, if outside of the framework assembly.
		/// </summary>
		public string AssemblyFileName { get; set; }

		/// <summary>
		/// The type name of the expression.
		/// </summary>
		public string TypeName { get; set; }


		public ExpressionAssemblyAttribute(string assemblyFileName, string typeName)
		{
			AssemblyFileName = assemblyFileName;
			TypeName = typeName;
		}

		public ExpressionAssemblyAttribute()
		{
		}
	}

}
