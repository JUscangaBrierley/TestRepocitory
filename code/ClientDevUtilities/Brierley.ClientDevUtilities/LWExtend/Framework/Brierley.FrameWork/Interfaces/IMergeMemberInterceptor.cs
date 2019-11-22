using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Interfaces
{
    public interface IMergeMemberInterceptor : ILWInterceptor
    {
        void BeforeMerge(Member victim, Member survivor);
        void AfterMerge(Member victim, Member survivor);
    }
}
