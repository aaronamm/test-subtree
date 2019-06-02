using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using System;

namespace CodeSanook.Common.Models
{
    public static class ContentHelper
    {
        public static void AlterTypeDefinition<TMainPart>(
            this IContentDefinitionManager manager,
            Action<ContentTypeDefinitionBuilder> alteration
        ) where TMainPart : ContentPart
        {
            var contentTypeName = typeof(TMainPart).Name.Replace("Part", string.Empty);
            manager.AlterTypeDefinition(contentTypeName, alteration);
        }
    }
}