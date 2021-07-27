using AutoMapper;

using System.Collections.Generic;

namespace WebFramework.CustomMapping
{
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile(IEnumerable<IHaveCustomMapping> haveCustomMappings)
        {
            foreach (IHaveCustomMapping item in haveCustomMappings)
            {
                item.CreateMappings(this);
            }
        }
    }
}
