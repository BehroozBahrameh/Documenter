using System.Collections.Generic;

namespace Rocket.Web.Documenter.Core.Apenders
{
    public interface IDocumenterAppender
    {
        void Create(IEnumerable<ApiDocument> models);
    }
}
