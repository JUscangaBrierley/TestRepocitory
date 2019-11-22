using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The Expression class is an abstract class that represents the foundation of the entire system. It defines the basic characteristics
    /// of any expression contained in the system including an abstract evaluate definition.
    /// </summary>
    [Serializable]
	public abstract class Expression : MarshalByRefObject, IExpression
    {
        private string _Id = string.Empty;


        /// <summary>
        /// Returns the ID assigned to this instance.
        /// </summary>
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>
        /// Returns the syntax string implemented by this class.
        /// </summary>
        public virtual string Syntax
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Performs an evaluation on the expression tree.
        /// </summary>
        /// <param name="contextObject">The current execution context of the expression</param>
        /// <returns>An object</returns>
        public virtual object evaluate(ContextObject contextObject)
        {
            return new object();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string parseMetaData()
        {
			return this.GetType().Name;
        }

		/// <summary>
		/// This method resolves a member from the context object.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		protected virtual Member ResolveMember(IAttributeSetContainer owner)
		{
			Member member = null;
			if (owner != null)
			{
				if (owner is Member)
				{
					member = (Member)owner;
				}
				else if (owner is VirtualCard)
				{
					member = ((VirtualCard)owner).Member;
				}
				else
				{
					IClientDataObject obj = owner as ClientDataObject;
					if (obj != null)
					{
						member = ResolveMember(obj.Parent);
					}					
				}
			}
			return member;
		}

		protected virtual VirtualCard ResolveLoyaltyCard(IAttributeSetContainer owner)
		{
			VirtualCard vc = null;
			if (owner != null)
			{
				if (!(owner is Member))
				{
					if (owner is VirtualCard)
					{
						vc = (VirtualCard)owner;
					}
					else
					{
						IClientDataObject obj = owner as ClientDataObject;
						if (obj != null)
						{
							vc = ResolveLoyaltyCard(obj.Parent);
						}
					}
				}				
			}
			return vc;
		}
    }
}
