// <copyright file="IMergeAccount.cs" company="B+P">
// Copyright (c) B+P. All rights reserved.
// </copyright>
namespace Brierley.AEModules.MergeAccount
{
    #region Using Statement
    using Brierley.FrameWork.Data.DomainModel;
    #endregion

    /// <summary>
    /// This interface is required when user required before and after merge interceptor 
    /// </summary>
    public interface IMergeAccount
    {
        /// <summary>
        /// After merge method to implement
        /// </summary>
        /// <param name="fromMember">From member after merge</param>
        /// <param name="toMember">To member after merge</param>
        void AfterMerge(Member fromMember, Member toMember);

        /// <summary>
        /// Before merge method to implement
        /// </summary>
        /// <param name="fromMember">From member before merge</param>
        /// <param name="toMember">To member before merge</param>
        void BeforeMerge(Member fromMember, Member toMember);
    }
}
