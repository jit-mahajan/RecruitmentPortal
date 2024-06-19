using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;

namespace RecruitmentPortal.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        /*
        public MappingProfile()
        {
            CreateMap<JobApplication, JobApplicationDto>()
                .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.Jobs))
                .ForMember(dest => dest.Candidate, opt => opt.MapFrom(src => src.Users));
            CreateMap<Jobs, JobDto>();
            CreateMap<Users, CandidateDto>();
        }
        */
    }
}
